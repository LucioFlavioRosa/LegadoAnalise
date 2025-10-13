using System.Collections.Generic;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Repositories
{
    /// <summary>
    /// Contrato para operações de acesso a dados de Cargos.
    /// </summary>
    public interface ICargoRepository
    {
        Cargo ObterPorId(int idCargo);
        IEnumerable<Cargo> ListarTodos();
        void Inserir(Cargo cargo);
        void Atualizar(Cargo cargo);
        void Inativar(int idCargo);
    }
}
