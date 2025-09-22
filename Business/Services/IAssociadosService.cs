using Business.Model;

namespace Business.Services
{
    public interface IAssociadosService
    {
        Task<AssociadoInfo> ObterAssociadoMentorCargoAsync(int associadoId);
    }
}