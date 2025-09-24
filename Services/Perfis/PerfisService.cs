using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Perfis.Common;

namespace Peers.Moderno.Services.Perfis;

public class PerfisService : IPerfisService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public PerfisService(ApplicationDbContext context, ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<List<PerfilDto>> ListarPerfisAsync()
    {
        try
        {
            var perfis = await _context.Perfis
                .OrderBy(p => p.Nome)
                .ToListAsync();

            var result = perfis.Select(PerfisMapper.ToDto).ToList();

            _telemetryService.TrackEvent("PerfisListados", new Dictionary<string, string>
            {
                { "Count", result.Count.ToString() }
            });

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarPerfisAsync" },
                { "Component", "PerfisService" }
            });
            throw;
        }
    }

    public async Task<PerfilDto?> ObterPerfilAsync(int id)
    {
        try
        {
            var perfil = await _context.Perfis
                .FirstOrDefaultAsync(p => p.Id == id);

            if (perfil == null)
                return null;

            var result = PerfisMapper.ToDto(perfil);

            _telemetryService.TrackEvent("PerfilObtido", new Dictionary<string, string>
            {
                { "PerfilId", id.ToString() }
            });

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPerfilAsync" },
                { "Component", "PerfisService" },
                { "PerfilId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InserirPerfilAsync(PerfilDto perfilDto)
    {
        try
        {
            if (await ExistePerfilAsync(perfilDto.Nome))
                return false;

            var perfil = PerfisMapper.ToEntity(perfilDto);
            perfil.DHC = DateTime.Now;

            _context.Perfis.Add(perfil);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerfilInserido", new Dictionary<string, string>
                {
                    { "PerfilNome", perfilDto.Nome },
                    { "Status", perfilDto.Ativo.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InserirPerfilAsync" },
                { "Component", "PerfisService" },
                { "PerfilNome", perfilDto.Nome }
            });
            throw;
        }
    }

    public async Task<bool> AlterarPerfilAsync(PerfilDto perfilDto)
    {
        try
        {
            if (await ExistePerfilAsync(perfilDto.Nome, perfilDto.Id))
                return false;

            var perfil = await _context.Perfis
                .FirstOrDefaultAsync(p => p.Id == perfilDto.Id);

            if (perfil == null)
                return false;

            perfil.Nome = perfilDto.Nome;
            perfil.Ativo = perfilDto.Ativo;
            perfil.DHC = DateTime.Now;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerfilAlterado", new Dictionary<string, string>
                {
                    { "PerfilId", perfilDto.Id.ToString() },
                    { "PerfilNome", perfilDto.Nome },
                    { "Status", perfilDto.Ativo.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AlterarPerfilAsync" },
                { "Component", "PerfisService" },
                { "PerfilId", perfilDto.Id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InativarPerfilAsync(int id)
    {
        try
        {
            var perfil = await _context.Perfis
                .FirstOrDefaultAsync(p => p.Id == id);

            if (perfil == null)
                return false;

            perfil.Ativo = false;
            perfil.DHC = DateTime.Now;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerfilInativado", new Dictionary<string, string>
                {
                    { "PerfilId", id.ToString() },
                    { "PerfilNome", perfil.Nome }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InativarPerfilAsync" },
                { "Component", "PerfisService" },
                { "PerfilId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> ExistePerfilAsync(string nome, int? idExcluir = null)
    {
        try
        {
            var query = _context.Perfis.Where(p => p.Nome.ToLower() == nome.ToLower());

            if (idExcluir.HasValue)
                query = query.Where(p => p.Id != idExcluir.Value);

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExistePerfilAsync" },
                { "Component", "PerfisService" },
                { "Nome", nome }
            });
            throw;
        }
    }
}