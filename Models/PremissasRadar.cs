namespace Peers.Moderno.Models;

public class PremissasRadar
{
    public int IdPremissa { get; set; }
    public int IdEixo { get; set; }
    public int IdCargo { get; set; }
    public int IdNivel { get; set; }
    public int IdEmpresa { get; set; }
    public int ValorRadarPeers { get; set; }
    public int ValorBaseAutoAvaliacao { get; set; }
    public int ValorBaseAvaliacaoGestor { get; set; }
    public int USR { get; set; }
    public DateTime DHC { get; set; }
    public int ATV { get; set; }
    
    // Navegação
    public virtual Eixo? Eixo { get; set; }
    public virtual Cargo? Cargo { get; set; }
    public virtual CargoNivel? CargoNivel { get; set; }
}