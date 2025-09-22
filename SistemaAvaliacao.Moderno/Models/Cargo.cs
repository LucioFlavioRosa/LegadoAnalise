using System.ComponentModel.DataAnnotations;

namespace SistemaAvaliacao.Moderno.Models;

public class Cargo
{
    [Key]
    public int IdCargo { get; set; }
    
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
    public virtual ICollection<Promocao> PromocoesCargoAnterior { get; set; } = new List<Promocao>();
    public virtual ICollection<Promocao> PromocoesCargoNovo { get; set; } = new List<Promocao>();
}