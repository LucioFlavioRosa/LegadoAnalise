namespace Peers.Moderno.Models;

public class ConsideracoesMentorModel
{
    public int idConsideracoesMentor { get; set; }
    public int idMentor { get; set; }
    public string Mentor { get; set; } = string.Empty;
    public int idAssociado { get; set; }
    public string Associado { get; set; } = string.Empty;
    public int idPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public string ElegivelPromocao { get; set; } = string.Empty;
    public string InputPromocao { get; set; } = string.Empty;
    public string TrajetoriaAssociado { get; set; } = string.Empty;
    public string PontosFortes { get; set; } = string.Empty;
    public string PontosFracos { get; set; } = string.Empty;
    public string MentorPodeVer { get; set; } = string.Empty;
    public string AcaoComite { get; set; } = string.Empty;
    public string PontosFortesRH { get; set; } = string.Empty;
    public string PontosFracosRH { get; set; } = string.Empty;
    public string SalarioAtual { get; set; } = string.Empty;
    public string SalarioNovo { get; set; } = string.Empty;
    public string RegimeContratacaoAtual { get; set; } = string.Empty;
    public string RegimeContratacaoNovo { get; set; } = string.Empty;
    public string MentoriaRealizada { get; set; } = string.Empty;
}