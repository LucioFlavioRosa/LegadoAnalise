using OfficeOpenXml;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Consolidacao.Common;

public interface IImportFileService
{
    Task<ImportResult<ConsideracoesMentorModel>> ImportConsideracoesMentorAsync(Stream fileStream);
    Task<ImportResult<PerformanceNotasModelExport>> ImportPerformanceNotasAsync(Stream fileStream);
}

public class ImportFileService : IImportFileService
{
    private readonly ITelemetryService _telemetryService;

    public ImportFileService(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<ImportResult<ConsideracoesMentorModel>> ImportConsideracoesMentorAsync(Stream fileStream)
    {
        var result = new ImportResult<ConsideracoesMentorModel>();
        
        try
        {
            _telemetryService.TrackEvent("ImportFileService.ImportConsideracoesMentor");

            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
            {
                result.AddError("O arquivo Excel não possui nenhuma planilha.");
                return result;
            }

            int rowCount = worksheet.Dimension?.End.Row ?? 0;
            
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var item = new ConsideracoesMentorModel
                    {
                        idConsideracoesMentor = worksheet.Cells[row, 1].GetValue<int?>() ?? 0,
                        idMentor = worksheet.Cells[row, 2].GetValue<int?>() ?? 0,
                        Mentor = worksheet.Cells[row, 3].GetValue<string>() ?? string.Empty,
                        idAssociado = worksheet.Cells[row, 4].GetValue<int?>() ?? 0,
                        Associado = worksheet.Cells[row, 5].GetValue<string>() ?? string.Empty,
                        idPeriodo = worksheet.Cells[row, 6].GetValue<int?>() ?? 0,
                        Periodo = worksheet.Cells[row, 7].GetValue<string>() ?? string.Empty,
                        TipoAvaliacao = worksheet.Cells[row, 8].GetValue<string>() ?? "desempenho",
                        Escopo = worksheet.Cells[row, 9].GetValue<string>() ?? "projeto",
                        ElegivelPromocao = worksheet.Cells[row, 10].GetValue<string>() ?? "Não",
                        InputPromocao = worksheet.Cells[row, 11].GetValue<string>() ?? "Não",
                        TrajetoriaAssociado = worksheet.Cells[row, 12].GetValue<string>() ?? string.Empty,
                        PontosFortes = worksheet.Cells[row, 13].GetValue<string>() ?? string.Empty,
                        PontosFracos = worksheet.Cells[row, 14].GetValue<string>() ?? string.Empty,
                        MentorPodeVer = worksheet.Cells[row, 15].GetValue<string>() ?? "Não",
                        AcaoComite = worksheet.Cells[row, 16].GetValue<string>() ?? string.Empty,
                        PontosFortesRH = worksheet.Cells[row, 17].GetValue<string>() ?? string.Empty,
                        PontosFracosRH = worksheet.Cells[row, 18].GetValue<string>() ?? string.Empty,
                        SalarioAtual = worksheet.Cells[row, 19].GetValue<double?>()?.ToString() ?? "0",
                        SalarioNovo = worksheet.Cells[row, 20].GetValue<double?>()?.ToString() ?? "0",
                        RegimeContratacaoAtual = worksheet.Cells[row, 21].GetValue<string>() ?? "CLT",
                        RegimeContratacaoNovo = worksheet.Cells[row, 22].GetValue<string>() ?? "CLT",
                        MentoriaRealizada = worksheet.Cells[row, 23].GetValue<string>() ?? "Não"
                    };

                    if (item.idMentor > 0 && item.idAssociado > 0 && item.idPeriodo > 0)
                    {
                        result.ValidItems.Add(item);
                    }
                    else
                    {
                        result.InvalidItems.Add(item);
                        result.AddWarning($"Linha {row}: Dados obrigatórios não preenchidos (Mentor, Associado, Período)");
                    }
                }
                catch (Exception ex)
                {
                    result.AddError($"Erro na linha {row}: {ex.Message}");
                }
            }

            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ImportConsideracoesMentorAsync" }
            });
            result.AddError($"Erro ao processar arquivo: {ex.Message}");
            return result;
        }
    }

    public async Task<ImportResult<PerformanceNotasModelExport>> ImportPerformanceNotasAsync(Stream fileStream)
    {
        var result = new ImportResult<PerformanceNotasModelExport>();
        
        try
        {
            _telemetryService.TrackEvent("ImportFileService.ImportPerformanceNotas");

            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
            {
                result.AddError("O arquivo Excel não possui nenhuma planilha.");
                return result;
            }

            int rowCount = worksheet.Dimension?.End.Row ?? 0;
            
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var item = new PerformanceNotasModelExport
                    {
                        IdNotaPerformance = worksheet.Cells[row, 1].GetValue<int?>() ?? 0,
                        IdAssociado = worksheet.Cells[row, 2].GetValue<int?>() ?? 0,
                        Associado = worksheet.Cells[row, 3].GetValue<string>() ?? string.Empty,
                        IdCargo = worksheet.Cells[row, 4].GetValue<int?>() ?? 0,
                        Cargo = worksheet.Cells[row, 5].GetValue<string>() ?? string.Empty,
                        IdProjeto = worksheet.Cells[row, 6].GetValue<int?>() ?? 0,
                        Projeto = worksheet.Cells[row, 7].GetValue<string>() ?? string.Empty,
                        IdPeriodo = worksheet.Cells[row, 8].GetValue<int?>() ?? 0,
                        Periodo = worksheet.Cells[row, 9].GetValue<string>() ?? string.Empty,
                        IdPerformance = worksheet.Cells[row, 10].GetValue<int?>() ?? 0,
                        Performance = worksheet.Cells[row, 11].GetValue<string>() ?? string.Empty,
                        IdNotaAutoAvaliacao = worksheet.Cells[row, 12].GetValue<int?>() ?? 0,
                        NotaAutoAvaliacao = worksheet.Cells[row, 13].GetValue<string>() ?? string.Empty,
                        ComentariosAutoAvaliacao = worksheet.Cells[row, 14].GetValue<string>() ?? "-",
                        IdNotaAvaliacaoAsCegas = worksheet.Cells[row, 15].GetValue<int?>() ?? 0,
                        NotaAvaliacaoAsCegas = worksheet.Cells[row, 16].GetValue<string>() ?? "-",
                        ComentariosAvaliacaoAsCegas = worksheet.Cells[row, 17].GetValue<string>() ?? "-",
                        IdNotaAvaliacaoGestor = worksheet.Cells[row, 18].GetValue<int?>() ?? 0,
                        NotaAvaliacaoGestor = worksheet.Cells[row, 19].GetValue<string>() ?? "-",
                        ComentariosAvaliacaoGestor = worksheet.Cells[row, 20].GetValue<string>() ?? "-",
                        IdNotaFeedback = worksheet.Cells[row, 21].GetValue<int?>() ?? 0,
                        NotaFeedback = worksheet.Cells[row, 22].GetValue<string>() ?? "-",
                        ComentariosFeedback = worksheet.Cells[row, 23].GetValue<string>() ?? "-",
                        IdNotaComite = worksheet.Cells[row, 24].GetValue<int?>() ?? 0,
                        NotaComite = worksheet.Cells[row, 25].GetValue<string>() ?? "-",
                        NotaFinal = worksheet.Cells[row, 26].GetValue<int?>() ?? 0
                    };

                    if (item.IdAssociado > 0 && item.IdProjeto > 0 && item.IdCargo > 0 && item.IdPeriodo > 0 && item.IdPerformance > 0)
                    {
                        result.ValidItems.Add(item);
                    }
                    else
                    {
                        result.InvalidItems.Add(item);
                        result.AddWarning($"Linha {row}: Dados obrigatórios não preenchidos");
                    }
                }
                catch (Exception ex)
                {
                    result.AddError($"Erro na linha {row}: {ex.Message}");
                }
            }

            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ImportPerformanceNotasAsync" }
            });
            result.AddError($"Erro ao processar arquivo: {ex.Message}");
            return result;
        }
    }
}

public class ImportResult<T>
{
    public List<T> ValidItems { get; set; } = new();
    public List<T> InvalidItems { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool HasErrors => Errors.Count > 0;
    public bool HasWarnings => Warnings.Count > 0;
    public int TotalProcessed => ValidItems.Count + InvalidItems.Count;

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);
}