namespace Peers.Moderno.Services.PDI.Common.Models;

public class PDIPeriodoModel
{
    public string id { get; set; } = string.Empty;
    public string active { get; set; } = string.Empty;
    public string arialabelled { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public List<PDIColunasModel> PDIColunas { get; set; } = new();
}

public class PDIColunasModel
{
    public List<PDIRespostasModel> PDIRespostas { get; set; } = new();
}

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
    public int idPDIResposta { get; set; }
    public string OnInput { get; set; } = string.Empty;
}

public class PDIPillsModel
{
    public string id { get; set; } = string.Empty;
    public string href { get; set; } = string.Empty;
    public string ariacontrols { get; set; } = string.Empty;
    public string ariaselected { get; set; } = string.Empty;
    public string active { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
}