using System.Collections.Generic;
using SistemaAvaliacao.Models;
using SistemaAvaliacao.Repositories;

namespace SistemaAvaliacao.Services
{
    public class CargoService
    {
        private readonly ICargoRepository _cargoRepository;

        public CargoService(ICargoRepository cargoRepository)
        {
            _cargoRepository = cargoRepository;
        }

        public IEnumerable<CargoViewModel> ObterTodosCargos()
        {
            return _cargoRepository.ObterTodos();
        }

        public CargoViewModel ObterCargoPorId(int id)
        {
            return _cargoRepository.ObterPorId(id);
        }

        public void CadastrarOuAtualizarCargo(CargoViewModel cargo)
        {
            if (cargo.IdCargo == 0)
            {
                _cargoRepository.Inserir(cargo);
            }
            else
            {
                _cargoRepository.Atualizar(cargo);
            }
        }

        public void InativarCargo(int id)
        {
            _cargoRepository.Inativar(id);
        }

        public IEnumerable<CargoViewModel> ObterCargosParaProximoCargo()
        {
            return _cargoRepository.ObterParaProximoCargo();
        }
    }
}
