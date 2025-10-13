using SistemaAvaliacao.Domain.Models;
using System.Collections.Generic;

namespace SistemaAvaliacao.Services
{
    public class CargoService : ICargoService
    {
        public void CadastrarCargo(Cargo cargo)
        {
            // Implementação da lógica de cadastro de cargo
        }

        public void AtualizarCargo(Cargo cargo)
        {
            // Implementação da lógica de atualização de cargo
        }

        public void InativarCargo(int idCargo)
        {
            // Implementação da lógica de inativação de cargo
        }

        public List<Cargo> ListarCargos()
        {
            // Implementação da lógica de listagem de cargos
            return new List<Cargo>();
        }

        public Cargo ObterCargoPorId(int idCargo)
        {
            // Implementação da lógica de obtenção de cargo por id
            return null;
        }

        public void ExportarCargos(string caminhoArquivo)
        {
            // Implementação da lógica de exportação de cargos
        }
    }
}
