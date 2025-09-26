namespace Services.PDI.Common.Models;

public class PDIPeriodoModel
{
    public string Id { get; set; } = string.Empty;
    public string Active { get; set; } = string.Empty;
    public string AriaLabelled { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public List<PDIColunaModel> PDIColunas { get; set; } = new();
}

public class PDIColunaModel
{
    public List<PDIRespostaModel> PDIRespostas { get; set; } = new();
}

public class PDIRespostaModel
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