using Business.Model;
using Business.DataAccess;
using Microsoft.Extensions.Logging;

namespace Business.Services
{
    public class AssociadosService : IAssociadosService
    {
        private readonly ILogger<AssociadosService> _logger;
        private readonly IDataRepository _dataRepository;

        public AssociadosService(ILogger<AssociadosService> logger, IDataRepository dataRepository)
        {
            _logger = logger;
            _dataRepository = dataRepository;
        }

        public async Task<AssociadoInfo> ObterAssociadoMentorCargoAsync(int associadoId)
        {
            try
            {
                _logger.LogInformation("Buscando informações do associado {AssociadoId}", associadoId);
                
                return await _dataRepository.GetAssociadoInfoAsync(associadoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar informações do associado {AssociadoId}", associadoId);
                throw;
            }
        }
    }
}