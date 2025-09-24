using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using System.Data;

namespace Peers.Moderno.Services.Performance.Common;

public interface IPerformanceImportExportUtil
{
    Task<List<PerformanceExportModel>> PrepareExportDataAsync(List<Models.Performance> performances);
    byte[] GenerateExcelFile(List<PerformanceExportModel> exportData);
    DataTable ReadExcelFile(Stream fileStream);
    Task<PerformanceImportResult> ProcessImportDataAsync(DataTable importData, int userId);
}

public class PerformanceImportExportUtil : IPerformanceImportExportUtil
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;
    private readonly IPerformanceValidationUtil _validationUtil;

    public PerformanceImportExportUtil(
        ApplicationDbContext context,
        ITelemetryService telemetryService,
        IPerformanceValidationUtil validationUtil)
    {
        _context = context;
        _telemetryService = telemetryService;
        _validationUtil = validationUtil;
    }

    public async Task<List<PerformanceExportModel>> PrepareExportDataAsync(List<Models.Performance> performances)
    {
        try
        {
            var exportData = new List<PerformanceExportModel>();
            var cargos = await _context.Cargos.ToDictionaryAsync(c => c.IdCargo, c => c.Nome);
            var notas = await _context.Set<AvaliacaoCompetenciaNota>().ToDictionaryAsync(n => n.IdNota, n => n.CodigoNota);

            foreach (var performance in performances)
            {
                var exportItem = new PerformanceExportModel
                {
                    IdPerformance = performance.IdPerformance,
                    IdCargo = performance.IdCargo,
                    Cargo = cargos.GetValueOrDefault(performance.IdCargo, ""),
                    Performance = performance.Nome,
                    DescricaoAbaixo = performance.PerformanceAbaixo,
                    DescricaoEsperado = performance.PerformanceEsperado,
                    DescricaoAcima = performance.PerformanceAcima,
                    ATV = performance.ATV,
                    Abrangencia = performance.Abrangencia,
                    InputAutoAvaliacao = performance.InputAutoavaliacao ? 1 : 0,
                    IdNotaPadraoAutoAvaliacao = performance.NotaPadraoAutoAvaliacao,
                    NotaPadraoAutoAvaliacao = performance.NotaPadraoAutoAvaliacao.HasValue ? notas.GetValueOrDefault(performance.NotaPadraoAutoAvaliacao.Value, "") : "",
                    InputAvaliacaoAsCegas = performance.InputAvaliacaoAsCegas ? 1 : 0,
                    IdNotaPadraoAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas,
                    NotaPadraoAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas.HasValue ? notas.GetValueOrDefault(performance.NotaPadraoAvaliacaoAsCegas.Value, "") : "",
                    InputAvaliacaoGestor = performance.InputAvaliacaoGestor ? 1 : 0,
                    IdNotaPadraoAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor,
                    NotaPadraoAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor.HasValue ? notas.GetValueOrDefault(performance.NotaPadraoAvaliacaoGestor.Value, "") : ""
                };
                exportData.Add(exportItem);
            }

            return exportData;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "PrepareExportDataAsync" }
            });
            throw;
        }
    }

    public byte[] GenerateExcelFile(List<PerformanceExportModel> exportData)
    {
        try
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Performances");

            var headers = new string[]
            {
                "IdPerformance", "IdCargo", "Cargo", "Performance", "DescricaoAbaixo", "DescricaoEsperado",
                "DescricaoAcima", "ATV", "Abrangencia", "InputAutoAvaliacao", "IdNotaPadraoAutoAvaliacao",
                "NotaPadraoAutoAvaliacao", "InputAvaliacaoAsCegas", "IdNotaPadraoAvaliacaoAsCegas",
                "NotaPadraoAvaliacaoAsCegas", "InputAvaliacaoGestor", "IdNotaPadraoAvaliacaoGestor",
                "NotaPadraoAvaliacaoGestor"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

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
            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GenerateExcelFile" }
            });
            throw;
        }
    }

    public DataTable ReadExcelFile(Stream fileStream)
    {
        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
                throw new InvalidOperationException("Planilha não encontrada no arquivo");

            var dataTable = new DataTable();
            bool hasHeader = true;

            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
            {
                dataTable.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
            }

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

            return dataTable;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ReadExcelFile" }
            });
            throw;
        }
    }

    public async Task<PerformanceImportResult> ProcessImportDataAsync(DataTable importData, int userId)
    {
        var result = new PerformanceImportResult();

        try
        {
            foreach (DataRow row in importData.Rows)
            {
                try
                {
                    var performance = await MapRowToPerformanceAsync(row, userId);
                    if (performance == null)
                    {
                        result.LinhasDesconsideradas++;
                        continue;
                    }

                    var validationResult = await _validationUtil.ValidatePerformanceAsync(performance);
                    if (!validationResult.IsValid)
                    {
                        result.Erros.AddRange(validationResult.Errors);
                        result.LinhasDesconsideradas++;
                        continue;
                    }

                    if (performance.IdPerformance == 0)
                    {
                        _context.Set<Models.Performance>().Add(performance);
                        result.LinhasInseridas++;
                    }
                    else
                    {
                        var existing = await _context.Set<Models.Performance>()
                            .FirstOrDefaultAsync(p => p.IdPerformance == performance.IdPerformance);
                        
                        if (existing != null)
                        {
                            UpdateExistingPerformance(existing, performance);
                            result.LinhasAlteradas++;
                        }
                        else
                        {
                            result.LinhasDesconsideradas++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Erros.Add($"Erro na linha: {ex.Message}");
                    result.LinhasDesconsideradas++;
                }
            }

            if (result.Sucesso)
            {
                await _context.SaveChangesAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ProcessImportDataAsync" },
                { "UserId", userId.ToString() }
            });
            result.Erros.Add($"Erro geral na importação: {ex.Message}");
            return result;
        }
    }

    private async Task<Models.Performance?> MapRowToPerformanceAsync(DataRow row, int userId)
    {
        try
        {
            int idPerformance = !string.IsNullOrWhiteSpace(row[0]?.ToString()) ? Convert.ToInt32(row[0]) : 0;
            int idCargo = !string.IsNullOrWhiteSpace(row[1]?.ToString()) ? Convert.ToInt32(row[1]) : 0;
            string performance = !string.IsNullOrWhiteSpace(row[3]?.ToString()) ? row[3].ToString()! : "-";
            string descricaoAbaixo = !string.IsNullOrWhiteSpace(row[4]?.ToString()) ? row[4].ToString()! : "-";
            string descricaoEsperado = !string.IsNullOrWhiteSpace(row[5]?.ToString()) ? row[5].ToString()! : "-";
            string descricaoAcima = !string.IsNullOrWhiteSpace(row[6]?.ToString()) ? row[6].ToString()! : "-";
            int atv = !string.IsNullOrWhiteSpace(row[7]?.ToString()) ? Convert.ToInt32(row[7]) : 1;
            string abrangencia = !string.IsNullOrWhiteSpace(row[8]?.ToString()) ? row[8].ToString()! : "-";
            int inputAutoAvaliacao = !string.IsNullOrWhiteSpace(row[9]?.ToString()) ? Convert.ToInt32(row[9]) : 1;
            int? idNotaPadraoAutoAvaliacao = !string.IsNullOrWhiteSpace(row[10]?.ToString()) ? (int?)Convert.ToInt32(row[10]) : null;
            int inputAvaliacaoAsCegas = !string.IsNullOrWhiteSpace(row[12]?.ToString()) ? Convert.ToInt32(row[12]) : 1;
            int? idNotaAvaliacaoAsCegas = !string.IsNullOrWhiteSpace(row[13]?.ToString()) ? (int?)Convert.ToInt32(row[13]) : null;
            int inputAvaliacaoGestor = !string.IsNullOrWhiteSpace(row[15]?.ToString()) ? Convert.ToInt32(row[15]) : 1;
            int? idNotaPadraoAvaliacaoGestor = !string.IsNullOrWhiteSpace(row[16]?.ToString()) ? (int?)Convert.ToInt32(row[16]) : null;

            if (performance == "-" || idCargo <= 0 || descricaoAbaixo == "-" || descricaoEsperado == "-" || descricaoAcima == "-" || abrangencia == "-")
            {
                return null;
            }

            return new Models.Performance
            {
                IdPerformance = idPerformance,
                IdEmpresa = 1,
                IdCargo = idCargo,
                IdNivel = 1,
                Nome = performance,
                PerformanceAbaixo = descricaoAbaixo,
                PerformanceEsperado = descricaoEsperado,
                PerformanceAcima = descricaoAcima,
                Abrangencia = abrangencia,
                InputAutoavaliacao = inputAutoAvaliacao == 1,
                NotaPadraoAutoAvaliacao = idNotaPadraoAutoAvaliacao,
                InputAvaliacaoAsCegas = inputAvaliacaoAsCegas == 1,
                NotaPadraoAvaliacaoAsCegas = idNotaAvaliacaoAsCegas,
                InputAvaliacaoGestor = inputAvaliacaoGestor == 1,
                NotaPadraoAvaliacaoGestor = idNotaPadraoAvaliacaoGestor,
                ATV = atv,
                USR = userId,
                DHC = DateTime.Now
            };
        }
        catch
        {
            return null;
        }
    }

    private void UpdateExistingPerformance(Models.Performance existing, Models.Performance updated)
    {
        existing.IdCargo = updated.IdCargo;
        existing.Nome = updated.Nome;
        existing.PerformanceAbaixo = updated.PerformanceAbaixo;
        existing.PerformanceEsperado = updated.PerformanceEsperado;
        existing.PerformanceAcima = updated.PerformanceAcima;
        existing.Abrangencia = updated.Abrangencia;
        existing.InputAutoavaliacao = updated.InputAutoavaliacao;
        existing.NotaPadraoAutoAvaliacao = updated.NotaPadraoAutoAvaliacao;
        existing.InputAvaliacaoAsCegas = updated.InputAvaliacaoAsCegas;
        existing.NotaPadraoAvaliacaoAsCegas = updated.NotaPadraoAvaliacaoAsCegas;
        existing.InputAvaliacaoGestor = updated.InputAvaliacaoGestor;
        existing.NotaPadraoAvaliacaoGestor = updated.NotaPadraoAvaliacaoGestor;
        existing.ATV = updated.ATV;
        existing.USR = updated.USR;
        existing.DHC = DateTime.Now;
    }
}