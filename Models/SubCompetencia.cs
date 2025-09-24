using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("SUBCOMPETENCIAS")]
public class SubCompetencia
{
    [Key]
    public int IdSubCompetencia { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("SubCompetencia")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TipoAvaliacao { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DHC { get; set; }

    public int USR { get; set; }

    // Navigation properties
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();
}