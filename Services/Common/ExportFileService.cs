using OfficeOpenXml;
using System.ComponentModel;
using System.Reflection;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Common;

public interface IExportFileService
{
    Task<byte[]> ExportToExcelAsync<T>(List<T> data, string sheetName = "Data");
    Task<byte[]> ExportToExcelWithCustomColumnsAsync<T>(List<T> data, Dictionary<string, string> columnMappings, string sheetName = "Data");
    Task<byte[]> GenerateExcelSubCompetenciasAsync<T>(string fileName, List<T> data);
    Task<byte[]> GenerateExcelConsideracoesMentorAsync<T>(string fileName, List<T> data);
    string GenerateFileName(string prefix, string extension = ".xlsx");
}

public class ExportFileService : IExportFileService
{
    private readonly ITelemetryService _telemetryService;

    public ExportFileService(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<byte[]> ExportToExcelAsync<T>(List<T> data, string sheetName = "Data")
    {
        try
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            if (data == null || !data.Any())
            {
                worksheet.Cells[1, 1].Value = "Nenhum dado encontrado";
                return package.GetAsByteArray();
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
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
            for (int row = 0; row < data.Count; row++)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(data[row]);
                    worksheet.Cells[row + 2, col + 1].Value = FormatCellValue(value);
                }
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            _telemetryService.TrackEvent("ExcelExportCompleted", new Dictionary<string, string>
            {
                { "RecordCount", data.Count.ToString() },
                { "SheetName", sheetName },
                { "DataType", typeof(T).Name }
            });

            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExportToExcelAsync" },
                { "Component", "ExportFileService" },
                { "DataType", typeof(T).Name }
            });
            throw;
        }
    }

    public async Task<byte[]> ExportToExcelWithCustomColumnsAsync<T>(List<T> data, Dictionary<string, string> columnMappings, string sheetName = "Data")
    {
        try
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            if (data == null || !data.Any())
            {
                worksheet.Cells[1, 1].Value = "Nenhum dado encontrado";
                return package.GetAsByteArray();
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && columnMappings.ContainsKey(p.Name))
                .ToArray();

            // Cabeçalhos personalizados
            for (int i = 0; i < properties.Length; i++)
            {
                var columnName = columnMappings[properties[i].Name];
                worksheet.Cells[1, i + 1].Value = columnName;
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            // Dados
            for (int row = 0; row < data.Count; row++)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(data[row]);
                    worksheet.Cells[row + 2, col + 1].Value = FormatCellValue(value);
                }
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExportToExcelWithCustomColumnsAsync" },
                { "Component", "ExportFileService" },
                { "DataType", typeof(T).Name }
            });
            throw;
        }
    }

    public async Task<byte[]> GenerateExcelSubCompetenciasAsync<T>(string fileName, List<T> data)
    {
        var columnMappings = new Dictionary<string, string>
        {
            { "IdSubCompetencia", "Código" },
            { "Nome", "Sub Competência" },
            { "TipoAvaliacao", "Tipo Avaliação" },
            { "Ativo", "Status" }
        };

        return await ExportToExcelWithCustomColumnsAsync(data, columnMappings, "SubCompetencias");
    }

    public async Task<byte[]> GenerateExcelConsideracoesMentorAsync<T>(string fileName, List<T> data)
    {
        return await ExportToExcelAsync(data, "ConsideracoesMentor");
    }

    public string GenerateFileName(string prefix, string extension = ".xlsx")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return $"{prefix}_{timestamp}{extension}";
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

    private static object? FormatCellValue(object? value)
    {
        if (value == null)
            return string.Empty;

        if (value is bool boolValue)
            return boolValue ? "Ativo" : "Inativo";

        if (value is DateTime dateValue)
            return dateValue.ToString("dd/MM/yyyy HH:mm:ss");

        return value;
    }
}