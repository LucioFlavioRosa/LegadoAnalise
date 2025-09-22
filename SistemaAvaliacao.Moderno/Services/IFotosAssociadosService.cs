using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public interface IFotosAssociadosService
{
    Task<FotoAssociado?> ObterPorAssociadoAsync(int idAssociado);
    Task<bool> InserirAsync(FotoAssociado foto);
    Task<bool> AtualizarAsync(FotoAssociado foto);
    Task<bool> RemoverAsync(int idAssociado);
}