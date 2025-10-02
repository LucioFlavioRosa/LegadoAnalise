using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using System.Reflection;

namespace Peers.Moderno.Services
{
    public class ExportFileService : IExportFileService
    {
        public byte[] GenerateExcelConsideracoesMentor<T>(string fileName, List<T> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Export");
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = properties[col].GetValue(data[row]);
                    }
                }
                return package.GetAsByteArray();
            }
        }
    }
}