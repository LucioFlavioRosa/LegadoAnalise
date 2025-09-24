namespace Peers.Moderno.Services.Perfis.Common;

public static class StatusHelper
{
    public static string GetStatusText(bool ativo)
    {
        return ativo ? "Ativo" : "Inativo";
    }

    public static bool GetStatusValue(string statusText)
    {
        return string.Equals(statusText, "Ativo", StringComparison.OrdinalIgnoreCase);
    }

    public static int GetStatusInt(bool ativo)
    {
        return ativo ? 1 : 0;
    }

    public static bool GetStatusFromInt(int status)
    {
        return status == 1;
    }

    public static List<StatusOption> GetStatusOptions()
    {
        return new List<StatusOption>
        {
            new StatusOption { Value = true, Text = "Ativo" },
            new StatusOption { Value = false, Text = "Inativo" }
        };
    }
}

public class StatusOption
{
    public bool Value { get; set; }
    public string Text { get; set; } = string.Empty;
}