using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.Performance.Common
{
    public interface IPerformanceService
    {
        Task<List<PerformanceModel>> GetMentorPerformanceAsync(int idAssociado, int idProjeto, int idPeriodo);
        Task<List<PerformanceModel>> GetPerformanceListAsync(int idEmpresa, int idCargo, int idNivel, List<int> listaIdPerformances);
        string TruncateText(string texto, int maxLength);
        List<PerformanceModel> OrganizeByAbrangencia(List<PerformanceModel> performances);
    }
}
