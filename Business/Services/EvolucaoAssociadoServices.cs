using Business.Model;
using Business.DataAccess;
using Microsoft.Extensions.Logging;

namespace Business.Services
{
    public class EvolucaoAssociadoServices : IEvolucaoAssociadoServices
    {
        private readonly ILogger<EvolucaoAssociadoServices> _logger;
        private readonly IDataRepository _dataRepository;

        public EvolucaoAssociadoServices(ILogger<EvolucaoAssociadoServices> logger, IDataRepository dataRepository)
        {
            _logger = logger;
            _dataRepository = dataRepository;
        }

        public async Task<List<EVOLUCAOASSOCIADO>> EvolucaoAsync(int associadoId, string tipoAvaliacao, string escopo)
        {
            try
            {
                _logger.LogInformation("Buscando evolução para associado {AssociadoId}, tipo {TipoAvaliacao}, escopo {Escopo}", 
                    associadoId, tipoAvaliacao, escopo);
                
                return await _dataRepository.GetEvolucaoAssociadoAsync(associadoId, tipoAvaliacao, escopo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar evolução do associado {AssociadoId}", associadoId);
                throw;
            }
        }
    }
}