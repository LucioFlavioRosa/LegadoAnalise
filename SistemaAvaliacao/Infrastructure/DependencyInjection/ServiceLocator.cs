using System;
using SistemaAvaliacao.Repositories;
using SistemaAvaliacao.Services;
using SistemaAvaliacao.Presenters;

namespace SistemaAvaliacao.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Service Locator simples para resolver dependências da aplicação.
    /// Centraliza a configuração de instâncias e facilita testes.
    /// </summary>
    public static class ServiceLocator
    {
        private static ICargoRepository _cargoRepository;
        private static CargoService _cargoService;
        private static CargoPresenter _cargoPresenter;

        /// <summary>
        /// Resolve a instância de ICargoRepository.
        /// </summary>
        public static ICargoRepository GetCargoRepository()
        {
            if (_cargoRepository == null)
            {
                // Substitua por injeção real ou configuração conforme necessário
                _cargoRepository = new CargoRepository();
            }
            return _cargoRepository;
        }

        /// <summary>
        /// Resolve a instância de CargoService.
        /// </summary>
        public static CargoService GetCargoService()
        {
            if (_cargoService == null)
            {
                _cargoService = new CargoService(GetCargoRepository());
            }
            return _cargoService;
        }

        /// <summary>
        /// Resolve a instância de CargoPresenter.
        /// </summary>
        /// <param name="view">Instância da view a ser utilizada pelo Presenter.</param>
        public static CargoPresenter GetCargoPresenter(ICargoView view)
        {
            if (_cargoPresenter == null || _cargoPresenter.View != view)
            {
                _cargoPresenter = new CargoPresenter(view, GetCargoService());
            }
            return _cargoPresenter;
        }
    }
}
