namespace Peers.Moderno.Services.Competencias.Common.DTOs;

public class CompetenciaImportDto
{
    public int IdCompetencia { get; set; }
    public int IdCargo { get; set; }
    public int IdEixo { get; set; }
    public int IdSubCompetencia { get; set; }
    public int IdDimensao { get; set; }
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string CompetenciaAtual { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int ATV { get; set; }
}