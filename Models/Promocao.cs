using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("PROMOCOES")]
public class Promocao
{
    [Key]
    public int Id { get; set; }
    
    public int IdAssociado { get; set; }
    public int IdCargoAnterior { get; set; }
    public int IdCargoNovo { get; set; }
    
    public DateTime DataPromocao { get; set; }
    
    [StringLength(1000)]
    public string Comentarios { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; }
    
    // Navegação
    [ForeignKey("IdAssociado")]
    public virtual Associado? Associado { get; set; }
    
    [ForeignKey("IdCargoAnterior")]
    public virtual Cargo? CargoAnterior { get; set; }
    
    [ForeignKey("IdCargoNovo")]
    public virtual Cargo? CargoNovo { get; set; }
}

public class PromocaoHistorico
{
    public int Id { get; set; }
    public DateTime DataPromocao { get; set; }
    public string CargoAnterior { get; set; } = string.Empty;
    public string CargoNovo { get; set; } = string.Empty;
    public string Comentarios { get; set; } = string.Empty;
}