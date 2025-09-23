using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class PremissaService : IPremissaService
{
    public Premissa? ObterPremissaPorCargo(int idCargo)
    {
        return new Premissa
        {
            Id = 1,
            IdCargo = idCargo,
            ValorRadarPeers = 5
        };
    }
}