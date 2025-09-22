using Business.Model;

namespace Business.Services
{
    public interface IEvolucaoAssociadoServices
    {
        Task<List<EVOLUCAOASSOCIADO>> EvolucaoAsync(int associadoId, string tipoAvaliacao, string escopo);
    }
}