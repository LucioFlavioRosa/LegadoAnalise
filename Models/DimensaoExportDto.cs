namespace Peers.Moderno.Models;

public class DimensaoExportDto
{
    public int IdDimensao { get; set; }
    public string Dimensao { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public int Status { get; set; }
}