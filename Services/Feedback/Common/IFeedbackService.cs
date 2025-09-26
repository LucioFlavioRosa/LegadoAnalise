namespace Peers.Moderno.Services.Feedback.Common;

using System.Threading.Tasks;
using System.Collections.Generic;

public interface IFeedbackService
{
    Task<List<ProjetoFeedbackModel>> ListarProjetosAsync(int gestorId, int? projetoId = null, int? statusId = null, int? periodoId = null, int? clienteId = null);
    Task<List<ClienteFeedbackModel>> ListarClientesAsync();
    Task<List<PeriodoFeedbackModel>> ListarPeriodosAsync(int empresaId);
    Task<List<StatusFeedbackModel>> ListarStatusAsync();
    Task<List<ProjetoFeedbackModel>> FiltrarAvaliacoesAsync(FeedbackFiltroModel filtro, int gestorId, int empresaId);
    Task<FeedbackAvaliacaoEmailModel?> ObterAvaliacaoEmailAsync(int idAvaliacao);
    Task<List<AvaliacaoPerformanceModel>> ObterAvaliacoesPerformanceAsync(int idAssociado, int idProjeto, int idPeriodo, string etapa, bool apenasFeedback);
    Task<List<AvaliacaoCompetenciaModel>> ObterAvaliacoesCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string etapa, string tipoAvaliacao, string escopo, int idAvaliacaoEmail);
    Task<AssociadoFeedbackModel?> ObterAssociadoAsync(int idAssociado);
    Task<StatusFeedbackModel?> ObterStatusAvaliacaoAsync(string statusNome);
    Task AlterarAvaliacaoPerformanceAsync(int idAvaliacaoPerformance, AvaliacaoPerformanceModel avaliacao);
    Task AlterarAvaliacaoCompetenciaAsync(int idAvaliacaoCompetencia, AvaliacaoCompetenciaModel avaliacao);
    Task AvancarProximaEtapaPerformanceAsync(AvaliacaoPerformanceModel avaliacao, FeedbackAvaliacaoEmailModel avaliacaoEmail, string tipoAvaliacao);
    Task AvancarProximaEtapaCompetenciaAsync(AvaliacaoCompetenciaModel avaliacao, FeedbackAvaliacaoEmailModel avaliacaoEmail, string tipoAvaliacao);
    Task AvancarProximaEtapaEmailAsync(FeedbackAvaliacaoEmailModel avaliacaoEmail);
}