namespace Peers.Moderno.Models;

public class AvaliacaoCompetenciaNota
{
    public int IdNota { get; set; }
    public string CodigoNota { get; set; } = string.Empty;
    public bool IndFeedback { get; set; }
    public int ATV { get; set; }
}