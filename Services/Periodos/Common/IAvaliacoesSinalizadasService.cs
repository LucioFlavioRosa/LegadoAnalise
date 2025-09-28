using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.Periodos.Common;

public interface IAvaliacoesSinalizadasService
{
    Task<List<AvaliacaoSinalizadaDto>> ObterAvaliacoesSinalizadasAsync(int? empresaId = null);
    Task AtualizarAvaliacoesParaUltimoPeriodoAsync(int idUltimoPeriodo);
}

public class AvaliacaoSinalizadaDto
{
    public string Projeto { get; set; } = string.Empty;
    public string Respondente { get; set; } = string.Empty;
    public string Avaliador { get; set; } = string.Empty;
    public string DataInicio { get; set; } = string.Empty;
    public string DataTermino { get; set; } = string.Empty;
}
