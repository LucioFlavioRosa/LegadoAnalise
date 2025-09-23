using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("CLIENTES")]
public class Cliente
{
    [Key]
    public int IdCliente { get; set; }

    [Column("Cliente")]
    [StringLength(200)]
    public string Cliente1 { get; set; } = string.Empty;

    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string Telefones { get; set; } = string.Empty;

    [StringLength(100)]
    public string GestorCliente { get; set; } = string.Empty;

    public int IdAssociadoResponsavel { get; set; }

    public int ATV { get; set; }

    public DateTime DHC { get; set; }

    public int USR { get; set; }

    public int IdEmpresa { get; set; }

    [ForeignKey("IdAssociadoResponsavel")]
    public virtual Associado? AssociadoResponsavel { get; set; }
}