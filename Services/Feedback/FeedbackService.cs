using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Feedback.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Peers.Moderno.Services.Feedback;

public class FeedbackService : Peers.Moderno.Services.Feedback.Common.IFeedbackService
{
    private readonly ApplicationDbContext _context;
    private readonly IFeedbackValidator _validator;

    public FeedbackService(ApplicationDbContext context, IFeedbackValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<List<Competencia>> GetCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao)
    {
        return await _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .Where(c => c.Cargo.IdCargo == _context.Associados.Where(a => a.Id == idAssociado).Select(a => a.IdCargo).FirstOrDefault())
            .ToListAsync();
    }

    public async Task<bool> SalvarFeedbackAsync(List<FeedbackCompetenciaInput> feedbacks, int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao, bool finalizar = false)
    {
        foreach (var feedback in feedbacks)
        {
            var competencia = await _context.Competencias.FindAsync(feedback.IdCompetencia);
            if (competencia == null)
                continue;
            if (!_validator.ValidarNotas(feedback.NotaNivel1, feedback.NotaNivel2))
                return false;
            // Atualização dos campos de feedback na entidade apropriada
            // Exemplo: competencia.NotaNivel1Feedback = feedback.NotaNivel1;
            //          competencia.NotaNivel2Feedback = feedback.NotaNivel2;
            //          competencia.ComentariosFeedback = feedback.Comentario;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProjetoFeedbackModel>> ListarProjetosAsync(int gestorId, int? projetoId = null, int? statusId = null, int? periodoId = null, int? clienteId = null)
    {
        throw new System.NotImplementedException("TODO: Implementar listagem de projetos");
    }

    public async Task<List<ClienteFeedbackModel>> ListarClientesAsync()
    {
        throw new System.NotImplementedException("TODO: Implementar listagem de clientes");
    }

    public async Task<List<PeriodoFeedbackModel>> ListarPeriodosAsync(int empresaId)
    {
        throw new System.NotImplementedException("TODO: Implementar listagem de períodos");
    }

    public async Task<List<StatusFeedbackModel>> ListarStatusAsync()
    {
        throw new System.NotImplementedException("TODO: Implementar listagem de status");
    }

    public async Task<List<ProjetoFeedbackModel>> FiltrarAvaliacoesAsync(FeedbackFiltroModel filtro, int gestorId, int empresaId)
    {
        throw new System.NotImplementedException("TODO: Implementar filtro de avaliações");
    }

    public async Task<FeedbackAvaliacaoEmailModel?> ObterAvaliacaoEmailAsync(int idAvaliacao)
    {
        throw new System.NotImplementedException("TODO: Implementar obtenção de avaliação por email");
    }

    public async Task<List<AvaliacaoPerformanceModel>> ObterAvaliacoesPerformanceAsync(int idAssociado, int idProjeto, int idPeriodo, string etapa, bool apenasFeedback)
    {
        throw new System.NotImplementedException("TODO: Implementar obtenção de avaliações de performance");
    }

    public async Task<List<AvaliacaoCompetenciaModel>> ObterAvaliacoesCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string etapa, string tipoAvaliacao, string escopo, int idAvaliacaoEmail)
    {
        throw new System.NotImplementedException("TODO: Implementar obtenção de avaliações de competências");
    }

    public async Task<AssociadoFeedbackModel?> ObterAssociadoAsync(int idAssociado)
    {
        throw new System.NotImplementedException("TODO: Implementar obtenção de associado");
    }

    public async Task<StatusFeedbackModel?> ObterStatusAvaliacaoAsync(string statusNome)
    {
        throw new System.NotImplementedException("TODO: Implementar obtenção de status de avaliação");
    }

    public async Task AlterarAvaliacaoPerformanceAsync(int idAvaliacaoPerformance, AvaliacaoPerformanceModel avaliacao)
    {
        throw new System.NotImplementedException("TODO: Implementar alteração de avaliação de performance");
    }

    public async Task AlterarAvaliacaoCompetenciaAsync(int idAvaliacaoCompetencia, AvaliacaoCompetenciaModel avaliacao)
    {
        throw new System.NotImplementedException("TODO: Implementar alteração de avaliação de competência");
    }

    public async Task AvancarProximaEtapaPerformanceAsync(AvaliacaoPerformanceModel avaliacao, FeedbackAvaliacaoEmailModel avaliacaoEmail, string tipoAvaliacao)
    {
        throw new System.NotImplementedException("TODO: Implementar avanço para próxima etapa de performance");
    }

    public async Task AvancarProximaEtapaCompetenciaAsync(AvaliacaoCompetenciaModel avaliacao, FeedbackAvaliacaoEmailModel avaliacaoEmail, string tipoAvaliacao)
    {
        throw new System.NotImplementedException("TODO: Implementar avanço para próxima etapa de competência");
    }

    public async Task AvancarProximaEtapaEmailAsync(FeedbackAvaliacaoEmailModel avaliacaoEmail)
    {
        throw new System.NotImplementedException("TODO: Implementar avanço para próxima etapa de email");
    }
}

public class FeedbackCompetenciaInput
{
    public int IdCompetencia { get; set; }
    public int NotaNivel1 { get; set; }
    public int NotaNivel2 { get; set; }
    public string Comentario { get; set; } = string.Empty;
}
