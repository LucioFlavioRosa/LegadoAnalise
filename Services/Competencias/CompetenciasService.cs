using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Competencias.Common;
using OfficeOpenXml;

namespace Peers.Moderno.Services.Competencias;

public class CompetenciasService : ICompetenciasService
{
    private readonly ApplicationDbContext _context;
    private readonly ICompetenciasImportExportUtil _importExportUtil;
    private readonly ITelemetryService _telemetryService;

    public CompetenciasService(
        ApplicationDbContext context,
        ICompetenciasImportExportUtil importExportUtil,
        ITelemetryService telemetryService)
    {
        _context = context;
        _importExportUtil = importExportUtil;
        _telemetryService = telemetryService;
    }

    public async Task<bool> CadastrarAsync(Competencia competencia)
    {
        try
        {
            competencia.DHC = DateTime.Now;
            competencia.ATV = 1;

            _context.Competencias.Add(competencia);
            await _context.SaveChangesAsync();

            await AtualizarRelacaoCargoSubcompetenciaAsync(
                competencia.IdCargo,
                competencia.IdSubCompetencia,
                competencia.CompetenciaJR);

            _telemetryService.TrackEvent("CompetenciaCadastrada", new Dictionary<string, string>
            {
                { "IdCompetencia", competencia.IdCompetencia.ToString() },
                { "TipoAvaliacao", competencia.TipoAvaliacao }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao cadastrar competência");
            return false;
        }
    }

    public async Task<bool> AlterarAsync(Competencia competencia)
    {
        try
        {
            var competenciaExistente = await _context.Competencias
                .FirstOrDefaultAsync(c => c.IdCompetencia == competencia.IdCompetencia);

            if (competenciaExistente == null)
                return false;

            // Atualizar propriedades
            competenciaExistente.IdCargo = competencia.IdCargo;
            competenciaExistente.IdEixo = competencia.IdEixo;
            competenciaExistente.IdSubCompetencia = competencia.IdSubCompetencia;
            competenciaExistente.IdDimensao = competencia.IdDimensao;
            competenciaExistente.CompetenciaJR = competencia.CompetenciaJR;
            competenciaExistente.CompetenciaJRDetalhe = competencia.CompetenciaJRDetalhe;
            competenciaExistente.CompetenciaPL = competencia.CompetenciaPL;
            competenciaExistente.CompetenciaPLDetalhe = competencia.CompetenciaPLDetalhe;
            competenciaExistente.CompetenciaSR = competencia.CompetenciaSR;
            competenciaExistente.CompetenciaSRDetalhe = competencia.CompetenciaSRDetalhe;
            competenciaExistente.PalavrasChave = competencia.PalavrasChave;
            competenciaExistente.TipoAvaliacao = competencia.TipoAvaliacao;
            competenciaExistente.Escopo = competencia.Escopo;
            competenciaExistente.InputAutoAvaliacao = competencia.InputAutoAvaliacao;
            competenciaExistente.InputAvaliacaoAsCegas = competencia.InputAvaliacaoAsCegas;
            competenciaExistente.InputAvaliacaoGestor = competencia.InputAvaliacaoGestor;
            competenciaExistente.InputFeedback = competencia.InputFeedback;
            competenciaExistente.InputNivel1 = competencia.InputNivel1;
            competenciaExistente.InputNivel2 = competencia.InputNivel2;
            competenciaExistente.VisivelAutoAvaliacao = competencia.VisivelAutoAvaliacao;
            competenciaExistente.VisivelAvaliacaoAsCegas = competencia.VisivelAvaliacaoAsCegas;
            competenciaExistente.VisivelAvaliacaoGestor = competencia.VisivelAvaliacaoGestor;
            competenciaExistente.VisivelFeedback = competencia.VisivelFeedback;
            competenciaExistente.VisivelNivel1 = competencia.VisivelNivel1;
            competenciaExistente.VisivelNivel2 = competencia.VisivelNivel2;
            competenciaExistente.IdNotaPadraoNivel1 = competencia.IdNotaPadraoNivel1;
            competenciaExistente.IdNotaPadraoNivel2 = competencia.IdNotaPadraoNivel2;
            competenciaExistente.IdModo = competencia.IdModo;

            await _context.SaveChangesAsync();

            await AtualizarRelacaoCargoSubcompetenciaAsync(
                competencia.IdCargo,
                competencia.IdSubCompetencia,
                competencia.CompetenciaJR);

            _telemetryService.TrackEvent("CompetenciaAlterada", new Dictionary<string, string>
            {
                { "IdCompetencia", competencia.IdCompetencia.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao alterar competência");
            return false;
        }
    }

    public async Task<bool> ExcluirAsync(int idCompetencia)
    {
        try
        {
            var competencia = await _context.Competencias
                .FirstOrDefaultAsync(c => c.IdCompetencia == idCompetencia);

            if (competencia == null)
                return false;

            competencia.ATV = 0;
            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("CompetenciaInativada", new Dictionary<string, string>
            {
                { "IdCompetencia", idCompetencia.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao inativar competência");
            return false;
        }
    }

    public async Task<List<Competencia>> ListarAsync(bool apenasAtivas = true)
    {
        try
        {
            var query = _context.Competencias
                .Include(c => c.Cargo)
                .Include(c => c.Eixo)
                .Include(c => c.SubCompetencia)
                .Include(c => c.Dimensao)
                .AsQueryable();

            if (apenasAtivas)
                query = query.Where(c => c.ATV == 1);

            return await query.OrderBy(c => c.IdCompetencia).ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao listar competências");
            return new List<Competencia>();
        }
    }

    public async Task<Competencia?> ObterPorIdAsync(int idCompetencia)
    {
        try
        {
            return await _context.Competencias
                .Include(c => c.Cargo)
                .Include(c => c.Eixo)
                .Include(c => c.SubCompetencia)
                .Include(c => c.Dimensao)
                .FirstOrDefaultAsync(c => c.IdCompetencia == idCompetencia);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao obter competência por ID");
            return null;
        }
    }

    public async Task<byte[]> ExportarAsync()
    {
        try
        {
            var competencias = await ListarAsync(false);
            return await _importExportUtil.ExportarParaExcelAsync(competencias);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao exportar competências");
            throw;
        }
    }

    public async Task<ImportResult> ImportarAsync(Stream fileStream, int idEmpresa, int idUsuario)
    {
        try
        {
            var resultado = await _importExportUtil.ImportarDeExcelAsync(fileStream, idEmpresa, idUsuario);

            _telemetryService.TrackEvent("CompetenciasImportadas", new Dictionary<string, string>
            {
                { "LinhasInseridas", resultado.LinhasInseridas.ToString() },
                { "LinhasAlteradas", resultado.LinhasAlteradas.ToString() },
                { "LinhasDesconsideradas", resultado.LinhasDesconsideradas.ToString() },
                { "LinhasComErro", resultado.LinhasComErro.ToString() }
            });

            return resultado;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao importar competências");
            throw;
        }
    }

    public async Task<int> AgregarCompetenciaAvaliacoesAsync(int idCompetencia)
    {
        try
        {
            var competencia = await ObterPorIdAsync(idCompetencia);
            if (competencia == null || competencia.TipoAvaliacao != "desempenho")
                return 0;

            // Lógica de agregação seria implementada aqui
            // Por enquanto retornando 0 como placeholder
            return 0;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao agregar competência às avaliações");
            return 0;
        }
    }

    public async Task<RelacaoCargoSubcompetencia?> ObterRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubcompetencia)
    {
        try
        {
            return await _context.RelacoesCargosSubcompetencias
                .FirstOrDefaultAsync(r => r.IdCargo == idCargo && r.IdSubcompetencia == idSubcompetencia);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao obter relação cargo-subcompetência");
            return null;
        }
    }

    private async Task AtualizarRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubcompetencia, string descricao)
    {
        var relacao = await ObterRelacaoCargoSubcompetenciaAsync(idCargo, idSubcompetencia);

        if (relacao == null)
        {
            relacao = new RelacaoCargoSubcompetencia
            {
                IdCargo = idCargo,
                IdSubcompetencia = idSubcompetencia,
                Descricao = descricao
            };
            _context.RelacoesCargosSubcompetencias.Add(relacao);
        }
        else
        {
            relacao.Descricao = descricao;
        }

        await _context.SaveChangesAsync();
    }
}