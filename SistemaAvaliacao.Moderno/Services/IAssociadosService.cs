using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public interface IAssociadosService
{
    Task<List<Associado>> ObterTodosAsync(bool apenasAtivos = false);
    Task<Associado?> ObterPorIdAsync(int id);
    Task<Associado?> ObterPorEmailAsync(string email);
    Task<bool> InserirAsync(Associado associado);
    Task<bool> AtualizarAsync(Associado associado);
    Task<bool> InativarAsync(int id);
    Task<Associado?> ObterUltimoAsync();
    Task<List<Associado>> ObterMentoresAsync();
}