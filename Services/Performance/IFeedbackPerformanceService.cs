using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Performance;

public interface IFeedbackPerformanceService
{
    Task<FeedbackPerformanceViewModel> GetFeedbackAsync(int projetoId, int associadoId, int periodoId);
    Task<List<PerformanceItemViewModel>> GetPerformanceItemsAsync(int projetoId, int associadoId, int periodoId);
    Task<bool> SaveFeedbackAsync(FeedbackPerformanceSaveModel saveModel);
    Task<bool> FinalizarFeedbackAsync(int projetoId, int associadoId, int periodoId);
    Task<bool> ValidarNotasPreenchidasAsync(int projetoId, int associadoId, int periodoId);
}

public class FeedbackPerformanceViewModel
{
    public int ProjetoId { get; set; }
    public string ProjetoNome { get; set; } = string.Empty;
    public int AssociadoId { get; set; }
    public string AssociadoNome { get; set; } = string.Empty;
    public int PeriodoId { get; set; }
    public string PeriodoDescricao { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public string GestorNome { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
    public List<PerformanceItemViewModel> PerformanceItems { get; set; } = new();
}

public class PerformanceItemViewModel
{
    public int IdPerformance { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Abaixo { get; set; } = string.Empty;
    public string Esperado { get; set; } = string.Empty;
    public string Acima { get; set; } = string.Empty;
    public int? NotaAvaliado { get; set; }
    public int? NotaCegas { get; set; }
    public int? NotaGestor { get; set; }
    public int? NotaSelecionada { get; set; }
    public string ObservacaoAvaliado { get; set; } = string.Empty;
    public string ObservacaoGestor { get; set; } = string.Empty;
    public string ConsideracaoFeedback { get; set; } = string.Empty;
    public string Abrangencia { get; set; } = string.Empty;
    public string DisclaimerInput { get; set; } = string.Empty;
    public string SeparadorAbrangencia { get; set; } = string.Empty;
}

public class FeedbackPerformanceSaveModel
{
    public int ProjetoId { get; set; }
    public int AssociadoId { get; set; }
    public int PeriodoId { get; set; }
    public List<PerformanceNotaObservacao> NotasObservacoes { get; set; } = new();
}

public class PerformanceNotaObservacao
{
    public int IdPerformance { get; set; }
    public int NotaSelecionada { get; set; }
    public string ConsideracaoFeedback { get; set; } = string.Empty;
}
