using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Consolidacao.Common;

public interface IConsolidacaoService
{
    Task<List<ResultadoCompetenciaModel>> ObterCompetenciasProjetoAssociadoAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<List<ResultadoPerfomanceModel>> ObterPerformanceProjetoAssociadoAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<string> CalcularNotaCompetenciaComiteAsync(int id, int nivel, int nota);
    Task<string> CalcularNotaPerformanceComiteAsync(int id, int nota);
    Task<ConsideracoesMentorModel?> ObterConsideracoesMentorAsync(int idMentor, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo);
    Task AtualizarConsideracoesMentorAsync(ConsideracoesMentorModel consideracoesMentor);
    Task<AssociadoMentorCargoModel?> ObterAssociadoMentorCargoAsync(int idAssociado);
    Task<PeriodoModel?> ObterPeriodoAsync(int idPeriodo);
    Task<string> ObterFotoAssociadoAsync(int idAssociado);
    Task<string> GerarJsonRadarAsync(ResultadoProjetosModel projeto);
}