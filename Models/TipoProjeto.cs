using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("TIPOS_PROJETOS")]
public class TipoProjeto
{
    [Key]
    [Column("IdTipo")]
    public int IdTipo { get; set; }

    [Required]
    [Column("ProjetoTipo")]
    [MaxLength(500)]
    public string Nome { get; set; } = string.Empty;

    [Column("ATV")]
    public int ATV { get; set; } = 1;

    [Column("DHC")]
    public DateTime DHC { get; set; } = DateTime.Now;

    [NotMapped]
    public string StatusTexto => ATV == 1 ? "Ativo" : "Inativo";

    [NotMapped]
    public bool Ativo => ATV == 1;
}