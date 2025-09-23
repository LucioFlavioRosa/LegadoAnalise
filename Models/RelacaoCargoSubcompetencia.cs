using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("RELACAO_CARGO_SUBCOMPETENCIA")]
public class RelacaoCargoSubcompetencia
{
    [Key]
    public int Id { get; set; }
    
    [Column("idCargo")]
    public int IdCargo { get; set; }
    
    [Column("idSubcompetencia")]
    public int IdSubcompetencia { get; set; }
    
    public string Descricao { get; set; } = string.Empty;
    
    // Navigation Properties
    public virtual Cargo? Cargo { get; set; }
    public virtual SubCompetencia? SubCompetencia { get; set; }
}