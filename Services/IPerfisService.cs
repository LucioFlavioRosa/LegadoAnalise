using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public interface IPerfisService
    {
        Task<List<PERFIS>> ObterListaPerfisAsync(bool? ativo = null);
    }
}