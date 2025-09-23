using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IEvolucaoAssociadoService
{
    Task<List<EVOLUCAOASSOCIADO>> ObterEvolucaoAsync(int associadoId, string tipoAvaliacao, string escopo);
    Task<AssociadoInfo> ObterAssociadoMentorCargoAsync(int associadoId);
    string FormatarPercentagem(decimal nota);
    string FormatarDecimal(decimal nota);
    string GerarJsonRadar(List<EVOLUCAOASSOCIADO> listaProjetos, int idCargo);
}