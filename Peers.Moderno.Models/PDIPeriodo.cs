namespace Peers.Moderno.Models;

public class PDIPeriodo
{
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public ICollection<PDIQuestao> Questoes { get; set; } = new List<PDIQuestao>();
    public ICollection<PDIResposta> Respostas { get; set; } = new List<PDIResposta>();
}
