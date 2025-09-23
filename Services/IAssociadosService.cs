using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IAssociadosService
{
    Task<AssociadoInfo> ObterAssociadoMentorCargoAsync(int associadoId);
}