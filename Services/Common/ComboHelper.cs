using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Common;

public static class ComboHelper
{
    public static List<string> GetTiposAvaliacao()
    {
        return new List<string> { "[Selecionar]", "desempenho" };
    }

    public static List<string> GetEscopos()
    {
        return new List<string> { "[Selecionar]", "projeto" };
    }

    public static List<ComboItem> GetTiposAvaliacaoItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "desempenho", Text = "desempenho" }
        };
    }

    public static List<ComboItem> GetEscoposItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "projeto", Text = "projeto" }
        };
    }

    public static List<ComboItem> GetStatusItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
    }

    public static List<ComboItem> GetNotasCompetenciaItems(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "0", Text = "[Selecionar]", IsDisabled = true, AdditionalData = { ["Peso"] = 0 } });
        }
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "1", AdditionalData = { ["Peso"] = 1 } },
            new ComboItem { Value = "2", Text = "2", AdditionalData = { ["Peso"] = 2 } },
            new ComboItem { Value = "3", Text = "3", AdditionalData = { ["Peso"] = 3 } },
            new ComboItem { Value = "4", Text = "4", AdditionalData = { ["Peso"] = 4 } },
            new ComboItem { Value = "5", Text = "N/A", AdditionalData = { ["Peso"] = 5 } }
        });
        return items;
    }

    public static List<ComboItem> GetNotasPerformanceItems(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "0", Text = "[Selecionar]", IsDisabled = true, AdditionalData = { ["Peso"] = 0 } });
        }
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "1", AdditionalData = { ["Peso"] = 1 } },
            new ComboItem { Value = "2", Text = "2", AdditionalData = { ["Peso"] = 2 } },
            new ComboItem { Value = "3", Text = "3", AdditionalData = { ["Peso"] = 3 } },
            new ComboItem { Value = "4", Text = "4", AdditionalData = { ["Peso"] = 4 } },
            new ComboItem { Value = "5", Text = "N/A", AdditionalData = { ["Peso"] = 5 } }
        });
        return items;
    }

    public static List<ComboItem> GetAbrangenciaPerformanceItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "Individual", Text = "Individual" },
            new ComboItem { Value = "Coletivo", Text = "Coletivo" }
        };
    }

    public static ComboItem GetDefaultSelectionItem()
    {
        return new ComboItem { Value = "", Text = "[Selecionar]" };
    }

    public static bool IsValidSelection(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "";
    }

    public static string GetSelectedText(List<ComboItem> items, string? selectedValue)
    {
        if (string.IsNullOrEmpty(selectedValue))
            return "[Selecionar]";
        var item = items.FirstOrDefault(i => i.Value == selectedValue);
        return item?.Text ?? "[Selecionar]";
    }

    public static string GetStatusText(int status)
    {
        return status == 1 ? "Ativo" : "Inativo";
    }

    // Métodos para combos dinâmicos de Projetos, Clientes, Períodos e Status
    public static async Task<List<ComboItem>> GetProjetosComboAsync(ApplicationDbContext db, int? gestorId = null, int? clienteId = null, int? status = null, int? periodoId = null)
    {
        var query = db.Projetos.AsQueryable();
        if (gestorId.HasValue)
            query = query.Where(p => p.IdAssociadoGestor == gestorId.Value);
        if (clienteId.HasValue)
            query = query.Where(p => p.IdCliente == clienteId.Value);
        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);
        if (periodoId.HasValue)
            query = query.Where(p => p.AssociadosProjeto.Any(ap => ap.IdPeriodo == periodoId.Value));
        var projetos = await query.OrderBy(p => p.Nome).ToListAsync();
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        items.AddRange(projetos.Select(p => new ComboItem { Value = p.Id.ToString(), Text = p.Nome }));
        return items;
    }

    public static async Task<List<ComboItem>> GetClientesComboAsync(ApplicationDbContext db)
    {
        var clientes = await db.Clientes.OrderBy(c => c.Nome).ToListAsync();
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        items.AddRange(clientes.Select(c => new ComboItem { Value = c.IdCliente.ToString(), Text = c.Nome }));
        return items;
    }

    public static async Task<List<ComboItem>> GetPeriodosComboAsync(ApplicationDbContext db, int? empresaId = null)
    {
        var query = db.PeriodosAvaliacoes.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(p => p.IdEmpresa == empresaId.Value);
        var periodos = await query.OrderByDescending(p => p.IdPeriodo).ToListAsync();
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        items.AddRange(periodos.Select(p => new ComboItem { Value = p.IdPeriodo.ToString(), Text = p.Nome }));
        return items;
    }

    public static async Task<List<ComboItem>> GetStatusComboAsync(ApplicationDbContext db)
    {
        // Supondo que status de projetos estejam em uma tabela ou enum
        var statusList = new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
        return await Task.FromResult(statusList);
    }
}

public class ComboItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsSelected { get; set; } = false;
    public bool IsDisabled { get; set; } = false;
    public string CssClass { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
}
