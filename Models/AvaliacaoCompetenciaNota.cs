using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("AVALIACOESCOMPETENCIASNOTAS")]
public class AvaliacaoCompetenciaNota
{
    [Key]
    public int IdNota { get; set; }
    
    public string CodigoNota { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal? Valor { get; set; }
    public bool? IndFeedback { get; set; }
    public bool Ativo { get; set; } = true;
}