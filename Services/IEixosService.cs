using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IEixosService
{
    Task<List<Eixo>> ObterListaEixosAsync(bool ativo = true, string? tipoAvaliacao = null);
    Task<Eixo?> ObterEixoAsync(int id);
}