namespace Services.PDI.Common.Models;

public class PDIRespostasModel
{
    public string Titulo { get; set; } = string.Empty;
    public string TituloStyle { get; set; } = string.Empty;
    public string Subtitulo { get; set; } = string.Empty;
    public string SubtituloStyle { get; set; } = string.Empty;
    public string FlexGrow { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public string BorderStyle { get; set; } = string.Empty;
    public string Resposta { get; set; } = string.Empty;
    public bool RespostaEnabled { get; set; }
    public int IdPDIResposta { get; set; }
    public string OnInput { get; set; } = string.Empty;
}