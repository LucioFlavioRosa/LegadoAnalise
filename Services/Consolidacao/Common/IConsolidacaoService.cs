using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Consolidacao.Common;

public interface IConsolidacaoService
{
    Task<List<ResultadoCompetenciaModel>> ObterCompetenciasConsolidacaoAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<List<ResultadoPerfomanceModel>> ObterPerformanceConsolidacaoAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<string> CalcularNotaCompetenciaComiteAsync(int idAvaliacaoCompetencia, int nivel, int nota);
    Task<string> CalcularNotaPerformanceComiteAsync(int idAvaliacaoPerformance, int nota);
    Task<ConsideracoesMentorModel?> CarregarConsideracoesMentorAsync(int idMentor, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo);
    Task<bool> SalvarConsideracoesMentorAsync(ConsideracoesMentorModel consideracoes);
    Task<string> ObterJsonRadarAsync(int idAssociado, int idProjeto, int idPeriodo, int idCargo);
}
