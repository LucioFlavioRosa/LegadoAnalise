using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Data;

namespace Peers.Moderno.Services
{
    public interface IAssociadosService
    {
        Task<List<ASSOCIADOS>> ObterAssociadosAsync();
        Task<ASSOCIADOS?> ObterAssociadoAsync(int id);
        Task<bool> InserirAssociadoAsync(ASSOCIADOS associado);
        Task<bool> AlterarAssociadoAsync(int id, ASSOCIADOS associado);
        Task<bool> ExcluirAssociadoAsync(int id);
    }
}