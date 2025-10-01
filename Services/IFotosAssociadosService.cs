using System.Threading.Tasks;
using Models;

namespace Services
{
    public interface IFotosAssociadosService
    {
        Task<FOTOSASSOCIADOS?> ObterFotoPorAssociadoAsync(int idAssociado);
        Task<bool> AdicionarFotoAsync(FOTOSASSOCIADOS foto);
        Task<bool> AtualizarFotoAsync(FOTOSASSOCIADOS foto);
    }
}