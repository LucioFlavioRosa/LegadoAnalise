namespace Peers.Moderno.Models;

public class Eixo
{
    public int IdEixo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public int ATV { get; set; }
    
    // Navegação
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();
}