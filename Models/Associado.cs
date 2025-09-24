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
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Senha { get; set; } = string.Empty;

    [Column("IdMentor")]
    public int? IdMentor { get; set; }

    [Column("IdCargo")]
    public int? IdCargo { get; set; }

    [Column("IdPerfil")]
    public int? IdPerfil { get; set; }

    [Column("IdVertical")]
    public int? IdVertical { get; set; }

    [Column("ATV")]
    public bool Ativo { get; set; } = true;

    [Column("DHC")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Column("DataAtualizacao")]
    public DateTime? DataAtualizacao { get; set; }

    [StringLength(500)]
    public string? Observacoes { get; set; }

    [StringLength(255)]
    public string? Telefone { get; set; }

    [StringLength(255)]
    public string? Foto { get; set; }

    public DateTime? DataNascimento { get; set; }

    public DateTime? DataAdmissao { get; set; }

    public DateTime? DataDemissao { get; set; }

    // Navigation Properties
    [ForeignKey("IdMentor")]
    public virtual Associado? Mentor { get; set; }

    [ForeignKey("IdCargo")]
    public virtual Cargo? Cargo { get; set; }

    [ForeignKey("IdPerfil")]
    public virtual Perfil? Perfil { get; set; }

    [ForeignKey("IdVertical")]
    public virtual Vertical? Vertical { get; set; }

    // Reverse Navigation Properties
    public virtual ICollection<Associado> Mentorados { get; set; } = new List<Associado>();
    public virtual ICollection<Promocao> Promocoes { get; set; } = new List<Promocao>();

    // Computed Properties
    [NotMapped]
    public string StatusTexto => Ativo ? "Ativo" : "Inativo";

    [NotMapped]
    public string NomeMentor => Mentor?.Nome ?? "Sem mentor";

    [NotMapped]
    public string NomeCargo => Cargo?.Nome ?? "Sem cargo";

    [NotMapped]
    public string NomePerfil => Perfil?.Nome ?? "Sem perfil";

    [NotMapped]
    public string NomeVertical => Vertical?.Nome ?? "Sem vertical";

    [NotMapped]
    public bool IsDefaultPassword => string.Equals(Senha?.Trim(), "avaliacao", StringComparison.OrdinalIgnoreCase);

    [NotMapped]
    public int QuantidadeMentorados => Mentorados?.Count ?? 0;

    [NotMapped]
    public bool PodeSerMentor => Ativo && IdCargo.HasValue;

    [NotMapped]
    public string InitialsAvatar
    {
        get
        {
            if (string.IsNullOrEmpty(Nome)) return "??";
            var parts = Nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
        }
    }
}