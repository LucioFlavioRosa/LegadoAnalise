namespace Peers.Moderno.Models;

public class Cargo
{
    public int IdCargo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int? IdProximoCargo { get; set; }
    public int TempoMinimoPromocao { get; set; } = 12;
    public string? Funcao { get; set; }
    public string? Autonomia { get; set; }
    public string? EscopoDeAtuacao { get; set; }
    public string? NivelInterlocucao { get; set; }
    public int ATV { get; set; } = 1;
    public DateTime DHC { get; set; } = DateTime.Now;
    
    // Navigation properties
    public virtual Cargo? ProximoCargo { get; set; }
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();
}