using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public interface IImportFileService
    {
        Task<(int inseridos, int alterados, int desconsiderados)> ImportarAssociadosExcelAsync(Stream fileStream);
        Task<(int inseridos, int alterados, int desconsiderados)> ImportarPromocoesExcelAsync(Stream fileStream);
    }
}