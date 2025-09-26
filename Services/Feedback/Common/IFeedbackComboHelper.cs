namespace Peers.Moderno.Services.Feedback.Common;

using System.Threading.Tasks;
using System.Collections.Generic;

public interface IFeedbackComboHelper
{
    Task<List<ClienteFeedbackModel>> ListarClientesAsync();
    Task<List<PeriodoFeedbackModel>> ListarPeriodosAsync(int empresaId);
    Task<List<StatusFeedbackModel>> ListarStatusAsync();
}