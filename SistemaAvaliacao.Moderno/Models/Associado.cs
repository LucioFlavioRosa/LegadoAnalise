using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAvaliacao.Moderno.Models;

public class Associado
{
    [Key]
    public int IdAssociado { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Senha { get; set; } = string.Empty;
    
    public int IdCargo { get; set; }
    public int IdPerfil { get; set; }
    public int IdEmpresa { get; set; }
    public int? IdAssociadoMentor { get; set; }
    public int IdVertical { get; set; }
    public int IdNivel { get; set; } = 1;
    public int IdStatus { get; set; } = 1;
    
    [StringLength(100)]
    public string? Vertical { get; set; }
    
    public DateTime? DataAdmissao { get; set; }
    
    [StringLength(500)]
    public string? FotoNome { get; set; }
    
    public int ATV { get; set; } = 1;
    public int USR { get; set; }
    public DateTime DHC { get; set; } = DateTime.Now;
    
    [ForeignKey("IdCargo")]
    public virtual Cargo? Cargo { get; set; }
    
    [ForeignKey("IdPerfil")]
    public virtual Perfil? Perfil { get; set; }
    
    [ForeignKey("IdAssociadoMentor")]
    public virtual Associado? Mentor { get; set; }
    
    [ForeignKey("IdVertical")]
    public virtual Vertical? VerticalNavigation { get; set; }
    
    public virtual ICollection<Promocao> PromocoesComoAssociado { get; set; } = new List<Promocao>();
    public virtual ICollection<Associado> Mentorados { get; set; } = new List<Associado>();
    public virtual ICollection<FotoAssociado> Fotos { get; set; } = new List<FotoAssociado>();
}