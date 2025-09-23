using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class AssociadosService : IAssociadosService
{
    public async Task<AssociadoInfo> ObterAssociadoMentorCargoAsync(int associadoId)
    {
        await Task.Delay(1);
        return new AssociadoInfo
        {
            Nome = "Nome do Associado",
            Mentor = "Nome do Mentor",
            Cargo = "Cargo Atual",
            ProximoCargo = "Próximo Cargo"
        };
    }
}