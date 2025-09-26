namespace Services.PDI.Common.Models;

public class PDIPeriodosModel
{
    public string Id { get; set; } = string.Empty;
    public string Active { get; set; } = string.Empty;
    public string Arialabelled { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public List<PDIColunasModel> PDIColunas { get; set; } = new();
}