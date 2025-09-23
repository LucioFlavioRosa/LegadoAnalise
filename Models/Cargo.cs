namespace Peers.Moderno.Models;

public class Cargo
{
    public int IdCargo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int ATV { get; set; }
    
    // Navegação
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();
    public virtual ICollection<RelacaoCargoSubcompetencia> RelacoesCargo { get; set; } = new List<RelacaoCargoSubcompetencia>();
}