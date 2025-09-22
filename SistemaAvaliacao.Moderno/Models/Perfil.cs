using System.ComponentModel.DataAnnotations;

namespace SistemaAvaliacao.Moderno.Models;

public class Perfil
{
    [Key]
    public int IdPerfil { get; set; }
    
    [Required]
    [StringLength(10)]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    public int ATV { get; set; } = 1;
    public int USR { get; set; }
    public DateTime DHC { get; set; } = DateTime.Now;
    
    public virtual ICollection<Associado> Associados { get; set; } = new List<Associado>();
}