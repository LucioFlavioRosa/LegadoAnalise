using System.Collections.Generic;

namespace Services.FrentesInternas.Common.Models;

public class FrentePill
{
    public int idPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public string id { get; set; } = string.Empty;
    public string active { get; set; } = string.Empty;
    public string arialabelled { get; set; } = string.Empty;
    public List<AlocacaoInternaPill> alocacoes { get; set; } = new();
}