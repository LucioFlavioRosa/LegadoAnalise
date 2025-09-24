using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Projetos.Common;

public interface ITiposProjetosService
{
    Task<List<TipoProjeto>> ListarAsync();
    Task<List<TipoProjeto>> ListarAtivosAsync();
    Task<TipoProjeto?> ObterPorIdAsync(int id);
    Task<bool> InserirAsync(TipoProjeto tipoProjeto);
    Task<bool> AlterarAsync(TipoProjeto tipoProjeto);
    Task<bool> InativarAsync(int id);
    Task<bool> ExisteNomeAsync(string nome, int? idExcluir = null);
}