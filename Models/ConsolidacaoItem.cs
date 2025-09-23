namespace Peers.Moderno.Models;

public class ConsolidacaoItem
{
    public string Periodo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Projeto { get; set; } = string.Empty;
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public int IdProjeto { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
}