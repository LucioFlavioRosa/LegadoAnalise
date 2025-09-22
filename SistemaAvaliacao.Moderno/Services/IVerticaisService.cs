using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public interface IVerticaisService
{
    Task<List<Vertical>> ObterTodasAsync(bool apenasAtivas = false);
    Task<Vertical?> ObterPorIdAsync(int id);
}