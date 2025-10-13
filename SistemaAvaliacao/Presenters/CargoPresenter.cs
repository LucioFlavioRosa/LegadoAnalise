using SistemaAvaliacao.Domain.Models;
using SistemaAvaliacao.Services;
using System.Collections.Generic;

namespace SistemaAvaliacao.Presenters
{
    public class CargoPresenter
    {
        private readonly CargoService _cargoService;

        public CargoPresenter(CargoService cargoService)
        {
            _cargoService = cargoService;
        }

        public Cargo ObterCargoPorId(int idCargo)
        {
            return _cargoService.ObterPorId(idCargo);
        }

        public IEnumerable<Cargo> ListarCargos()
        {
            return _cargoService.Listar();
        }

        public void CadastrarCargo(Cargo cargo)
        {
            _cargoService.Inserir(cargo);
        }

        public void AtualizarCargo(Cargo cargo)
        {
            _cargoService.Atualizar(cargo);
        }

        public void InativarCargo(int idCargo)
        {
            _cargoService.Inativar(idCargo);
        }
    }
}
