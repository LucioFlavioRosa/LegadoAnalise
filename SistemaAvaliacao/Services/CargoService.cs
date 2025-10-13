using SistemaAvaliacao.Domain.Models;
using System.Collections.Generic;

namespace SistemaAvaliacao.Services
{
    public interface ICargoRepository
    {
        Cargo ObterPorId(int idCargo);
        IEnumerable<Cargo> Listar();
        void Inserir(Cargo cargo);
        void Atualizar(Cargo cargo);
        void Inativar(int idCargo);
    }

    public class CargoService
    {
        private readonly ICargoRepository _cargoRepository;

        public CargoService(ICargoRepository cargoRepository)
        {
            _cargoRepository = cargoRepository;
        }

        public Cargo ObterPorId(int idCargo)
        {
            return _cargoRepository.ObterPorId(idCargo);
        }

        public IEnumerable<Cargo> Listar()
        {
            return _cargoRepository.Listar();
        }

        public void Inserir(Cargo cargo)
        {
            _cargoRepository.Inserir(cargo);
        }

        public void Atualizar(Cargo cargo)
        {
            _cargoRepository.Atualizar(cargo);
        }

        public void Inativar(int idCargo)
        {
            _cargoRepository.Inativar(idCargo);
        }
    }
}
