using SistemaAvaliacao.Services;
using SistemaAvaliacao.Domain.Models;
using System.Collections.Generic;

namespace SistemaAvaliacao.Presenters
{
    public class CargoPresenter
    {
        private readonly ICargoView _view;
        private readonly CargoService _cargoService;

        public CargoPresenter(ICargoView view, CargoService cargoService)
        {
            _view = view;
            _cargoService = cargoService;
        }

        public void CarregarCargos()
        {
            var cargos = _cargoService.ObterTodos();
            _view.PreencherGrid(cargos);
        }

        public void SalvarCargo()
        {
            var cargo = _view.ObterDadosFormulario();
            var resultado = _cargoService.Salvar(cargo);
            if (resultado.Sucesso)
            {
                _view.ExibirMensagem("Cargo salvo com sucesso.");
                CarregarCargos();
                _view.LimparFormulario();
            }
            else
            {
                _view.ExibirMensagem(resultado.Mensagem);
            }
        }

        public void AlterarCargo(int idCargo)
        {
            var cargo = _cargoService.ObterPorId(idCargo);
            _view.PreencherFormulario(cargo);
        }

        public void InativarCargo(int idCargo)
        {
            var resultado = _cargoService.Inativar(idCargo);
            if (resultado.Sucesso)
            {
                _view.ExibirMensagem("Cargo inativado com sucesso.");
                CarregarCargos();
            }
            else
            {
                _view.ExibirMensagem(resultado.Mensagem);
            }
        }

        public void ExportarCargos()
        {
            var cargos = _cargoService.ObterTodos();
            _cargoService.ExportarParaExcel(cargos);
        }
    }
}
