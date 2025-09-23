using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("EIXOS")]
public class Eixo
{
    [Key]
    public int IdEixo { get; set; }
    
    [Column("Eixo")]
    [Required]
    [StringLength(500)]
    public string Nome { get; set; } = string.Empty;
    
    public int ATV { get; set; } = 1;
    
    public int? USR { get; set; }
    
    public DateTime? DHC { get; set; }
    
    [StringLength(50)]
    public string? TipoAvaliacao { get; set; }
    
    // Navigation properties
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();
}