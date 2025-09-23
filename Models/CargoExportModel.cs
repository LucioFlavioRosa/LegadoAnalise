namespace Peers.Moderno.Models;

public class CargoExportModel
{
    public int IdCargo { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int? IdProximoCargo { get; set; }
    public string ProximoCargo { get; set; } = string.Empty;
    public int TempoMinimoPromocao { get; set; }
    public string? Funcao { get; set; }
    public string? Autonomia { get; set; }
    public string? EscopoDeAtuacao { get; set; }
    public string? NivelInterlocucao { get; set; }
    public int ATV { get; set; }
}