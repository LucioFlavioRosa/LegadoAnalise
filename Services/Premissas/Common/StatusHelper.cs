namespace Peers.Moderno.Services.Premissas.Common;

public static class StatusHelper
{
    public static string StatusRadar(int atv)
    {
        return atv == 0 ? "Inativo" : "Ativo";
    }

    public static string GetStatusClass(int atv)
    {
        return atv == 1 ? "badge badge-success" : "badge badge-secondary";
    }

    public static string GetStatusIcon(int atv)
    {
        return atv == 1 ? "fas fa-check-circle text-success" : "fas fa-times-circle text-secondary";
    }

    public static bool IsAtivo(int atv)
    {
        return atv == 1;
    }

    public static int ToggleStatus(int currentStatus)
    {
        return currentStatus == 1 ? 0 : 1;
    }

    public static List<(int Value, string Text)> GetStatusOptions()
    {
        return new List<(int Value, string Text)>
        {
            (1, "Ativo"),
            (0, "Inativo")
        };
    }
}