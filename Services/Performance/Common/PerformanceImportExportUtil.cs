using OfficeOpenXml;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using System.Data;

namespace Peers.Moderno.Services.Performance.Common;

public interface IPerformanceImportExportUtil
{
    Task<byte[]> ExportPerformancesToExcelAsync(List<Performance> performances);
    Task<PerformanceImportResult> ImportPerformancesFromExcelAsync(Stream fileStream);
    Task<List<PerformanceExportModel>> PrepareExportDataAsync(List<Performance> performances);
    Task<List<PerformanceImportModel>> ParseImportDataAsync(DataTable dataTable);
    bool ValidateExcelStructure(DataTable dataTable);
}

public class PerformanceImportExportUtil : IPerformanceImportExportUtil
{
    private readonly ITelemetryService _telemetryService;
    private readonly IPerformanceValidationUtil _validationUtil;
    private readonly IPerformanceComboHelper _comboHelper;

    public PerformanceImportExportUtil(
        ITelemetryService telemetryService,
        IPerformanceValidationUtil validationUtil,
        IPerformanceComboHelper comboHelper)
    {
        _telemetryService = telemetryService;
        _validationUtil = validationUtil;
        _comboHelper = comboHelper;
    }

    public async Task<byte[]> ExportPerformancesToExcelAsync(List<Performance> performances)
    {
        try
        {
            var exportData = await PrepareExportDataAsync(performances);
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Performances");
            
            // Headers
            var headers = new string[]
            {
                "IdPerformance", "IdCargo", "Cargo", "Performance", "DescricaoAbaixo",
                "DescricaoEsperado", "DescricaoAcima", "ATV", "Abrangencia",
                "InputAutoAvaliacao", "IdNotaPadraoAutoAvaliacao", "NotaPadraoAutoAvaliacao",
                "InputAvaliacaoAsCegas", "IdNotaPadraoAvaliacaoAsCegas", "NotaPadraoAvaliacaoAsCegas",
                "InputAvaliacaoGestor", "IdNotaPadraoAvaliacaoGestor", "NotaPadraoAvaliacaoGestor"
            };
            
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }
            
            // Data
            for (int row = 0; row < exportData.Count; row++)
            {
                var item = exportData[row];
                var excelRow = row + 2;
                
                worksheet.Cells[excelRow, 1].Value = item.IdPerformance;
                worksheet.Cells[excelRow, 2].Value = item.IdCargo;
                worksheet.Cells[excelRow, 3].Value = item.Cargo;
                worksheet.Cells[excelRow, 4].Value = item.Performance;
                worksheet.Cells[excelRow, 5].Value = item.DescricaoAbaixo;
                worksheet.Cells[excelRow, 6].Value = item.DescricaoEsperado;
                worksheet.Cells[excelRow, 7].Value = item.DescricaoAcima;
                worksheet.Cells[excelRow, 8].Value = item.ATV;
                worksheet.Cells[excelRow, 9].Value = item.Abrangencia;
                worksheet.Cells[excelRow, 10].Value = item.InputAutoAvaliacao;
                worksheet.Cells[excelRow, 11].Value = item.IdNotaPadraoAutoAvaliacao;
                worksheet.Cells[excelRow, 12].Value = item.NotaPadraoAutoAvaliacao;
                worksheet.Cells[excelRow, 13].Value = item.InputAvaliacaoAsCegas;
                worksheet.Cells[excelRow, 14].Value = item.IdNotaPadraoAvaliacaoAsCegas;
                worksheet.Cells[excelRow, 15].Value = item.NotaPadraoAvaliacaoAsCegas;
                worksheet.Cells[excelRow, 16].Value = item.InputAvaliacaoGestor;
                worksheet.Cells[excelRow, 17].Value = item.IdNotaPadraoAvaliacaoGestor;
                worksheet.Cells[excelRow, 18].Value = item.NotaPadraoAvaliacaoGestor;
            }
            
            worksheet.Cells.AutoFitColumns();
            
            _telemetryService.TrackEvent("PerformanceExported", new Dictionary<string, string>
            {
                { "RecordCount", exportData.Count.ToString() },
                { "ExportType", "Excel" }
            });
            
            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExportPerformancesToExcelAsync" },
                { "Component", "PerformanceImportExportUtil" }
            });
            throw;
        }
    }

    public async Task<PerformanceImportResult> ImportPerformancesFromExcelAsync(Stream fileStream)
    {
        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
            {
                return PerformanceImportResult.Error("Planilha não encontrada");
            }
            
            var dataTable = new DataTable();
            bool hasHeader = true;
            
            // Create columns
            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
            {
                dataTable.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
            }
            
            // Validate structure
            if (!ValidateExcelStructure(dataTable))
            {
                return PerformanceImportResult.Error("Estrutura da planilha inválida");
            }
            
            // Read data
            var startRow = hasHeader ? 2 : 1;
            for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                DataRow row = dataTable.NewRow();
                foreach (var cell in wsRow)
                {
                    row[cell.Start.Column - 1] = cell.Text;
                }
                dataTable.Rows.Add(row);
            }
            
            var importModels = await ParseImportDataAsync(dataTable);
            var validationResults = new List<PerformanceValidationResult>();
            
            foreach (var model in importModels)
            {
                var validation = await _validationUtil.ValidatePerformanceAsync(model.ToPerformance());
                validationResults.Add(validation);
            }
            
            var validItems = importModels.Where((model, index) => validationResults[index].IsValid).ToList();
            var invalidItems = importModels.Where((model, index) => !validationResults[index].IsValid)
                .Select((model, index) => new PerformanceImportError
                {
                    RowNumber = index + 2,
                    ErrorMessage = validationResults.FirstOrDefault(v => !v.IsValid)?.ErrorMessage ?? "Erro desconhecido",
                    Data = model
                }).ToList();
            
            _telemetryService.TrackEvent("PerformanceImported", new Dictionary<string, string>
            {
                { "TotalRecords", importModels.Count.ToString() },
                { "ValidRecords", validItems.Count.ToString() },
                { "InvalidRecords", invalidItems.Count.ToString() }
            });
            
            return PerformanceImportResult.Success(validItems, invalidItems);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ImportPerformancesFromExcelAsync" },
                { "Component", "PerformanceImportExportUtil" }
            });
            return PerformanceImportResult.Error($"Erro ao importar: {ex.Message}");
        }
    }

    public async Task<List<PerformanceExportModel>> PrepareExportDataAsync(List<Performance> performances)
    {
        var exportData = new List<PerformanceExportModel>();
        
        foreach (var performance in performances)
        {
            exportData.Add(new PerformanceExportModel
            {
                IdPerformance = performance.IdPerformance,
                IdCargo = performance.IdCargo,
                Cargo = performance.Cargo?.Nome ?? "",
                Performance = performance.Nome,
                DescricaoAbaixo = performance.PerformanceAbaixo,
                DescricaoEsperado = performance.PerformanceEsperado,
                DescricaoAcima = performance.PerformanceAcima,
                ATV = performance.Ativo ? 1 : 0,
                Abrangencia = performance.Abrangencia,
                InputAutoAvaliacao = performance.InputAutoavaliacao ? 1 : 0,
                IdNotaPadraoAutoAvaliacao = performance.NotaPadraoAutoAvaliacao,
                NotaPadraoAutoAvaliacao = performance.NotaAutoAvaliacao?.CodigoNota ?? "",
                InputAvaliacaoAsCegas = performance.InputAvaliacaoAsCegas ? 1 : 0,
                IdNotaPadraoAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas,
                NotaPadraoAvaliacaoAsCegas = performance.NotaAvaliacaoAsCegas?.CodigoNota ?? "",
                InputAvaliacaoGestor = performance.InputAvaliacaoGestor ? 1 : 0,
                IdNotaPadraoAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor,
                NotaPadraoAvaliacaoGestor = performance.NotaAvaliacaoGestor?.CodigoNota ?? ""
            });
        }
        
        return await Task.FromResult(exportData);
    }

    public async Task<List<PerformanceImportModel>> ParseImportDataAsync(DataTable dataTable)
    {
        var importModels = new List<PerformanceImportModel>();
        
        foreach (DataRow row in dataTable.Rows)
        {
            var model = new PerformanceImportModel
            {
                IdPerformance = ParseInt(row[0]),
                IdCargo = ParseInt(row[1]) ?? 0,
                Performance = ParseString(row[3]),
                DescricaoAbaixo = ParseString(row[4]),
                DescricaoEsperado = ParseString(row[5]),
                DescricaoAcima = ParseString(row[6]),
                ATV = ParseInt(row[7]) ?? 1,
                Abrangencia = ParseString(row[8]),
                InputAutoAvaliacao = ParseInt(row[9]) ?? 1,
                IdNotaPadraoAutoAvaliacao = ParseInt(row[10]),
                InputAvaliacaoAsCegas = ParseInt(row[12]) ?? 1,
                IdNotaPadraoAvaliacaoAsCegas = ParseInt(row[13]),
                InputAvaliacaoGestor = ParseInt(row[15]) ?? 1,
                IdNotaPadraoAvaliacaoGestor = ParseInt(row[16])
            };
            
            importModels.Add(model);
        }
        
        return await Task.FromResult(importModels);
    }

    public bool ValidateExcelStructure(DataTable dataTable)
    {
        var requiredColumns = new string[]
        {
            "IdPerformance", "IdCargo", "Cargo", "Performance", "DescricaoAbaixo",
            "DescricaoEsperado", "DescricaoAcima", "ATV", "Abrangencia"
        };
        
        return requiredColumns.All(col => dataTable.Columns.Contains(col));
    }

    private int? ParseInt(object value)
    {
        if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
            return null;
        
        if (int.TryParse(value.ToString(), out int result))
            return result;
        
        return null;
    }

    private string ParseString(object value)
    {
        if (value == null || value == DBNull.Value)
            return string.Empty;
        
        return value.ToString()?.Trim() ?? string.Empty;
    }
}

public class PerformanceExportModel
{
    public int IdPerformance { get; set; }
    public int IdCargo { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public string Performance { get; set; } = string.Empty;
    public string DescricaoAbaixo { get; set; } = string.Empty;
    public string DescricaoEsperado { get; set; } = string.Empty;
    public string DescricaoAcima { get; set; } = string.Empty;
    public int ATV { get; set; }
    public string Abrangencia { get; set; } = string.Empty;
    public int InputAutoAvaliacao { get; set; }
    public int? IdNotaPadraoAutoAvaliacao { get; set; }
    public string NotaPadraoAutoAvaliacao { get; set; } = string.Empty;
    public int InputAvaliacaoAsCegas { get; set; }
    public int? IdNotaPadraoAvaliacaoAsCegas { get; set; }
    public string NotaPadraoAvaliacaoAsCegas { get; set; } = string.Empty;
    public int InputAvaliacaoGestor { get; set; }
    public int? IdNotaPadraoAvaliacaoGestor { get; set; }
    public string NotaPadraoAvaliacaoGestor { get; set; } = string.Empty;
}

public class PerformanceImportModel
{
    public int? IdPerformance { get; set; }
    public int IdCargo { get; set; }
    public string Performance { get; set; } = string.Empty;
    public string DescricaoAbaixo { get; set; } = string.Empty;
    public string DescricaoEsperado { get; set; } = string.Empty;
    public string DescricaoAcima { get; set; } = string.Empty;
    public int ATV { get; set; }
    public string Abrangencia { get; set; } = string.Empty;
    public int InputAutoAvaliacao { get; set; }
    public int? IdNotaPadraoAutoAvaliacao { get; set; }
    public int InputAvaliacaoAsCegas { get; set; }
    public int? IdNotaPadraoAvaliacaoAsCegas { get; set; }
    public int InputAvaliacaoGestor { get; set; }
    public int? IdNotaPadraoAvaliacaoGestor { get; set; }

    public Performance ToPerformance()
    {
        return new Performance
        {
            IdPerformance = IdPerformance ?? 0,
            IdCargo = IdCargo,
            Nome = Performance,
            PerformanceAbaixo = DescricaoAbaixo,
            PerformanceEsperado = DescricaoEsperado,
            PerformanceAcima = DescricaoAcima,
            Ativo = ATV == 1,
            Abrangencia = Abrangencia,
            InputAutoavaliacao = InputAutoAvaliacao == 1,
            NotaPadraoAutoAvaliacao = IdNotaPadraoAutoAvaliacao,
            InputAvaliacaoAsCegas = InputAvaliacaoAsCegas == 1,
            NotaPadraoAvaliacaoAsCegas = IdNotaPadraoAvaliacaoAsCegas,
            InputAvaliacaoGestor = InputAvaliacaoGestor == 1,
            NotaPadraoAvaliacaoGestor = IdNotaPadraoAvaliacaoGestor
        };
    }
}

public class PerformanceImportResult
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public List<PerformanceImportModel> ValidItems { get; private set; } = new();
    public List<PerformanceImportError> InvalidItems { get; private set; } = new();
    public int TotalProcessed => ValidItems.Count + InvalidItems.Count;
    public int ValidCount => ValidItems.Count;
    public int InvalidCount => InvalidItems.Count;

    private PerformanceImportResult() { }

    public static PerformanceImportResult Success(List<PerformanceImportModel> validItems, List<PerformanceImportError> invalidItems)
    {
        return new PerformanceImportResult
        {
            IsSuccess = true,
            ValidItems = validItems,
            InvalidItems = invalidItems
        };
    }

    public static PerformanceImportResult Error(string errorMessage)
    {
        return new PerformanceImportResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public class PerformanceImportError
{
    public int RowNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public PerformanceImportModel Data { get; set; } = new();
}