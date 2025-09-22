using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public interface IPromocoesService
{
    Task<List<Promocao>> ObterPorAssociadoAsync(int idAssociado);
    Task<List<Promocao>> ObterTodasAsync();
    Task<bool> InserirAsync(Promocao promocao);
    Task<bool> AtualizarAsync(Promocao promocao);
    Task<bool> AtualizarComentarioAsync(int idPromocao, string comentario);
    Task<Promocao?> ObterPorIdAsync(int idPromocao);
}