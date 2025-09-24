using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

public class PremissasRadar
{
    [Key]
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
    public int ATV { get; set; } = 1;
    
    [ForeignKey("IdEixo")]
    public virtual Eixo? Eixo { get; set; }
    
    [ForeignKey("IdCargo")]
    public virtual Cargo? Cargo { get; set; }
    
    [ForeignKey("IdNivel")]
    public virtual CargoNivel? CargoNivel { get; set; }
}