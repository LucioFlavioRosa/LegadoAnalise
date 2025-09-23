using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("DIMENSOES")]
public class Dimensao
{
    [Key]
    [Column("IdDimensao")]
    public int IdDimensao { get; set; }
    
    [Required]
    [StringLength(500)]
    [Column("Dimensao")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? TipoAvaliacao { get; set; }
    
    [Column("ATV")]
    public bool Ativo { get; set; } = true;
    
    [Column("DHC")]
    public DateTime DataCriacao { get; set; }
    
    public DateTime DataAlteracao { get; set; }
    
    [Column("USR")]
    public int? UsuarioCriacao { get; set; }
    
    // Navigation properties
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();
}