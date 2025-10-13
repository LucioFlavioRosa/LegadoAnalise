using SistemaAvaliacao.Domain.Models;
using SistemaAvaliacao.Services;
using System.Collections.Generic;

namespace SistemaAvaliacao.Presenters
{
    public class CargoPresenter
    {
        private readonly ICargoService _cargoService;

        public CargoPresenter(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        public void CadastrarOuAtualizarCargo(Cargo cargo)
        {
            if (cargo.IdCargo > 0)
            {
                _cargoService.AtualizarCargo(cargo);
            }
            else
            {
                _cargoService.CadastrarCargo(cargo);
            }
        }

        public void InativarCargo(int idCargo)
        {
            _cargoService.InativarCargo(idCargo);
        }

        public List<Cargo> ListarCargos()
        {
            return _cargoService.ListarCargos();
        }

        public Cargo ObterCargoPorId(int idCargo)
        {
            return _cargoService.ObterCargoPorId(idCargo);
        }

        public void ExportarCargos(string caminhoArquivo)
        {
            _cargoService.ExportarCargos(caminhoArquivo);
        }
    }
}
