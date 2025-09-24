using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Projetos.Common;

namespace Peers.Moderno.Services.Projetos;

public class TiposProjetosService : ITiposProjetosService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public TiposProjetosService(
        ApplicationDbContext context,
        ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<List<TipoProjeto>> ListarAsync()
    {
        try
        {
            var tipos = await _context.TiposProjetos
                .OrderBy(t => t.Nome)
                .ToListAsync();

            _telemetryService.TrackEvent("TiposProjetosListados", new Dictionary<string, string>
            {
                { "Count", tipos.Count.ToString() }
            });

            return tipos;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarAsync" },
                { "Component", "TiposProjetosService" }
            });
            throw;
        }
    }

    public async Task<List<TipoProjeto>> ListarAtivosAsync()
    {
        try
        {
            var tipos = await _context.TiposProjetos
                .Where(t => t.ATV == 1)
                .OrderBy(t => t.Nome)
                .ToListAsync();

            return tipos;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarAtivosAsync" },
                { "Component", "TiposProjetosService" }
            });
            throw;
        }
    }

    public async Task<TipoProjeto?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _context.TiposProjetos
                .FirstOrDefaultAsync(t => t.IdTipo == id);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPorIdAsync" },
                { "Component", "TiposProjetosService" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InserirAsync(TipoProjeto tipoProjeto)
    {
        try
        {
            tipoProjeto.DHC = DateTime.Now;
            tipoProjeto.ATV = 1;

            _context.TiposProjetos.Add(tipoProjeto);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("TipoProjetoInserido", new Dictionary<string, string>
                {
                    { "Id", tipoProjeto.IdTipo.ToString() },
                    { "Nome", tipoProjeto.Nome }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InserirAsync" },
                { "Component", "TiposProjetosService" },
                { "Nome", tipoProjeto?.Nome ?? "Unknown" }
            });
            throw;
        }
    }

    public async Task<bool> AlterarAsync(TipoProjeto tipoProjeto)
    {
        try
        {
            var tipoExistente = await ObterPorIdAsync(tipoProjeto.IdTipo);
            if (tipoExistente == null)
                return false;

            tipoExistente.Nome = tipoProjeto.Nome;
            tipoExistente.ATV = tipoProjeto.ATV;
            tipoExistente.DHC = DateTime.Now;

            _context.TiposProjetos.Update(tipoExistente);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("TipoProjetoAlterado", new Dictionary<string, string>
                {
                    { "Id", tipoProjeto.IdTipo.ToString() },
                    { "Nome", tipoProjeto.Nome }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AlterarAsync" },
                { "Component", "TiposProjetosService" },
                { "Id", tipoProjeto?.IdTipo.ToString() ?? "Unknown" }
            });
            throw;
        }
    }

    public async Task<bool> InativarAsync(int id)
    {
        try
        {
            var tipo = await ObterPorIdAsync(id);
            if (tipo == null)
                return false;

            tipo.ATV = 0;
            tipo.DHC = DateTime.Now;

            _context.TiposProjetos.Update(tipo);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("TipoProjetoInativado", new Dictionary<string, string>
                {
                    { "Id", id.ToString() },
                    { "Nome", tipo.Nome }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InativarAsync" },
                { "Component", "TiposProjetosService" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? idExcluir = null)
    {
        try
        {
            var query = _context.TiposProjetos.Where(t => t.Nome == nome);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(t => t.IdTipo != idExcluir.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExisteNomeAsync" },
                { "Component", "TiposProjetosService" },
                { "Nome", nome }
            });
            throw;
        }
    }
}