namespace Peers.Moderno.Models;

public class CompetenciaExportModel
{
    public int IdCompetencia { get; set; }
    public int IdCargo { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int IdEixo { get; set; }
    public string Eixo { get; set; } = string.Empty;
    public int IdSubCompetencia { get; set; }
    public string SubCompetencia { get; set; } = string.Empty;
    public int IdDimensao { get; set; }
    public string Dimensao { get; set; } = string.Empty;
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string CompetenciaAtual { get; set; } = string.Empty;
    public int ATV { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
}