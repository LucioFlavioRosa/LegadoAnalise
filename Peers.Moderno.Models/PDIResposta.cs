namespace Peers.Moderno.Models;

public class PDIResposta
{
    public int IdPDIResposta { get; set; }
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public int IdPDIQuestao { get; set; }
    public string Resposta { get; set; } = string.Empty;
    public DateTime DHC { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public PDIPeriodo? Periodo { get; set; }
    public PDIQuestao? Questao { get; set; }
}
