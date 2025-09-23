namespace Peers.Moderno.Models;

public class RelacaoCargoSubcompetencia
{
    public int Id { get; set; }
    public int IdCargo { get; set; }
    public int IdSubcompetencia { get; set; }
    public string Descricao { get; set; } = string.Empty;
    
    // Navegação
    public virtual Cargo? Cargo { get; set; }
    public virtual SubCompetencia? SubCompetencia { get; set; }
}