using System.Collections.Generic;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Repositories
{
    /// <summary>
    /// Contrato para operações de persistência da entidade Cargo.
    /// </summary>
    public interface ICargoRepository
    {
        IEnumerable<Cargo> ObterTodos();
        Cargo ObterPorId(int idCargo);
        void Inserir(Cargo cargo);
        void Atualizar(Cargo cargo);
        void Inativar(int idCargo);
        byte[] ExportarParaExcel(IEnumerable<Cargo> cargos);
    }
}
