using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("PROJETOSCOMPLEXIDADES")]
public class ProjetoComplexidade
{
    [Key]
    public int IdComplexidade { get; set; }
    
    [StringLength(500)]
    public string? Complexidade { get; set; }
    
    [StringLength(100)]
    public string? Codigo { get; set; }
    
    public int ATV { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Peso { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PesoPonderado { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Ponderacao { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal FaixaInicial { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal FaixaFinal { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? SomaMinimaFator { get; set; }
    
    public DateTime DHC { get; set; }
    
    [StringLength(50)]
    public string? USR { get; set; }
}