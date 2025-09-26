using Services.FrentesInternas.Common.Models;
using System.Threading.Tasks;

namespace Services.FrentesInternas.Common;

public interface IFrentesInternasService
{
    Task<List<PDIPillsModel>> CarregarPeriodosAsync(int idUsuario);
    Task<List<FrentePill>> CarregarAvaliacoesAsync(int idUsuario);
    Task ValidarAlocacoesInternasAsync(int idPeriodo, int idUsuario);
    Task AtualizarNotaAsync(int idAvaliacao, int idNota);
    Task AtualizarComentarioAsync(int idAvaliacao, string comentario);
    Task AtualizarValidadoAsync(int idAvaliacao, bool validado);
}