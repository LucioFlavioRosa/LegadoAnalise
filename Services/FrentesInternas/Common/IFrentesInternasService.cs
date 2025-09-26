using Services.FrentesInternas.Common.Models;

namespace Services.FrentesInternas.Common;

public interface IFrentesInternasService
{
    Task<List<PDIPillsModel>> CarregarPeriodosAsync(int usuarioId);
    Task<List<FrentePill>> CarregarAvaliacoesAsync(int usuarioId);
    Task ValidarAlocacoesInternasAsync(int idPeriodo, int usuarioId);
    Task AtualizarNotaAsync(int idAvaliacao, int idNota);
    Task AtualizarComentarioAsync(int idAvaliacao, string comentario);
    Task AtualizarValidadoAsync(int idAvaliacao, bool validado);
}