using SistemaAvaliacao.Domain.Models;
using SistemaAvaliacao.Repositories;
using System.Collections.Generic;

namespace SistemaAvaliacao.Services
{
    public class ResultadoOperacao
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public interface IExportacaoService<T>
    {
        void ExportarParaExcel(IEnumerable<T> entidades);
    }

    public class CargoService
    {
        private readonly ICargoRepository _cargoRepository;
        private readonly IExportacaoService<Cargo> _exportacaoService;

        public CargoService(ICargoRepository cargoRepository, IExportacaoService<Cargo> exportacaoService)
        {
            _cargoRepository = cargoRepository;
            _exportacaoService = exportacaoService;
        }

        public IEnumerable<Cargo> ObterTodos()
        {
            return _cargoRepository.ObterTodos();
        }

        public Cargo ObterPorId(int idCargo)
        {
            return _cargoRepository.ObterPorId(idCargo);
        }

        public ResultadoOperacao Salvar(Cargo cargo)
        {
            if (!Validar(cargo, out string mensagem))
                return new ResultadoOperacao { Sucesso = false, Mensagem = mensagem };

            if (cargo.IdCargo == 0)
                _cargoRepository.Inserir(cargo);
            else
                _cargoRepository.Atualizar(cargo);

            return new ResultadoOperacao { Sucesso = true };
        }

        public ResultadoOperacao Inativar(int idCargo)
        {
            var cargo = _cargoRepository.ObterPorId(idCargo);
            if (cargo == null)
                return new ResultadoOperacao { Sucesso = false, Mensagem = "Cargo não encontrado." };
            _cargoRepository.Inativar(idCargo);
            return new ResultadoOperacao { Sucesso = true };
        }

        public void ExportarParaExcel(IEnumerable<Cargo> cargos)
        {
            _exportacaoService.ExportarParaExcel(cargos);
        }

        private bool Validar(Cargo cargo, out string mensagem)
        {
            if (string.IsNullOrWhiteSpace(cargo.NomeCargo))
            {
                mensagem = "O campo Cargo é obrigatório.";
                return false;
            }
            if (cargo.TempoMinimoPromocao < 0)
            {
                mensagem = "Tempo mínimo para promoção deve ser maior ou igual a zero.";
                return false;
            }
            mensagem = null;
            return true;
        }
    }
}
