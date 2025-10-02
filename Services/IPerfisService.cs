using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services
{
    public interface IPerfisService
    {
        Task<List<PERFIS>> ObterListaPerfisAsync(bool ativo);
    }
}