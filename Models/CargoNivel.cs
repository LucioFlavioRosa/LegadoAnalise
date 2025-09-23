namespace Peers.Moderno.Models;

public class CargoNivel
{
    public int IdNivel { get; set; }
    public string Nivel { get; set; } = string.Empty;
    public int ATV { get; set; }
    
    // Navegação
    public virtual ICollection<PremissasRadar> PremissasRadar { get; set; } = new List<PremissasRadar>();
}