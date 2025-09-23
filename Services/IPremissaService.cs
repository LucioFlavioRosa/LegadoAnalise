using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IPremissaService
{
    Premissa? ObterPremissaPorCargo(int idCargo);
}