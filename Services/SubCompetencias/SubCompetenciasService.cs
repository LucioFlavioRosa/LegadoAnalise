using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.SubCompetencias;

public class SubCompetenciasService : ISubCompetenciasService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public SubCompetenciasService(
        ApplicationDbContext context,
        ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<List<SubCompetencia>> ListarSubCompetenciasAsync()
    {
        try
        {
            var subCompetencias = await _context.SubCompetencias
                .AsNoTracking()
                .OrderBy(s => s.Nome)
                .ToListAsync();

            _telemetryService.TrackEvent("SubCompetenciasListadas", new Dictionary<string, string>
            {
                { "Count", subCompetencias.Count.ToString() }
            });

            return subCompetencias;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarSubCompetenciasAsync" },
                { "Component", "SubCompetenciasService" }
            });
            throw;
        }
    }

    public async Task<List<SubCompetencia>> ListarSubCompetenciasAtivasAsync()
    {
        try
        {
            var subCompetencias = await _context.SubCompetencias
                .AsNoTracking()
                .Where(s => s.Ativo)
                .OrderBy(s => s.Nome)
                .ToListAsync();

            return subCompetencias;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarSubCompetenciasAtivasAsync" },
                { "Component", "SubCompetenciasService" }
            });
            throw;
        }
    }

    public async Task<SubCompetencia?> ObterSubCompetenciaAsync(int id)
    {
        try
        {
            return await _context.SubCompetencias
                .FirstOrDefaultAsync(s => s.IdSubCompetencia == id);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterSubCompetenciaAsync" },
                { "Component", "SubCompetenciasService" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InserirSubCompetenciaAsync(SubCompetencia subCompetencia)
    {
        try
        {
            subCompetencia.DHC = DateTime.Now;
            subCompetencia.Ativo = true;

            _context.SubCompetencias.Add(subCompetencia);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("SubCompetenciaInserida", new Dictionary<string, string>
                {
                    { "Id", subCompetencia.IdSubCompetencia.ToString() },
                    { "Nome", subCompetencia.Nome },
                    { "Usuario", subCompetencia.USR.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InserirSubCompetenciaAsync" },
                { "Component", "SubCompetenciasService" },
                { "Nome", subCompetencia?.Nome ?? "Unknown" }
            });
            throw;
        }
    }

    public async Task<bool> AlterarSubCompetenciaAsync(SubCompetencia subCompetencia)
    {
        try
        {
            var subCompetenciaExistente = await ObterSubCompetenciaAsync(subCompetencia.IdSubCompetencia);
            if (subCompetenciaExistente == null)
                return false;

            subCompetenciaExistente.Nome = subCompetencia.Nome;
            subCompetenciaExistente.Ativo = subCompetencia.Ativo;
            subCompetenciaExistente.DHC = DateTime.Now;
            subCompetenciaExistente.USR = subCompetencia.USR;
            subCompetenciaExistente.TipoAvaliacao = subCompetencia.TipoAvaliacao;

            _context.SubCompetencias.Update(subCompetenciaExistente);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("SubCompetenciaAlterada", new Dictionary<string, string>
                {
                    { "Id", subCompetencia.IdSubCompetencia.ToString() },
                    { "Nome", subCompetencia.Nome },
                    { "Usuario", subCompetencia.USR.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AlterarSubCompetenciaAsync" },
                { "Component", "SubCompetenciasService" },
                { "Id", subCompetencia?.IdSubCompetencia.ToString() ?? "Unknown" }
            });
            throw;
        }
    }

    public async Task<bool> ExcluirSubCompetenciaAsync(int id)
    {
        try
        {
            var subCompetencia = await ObterSubCompetenciaAsync(id);
            if (subCompetencia == null)
                return false;

            _context.SubCompetencias.Remove(subCompetencia);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("SubCompetenciaExcluida", new Dictionary<string, string>
                {
                    { "Id", id.ToString() },
                    { "Nome", subCompetencia.Nome }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExcluirSubCompetenciaAsync" },
                { "Component", "SubCompetenciasService" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InativarSubCompetenciaAsync(int id)
    {
        try
        {
            var subCompetencia = await ObterSubCompetenciaAsync(id);
            if (subCompetencia == null)
                return false;

            subCompetencia.Ativo = false;
            subCompetencia.DHC = DateTime.Now;

            _context.SubCompetencias.Update(subCompetencia);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("SubCompetenciaInativada", new Dictionary<string, string>
                {
                    { "Id", id.ToString() },
                    { "Nome", subCompetencia.Nome }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InativarSubCompetenciaAsync" },
                { "Component", "SubCompetenciasService" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> ExisteSubCompetenciaAsync(string nome, int? idExcluir = null)
    {
        try
        {
            var query = _context.SubCompetencias.Where(s => s.Nome.ToLower() == nome.ToLower());
            
            if (idExcluir.HasValue)
            {
                query = query.Where(s => s.IdSubCompetencia != idExcluir.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExisteSubCompetenciaAsync" },
                { "Component", "SubCompetenciasService" },
                { "Nome", nome }
            });
            throw;
        }
    }

    public async Task<int> ContarSubCompetenciasAtivasAsync()
    {
        try
        {
            return await _context.SubCompetencias
                .CountAsync(s => s.Ativo);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ContarSubCompetenciasAtivasAsync" },
                { "Component", "SubCompetenciasService" }
            });
            throw;
        }
    }

    public async Task<List<SubCompetencia>> BuscarSubCompetenciasPorNomeAsync(string nome)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nome))
                return new List<SubCompetencia>();

            return await _context.SubCompetencias
                .AsNoTracking()
                .Where(s => s.Nome.Contains(nome))
                .OrderBy(s => s.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "BuscarSubCompetenciasPorNomeAsync" },
                { "Component", "SubCompetenciasService" },
                { "Nome", nome }
            });
            throw;
        }
    }
}