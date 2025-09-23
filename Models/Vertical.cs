namespace Peers.Moderno.Models;

public class Vertical
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    
    // Navegação
    public virtual ICollection<Associado> Associados { get; set; } = new List<Associado>();
}