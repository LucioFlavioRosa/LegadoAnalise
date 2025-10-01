using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public interface IAssociadosService
    {
        Task<List<ASSOCIADOS>> ObterAssociadosAsync(bool? ativo = null);
        Task<ASSOCIADOS?> ObterAssociadoAsync(int id);
        Task<ASSOCIADOS?> ObterAssociadoPorEmailAsync(string email);
        Task<bool> InserirAssociadoAsync(ASSOCIADOS associado);
        Task<bool> AlterarAssociadoAsync(int id, ASSOCIADOS associado);
        Task<bool> InativarAssociadoAsync(int id);
        Task<ASSOCIADOS?> ObterUltimoAssociadoAsync();
    }
}