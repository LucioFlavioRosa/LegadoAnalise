namespace Services.PDI.Common;

public interface IPDIHelper
{
    string NormalizeResposta(string resposta);
    string GetTituloStyle(string titulo);
    string GetSubtituloStyle(string subtitulo);
    string GetBorderStyle(bool bordaEsquerda, bool bordaDireita, bool bordaCima, bool bordaBaixo);
}

public class PDIHelper : IPDIHelper
{
    public string NormalizeResposta(string resposta)
    {
        if (string.IsNullOrEmpty(resposta))
            return string.Empty;
        return resposta.Replace("\n", " ").Replace("\r", " ").Trim();
    }

    public string GetTituloStyle(string titulo)
    {
        return string.IsNullOrWhiteSpace(titulo) ? "white" : "#021240";
    }

    public string GetSubtituloStyle(string subtitulo)
    {
        return !string.IsNullOrWhiteSpace(subtitulo)
            ? "style=\"align-self:center;background-color:#F2F2F2;font-weight:bolder;width:40%;min-width:80px;text-align:center;padding:2px\""
            : string.Empty;
    }

    public string GetBorderStyle(bool bordaEsquerda, bool bordaDireita, bool bordaCima, bool bordaBaixo)
    {
        var style = string.Empty;
        if (!bordaEsquerda) style += "border-left:none;";
        if (!bordaDireita) style += "border-right:none;";
        if (!bordaCima) style += "border-top:none;";
        if (!bordaBaixo) style += "border-bottom:none;";
        return style;
    }
}
