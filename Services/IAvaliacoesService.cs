using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IAvaliacoesService
{
    Task<List<AvaliacaoCompetenciaNota>> ObterAvaliacaoCompetenciasNotasAsync();
    Task<List<ModoCalculoCompetencia>> ObterModosCalculosCompetenciasAsync();
}