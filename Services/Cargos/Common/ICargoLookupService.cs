namespace Peers.Moderno.Services.Cargos.Common;

public interface ICargoLookupService
{
    Task<Models.Cargo?> BuscarPorCodigoAsync(string codigo);
}