using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IEvolucaoAssociadoDataService
{
    Task<List<EVOLUCAOASSOCIADO>> ObterEvolucaoAsync(int associadoId, string tipoAvaliacao, string escopo);
}