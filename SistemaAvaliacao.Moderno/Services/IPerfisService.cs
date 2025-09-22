using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public interface IPerfisService
{
    Task<List<Perfil>> ObterTodosAsync(bool apenasAtivos = false);
    Task<Perfil?> ObterPorIdAsync(int id);
    Task<bool> InserirAsync(Perfil perfil);
    Task<bool> AtualizarAsync(Perfil perfil);
}