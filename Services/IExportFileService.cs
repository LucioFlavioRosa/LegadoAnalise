using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IExportFileService
    {
        Task<byte[]> ExportarAssociadosExcelAsync<T>(List<T> dados);
        Task<byte[]> ExportarPromocoesExcelAsync<T>(List<T> dados);
    }
}