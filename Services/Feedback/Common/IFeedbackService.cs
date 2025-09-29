namespace Peers.Moderno.Services.Feedback.Common;

using System.Threading.Tasks;
using System.Collections.Generic;
using Peers.Moderno.Services.Feedback.Common;

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

public class ProjetoFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class ClienteFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class PeriodoFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class StatusFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class FeedbackFiltroModel
{
    public int? ProjetoId { get; set; }
    public int? StatusId { get; set; }
    public int? PeriodoId { get; set; }
    public int? ClienteId { get; set; }
}

public class FeedbackAvaliacaoEmailModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class AvaliacaoPerformanceModel
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

public class AvaliacaoCompetenciaModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class AssociadoFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
