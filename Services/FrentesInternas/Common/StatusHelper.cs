namespace Peers.Moderno.Services.FrentesInternas.Common;

public class StatusHelper : IStatusHelper
{
    public string ConvertStatusToText(bool ativo)
    {
        return ativo ? "Ativo" : "Inativo";
    }

    public string ConvertStatusToText(int status)
    {
        return status == 1 ? "Ativo" : "Inativo";
    }

    public bool ConvertTextToStatus(string statusText)
    {
        return string.Equals(statusText, "Ativo", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(statusText, "True", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(statusText, "Verdadeiro", StringComparison.OrdinalIgnoreCase);
    }

    public int ConvertBoolToInt(bool ativo)
    {
        return ativo ? 1 : 0;
    }

    public bool ConvertIntToBool(int status)
    {
        return status == 1;
    }

    public List<StatusItem> GetStatusItems()
    {
        return new List<StatusItem>
        {
            new StatusItem { Value = "1", Text = "Ativo", Selected = true },
            new StatusItem { Value = "0", Text = "Inativo", Selected = false }
        };
    }
}