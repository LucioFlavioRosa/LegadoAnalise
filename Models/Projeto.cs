using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("PROJETOS")]
public class Projeto
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Nome { get; set; } = string.Empty;
    
    public DateTime? DataInicio { get; set; }
    
    public DateTime? DataFim { get; set; }
    
    public int? IdCliente { get; set; }
    
    public int? IdAssociadoResponsavel { get; set; }
    
    public int? IdAssociadoGestor { get; set; }
    
    public int? IdTipoProjeto { get; set; }
    
    public int? IdComplexidade { get; set; }
    
    public bool Ativo { get; set; } = true;
    
    public int? UsuarioCriacao { get; set; }
    
    public DateTime? DataCriacao { get; set; }
    
    public int? UsuarioAlteracao { get; set; }
    
    public DateTime? DataAlteracao { get; set; }
    
    [MaxLength(1000)]
    public string? Observacoes { get; set; }
    
    // Navigation Properties
    [ForeignKey("IdCliente")]
    public virtual Cliente? Cliente { get; set; }
    
    [ForeignKey("IdAssociadoResponsavel")]
    public virtual Associado? AssociadoResponsavel { get; set; }
    
    [ForeignKey("IdAssociadoGestor")]
    public virtual Associado? AssociadoGestor { get; set; }
    
    [ForeignKey("IdTipoProjeto")]
    public virtual TipoProjeto? TipoProjeto { get; set; }
    
    [ForeignKey("IdComplexidade")]
    public virtual ProjetoComplexidade? Complexidade { get; set; }
    
    public virtual ICollection<AssociadoProjeto> AssociadosProjeto { get; set; } = new List<AssociadoProjeto>();
}

[Table("ASSOCIADOS_PROJETOS")]
public class AssociadoProjeto
{
    [Key]
    public int Id { get; set; }
    
    public int IdProjeto { get; set; }
    
    public int IdAssociado { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public DateTime? DataFim { get; set; }
    
    public bool Ativo { get; set; } = true;
    
    public int? IdCargoProjeto { get; set; }
    
    public int? IdAvaliador { get; set; }
    
    // Navigation Properties
    [ForeignKey("IdProjeto")]
    public virtual Projeto Projeto { get; set; } = null!;
    
    [ForeignKey("IdAssociado")]
    public virtual Associado Associado { get; set; } = null!;
    
    [ForeignKey("IdCargoProjeto")]
    public virtual Cargo? CargoProjeto { get; set; }
    
    [ForeignKey("IdAvaliador")]
    public virtual Associado? Avaliador { get; set; }
}

[Table("TIPOS_PROJETOS")]
public class TipoProjeto
{
    [Key]
    public int IdTipo { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Nome { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
    
    public int? UsuarioCriacao { get; set; }
    
    public DateTime? DataCriacao { get; set; }
    
    public int? UsuarioAlteracao { get; set; }
    
    public DateTime? DataAlteracao { get; set; }
}