using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class CompetenciasService : ICompetenciasService
{
    private readonly ApplicationDbContext _context;

    public CompetenciasService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Competencia>> ObterListaCompetenciasAsync(bool? ativo = null)
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

    public async Task<Competencia?> ObterCompetenciaAsync(int id)
    {
        return await _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .FirstOrDefaultAsync(c => c.IdCompetencia == id);
    }

    public async Task<Competencia?> ObterCompetenciaAsync(int idEmpresa, int idCargo, int idNivel, int idEixo, int idSubCompetencia, int idDimensao, string detalhe)
    {
        return await _context.Competencias
            .FirstOrDefaultAsync(c => c.IdEmpresa == idEmpresa &&
                                    c.IdCargo == idCargo &&
                                    c.IdNivel == idNivel &&
                                    c.IdEixo == idEixo &&
                                    c.IdSubCompetencia == idSubCompetencia &&
                                    c.IdDimensao == idDimensao &&
                                    c.CompetenciaJRDetalhe == detalhe);
    }

    public async Task<bool> InserirCompetenciaAsync(Competencia competencia)
    {
        try
        {
            _context.Competencias.Add(competencia);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AlterarCompetenciaAsync(Competencia competencia)
    {
        try
        {
            _context.Competencias.Update(competencia);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ExcluirCompetenciaAsync(int id)
    {
        try
        {
            var competencia = await _context.Competencias.FindAsync(id);
            if (competencia != null)
            {
                competencia.ATV = 0;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<CompetenciaExportModel>> ObterCompetenciasParaExportAsync()
    {
        return await _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .Select(c => new CompetenciaExportModel
            {
                IdCompetencia = c.IdCompetencia,
                IdCargo = c.IdCargo,
                Cargo = c.Cargo!.Nome,
                IdEixo = c.IdEixo,
                Eixo = c.Eixo!.Nome,
                IdSubCompetencia = c.IdSubCompetencia,
                SubCompetencia = c.SubCompetencia!.Nome,
                IdDimensao = c.IdDimensao,
                Dimensao = c.Dimensao!.Nome,
                DetalheNivelAtual = c.CompetenciaJRDetalhe,
                CompetenciaAtual = c.CompetenciaJR,
                ATV = c.ATV,
                TipoAvaliacao = c.TipoAvaliacao,
                Escopo = c.Escopo,
                PalavrasChave = c.PalavrasChave
            })
            .ToListAsync();
    }
}