using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Competencias.Common;

namespace Peers.Moderno.Services.Competencias;

public class CompetenciasService : ICompetenciasService
{
    private readonly ApplicationDbContext _context;
    private readonly CompetenciasImportExportUtil _importExportUtil;

    public CompetenciasService(ApplicationDbContext context, CompetenciasImportExportUtil importExportUtil)
    {
        _context = context;
        _importExportUtil = importExportUtil;
    }

    public async Task<bool> CadastrarAsync(Competencia competencia)
    {
        try
        {
            competencia.DataCriacao = DateTime.Now;
            _context.Competencias.Add(competencia);
            await _context.SaveChangesAsync();

            // Atualizar relação cargo-subcompetência
            await AtualizarRelacaoCargoSubcompetenciaAsync(competencia.IdCargo, competencia.IdSubCompetencia, competencia.CompetenciaJR);
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AlterarAsync(Competencia competencia)
    {
        try
        {
            _context.Entry(competencia).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Atualizar relação cargo-subcompetência
            await AtualizarRelacaoCargoSubcompetenciaAsync(competencia.IdCargo, competencia.IdSubCompetencia, competencia.CompetenciaJR);
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ExcluirAsync(int idCompetencia)
    {
        try
        {
            var competencia = await _context.Competencias.FindAsync(idCompetencia);
            if (competencia == null) return false;

            competencia.Ativo = false;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
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
            query = query.Where(c => c.Ativo == ativo.Value);
        }

        return await query.OrderBy(c => c.IdCompetencia).ToListAsync();
    }

    public async Task<Competencia?> ObterPorIdAsync(int idCompetencia)
    {
        return await _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .FirstOrDefaultAsync(c => c.IdCompetencia == idCompetencia);
    }

    public async Task<Competencia?> ObterCompetenciaExistenteAsync(int idEmpresa, int idCargo, int idNivel, int idEixo, int idSubCompetencia, int idDimensao, string detalhamento)
    {
        return await _context.Competencias
            .FirstOrDefaultAsync(c => 
                c.IdEmpresa == idEmpresa &&
                c.IdCargo == idCargo &&
                c.IdNivel == idNivel &&
                c.IdEixo == idEixo &&
                c.IdSubCompetencia == idSubCompetencia &&
                c.IdDimensao == idDimensao &&
                c.CompetenciaJRDetalhe == detalhamento);
    }

    public async Task<List<CompetenciaExportModel>> ExportarAsync()
    {
        var competencias = await _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .ToListAsync();

        return competencias.Select(c => new CompetenciaExportModel
        {
            IdCompetencia = c.IdCompetencia,
            IdCargo = c.IdCargo,
            Cargo = c.Cargo?.Nome ?? string.Empty,
            IdEixo = c.IdEixo,
            Eixo = c.Eixo?.Nome ?? string.Empty,
            IdSubCompetencia = c.IdSubCompetencia,
            SubCompetencia = c.SubCompetencia?.Nome ?? string.Empty,
            IdDimensao = c.IdDimensao,
            Dimensao = c.Dimensao?.Nome ?? string.Empty,
            DetalheNivelAtual = c.CompetenciaJRDetalhe,
            CompetenciaAtual = c.CompetenciaJR,
            PalavrasChave = c.PalavrasChave ?? string.Empty,
            TipoAvaliacao = c.TipoAvaliacao,
            Escopo = c.Escopo,
            ATV = c.Ativo ? 1 : 0
        }).ToList();
    }

    public async Task<ImportResult> ImportarAsync(ExcelPackage package, int idEmpresa, int idUsuario)
    {
        return await _importExportUtil.ProcessarImportacaoAsync(package, idEmpresa, idUsuario);
    }

    public async Task<int> AgregarCompetenciaAvaliacoesAsync(int idCompetencia)
    {
        // Esta funcionalidade requer integração com o serviço de avaliações
        // Por enquanto, retornamos 0 como placeholder
        // TODO: Implementar quando o serviço de avaliações estiver disponível
        await Task.CompletedTask;
        return 0;
    }

    private async Task AtualizarRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubcompetencia, string descricao)
    {
        var relacao = await _context.RelacoesCargosSubcompetencias
            .FirstOrDefaultAsync(r => r.IdCargo == idCargo && r.IdSubcompetencia == idSubcompetencia);

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

    public async Task<RelacaoCargoSubcompetencia?> ObterRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubcompetencia)
    {
        return await _context.RelacoesCargosSubcompetencias
            .FirstOrDefaultAsync(r => r.IdCargo == idCargo && r.IdSubcompetencia == idSubcompetencia);
    }

    public async Task<List<AvaliacaoCompetenciaNota>> ObterNotasAvaliacaoAsync()
    {
        return await _context.AvaliacoesCompetenciasNotas
            .Where(n => n.IndFeedback != true && n.Ativo)
            .ToListAsync();
    }

    public async Task<List<ModoCalculoCompetencia>> ObterModosCalculoAsync()
    {
        return await _context.ModosCalculosCompetencias.ToListAsync();
    }
}