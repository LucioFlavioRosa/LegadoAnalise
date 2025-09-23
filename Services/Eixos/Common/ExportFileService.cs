using OfficeOpenXml;
using System.ComponentModel;
using System.Reflection;

namespace Peers.Moderno.Services.Eixos.Common;

public class ExportFileService : IExportFileService
{
    public ExportFileService()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<byte[]> GenerateExcelAsync<T>(string fileName, IEnumerable<T> data)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Dados");

        var dataList = data.ToList();
        if (!dataList.Any())
        {
            worksheet.Cells[1, 1].Value = "Nenhum dado encontrado";
            return package.GetAsByteArray();
        }

        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && IsSimpleType(p.PropertyType))
            .ToArray();

        // Cabeçalhos
        for (int i = 0; i < properties.Length; i++)
        {
            var displayName = GetDisplayName(properties[i]);
            worksheet.Cells[1, i + 1].Value = displayName;
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        }

        // Dados
        for (int row = 0; row < dataList.Count; row++)
        {
            var item = dataList[row];
            for (int col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(item);
                worksheet.Cells[row + 2, col + 1].Value = value;
            }
        }

        // Auto-fit colunas
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return await Task.FromResult(package.GetAsByteArray());
    }

    public async Task<byte[]> GenerateExcelWithSheetsAsync(string fileName, Dictionary<string, object> sheets)
    {
        using var package = new ExcelPackage();

        foreach (var sheet in sheets)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheet.Key);
            
            if (sheet.Value is IEnumerable<object> data)
            {
                await PopulateWorksheetAsync(worksheet, data);
            }
        }

        return package.GetAsByteArray();
    }

    public string GetContentType()
    {
        return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }

    private async Task PopulateWorksheetAsync(ExcelWorksheet worksheet, IEnumerable<object> data)
    {
        var dataList = data.ToList();
        if (!dataList.Any())
        {
            worksheet.Cells[1, 1].Value = "Nenhum dado encontrado";
            return;
        }

        var firstItem = dataList.First();
        var properties = firstItem.GetType().GetProperties()
            .Where(p => p.CanRead && IsSimpleType(p.PropertyType))
            .ToArray();

        // Cabeçalhos
        for (int i = 0; i < properties.Length; i++)
        {
            var displayName = GetDisplayName(properties[i]);
            worksheet.Cells[1, i + 1].Value = displayName;
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        }

        // Dados
        for (int row = 0; row < dataList.Count; row++)
        {
            var item = dataList[row];
            for (int col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(item);
                worksheet.Cells[row + 2, col + 1].Value = value;
            }
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        await Task.CompletedTask;
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               Nullable.GetUnderlyingType(type) != null;
    }

    private static string GetDisplayName(PropertyInfo property)
    {
        var displayAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
        return displayAttribute?.DisplayName ?? property.Name;
    }
}