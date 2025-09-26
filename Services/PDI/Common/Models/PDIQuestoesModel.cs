namespace Services.PDI.Common.Models;

public class PDIQuestoesModel
{
    public int IdPDIQuestoes { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Subtitulo { get; set; } = string.Empty;
    public int ColunaPosicao { get; set; }
    public decimal ColunaTamanho { get; set; }
    public int FixTamanho { get; set; }
    public string Icone { get; set; } = string.Empty;
    public bool BordaEsquerda { get; set; }
    public bool BordaDireita { get; set; }
    public bool BordaCima { get; set; }
    public bool BordaBaixo { get; set; }
}