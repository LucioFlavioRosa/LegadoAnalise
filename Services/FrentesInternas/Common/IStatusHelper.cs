namespace Peers.Moderno.Services.FrentesInternas.Common;

public interface IStatusHelper
{
    string ConvertStatusToText(bool ativo);
    string ConvertStatusToText(int status);
    bool ConvertTextToStatus(string statusText);
    int ConvertBoolToInt(bool ativo);
    bool ConvertIntToBool(int status);
    List<StatusItem> GetStatusItems();
}

public class StatusItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; }
}