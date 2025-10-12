using System.Collections.Generic;
using SistemaAvaliacao.Models;

namespace SistemaAvaliacao.Repositories
{
    public interface ICargoRepository
    {
        IEnumerable<CargoViewModel> ObterTodos();
        CargoViewModel ObterPorId(int id);
        void Inserir(CargoViewModel cargo);
        void Atualizar(CargoViewModel cargo);
        void Inativar(int id);
        IEnumerable<CargoViewModel> ObterParaProximoCargo();
    }
}
