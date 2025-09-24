using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Premissas.Common;

namespace Peers.Moderno.Services.Premissas;

public class PremissasService : IPremissasService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;
    private readonly IUserContextService _userContextService;

    public PremissasService(
        ApplicationDbContext context,
        ITelemetryService telemetryService,
        IUserContextService userContextService)
    {
        _context = context;
        _telemetryService = telemetryService;
        _userContextService = userContextService;
    }

    public async Task<List<PremissasRadar>> ListarAsync()
    {
        try
        {
            var premissas = await _context.PremissasRadar
                .Include(p => p.Eixo)
                .Include(p => p.Cargo)
                .Include(p => p.CargoNivel)
                .OrderBy(p => p.Eixo!.Nome)
                .ThenBy(p => p.Cargo!.Nome)
                .ThenBy(p => p.CargoNivel!.Nivel)
                .ToListAsync();

            _telemetryService.TrackEvent("PremissasListadas", new Dictionary<string, string>
            {
                { "Count", premissas.Count.ToString() }
            });

            return premissas;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarAsync" },
                { "Component", "PremissasService" }
            });
            throw;
        }
    }

    public async Task<bool> InserirAsync(PremissasRadar premissa)
    {
        try
        {
            var usuario = await _userContextService.GetUsuarioLogadoAsync();
            if (usuario == null)
                return false;

            if (await ExistePremissaAsync(premissa.IdEixo, premissa.IdCargo, premissa.IdNivel))
                return false;

            premissa.IdEmpresa = usuario.IdEmpresa ?? 0;
            premissa.USR = usuario.Id;
            premissa.DHC = DateTime.Now;
            premissa.ATV = 1;

            _context.PremissasRadar.Add(premissa);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PremissaInserida", new Dictionary<string, string>
                {
                    { "IdPremissa", premissa.IdPremissa.ToString() },
                    { "UserId", usuario.Id.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InserirAsync" },
                { "Component", "PremissasService" }
            });
            return false;
        }
    }

    public async Task<bool> AlterarAsync(PremissasRadar premissa)
    {
        try
        {
            var usuario = await _userContextService.GetUsuarioLogadoAsync();
            if (usuario == null)
                return false;

            var premissaExistente = await _context.PremissasRadar
                .FirstOrDefaultAsync(p => p.IdPremissa == premissa.IdPremissa);

            if (premissaExistente == null)
                return false;

            if (await ExistePremissaAsync(premissa.IdEixo, premissa.IdCargo, premissa.IdNivel, premissa.IdPremissa))
                return false;

            premissaExistente.IdEixo = premissa.IdEixo;
            premissaExistente.IdCargo = premissa.IdCargo;
            premissaExistente.IdNivel = premissa.IdNivel;
            premissaExistente.ValorRadarPeers = premissa.ValorRadarPeers;
            premissaExistente.ValorBaseAutoAvaliacao = premissa.ValorBaseAutoAvaliacao;
            premissaExistente.ValorBaseAvaliacaoGestor = premissa.ValorBaseAvaliacaoGestor;
            premissaExistente.USR = usuario.Id;
            premissaExistente.DHC = DateTime.Now;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PremissaAlterada", new Dictionary<string, string>
                {
                    { "IdPremissa", premissa.IdPremissa.ToString() },
                    { "UserId", usuario.Id.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AlterarAsync" },
                { "Component", "PremissasService" }
            });
            return false;
        }
    }

    public async Task<bool> InativarAsync(int id)
    {
        try
        {
            var premissa = await _context.PremissasRadar
                .FirstOrDefaultAsync(p => p.IdPremissa == id);

            if (premissa == null)
                return false;

            premissa.ATV = premissa.ATV == 1 ? 0 : 1;
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PremissaInativada", new Dictionary<string, string>
                {
                    { "IdPremissa", id.ToString() },
                    { "NovoStatus", premissa.ATV.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InativarAsync" },
                { "Component", "PremissasService" }
            });
            return false;
        }
    }

    public async Task<PremissasRadar?> ObterAsync(int id)
    {
        try
        {
            return await _context.PremissasRadar
                .Include(p => p.Eixo)
                .Include(p => p.Cargo)
                .Include(p => p.CargoNivel)
                .FirstOrDefaultAsync(p => p.IdPremissa == id);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterAsync" },
                { "Component", "PremissasService" },
                { "IdPremissa", id.ToString() }
            });
            return null;
        }
    }

    public async Task<List<Eixo>> ListarEixosAsync()
    {
        try
        {
            return await _context.Eixos
                .Where(e => e.Ativo == 1)
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarEixosAsync" },
                { "Component", "PremissasService" }
            });
            return new List<Eixo>();
        }
    }

    public async Task<List<Cargo>> ListarCargosAsync()
    {
        try
        {
            return await _context.Cargos
                .Where(c => c.Ativo == 1)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarCargosAsync" },
                { "Component", "PremissasService" }
            });
            return new List<Cargo>();
        }
    }

    public async Task<List<CargoNivel>> ListarNiveisAsync()
    {
        try
        {
            return await _context.CargosNiveis
                .Where(n => n.Ativo == 1)
                .OrderBy(n => n.Nivel)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarNiveisAsync" },
                { "Component", "PremissasService" }
            });
            return new List<CargoNivel>();
        }
    }

    public async Task<bool> ExistePremissaAsync(int idEixo, int idCargo, int idNivel, int? idPremissaExcluir = null)
    {
        try
        {
            var query = _context.PremissasRadar
                .Where(p => p.IdEixo == idEixo && p.IdCargo == idCargo && p.IdNivel == idNivel);

            if (idPremissaExcluir.HasValue)
            {
                query = query.Where(p => p.IdPremissa != idPremissaExcluir.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExistePremissaAsync" },
                { "Component", "PremissasService" }
            });
            return false;
        }
    }
}