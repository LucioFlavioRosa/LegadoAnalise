using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("ASSOCIADOS")]
public class Associado
{
    [Key]
    [Column("IdAssociado")]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(255)]
    public string? Email { get; set; }
    
    public bool Ativo { get; set; }
    
    public bool Socio { get; set; }
    
    public int? IdCargo { get; set; }
    
    public int? IdMentor { get; set; }
    
    public int? IdPerfil { get; set; }
    
    public int? IdVertical { get; set; }
    
    [ForeignKey("IdCargo")]
    public virtual Cargo? Cargo { get; set; }
    
    [ForeignKey("IdMentor")]
    public virtual Associado? Mentor { get; set; }
    
    public virtual ICollection<Associado> Mentorados { get; set; } = new List<Associado>();
    
    [ForeignKey("IdPerfil")]
    public virtual Perfil? Perfil { get; set; }
    
    [ForeignKey("IdVertical")]
    public virtual Vertical? Vertical { get; set; }
    
    public virtual ICollection<Promocao> Promocoes { get; set; } = new List<Promocao>();
}