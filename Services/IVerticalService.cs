using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public interface IVerticalService
    {
        Task<List<VERTICAL>> ListarVerticaisAsync();
    }
}