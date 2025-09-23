using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IPremissasService
{
    Task<List<PremissasRadar>> ObterListaAsync();
    Task<PremissasRadar?> ObterAsync(int id);
    Task<bool> InserirAsync(PremissasRadar premissa);
    Task<bool> AlterarAsync(PremissasRadar premissa);
    Task<bool> InativarAsync(int id);
    Task<List<Eixo>> ObterEixosAsync();
    Task<List<Cargo>> ObterCargosAsync();
    Task<List<CargoNivel>> ObterNiveisAsync();
}