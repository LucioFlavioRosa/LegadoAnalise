using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAvaliacao.Moderno.Models;

public class Promocao
{
    [Key]
    public int IdPromocao { get; set; }
    
    public int IdAssociado { get; set; }
    public int IdCargoAnterior { get; set; }
    public int IdCargoNovo { get; set; }
    
    public DateTime? DataPromocao { get; set; }
    
    [StringLength(500)]
    public string? Comentarios { get; set; }
    
    public bool ATV { get; set; } = true;
    public DateTime DHC { get; set; } = DateTime.Now;
    
    [ForeignKey("IdAssociado")]
    public virtual Associado? Associado { get; set; }
    
    [ForeignKey("IdCargoAnterior")]
    public virtual Cargo? CargoAnterior { get; set; }
    
    [ForeignKey("IdCargoNovo")]
    public virtual Cargo? CargoNovo { get; set; }
}