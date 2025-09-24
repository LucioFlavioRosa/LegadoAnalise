namespace Peers.Moderno.Models;

public class SubCompetencia
{
    public int IdSubCompetencia { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool ATV { get; set; } = true;
    public string? TipoAvaliacao { get; set; }
    public DateTime DHC { get; set; } = DateTime.UtcNow;
    public int USR { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    // Navigation properties
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();

    // Computed properties
    public bool Ativo => ATV;
    public string StatusTexto => ATV ? "Ativo" : "Inativo";
    public string TipoAvaliacaoTexto => TipoAvaliacao ?? "Não definido";
}