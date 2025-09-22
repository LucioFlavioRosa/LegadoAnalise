using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public interface ICargosService
{
    Task<List<Cargo>> ObterTodosAsync(bool apenasAtivos = false);
    Task<Cargo?> ObterPorIdAsync(int id);
    Task<bool> InserirAsync(Cargo cargo);
    Task<bool> AtualizarAsync(Cargo cargo);
}