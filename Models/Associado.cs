using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("ASSOCIADOS")]
public class Associado
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string Senha { get; set; } = string.Empty;
    
    public int IdCargo { get; set; }
    public int IdMentor { get; set; }
    public int IdPerfil { get; set; }
    public int IdVertical { get; set; }
    
    public DateTime? DataAdmissao { get; set; }
    public bool Ativo { get; set; } = true;
    
    public string? FotoBase64 { get; set; }
    
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
    
    // Navegação
    [ForeignKey("IdCargo")]
    public virtual Cargo? Cargo { get; set; }
    
    [ForeignKey("IdMentor")]
    public virtual Associado? Mentor { get; set; }
    
    [ForeignKey("IdPerfil")]
    public virtual Perfil? Perfil { get; set; }
    
    [ForeignKey("IdVertical")]
    public virtual Vertical? Vertical { get; set; }
    
    public virtual ICollection<Promocao> Promocoes { get; set; } = new List<Promocao>();
    public virtual ICollection<Associado> Mentorados { get; set; } = new List<Associado>();
}

public class AssociadoListaItem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeCargo { get; set; } = string.Empty;
    public string NomeMentor { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}

public class DropdownItem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}