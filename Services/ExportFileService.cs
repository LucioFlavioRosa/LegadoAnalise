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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Dados");
                if (data.Count > 0)
                {
                    var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    for (int i = 0; i < props.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = props[i].Name;
                    }
                    for (int row = 0; row < data.Count; row++)
                    {
                        for (int col = 0; col < props.Length; col++)
                        {
                            worksheet.Cells[row + 2, col + 1].Value = props[col].GetValue(data[row]);
                        }
                    }
                }
                return package.GetAsByteArray();
            }
        }
    }
}