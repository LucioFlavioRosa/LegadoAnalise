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
}

public class ComboItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}