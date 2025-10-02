using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services
{
    public interface IVerticalService
    {
        Task<List<VERTICAL>> ListarVerticaisAsync();
    }
}