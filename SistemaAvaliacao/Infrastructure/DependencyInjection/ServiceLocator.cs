using System;
using SistemaAvaliacao.Services;
using SistemaAvaliacao.Repositories;
using SistemaAvaliacao.Presenters;

namespace SistemaAvaliacao.Infrastructure.DependencyInjection
{
    public static class ServiceLocator
    {
        private static ICargoRepository _cargoRepository;
        private static CargoService _cargoService;
        private static ExportService _exportService;
        private static CargoPresenter _cargoPresenter;

        public static ICargoRepository GetCargoRepository()
        {
            if (_cargoRepository == null)
                _cargoRepository = new CargoRepository();
            return _cargoRepository;
        }

        public static CargoService GetCargoService()
        {
            if (_cargoService == null)
                _cargoService = new CargoService(GetCargoRepository());
            return _cargoService;
        }

        public static ExportService GetExportService()
        {
            if (_exportService == null)
                _exportService = new ExportService();
            return _exportService;
        }

        public static CargoPresenter GetCargoPresenter()
        {
            if (_cargoPresenter == null)
                _cargoPresenter = new CargoPresenter(GetCargoService(), GetExportService());
            return _cargoPresenter;
        }
    }
}
