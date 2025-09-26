using Services.PDI.Common.Models;

namespace Services.PDI.Common;

public interface IPDIService
{
    Task<List<PDIPeriodosModel>> ObterPeriodosAsync(int idAssociado);
    Task<List<PDIQuestoesModel>> ObterQuestoesAsync(int idPeriodo);
    Task<List<PDIRespostasModel>> ObterRespostasAsync(int idAssociado, int idPeriodo);
    Task AtualizarRespostaAsync(int idPDIResposta, string resposta);
}