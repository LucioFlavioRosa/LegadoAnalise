using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Dimensoes.Common;

public interface IDimensoesService
{
    Task<List<Dimensao>> ListarAsync();
    Task<Dimensao?> ObterPorIdAsync(int id);
    Task<Dimensao> InserirAsync(Dimensao dimensao);
    Task<Dimensao> AlterarAsync(Dimensao dimensao);
    Task<bool> InativarAsync(int id);
    Task<bool> ExisteAsync(int id);
    Task<bool> ExisteNomeAsync(string nome, int? idExcluir = null);
}