using SistemaAvaliacao.Moderno.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaAvaliacao.Moderno.Services
{
    public interface ICargoService
    {
        Task<List<Cargo>> ObterTodosAsync();
        Task<Cargo?> ObterPorIdAsync(int id);
        Task<bool> CriarAsync(Cargo cargo);
        Task<bool> AtualizarAsync(Cargo cargo);
        Task<bool> InativarAsync(int id);
        Task<List<Cargo>> ObterCargosAtivosAsync();
    }
}
