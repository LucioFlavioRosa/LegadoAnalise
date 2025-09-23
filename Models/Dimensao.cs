namespace Peers.Moderno.Models;

public class Dimensao
{
    public int IdDimensao { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public int ATV { get; set; }
    
    // Navegação
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();
}