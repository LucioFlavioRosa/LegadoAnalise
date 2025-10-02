using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services
{
    public interface IFotosAssociadosService
    {
        Task<FOTOSASSOCIADOS> ObterFotoPorAssociadoAsync(int idAssociado);
        Task<bool> AdicionarFotoAsync(FOTOSASSOCIADOS foto);
        Task<bool> AtualizarFotoAsync(FOTOSASSOCIADOS foto);
    }
}