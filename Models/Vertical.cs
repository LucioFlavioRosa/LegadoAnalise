using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("VERTICAIS")]
public class Vertical
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
    
    // Navegação
    public virtual ICollection<Associado> Associados { get; set; } = new List<Associado>();
}