using Peers.Moderno.Services.Feedback.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Peers.Moderno.Services.Feedback;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackComboHelper _comboHelper;
    private readonly IFeedbackRepository _repository;

    public FeedbackService(IFeedbackComboHelper comboHelper, IFeedbackRepository repository)
    {
        _comboHelper = comboHelper;
        _repository = repository;
    }

    public async Task<List<ProjetoFeedbackModel>> ListarProjetosAsync(int gestorId, int? projetoId = null, int? statusId = null, int? periodoId = null, int? clienteId = null)
    {
        return await _repository.ListarProjetosAsync(gestorId, projetoId, statusId, periodoId, clienteId);
    }

    public async Task<List<ClienteFeedbackModel>> ListarClientesAsync()
    {
        return await _comboHelper.ListarClientesAsync();
    }

    public async Task<List<PeriodoFeedbackModel>> ListarPeriodosAsync(int empresaId)
    {
        return await _comboHelper.ListarPeriodosAsync(empresaId);
    }

    public async Task<List<StatusFeedbackModel>> ListarStatusAsync()
    {
        return await _comboHelper.ListarStatusAsync();
    }

    public async Task<List<ProjetoFeedbackModel>> FiltrarAvaliacoesAsync(FeedbackFiltroModel filtro, int gestorId, int empresaId)
    {
        return await _repository.FiltrarAvaliacoesAsync(filtro, gestorId, empresaId);
    }

    public async Task<FeedbackAvaliacaoEmailModel?> ObterAvaliacaoEmailAsync(int idAvaliacao)
    {
        return await _repository.ObterAvaliacaoEmailAsync(idAvaliacao);
    }

    public async Task<List<AvaliacaoPerformanceModel>> ObterAvaliacoesPerformanceAsync(int idAssociado, int idProjeto, int idPeriodo, string etapa, bool apenasFeedback)
    {
        return await _repository.ObterAvaliacoesPerformanceAsync(idAssociado, idProjeto, idPeriodo, etapa, apenasFeedback);
    }

    public async Task<List<AvaliacaoCompetenciaModel>> ObterAvaliacoesCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string etapa, string tipoAvaliacao, string escopo, int idAvaliacaoEmail)
    {
        return await _repository.ObterAvaliacoesCompetenciasAsync(idAssociado, idProjeto, idPeriodo, etapa, tipoAvaliacao, escopo, idAvaliacaoEmail);
    }

    public async Task<AssociadoFeedbackModel?> ObterAssociadoAsync(int idAssociado)
    {
        return await _repository.ObterAssociadoAsync(idAssociado);
    }

    public async Task<StatusFeedbackModel?> ObterStatusAvaliacaoAsync(string statusNome)
    {
        return await _repository.ObterStatusAvaliacaoAsync(statusNome);
    }

    public async Task AlterarAvaliacaoPerformanceAsync(int idAvaliacaoPerformance, AvaliacaoPerformanceModel avaliacao)
    {
        await _repository.AlterarAvaliacaoPerformanceAsync(idAvaliacaoPerformance, avaliacao);
    }

    public async Task AlterarAvaliacaoCompetenciaAsync(int idAvaliacaoCompetencia, AvaliacaoCompetenciaModel avaliacao)
    {
        await _repository.AlterarAvaliacaoCompetenciaAsync(idAvaliacaoCompetencia, avaliacao);
    }

    public async Task AvancarProximaEtapaPerformanceAsync(AvaliacaoPerformanceModel avaliacao, FeedbackAvaliacaoEmailModel avaliacaoEmail, string tipoAvaliacao)
    {
        await _repository.AvancarProximaEtapaPerformanceAsync(avaliacao, avaliacaoEmail, tipoAvaliacao);
    }

    public async Task AvancarProximaEtapaCompetenciaAsync(AvaliacaoCompetenciaModel avaliacao, FeedbackAvaliacaoEmailModel avaliacaoEmail, string tipoAvaliacao)
    {
        await _repository.AvancarProximaEtapaCompetenciaAsync(avaliacao, avaliacaoEmail, tipoAvaliacao);
    }

    public async Task AvancarProximaEtapaEmailAsync(FeedbackAvaliacaoEmailModel avaliacaoEmail)
    {
        await _repository.AvancarProximaEtapaEmailAsync(avaliacaoEmail);
    }
}
