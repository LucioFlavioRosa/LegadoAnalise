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

    public static List<ComboItem> GetStatusProjetoItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
    }

    public static List<ComboItem> GetGenericStatusItems(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        });
        
        return items;
    }

    public static List<ComboItem> CreateComboFromList<T>(IEnumerable<T> items, Func<T, string> valueSelector, Func<T, string> textSelector, bool includeSelecionar = true)
    {
        var comboItems = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            comboItems.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        comboItems.AddRange(items.Select(item => new ComboItem
        {
            Value = valueSelector(item),
            Text = textSelector(item)
        }));
        
        return comboItems;
    }

    public static List<ComboItem> CreateComboFromDictionary(Dictionary<string, string> items, bool includeSelecionar = true)
    {
        var comboItems = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            comboItems.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        comboItems.AddRange(items.Select(kvp => new ComboItem
        {
            Value = kvp.Key,
            Text = kvp.Value
        }));
        
        return comboItems;
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

    // Métodos específicos para SubCompetências
    public static List<ComboItem> GetSubCompetenciasStatusItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
    }

    public static List<ComboItem> GetTiposAvaliacaoSubCompetencias()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "desempenho", Text = "Desempenho" },
            new ComboItem { Value = "lideranca", Text = "Liderança" }
        };
    }

    public static string GetStatusText(bool ativo)
    {
        return ativo ? "Ativo" : "Inativo";
    }

    public static string GetStatusBadgeClass(bool ativo)
    {
        return ativo ? "badge-success" : "badge-secondary";
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