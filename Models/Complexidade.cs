using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("PROJETOSCOMPLEXIDADES")]
public class Complexidade
{
    [Key]
    [Column("IdComplexidade")]
    public int IdComplexidade { get; set; }

    [Column("Complexidade")]
    [StringLength(500)]
    public string ComplexidadeNome { get; set; } = string.Empty;

    [Column("Codigo")]
    [StringLength(100)]
    public string Codigo { get; set; } = string.Empty;

    [Column("ATV")]
    public int ATV { get; set; }

    [Column("Peso")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Peso { get; set; }

    [Column("PesoPonderado")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PesoPonderado { get; set; }

    [Column("Ponderacao")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Ponderacao { get; set; }

    [Column("FaixaInicial")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal FaixaInicial { get; set; }

    [Column("FaixaFinal")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal FaixaFinal { get; set; }

    [Column("SomaMinimaFator")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? SomaMinimaFator { get; set; }

    [Column("DHC")]
    public DateTime DHC { get; set; }

    [Column("USR")]
    [StringLength(50)]
    public string USR { get; set; } = string.Empty;

    [NotMapped]
    public bool Ativo => ATV == 1;

    [NotMapped]
    public string StatusTexto => Ativo ? "Ativo" : "Inativo";
}