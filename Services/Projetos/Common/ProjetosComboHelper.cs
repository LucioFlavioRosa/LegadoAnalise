using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Projetos.Common;

public interface IProjetosComboHelper
{
    Task<List<ComboItem>> GetClientesAsync();
    Task<List<ComboItem>> GetClientesAsync(bool incluirSelecionar);
    Task<List<ComboItem>> GetGestoresAsync();
    Task<List<ComboItem>> GetGestoresAsync(bool incluirSelecionar);
    Task<List<ComboItem>> GetResponsaveisAsync();
    Task<List<ComboItem>> GetResponsaveisAsync(bool incluirSelecionar);
    Task<List<ComboItem>> GetTiposProjetoAsync();
    Task<List<ComboItem>> GetTiposProjetoAsync(bool incluirSelecionar);
    Task<List<ComboItem>> GetComplexidadesAsync();
    Task<List<ComboItem>> GetComplexidadesAsync(bool incluirSelecionar);
    Task<List<ComboItem>> GetAssociadosAsync();
    Task<List<ComboItem>> GetAssociadosAsync(bool incluirSelecionar);
    Task<List<ComboItem>> GetAssociadosAsync(bool incluirSelecionar, bool apenasAtivos);
    Task<List<ComboItem>> GetStatusProjetoAsync();
    Task<List<ComboItem>> GetAssociadosPorPerfilAsync(int idPerfil);
    Task<List<ComboItem>> GetAssociadosPorPerfilAsync(int idPerfil, bool incluirSelecionar);
}

public class ProjetosComboHelper : IProjetosComboHelper
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public ProjetosComboHelper(
        ApplicationDbContext context,
        ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<List<ComboItem>> GetClientesAsync()
    {
        return await GetClientesAsync(true);
    }

    public async Task<List<ComboItem>> GetClientesAsync(bool incluirSelecionar)
    {
        try
        {
            var clientes = await _context.Clientes
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new ComboItem
                {
                    Value = c.IdCliente.ToString(),
                    Text = c.Nome
                })
                .ToListAsync();

            if (incluirSelecionar)
            {
                clientes.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
            }

            _telemetryService.TrackEvent("ClientesComboCarregado", new Dictionary<string, string>
            {
                { "TotalClientes", clientes.Count.ToString() },
                { "IncluirSelecionar", incluirSelecionar.ToString() }
            });

            return clientes;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetClientesAsync" },
                { "Component", "ProjetosComboHelper" }
            });
            return incluirSelecionar ? new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } } : new List<ComboItem>();
        }
    }

    public async Task<List<ComboItem>> GetGestoresAsync()
    {
        return await GetGestoresAsync(true);
    }

    public async Task<List<ComboItem>> GetGestoresAsync(bool incluirSelecionar)
    {
        return await GetAssociadosPorPerfilAsync(2, incluirSelecionar);
    }

    public async Task<List<ComboItem>> GetResponsaveisAsync()
    {
        return await GetResponsaveisAsync(true);
    }

    public async Task<List<ComboItem>> GetResponsaveisAsync(bool incluirSelecionar)
    {
        return await GetAssociadosPorPerfilAsync(3, incluirSelecionar);
    }

    public async Task<List<ComboItem>> GetTiposProjetoAsync()
    {
        return await GetTiposProjetoAsync(true);
    }

    public async Task<List<ComboItem>> GetTiposProjetoAsync(bool incluirSelecionar)
    {
        try
        {
            var tipos = await _context.TiposProjetos
                .Where(t => t.Ativo)
                .OrderBy(t => t.Nome)
                .Select(t => new ComboItem
                {
                    Value = t.IdTipo.ToString(),
                    Text = t.Nome
                })
                .ToListAsync();

            if (incluirSelecionar)
            {
                tipos.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
            }

            _telemetryService.TrackEvent("TiposProjetoComboCarregado", new Dictionary<string, string>
            {
                { "TotalTipos", tipos.Count.ToString() },
                { "IncluirSelecionar", incluirSelecionar.ToString() }
            });

            return tipos;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetTiposProjetoAsync" },
                { "Component", "ProjetosComboHelper" }
            });
            return incluirSelecionar ? new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } } : new List<ComboItem>();
        }
    }

    public async Task<List<ComboItem>> GetComplexidadesAsync()
    {
        return await GetComplexidadesAsync(true);
    }

    public async Task<List<ComboItem>> GetComplexidadesAsync(bool incluirSelecionar)
    {
        try
        {
            var complexidades = await _context.ProjetosComplexidades
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new ComboItem
                {
                    Value = c.IdComplexidade.ToString(),
                    Text = c.Nome
                })
                .ToListAsync();

            if (incluirSelecionar)
            {
                complexidades.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
            }

            _telemetryService.TrackEvent("ComplexidadesComboCarregado", new Dictionary<string, string>
            {
                { "TotalComplexidades", complexidades.Count.ToString() },
                { "IncluirSelecionar", incluirSelecionar.ToString() }
            });

            return complexidades;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetComplexidadesAsync" },
                { "Component", "ProjetosComboHelper" }
            });
            return incluirSelecionar ? new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } } : new List<ComboItem>();
        }
    }

    public async Task<List<ComboItem>> GetAssociadosAsync()
    {
        return await GetAssociadosAsync(true, true);
    }

    public async Task<List<ComboItem>> GetAssociadosAsync(bool incluirSelecionar)
    {
        return await GetAssociadosAsync(incluirSelecionar, true);
    }

    public async Task<List<ComboItem>> GetAssociadosAsync(bool incluirSelecionar, bool apenasAtivos)
    {
        try
        {
            var query = _context.Associados.AsQueryable();

            if (apenasAtivos)
            {
                query = query.Where(a => a.Ativo);
            }

            var associados = await query
                .OrderBy(a => a.Nome)
                .Select(a => new ComboItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                })
                .ToListAsync();

            if (incluirSelecionar)
            {
                associados.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
            }

            _telemetryService.TrackEvent("AssociadosComboCarregado", new Dictionary<string, string>
            {
                { "TotalAssociados", associados.Count.ToString() },
                { "IncluirSelecionar", incluirSelecionar.ToString() },
                { "ApenasAtivos", apenasAtivos.ToString() }
            });

            return associados;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetAssociadosAsync" },
                { "Component", "ProjetosComboHelper" }
            });
            return incluirSelecionar ? new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } } : new List<ComboItem>();
        }
    }

    public async Task<List<ComboItem>> GetStatusProjetoAsync()
    {
        try
        {
            var statusList = new List<ComboItem>
            {
                new ComboItem { Value = "", Text = "[Selecionar]" },
                new ComboItem { Value = "1", Text = "Ativo" },
                new ComboItem { Value = "0", Text = "Inativo" }
            };

            _telemetryService.TrackEvent("StatusProjetoComboCarregado", new Dictionary<string, string>
            {
                { "TotalStatus", statusList.Count.ToString() }
            });

            return statusList;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetStatusProjetoAsync" },
                { "Component", "ProjetosComboHelper" }
            });
            return new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        }
    }

    public async Task<List<ComboItem>> GetAssociadosPorPerfilAsync(int idPerfil)
    {
        return await GetAssociadosPorPerfilAsync(idPerfil, true);
    }

    public async Task<List<ComboItem>> GetAssociadosPorPerfilAsync(int idPerfil, bool incluirSelecionar)
    {
        try
        {
            var associados = await _context.Associados
                .Where(a => a.IdPerfil == idPerfil && a.Ativo)
                .OrderBy(a => a.Nome)
                .Select(a => new ComboItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                })
                .ToListAsync();

            if (incluirSelecionar)
            {
                associados.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
            }

            _telemetryService.TrackEvent("AssociadosPorPerfilComboCarregado", new Dictionary<string, string>
            {
                { "PerfilId", idPerfil.ToString() },
                { "TotalAssociados", associados.Count.ToString() },
                { "IncluirSelecionar", incluirSelecionar.ToString() }
            });

            return associados;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetAssociadosPorPerfilAsync" },
                { "Component", "ProjetosComboHelper" },
                { "PerfilId", idPerfil.ToString() }
            });
            return incluirSelecionar ? new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } } : new List<ComboItem>();
        }
    }
}