using System.ComponentModel.DataAnnotations;

namespace SistemaAvaliacao.Moderno.Models;

public class Empresa
{
    [Key]
    public int IdEmpresa { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Nome { get; set; } = string.Empty;
    
    public int ATV { get; set; } = 1;
    public int USR { get; set; }
    public DateTime DHC { get; set; } = DateTime.Now;
    
    public virtual ICollection<Associado> Associados { get; set; } = new List<Associado>();
}