using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAvaliacao.Moderno.Models;

public class FotoAssociado
{
    [Key]
    public int IdFoto { get; set; }
    
    public int IdAssociado { get; set; }
    
    [StringLength(200)]
    public string? AssociadoFoto { get; set; }
    
    [StringLength(200)]
    public string? NomeFoto { get; set; }
    
    public string? Imagem { get; set; }
    
    [ForeignKey("IdAssociado")]
    public virtual Associado? Associado { get; set; }
}