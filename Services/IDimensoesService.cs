using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IDimensoesService
{
    Task<List<Dimensao>> ObterListaDimensoesAsync(bool ativo = true, string? tipoAvaliacao = null);
    Task<Dimensao?> ObterDimensaoAsync(int id);
}