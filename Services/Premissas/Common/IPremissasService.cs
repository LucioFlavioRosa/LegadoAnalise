using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Premissas.Common;

public interface IPremissasService
{
    Task<List<PremissasRadar>> ListarAsync();
    Task<bool> InserirAsync(PremissasRadar premissa);
    Task<bool> AlterarAsync(PremissasRadar premissa);
    Task<bool> InativarAsync(int id);
    Task<PremissasRadar?> ObterAsync(int id);
    Task<List<Eixo>> ListarEixosAsync();
    Task<List<Cargo>> ListarCargosAsync();
    Task<List<CargoNivel>> ListarNiveisAsync();
    Task<bool> ExistePremissaAsync(int idEixo, int idCargo, int idNivel, int? idPremissaExcluir = null);
}