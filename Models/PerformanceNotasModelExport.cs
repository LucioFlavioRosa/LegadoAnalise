namespace Peers.Moderno.Models;

public class PerformanceNotasModelExport
{
    public int IdNotaPerformance { get; set; }
    public int IdAssociado { get; set; }
    public string Associado { get; set; } = string.Empty;
    public int IdCargo { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int IdProjeto { get; set; }
    public string Projeto { get; set; } = string.Empty;
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public int IdPerformance { get; set; }
    public string Performance { get; set; } = string.Empty;
    public int IdNotaAutoAvaliacao { get; set; }
    public string NotaAutoAvaliacao { get; set; } = string.Empty;
    public string ComentariosAutoAvaliacao { get; set; } = string.Empty;
    public int IdNotaAvaliacaoAsCegas { get; set; }
    public string NotaAvaliacaoAsCegas { get; set; } = string.Empty;
    public string ComentariosAvaliacaoAsCegas { get; set; } = string.Empty;
    public int IdNotaAvaliacaoGestor { get; set; }
    public string NotaAvaliacaoGestor { get; set; } = string.Empty;
    public string ComentariosAvaliacaoGestor { get; set; } = string.Empty;
    public int IdNotaFeedback { get; set; }
    public string NotaFeedback { get; set; } = string.Empty;
    public string ComentariosFeedback { get; set; } = string.Empty;
    public int IdNotaComite { get; set; }
    public string NotaComite { get; set; } = string.Empty;
    public int NotaFinal { get; set; }
}