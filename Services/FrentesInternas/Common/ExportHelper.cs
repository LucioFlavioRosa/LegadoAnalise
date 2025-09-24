using OfficeOpenXml;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.FrentesInternas.Common;

public class ExportHelper : IExportHelper
{
    private readonly ITelemetryService _telemetryService;
    private const int MAX_EXPORT_RECORDS = 50000;

    public ExportHelper(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<byte[]> ExportarAvaliacoesAlocacaoAsync(List<AlocacaoExportModel> avaliacoes, string fileName)
    {
        try
        {
            if (!await ValidarDadosExportacaoAsync(avaliacoes))
                throw new InvalidOperationException("Dados de exportação inválidos");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Avaliações Alocação Interna");

            var headers = new string[]
            {
                "ID Avaliação",
                "ID Alocação Interna",
                "Alocação Interna",
                "ID Período",
                "Período",
                "ID Líder Alocação",
                "Líder Alocação",
                "ID Avaliado",
                "Avaliado",
                "ID Nota",
                "Nota",
                "Comentários",
                "Data/Hora Nota",
                "Validado MD",
                "Data/Hora Validado MD"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            for (int i = 0; i < avaliacoes.Count; i++)
            {
                var avaliacao = avaliacoes[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = avaliacao.IdAvaliacaoAlocacao;
                worksheet.Cells[row, 2].Value = avaliacao.IdAlocacaoInterna;
                worksheet.Cells[row, 3].Value = avaliacao.AlocacaoInterna;
                worksheet.Cells[row, 4].Value = avaliacao.IdPeriodo;
                worksheet.Cells[row, 5].Value = avaliacao.Periodo;
                worksheet.Cells[row, 6].Value = avaliacao.IdLiderAlocacao;
                worksheet.Cells[row, 7].Value = avaliacao.LiderAlocacao;
                worksheet.Cells[row, 8].Value = avaliacao.IdAvaliado;
                worksheet.Cells[row, 9].Value = avaliacao.Avaliado;
                worksheet.Cells[row, 10].Value = avaliacao.IdNota;
                worksheet.Cells[row, 11].Value = avaliacao.Nota;
                worksheet.Cells[row, 12].Value = avaliacao.Comentarios;
                worksheet.Cells[row, 13].Value = avaliacao.DHCNota;
                worksheet.Cells[row, 14].Value = avaliacao.ValidadoMD;
                worksheet.Cells[row, 15].Value = avaliacao.DHCValidadoMD;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            _telemetryService.TrackEvent("FrentesInternas_ExportarAvaliacoes", new Dictionary<string, string>
            {
                { "TotalRegistros", avaliacoes.Count.ToString() },
                { "NomeArquivo", fileName }
            });

            return await Task.FromResult(package.GetAsByteArray());
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<byte[]> ExportarFrentesInternasAsync(List<FrenteInternaModel> frentes, string fileName)
    {
        try
        {
            if (!await ValidarDadosExportacaoAsync(frentes))
                throw new InvalidOperationException("Dados de exportação inválidos");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Frentes Internas");

            var headers = new string[]
            {
                "ID",
                "Frente Interna",
                "Total Líderes",
                "Líderes",
                "Status",
                "Data Criação",
                "Usuário"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            for (int i = 0; i < frentes.Count; i++)
            {
                var frente = frentes[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = frente.IdFrenteInterna;
                worksheet.Cells[row, 2].Value = frente.FrenteInterna;
                worksheet.Cells[row, 3].Value = frente.TotalLideres;
                worksheet.Cells[row, 4].Value = frente.Lideres.Replace("<br/>", "; ");
                worksheet.Cells[row, 5].Value = frente.Ativo ? "Ativo" : "Inativo";
                worksheet.Cells[row, 6].Value = frente.DHC.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 7].Value = frente.USR;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            _telemetryService.TrackEvent("FrentesInternas_ExportarFrentes", new Dictionary<string, string>
            {
                { "TotalRegistros", frentes.Count.ToString() },
                { "NomeArquivo", fileName }
            });

            return await Task.FromResult(package.GetAsByteArray());
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public string GerarNomeArquivo(string prefixo, string extensao = ".xlsx")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return $"{prefixo}_{timestamp}{extensao}";
    }

    public async Task<bool> ValidarDadosExportacaoAsync<T>(List<T> dados) where T : class
    {
        await Task.CompletedTask;
        
        if (dados == null || !dados.Any())
            return false;

        if (dados.Count > MAX_EXPORT_RECORDS)
        {
            throw new InvalidOperationException($"Número de registros excede o limite máximo de {MAX_EXPORT_RECORDS}");
        }

        return true;
    }
}