using Business.Model;
using Business.DataAccess;
using Microsoft.Extensions.Logging;

namespace Business.Services
{
    public class PremissaService : IPremissaService
    {
        private readonly ILogger<PremissaService> _logger;
        private readonly IDataRepository _dataRepository;

        public PremissaService(ILogger<PremissaService> logger, IDataRepository dataRepository)
        {
            _logger = logger;
            _dataRepository = dataRepository;
        }

        public async Task<Premissa?> ObterPremissaPorCardoAsync(int idCargo)
        {
            try
            {
                _logger.LogInformation("Buscando premissa para cargo {IdCargo}", idCargo);
                
                return await _dataRepository.GetPremissaPorCargoAsync(idCargo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar premissa para cargo {IdCargo}", idCargo);
                throw;
            }
        }
    }
}