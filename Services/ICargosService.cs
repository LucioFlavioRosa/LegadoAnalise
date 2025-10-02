using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services
{
    public interface ICargosService
    {
        Task<List<CARGOS>> ObterListaCargosAsync(bool ativo);
        Task<List<PROMOCOES>> ObterTodasPromocoesAssociadoAsync(int idAssociado);
        Task<bool> AdicionarPromocaoAsync(PROMOCOES promocao);
        Task<bool> AlterarPromocaoComentarioAsync(int idPromocao, string comentario);
        Task<List<PROMOCOES>> ObterListaPromocoesAsync();
        Task<PROMOCOES> ObterPromocaoAsync(int idAssociado, int idCargoAnterior, int idCargoNovo);
        Task<bool> AlterarPromocaoAsync(PROMOCOES promocao);
    }
}