using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("MODOSCALCULOSCOMPETENCIAS")]
public class ModoCalculoCompetencia
{
    [Key]
    [Column("idModo")]
    public int IdModo { get; set; }
    
    public string ModoDesc { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public bool Ativo { get; set; } = true;
}