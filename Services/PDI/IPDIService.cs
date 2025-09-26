using Peers.Moderno.Services.PDI.Common.Models;

namespace Peers.Moderno.Services.PDI;

public interface IPDIService
{
    Task<List<PDIPeriodoModel>> GetPdiPeriodosAsync(int idAssociado);
    Task<List<PDIPillsModel>> GetPdiPillsAsync(int idAssociado);
    Task AtualizarRespostaAsync(int idPDIResposta, string resposta);
    Task ValidaPDIRespostasAsync(int idAssociado, int idPeriodo);
}