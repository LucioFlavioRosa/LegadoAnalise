using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("ASSOCIADOS")]
public class Associado
{
    [Key]
    [Column("IdAssociado")]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(100)]
    public string Senha { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public int IdCargo { get; set; }
    public virtual Cargo? Cargo { get; set; }

    public int? IdMentor { get; set; }
    public virtual Associado? Mentor { get; set; }
    public virtual ICollection<Associado> Mentorados { get; set; } = new List<Associado>();

    public int IdPerfil { get; set; }
    public virtual Perfil? Perfil { get; set; }

    public int? IdVertical { get; set; }
    public virtual Vertical? Vertical { get; set; }

    public virtual ICollection<Promocao> Promocoes { get; set; } = new List<Promocao>();

    [StringLength(500)]
    public string? Foto { get; set; }

    public DateTime? DataAdmissao { get; set; }
    public DateTime? DataDemissao { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAlteracao { get; set; }
}

[Table("PROMOCOES")]
public class Promocao
{
    [Key]
    [Column("IdPromocao")]
    public int Id { get; set; }

    public int IdAssociado { get; set; }
    public virtual Associado? Associado { get; set; }

    public int IdCargoAnterior { get; set; }
    public virtual Cargo? CargoAnterior { get; set; }

    public int IdCargoNovo { get; set; }
    public virtual Cargo? CargoNovo { get; set; }

    public DateTime DataPromocao { get; set; }
    public string? Observacoes { get; set; }
}

[Table("PERFIS")]
public class Perfil
{
    [Key]
    [Column("IdPerfil")]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Column("Perfil")]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;

    public virtual ICollection<Associado> Associados { get; set; } = new List<Associado>();
}

[Table("VERTICAIS")]
public class Vertical
{
    [Key]
    [Column("IdVertical")]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Column("Descricao")]
    public string Nome { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public virtual ICollection<Associado> Associados { get; set; } = new List<Associado>();
}