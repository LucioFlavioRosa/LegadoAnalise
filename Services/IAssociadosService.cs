using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services
{
    public interface IAssociadosService
    {
        Task<List<ASSOCIADOS>> ObterAssociadosAsync();
        Task<ASSOCIADOS> ObterAssociadoAsync(int id);
        Task<ASSOCIADOS> ObterAssociadoPorEmailAsync(string email);
        Task<ASSOCIADOS> ObterUltimoAssociadoAsync();
        Task<bool> InserirAssociadoAsync(ASSOCIADOS associado);
        Task<bool> AlteraAssociadoAsync(int id, ASSOCIADOS associado);
        Task<bool> ExcluiAssociadoAsync(int id, ASSOCIADOS associado);
    }
}