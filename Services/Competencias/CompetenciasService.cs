using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Competencias.Common;
using Peers.Moderno.Services.Competencias.Common.DTOs;
using Peers.Moderno.Services.Cargos;
using Peers.Moderno.Services;

namespace Peers.Moderno.Services.Competencias;

public class CompetenciasService : ICompetenciasService
{
    private readonly ApplicationDbContext _context;
    private readonly ICompetenciasValidator _validator;
    private readonly CompetenciasImportExportUtil _importExportUtil;
    private readonly ICargosService _cargosService;
    private readonly IAvaliacoesService _avaliacoesService;

    public CompetenciasService(
        ApplicationDbContext context,
        ICompetenciasValidator validator,
        CompetenciasImportExportUtil importExportUtil,
        ICargosService cargosService,
        IAvaliacoesService avaliacoesService)
    {
        _context = context;
        _validator = validator;
        _importExportUtil = importExportUtil;
        _cargosService = cargosService;
        _avaliacoesService = avaliacoesService;
    }

    public async Task<List<Competencia>> ListarAsync(bool? ativo = null)
    {
        var query = _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .AsQueryable();

        if (ativo.HasValue)
        {
            query = query.Where(c => c.ATV == (ativo.Value ? 1 : 0));
        }

        return await query.OrderBy(c => c.IdCompetencia).ToListAsync();
    }

    public async Task<Competencia?> ObterPorIdAsync(int id)
    {
        return await _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .FirstOrDefaultAsync(c => c.IdCompetencia == id);
    }

    public async Task<Competencia?> ObterCompetenciaAsync(int idEmpresa, int idCargo, int idNivel, int idEixo, int idSubCompetencia, int idDimensao, string detalhamento)
    {
        return await _importExportUtil.ObterCompetenciaExistenteAsync(idEmpresa, idCargo, idNivel, idEixo, idSubCompetencia, idDimensao, detalhamento);
    }

    public async Task<bool> CadastrarAsync(CompetenciaDto competenciaDto)
    {
        var validation = _validator.ValidateForInsert(competenciaDto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validation.Errors));
        }

        var competencia = MapearDtoParaEntidade(competenciaDto);
        competencia.DHC = DateTime.Now;

        _context.Competencias.Add(competencia);

        // Atualizar relação cargo-subcompetência se for desempenho
        if (competenciaDto.TipoAvaliacao == "desempenho")
        {
            await AtualizarRelacaoCargoSubcompetenciaAsync(competenciaDto.IdCargo, competenciaDto.IdSubCompetencia, competenciaDto.RelacaoSubcompetencia);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AlterarAsync(int id, CompetenciaDto competenciaDto)
    {
        var validation = _validator.ValidateForUpdate(id, competenciaDto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validation.Errors));
        }

        var competencia = await _context.Competencias.FindAsync(id);
        if (competencia == null)
        {
            return false;
        }

        AtualizarEntidadeComDto(competencia, competenciaDto);
        competencia.DHC = DateTime.Now;

        // Atualizar relação cargo-subcompetência se for desempenho
        if (competenciaDto.TipoAvaliacao == "desempenho")
        {
            await AtualizarRelacaoCargoSubcompetenciaAsync(competenciaDto.IdCargo, competenciaDto.IdSubCompetencia, competenciaDto.RelacaoSubcompetencia);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        var competencia = await _context.Competencias.FindAsync(id);
        if (competencia == null)
        {
            return false;
        }

        competencia.ATV = 0;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<CompetenciaExportDto>> ExportarAsync()
    {
        var competencias = await _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .ToListAsync();

        return competencias.Select(c => new CompetenciaExportDto
        {
            IdCompetencia = c.IdCompetencia,
            IdCargo = c.IdCargo,
            Cargo = c.Cargo?.Nome ?? "",
            IdEixo = c.IdEixo,
            Eixo = c.Eixo?.Nome ?? "",
            IdSubCompetencia = c.IdSubCompetencia,
            SubCompetencia = c.SubCompetencia?.Nome ?? "",
            IdDimensao = c.IdDimensao,
            Dimensao = c.Dimensao?.Nome ?? "",
            DetalheNivelAtual = c.CompetenciaJRDetalhe ?? "",
            CompetenciaAtual = c.CompetenciaJR ?? "",
            PalavrasChave = c.PalavrasChave ?? "",
            TipoAvaliacao = c.TipoAvaliacao ?? "",
            Escopo = c.Escopo ?? "",
            ATV = c.ATV ?? 1
        }).ToList();
    }

    public async Task<ImportResultDto> ImportarAsync(Stream fileStream)
    {
        var result = new ImportResultDto();
        
        try
        {
            var competenciasImport = await _importExportUtil.LerExcelAsync(fileStream);
            
            foreach (var item in competenciasImport)
            {
                var validation = _validator.ValidateImportData(item);
                if (!validation.IsValid)
                {
                    result.LinhasDesconsideradas++;
                    result.Erros.AddRange(validation.Errors);
                    continue;
                }

                try
                {
                    var existeCompetencia = await ObterCompetenciaAsync(1, item.IdCargo, 1, item.IdEixo, item.IdSubCompetencia, item.IdDimensao, item.DetalheNivelAtual);
                    
                    if (item.IdCompetencia == 0 && existeCompetencia == null)
                    {
                        // Inserir nova competência
                        var novaCompetencia = CriarCompetenciaDeImport(item);
                        _context.Competencias.Add(novaCompetencia);
                        
                        await AtualizarRelacaoCargoSubcompetenciaAsync(item.IdCargo, item.IdSubCompetencia, item.CompetenciaAtual);
                        
                        result.LinhasInseridas++;
                    }
                    else if (existeCompetencia != null && existeCompetencia.ATV == 0)
                    {
                        result.LinhasDesconsideradas++;
                    }
                    else
                    {
                        // Alterar competência existente
                        var competenciaParaAlterar = existeCompetencia ?? await _context.Competencias.FindAsync(item.IdCompetencia);
                        if (competenciaParaAlterar != null)
                        {
                            AtualizarCompetenciaDeImport(competenciaParaAlterar, item);
                            await AtualizarRelacaoCargoSubcompetenciaAsync(item.IdCargo, item.IdSubCompetencia, item.CompetenciaAtual);
                            result.LinhasAlteradas++;
                        }
                        else
                        {
                            result.LinhasComErro++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.LinhasComErro++;
                    result.Erros.Add($"Erro na linha: {ex.Message}");
                }
            }
            
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.Erros.Add($"Erro geral: {ex.Message}");
        }
        
        return result;
    }

    public async Task<int> AgregarCompetenciaAsync(int idCompetencia)
    {
        var competencia = await ObterPorIdAsync(idCompetencia);
        if (competencia == null)
        {
            throw new ArgumentException("Competência não encontrada");
        }

        // Implementar lógica de agregação usando o serviço de avaliações
        // Esta é uma versão simplificada - a lógica completa seria implementada conforme necessário
        return await Task.FromResult(0);
    }

    private Competencia MapearDtoParaEntidade(CompetenciaDto dto)
    {
        var competencia = new Competencia
        {
            IdEmpresa = dto.IdEmpresa,
            IdCargo = dto.IdCargo,
            IdNivel = dto.IdNivel,
            IdEixo = dto.IdEixo,
            IdSubCompetencia = dto.IdSubCompetencia,
            IdDimensao = dto.IdDimensao,
            CompetenciaJR = dto.NivelAtual,
            CompetenciaJRDetalhe = dto.DetalhamentoNivelAtual,
            CompetenciaPL = dto.ProximoNivel.IsNullOrEmpty() ? "Não aplicável" : dto.ProximoNivel,
            CompetenciaPLDetalhe = dto.DetalhamentoProximoNivel,
            CompetenciaSR = "Não aplicável",
            CompetenciaSRDetalhe = "Não informado - Nova matriz de cargos",
            PalavrasChave = dto.PalavrasChave,
            TipoAvaliacao = dto.TipoAvaliacao,
            Escopo = dto.TipoAvaliacao == "lideranca" ? dto.Escopo.Replace("Por ", "") : "projeto",
            ATV = dto.Ativo ? 1 : 0,
            InputAutoAvaliacao = dto.InputAutoAvaliacao,
            InputAvaliacaoAsCegas = dto.InputAvaliacaoAsCegas,
            InputAvaliacaoGestor = dto.InputAvaliacaoGestor,
            InputFeedback = dto.InputFeedback,
            InputNivel1 = dto.InputNivel1,
            InputNivel2 = dto.InputNivel2,
            VisivelAutoAvaliacao = dto.VisivelAutoAvaliacao,
            VisivelAvaliacaoAsCegas = dto.VisivelAvaliacaoAsCegas,
            VisivelAvaliacaoGestor = dto.VisivelAvaliacaoGestor,
            VisivelFeedback = dto.VisivelFeedback,
            VisivelNivel1 = dto.VisivelNivel1,
            VisivelNivel2 = dto.VisivelNivel2,
            IdNotaPadraoNivel1 = dto.IdNotaPadraoNivel1,
            IdNotaPadraoNivel2 = dto.IdNotaPadraoNivel2,
            IdModo = dto.IdModoCalculo
        };

        if (dto.TipoAvaliacao == "lideranca")
        {
            competencia.CompetenciaJR = dto.DetalhamentoLideranca;
            competencia.CompetenciaPL = dto.DetalhamentoLideranca;
            competencia.CompetenciaSR = dto.DetalhamentoLideranca;
            competencia.CompetenciaJRDetalhe = dto.DetalhamentoLideranca;
            competencia.CompetenciaPLDetalhe = dto.DetalhamentoLideranca;
            competencia.CompetenciaSRDetalhe = dto.DetalhamentoLideranca;
            competencia.IdCargo = 96; // Cargo fixo para liderança
            competencia.IdDimensao = 10; // Dimensão fixa para liderança
        }

        return competencia;
    }

    private void AtualizarEntidadeComDto(Competencia competencia, CompetenciaDto dto)
    {
        competencia.IdCargo = dto.IdCargo;
        competencia.IdNivel = dto.IdNivel;
        competencia.IdEixo = dto.IdEixo;
        competencia.IdSubCompetencia = dto.IdSubCompetencia;
        competencia.IdDimensao = dto.IdDimensao;
        competencia.CompetenciaJR = dto.NivelAtual;
        competencia.CompetenciaJRDetalhe = dto.DetalhamentoNivelAtual;
        competencia.CompetenciaPL = dto.ProximoNivel.IsNullOrEmpty() ? "Não aplicável" : dto.ProximoNivel;
        competencia.CompetenciaPLDetalhe = dto.DetalhamentoProximoNivel;
        competencia.PalavrasChave = dto.PalavrasChave;
        competencia.TipoAvaliacao = dto.TipoAvaliacao;
        competencia.Escopo = dto.TipoAvaliacao == "lideranca" ? dto.Escopo.Replace("Por ", "") : "projeto";
        competencia.ATV = dto.Ativo ? 1 : 0;
        competencia.InputAutoAvaliacao = dto.InputAutoAvaliacao;
        competencia.InputAvaliacaoAsCegas = dto.InputAvaliacaoAsCegas;
        competencia.InputAvaliacaoGestor = dto.InputAvaliacaoGestor;
        competencia.InputFeedback = dto.InputFeedback;
        competencia.InputNivel1 = dto.InputNivel1;
        competencia.InputNivel2 = dto.InputNivel2;
        competencia.VisivelAutoAvaliacao = dto.VisivelAutoAvaliacao;
        competencia.VisivelAvaliacaoAsCegas = dto.VisivelAvaliacaoAsCegas;
        competencia.VisivelAvaliacaoGestor = dto.VisivelAvaliacaoGestor;
        competencia.VisivelFeedback = dto.VisivelFeedback;
        competencia.VisivelNivel1 = dto.VisivelNivel1;
        competencia.VisivelNivel2 = dto.VisivelNivel2;
        competencia.IdNotaPadraoNivel1 = dto.IdNotaPadraoNivel1;
        competencia.IdNotaPadraoNivel2 = dto.IdNotaPadraoNivel2;
        competencia.IdModo = dto.IdModoCalculo;

        if (dto.TipoAvaliacao == "lideranca")
        {
            competencia.CompetenciaJR = dto.DetalhamentoLideranca;
            competencia.CompetenciaPL = dto.DetalhamentoLideranca;
            competencia.CompetenciaSR = dto.DetalhamentoLideranca;
            competencia.CompetenciaJRDetalhe = dto.DetalhamentoLideranca;
            competencia.CompetenciaPLDetalhe = dto.DetalhamentoLideranca;
            competencia.CompetenciaSRDetalhe = dto.DetalhamentoLideranca;
            competencia.IdCargo = 96;
            competencia.IdDimensao = 10;
        }
    }

    private Competencia CriarCompetenciaDeImport(CompetenciaImportDto item)
    {
        return new Competencia
        {
            IdEmpresa = 1, // Valor padrão
            IdCargo = item.IdCargo,
            IdNivel = 1,
            IdEixo = item.IdEixo,
            IdSubCompetencia = item.IdSubCompetencia,
            IdDimensao = item.IdDimensao,
            CompetenciaJR = item.CompetenciaAtual,
            CompetenciaJRDetalhe = item.DetalheNivelAtual,
            CompetenciaPL = item.CompetenciaAtual,
            CompetenciaPLDetalhe = item.DetalheNivelAtual,
            CompetenciaSR = item.CompetenciaAtual,
            CompetenciaSRDetalhe = item.DetalheNivelAtual,
            PalavrasChave = item.PalavrasChave,
            TipoAvaliacao = item.TipoAvaliacao,
            Escopo = item.Escopo,
            ATV = item.ATV,
            DHC = DateTime.Now,
            InputAutoAvaliacao = true,
            InputAvaliacaoAsCegas = true,
            InputAvaliacaoGestor = true,
            InputFeedback = true,
            InputNivel1 = true,
            InputNivel2 = true,
            VisivelAutoAvaliacao = true,
            VisivelAvaliacaoAsCegas = true,
            VisivelAvaliacaoGestor = true,
            VisivelFeedback = true,
            VisivelNivel1 = true,
            VisivelNivel2 = true,
            IdModo = 1 // Valor padrão
        };
    }

    private void AtualizarCompetenciaDeImport(Competencia competencia, CompetenciaImportDto item)
    {
        competencia.IdCargo = item.IdCargo;
        competencia.IdEixo = item.IdEixo;
        competencia.IdSubCompetencia = item.IdSubCompetencia;
        competencia.IdDimensao = item.IdDimensao;
        competencia.CompetenciaJR = item.CompetenciaAtual;
        competencia.CompetenciaJRDetalhe = item.DetalheNivelAtual;
        competencia.CompetenciaPL = item.CompetenciaAtual;
        competencia.CompetenciaPLDetalhe = item.DetalheNivelAtual;
        competencia.CompetenciaSR = item.CompetenciaAtual;
        competencia.CompetenciaSRDetalhe = item.DetalheNivelAtual;
        competencia.PalavrasChave = item.PalavrasChave;
        competencia.TipoAvaliacao = item.TipoAvaliacao;
        competencia.Escopo = item.Escopo;
        competencia.ATV = item.ATV;
        competencia.DHC = DateTime.Now;
    }

    private async Task AtualizarRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubCompetencia, string descricao)
    {
        var relacao = await _context.RelacoesCargosSubcompetencias
            .FirstOrDefaultAsync(r => r.IdCargo == idCargo && r.IdSubcompetencia == idSubCompetencia);

        if (relacao == null)
        {
            relacao = new RelacaoCargoSubcompetencia
            {
                IdCargo = idCargo,
                IdSubcompetencia = idSubCompetencia,
                Descricao = descricao
            };
            _context.RelacoesCargosSubcompetencias.Add(relacao);
        }
        else
        {
            relacao.Descricao = descricao;
        }
    }
}