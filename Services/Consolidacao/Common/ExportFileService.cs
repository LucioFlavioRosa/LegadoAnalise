using OfficeOpenXml;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Consolidacao.Common;

public interface IExportFileService
{
    Task<byte[]> GenerateExcelConsideracoesMentorAsync(string fileName, List<ConsideracoesMentorModel> data);
    Task<byte[]> GenerateExcelPerformanceNotasAsync(string fileName, List<PerformanceNotasModelExport> data);
}

public class ExportFileService : IExportFileService
{
    private readonly ITelemetryService _telemetryService;

    public ExportFileService(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<byte[]> GenerateExcelConsideracoesMentorAsync(string fileName, List<ConsideracoesMentorModel> data)
    {
        try
        {
            _telemetryService.TrackEvent("ExportFileService.GenerateExcelConsideracoesMentor", new Dictionary<string, string>
            {
                { "FileName", fileName },
                { "RecordCount", data.Count.ToString() }
            });

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Considerações Mentor");

            // Headers
            var headers = new string[]
            {
                "ID", "ID Mentor", "Mentor", "ID Associado", "Associado", "ID Período", "Período",
                "Tipo Avaliação", "Escopo", "Elegível Promoção", "Input Promoção", "Trajetória Associado",
                "Pontos Fortes", "Pontos Fracos", "Mentor Pode Ver", "Ação Comitê", "Pontos Fortes RH",
                "Pontos Fracos RH", "Salário Atual", "Salário Novo", "Regime Atual", "Regime Novo", "Mentoria Realizada"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // Data
            for (int row = 0; row < data.Count; row++)
            {
                var item = data[row];
                worksheet.Cells[row + 2, 1].Value = item.idConsideracoesMentor;
                worksheet.Cells[row + 2, 2].Value = item.idMentor;
                worksheet.Cells[row + 2, 3].Value = item.Mentor;
                worksheet.Cells[row + 2, 4].Value = item.idAssociado;
                worksheet.Cells[row + 2, 5].Value = item.Associado;
                worksheet.Cells[row + 2, 6].Value = item.idPeriodo;
                worksheet.Cells[row + 2, 7].Value = item.Periodo;
                worksheet.Cells[row + 2, 8].Value = item.TipoAvaliacao;
                worksheet.Cells[row + 2, 9].Value = item.Escopo;
                worksheet.Cells[row + 2, 10].Value = item.ElegivelPromocao;
                worksheet.Cells[row + 2, 11].Value = item.InputPromocao;
                worksheet.Cells[row + 2, 12].Value = item.TrajetoriaAssociado;
                worksheet.Cells[row + 2, 13].Value = item.PontosFortes;
                worksheet.Cells[row + 2, 14].Value = item.PontosFracos;
                worksheet.Cells[row + 2, 15].Value = item.MentorPodeVer;
                worksheet.Cells[row + 2, 16].Value = item.AcaoComite;
                worksheet.Cells[row + 2, 17].Value = item.PontosFortesRH;
                worksheet.Cells[row + 2, 18].Value = item.PontosFracosRH;
                worksheet.Cells[row + 2, 19].Value = item.SalarioAtual;
                worksheet.Cells[row + 2, 20].Value = item.SalarioNovo;
                worksheet.Cells[row + 2, 21].Value = item.RegimeContratacaoAtual;
                worksheet.Cells[row + 2, 22].Value = item.RegimeContratacaoNovo;
                worksheet.Cells[row + 2, 23].Value = item.MentoriaRealizada;
            }

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GenerateExcelConsideracoesMentorAsync" },
                { "FileName", fileName }
            });
            throw;
        }
    }

    public async Task<byte[]> GenerateExcelPerformanceNotasAsync(string fileName, List<PerformanceNotasModelExport> data)
    {
        try
        {
            _telemetryService.TrackEvent("ExportFileService.GenerateExcelPerformanceNotas", new Dictionary<string, string>
            {
                { "FileName", fileName },
                { "RecordCount", data.Count.ToString() }
            });

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Notas Performance");

            // Headers
            var headers = new string[]
            {
                "ID Nota Performance", "ID Associado", "Associado", "ID Cargo", "Cargo", "ID Projeto", "Projeto",
                "ID Período", "Período", "ID Performance", "Performance", "ID Nota Auto Avaliação", "Nota Auto Avaliação",
                "Comentários Auto Avaliação", "ID Nota Avaliação Às Cegas", "Nota Avaliação Às Cegas", "Comentários Avaliação Às Cegas",
                "ID Nota Avaliação Gestor", "Nota Avaliação Gestor", "Comentários Avaliação Gestor", "ID Nota Feedback",
                "Nota Feedback", "Comentários Feedback", "ID Nota Comitê", "Nota Comitê", "Nota Final"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // Data
            for (int row = 0; row < data.Count; row++)
            {
                var item = data[row];
                worksheet.Cells[row + 2, 1].Value = item.IdNotaPerformance;
                worksheet.Cells[row + 2, 2].Value = item.IdAssociado;
                worksheet.Cells[row + 2, 3].Value = item.Associado;
                worksheet.Cells[row + 2, 4].Value = item.IdCargo;
                worksheet.Cells[row + 2, 5].Value = item.Cargo;
                worksheet.Cells[row + 2, 6].Value = item.IdProjeto;
                worksheet.Cells[row + 2, 7].Value = item.Projeto;
                worksheet.Cells[row + 2, 8].Value = item.IdPeriodo;
                worksheet.Cells[row + 2, 9].Value = item.Periodo;
                worksheet.Cells[row + 2, 10].Value = item.IdPerformance;
                worksheet.Cells[row + 2, 11].Value = item.Performance;
                worksheet.Cells[row + 2, 12].Value = item.IdNotaAutoAvaliacao;
                worksheet.Cells[row + 2, 13].Value = item.NotaAutoAvaliacao;
                worksheet.Cells[row + 2, 14].Value = item.ComentariosAutoAvaliacao;
                worksheet.Cells[row + 2, 15].Value = item.IdNotaAvaliacaoAsCegas;
                worksheet.Cells[row + 2, 16].Value = item.NotaAvaliacaoAsCegas;
                worksheet.Cells[row + 2, 17].Value = item.ComentariosAvaliacaoAsCegas;
                worksheet.Cells[row + 2, 18].Value = item.IdNotaAvaliacaoGestor;
                worksheet.Cells[row + 2, 19].Value = item.NotaAvaliacaoGestor;
                worksheet.Cells[row + 2, 20].Value = item.ComentariosAvaliacaoGestor;
                worksheet.Cells[row + 2, 21].Value = item.IdNotaFeedback;
                worksheet.Cells[row + 2, 22].Value = item.NotaFeedback;
                worksheet.Cells[row + 2, 23].Value = item.ComentariosFeedback;
                worksheet.Cells[row + 2, 24].Value = item.IdNotaComite;
                worksheet.Cells[row + 2, 25].Value = item.NotaComite;
                worksheet.Cells[row + 2, 26].Value = item.NotaFinal;
            }

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GenerateExcelPerformanceNotasAsync" },
                { "FileName", fileName }
            });
            throw;
        }
    }
}