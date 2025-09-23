namespace Peers.Moderno.Models;

public class Promocao
{
    public int Id { get; set; }
    public int IdAssociado { get; set; }
    public int IdCargoAnterior { get; set; }
    public int IdCargoNovo { get; set; }
    public DateTime DataPromocao { get; set; }
    public string Comentarios { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; }
    
    // Navegação
    public virtual Associado? Associado { get; set; }
    public virtual Cargo? CargoAnterior { get; set; }
    public virtual Cargo? CargoNovo { get; set; }
}

public class PromocaoHistorico
{
    public int Id { get; set; }
    public DateTime DataPromocao { get; set; }
    public string CargoAnterior { get; set; } = string.Empty;
    public string CargoNovo { get; set; } = string.Empty;
    public string Comentarios { get; set; } = string.Empty;
}