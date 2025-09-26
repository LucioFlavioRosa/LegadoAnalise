namespace Peers.Moderno.Models;

public class PDIQuestao
{
    public int IdPDIQuestao { get; set; }
    public int IdPeriodo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Subtitulo { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public int ColunaPosicao { get; set; }
    public double ColunaTamanho { get; set; }
    public int Ordem { get; set; }
    public bool BordaEsquerda { get; set; } = true;
    public bool BordaDireita { get; set; } = true;
    public bool BordaCima { get; set; } = true;
    public bool BordaBaixo { get; set; } = true;
    public double? FixTamanho { get; set; }
    public PDIPeriodo? Periodo { get; set; }
    public ICollection<PDIResposta> Respostas { get; set; } = new List<PDIResposta>();
}
