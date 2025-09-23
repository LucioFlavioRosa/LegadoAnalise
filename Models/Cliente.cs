using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("CLIENTES")]
public class Cliente
{
    [Key]
    public int IdCliente { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Cliente { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Telefones { get; set; }
    
    public int IdAssociacoResponsavel { get; set; }
    
    [StringLength(255)]
    public string? GestorCliente { get; set; }
    
    [StringLength(255)]
    public string? Email { get; set; }
    
    public int IdEmpresa { get; set; }
    
    public int USR { get; set; }
    
    public DateTime DHC { get; set; }
    
    public int ATV { get; set; }
    
    [ForeignKey("IdAssociacoResponsavel")]
    public virtual Associado? AssociadoResponsavel { get; set; }
}