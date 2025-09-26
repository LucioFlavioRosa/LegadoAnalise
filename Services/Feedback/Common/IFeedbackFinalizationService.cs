namespace Peers.Moderno.Services.Feedback.Common;

using System.Threading.Tasks;

public interface IFeedbackFinalizationService
{
    Task<FeedbackFinalizationResult> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId);
}

public class FeedbackFinalizationResult
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string? RedirectUrl { get; set; }
}