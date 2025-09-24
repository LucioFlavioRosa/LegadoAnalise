namespace Peers.Moderno.Models;

public class LiderFrenteInternaModel
{
    public int IdLiderFrenteInterna { get; set; }
    public int IdFrenteInterna { get; set; }
    public int IdAssociado { get; set; }
    public string NomeAssociado { get; set; } = string.Empty;
    public bool ATV { get; set; }
    public string StatusTexto => ATV ? "Ativo" : "Inativo";
    public DateTime DHC { get; set; }
    public int USR { get; set; }
    
    public FrenteInternaModel? FrenteInterna { get; set; }
    public Associado? Associado { get; set; }
}