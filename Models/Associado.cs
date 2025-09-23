namespace Peers.Moderno.Models;

public class Associado
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? IdCargo { get; set; }
    public int? IdMentor { get; set; }
    public int? IdPerfil { get; set; }
    public int? IdVertical { get; set; }
    public int? IdEmpresa { get; set; }
    public string Senha { get; set; } = string.Empty;
    public DateTime? DataAdmissao { get; set; }
    public bool Ativo { get; set; } = true;
    public string? FotoBase64 { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
    
    // Navegação
    public virtual Cargo? Cargo { get; set; }
    public virtual Associado? Mentor { get; set; }
    public virtual Perfil? Perfil { get; set; }
    public virtual Vertical? Vertical { get; set; }
    public virtual ICollection<Associado> Mentorados { get; set; } = new List<Associado>();
    public virtual ICollection<Promocao> Promocoes { get; set; } = new List<Promocao>();
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