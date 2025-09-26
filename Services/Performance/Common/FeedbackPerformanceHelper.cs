namespace Peers.Moderno.Services.Performance.Common;

public static class FeedbackPerformanceHelper
{
    public static string TruncaTexto(string texto, int maxLength)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;
        if (texto.Length <= maxLength)
            return texto;
        return texto.Substring(0, maxLength) + "...";
    }

    public static string GetDisclaimerInputText(bool inputAutoavaliacao)
    {
        return inputAutoavaliacao ? string.Empty : "Esta nota não requer preenchimento do avaliado";
    }

    public static string GetSeparadorAbrangencia(string abrangencia)
    {
        if (string.IsNullOrEmpty(abrangencia))
            return string.Empty;
        return $"data-abrangencia='{abrangencia}'";
    }
}
