using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;

namespace Services
{
    public class ExportFileService : IExportFileService
    {
        public async Task<byte[]> ExportarAssociadosExcelAsync<T>(List<T> dados)
        {
            return await ExportarExcelAsync(dados, "Associados");
        }

        public async Task<byte[]> ExportarPromocoesExcelAsync<T>(List<T> dados)
        {
            return await ExportarExcelAsync(dados, "Promocoes");
        }

        private Task<byte[]> ExportarExcelAsync<T>(List<T> dados, string sheetName)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1"].LoadFromCollection(dados, true);
                return Task.FromResult(package.GetAsByteArray());
            }
        }
    }
}