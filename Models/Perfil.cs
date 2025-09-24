using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("PERFIS")]
public class Perfil
{
    [Key]
    [Column("IdPerfil")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("Perfil")]
    public string Nome { get; set; } = string.Empty;

    [Column("ATV")]
    public bool Ativo { get; set; } = true;

    [Column("DHC")]
    public DateTime? DHC { get; set; }

    public virtual ICollection<Associado> Associados { get; set; } = new List<Associado>();
}