namespace Peers.Moderno.Models;

public class FrenteInternaModel
{
    public int IdFrenteInterna { get; set; }
    public string FrenteInterna { get; set; } = string.Empty;
    public int TotalLideres { get; set; }
    public string Lideres { get; set; } = string.Empty;
    public int ATV { get; set; }
    public bool Ativo => ATV == 1;
    public DateTime DHC { get; set; }
    public int USR { get; set; }
    
    public List<LiderFrenteInternaModel> LideresLista { get; set; } = new();
    public List<ParticipanteFrenteInternaModel> ParticipantesLista { get; set; } = new();
}