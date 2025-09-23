namespace Peers.Moderno.Models;

public class SubCompetencia
{
    public int IdSubCompetencia { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int ATV { get; set; }
    
    // Navegação
    public virtual ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();
    public virtual ICollection<RelacaoCargoSubcompetencia> RelacoesSubcompetencia { get; set; } = new List<RelacaoCargoSubcompetencia>();
}