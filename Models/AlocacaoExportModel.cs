namespace Peers.Moderno.Models;

public class AlocacaoExportModel
{
    public int IdAvaliacaoAlocacao { get; set; }
    public int IdAlocacaoInterna { get; set; }
    public string AlocacaoInterna { get; set; } = string.Empty;
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public int IdLiderAlocacao { get; set; }
    public string LiderAlocacao { get; set; } = string.Empty;
    public int IdAvaliado { get; set; }
    public string Avaliado { get; set; } = string.Empty;
    public int IdNota { get; set; }
    public string Nota { get; set; } = string.Empty;
    public string Comentarios { get; set; } = string.Empty;
    public string DHCNota { get; set; } = string.Empty;
    public string ValidadoMD { get; set; } = string.Empty;
    public string DHCValidadoMD { get; set; } = string.Empty;
}