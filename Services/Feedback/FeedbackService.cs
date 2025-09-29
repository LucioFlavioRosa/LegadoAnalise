using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Feedback.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Peers.Moderno.Services.Feedback;

public class FeedbackService : IFeedbackService
{
    private readonly ApplicationDbContext _context;
    private readonly IFeedbackValidator _validator;

    public FeedbackService(ApplicationDbContext context, IFeedbackValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<List<ProjetoFeedbackModel>> ListarProjetosAsync(int gestorId, int? projetoId = null, int? statusId = null, int? periodoId = null, int? clienteId = null)
    {
        return new List<ProjetoFeedbackModel>();
    }

    public async Task<List<ClienteFeedbackModel>> ListarClientesAsync()
    {
        return new List<ClienteFeedbackModel>();
    }

    public async Task<List<PeriodoFeedbackModel>> ListarPeriodosAsync(int empresaId)
    {
        return new List<PeriodoFeedbackModel>();
    }

    public async Task<List<StatusFeedbackModel>> ListarStatusAsync()
    {
        return new List<StatusFeedbackModel>();
    }

    public async Task<List<ProjetoFeedbackModel>> FiltrarAvaliacoesAsync(FeedbackFiltroModel filtro, int gestorId, int empresaId)
    {
        return new List<ProjetoFeedbackModel>();
    }

    public async Task<FeedbackAvaliacaoEmailModel?> ObterAvaliacaoEmailAsync(int idAvaliacao)
    {
        return null;
    }

    public async Task<List<AvaliacaoPerformanceModel>> ObterAvaliacoesPerformanceAsync(int idAssociado, int idProjeto, int idPeriodo, string etapa, bool apenasFeedback)
    {
        return new List<AvaliacaoPerformanceModel>();
    }

    public async Task<List<AvaliacaoCompetenciaModel>> ObterAvaliacoesCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string etapa, string tipoAvaliacao, string escopo, int idAvaliacaoEmail)
    {
        return new List<AvaliacaoCompetenciaModel>();
    }

    public async Task<AssociadoFeedbackModel?> ObterAssociadoAsync(int idAssociado)
    {
        return null;
    }

    public async Task<StatusFeedbackModel?> ObterStatusAvaliacaoAsync(string statusNome)
    {
        return null;
    }

    public async Task AlterarAvaliacaoPerformanceAsync(int idAvaliacaoPerformance, AvaliacaoPerformanceModel avaliacao)
    {
        await Task.CompletedTask;
    }

    public async Task AlterarAvaliacaoCompetenciaAsync(int idAvaliacaoCompetencia, AvaliacaoCompetenciaModel avaliacao)
    {
        await Task.CompletedTask;
    }

    public async Task AvancarProximaEtapaPerformanceAsync(AvaliacaoPerformanceModel avaliacao, FeedbackAvaliacaoEmailModel avaliacaoEmail, string tipoAvaliacao)
    {
        await Task.CompletedTask;
    }

    public async Task AvancarProximaEtapaCompetenciaAsync(AvaliacaoCompetenciaModel avaliacao, FeedbackAvaliacaoEmailModel avaliacaoEmail, string tipoAvaliacao)
    {
        await Task.CompletedTask;
    }

    public async Task AvancarProximaEtapaEmailAsync(FeedbackAvaliacaoEmailModel avaliacaoEmail)
    {
        await Task.CompletedTask;
    }
}
