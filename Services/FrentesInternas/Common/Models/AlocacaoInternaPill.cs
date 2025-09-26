using System.Collections.Generic;

namespace Services.FrentesInternas.Common.Models;

public class AlocacaoInternaPill
{
    public int idAlocacao { get; set; }
    public string Alocacao { get; set; } = string.Empty;
    public List<AvaliacaoAlocacaoPill> Avaliados { get; set; } = new();
}