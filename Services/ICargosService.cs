using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Data;

namespace Peers.Moderno.Services
{
    public interface ICargosService
    {
        Task<List<CARGOS>> ObterListaCargosAsync(bool? ativo = null);
        Task<bool> AdicionarPromocaoAsync(PROMOCOES promocao);
        Task<List<PROMOCOES>> ObterPromocoesAssociadoAsync(int idAssociado);
        Task<bool> AlterarPromocaoComentarioAsync(int id, string comentario);
    }
}