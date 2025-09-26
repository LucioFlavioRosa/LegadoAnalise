using Services.PDI.Common.Models;

namespace Services.PDI.Common;

public interface IPDIService
{
    Task<List<PDIPeriodoModel>> GetPeriodosAsync(int idAssociado);
    Task<List<PDIColunaModel>> GetColunasAsync(int idAssociado, int idPeriodo);
    Task<List<PDIRespostaModel>> GetRespostasAsync(int idAssociado, int idPeriodo);
    Task AtualizarRespostaAsync(int idPDIResposta, string resposta);
    Task ValidaPDIRespostasAsync(int idAssociado, int idPeriodo);
}