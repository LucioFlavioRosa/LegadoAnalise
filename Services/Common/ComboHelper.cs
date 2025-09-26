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

    public static List<ComboItem> GetNotasPerformanceItems(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "0", Text = "[Selecionar]", IsDisabled = true });
        }
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "1" },
            new ComboItem { Value = "2", Text = "2" },
            new ComboItem { Value = "3", Text = "3" },
            new ComboItem { Value = "4", Text = "4" },
            new ComboItem { Value = "5", Text = "N/A" }
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

    // ... (demais métodos permanecem iguais)

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

    // ... (demais métodos permanecem iguais)
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
