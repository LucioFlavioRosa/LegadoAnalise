using SistemaAvaliacao.Domain.Models;
using System.Collections.Generic;

namespace SistemaAvaliacao.Services
{
    public interface ICargoService
    {
        void CadastrarCargo(Cargo cargo);
        void AtualizarCargo(Cargo cargo);
        void InativarCargo(int idCargo);
        List<Cargo> ListarCargos();
        Cargo ObterCargoPorId(int idCargo);
        void ExportarCargos(string caminhoArquivo);
    }
}
