using OfficeOpenXml;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Dimensoes.Common;

namespace Peers.Moderno.Services.Dimensoes;

public class DimensoesExportService : IDimensoesExportService
{
    private readonly IDimensoesService _dimensoesService;
    private readonly ITelemetryService _telemetryService;

    public DimensoesExportService(IDimensoesService dimensoesService, ITelemetryService telemetryService)
    {
        _dimensoesService = dimensoesService;
        _telemetryService = telemetryService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<byte[]> ExportarDimensoesAsync()
    {
        try
        {
            var dimensoes = await _dimensoesService.ListarAsync();
            return await ExportarDimensoesAsync(dimensoes);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "ExportarDimensoes" }
            });
            throw;
        }
    }

    public async Task<byte[]> ExportarDimensoesAsync(List<Dimensao> dimensoes)
    {
        try
        {
            var dimensoesDto = dimensoes.Select(d => new DimensaoExportDto
            {
                IdDimensao = d.IdDimensao,
                Dimensao = d.Nome,
                TipoAvaliacao = d.TipoAvaliacao ?? "desempenho",
                Status = d.Ativo ? 1 : 0
            }).ToList();
            
            return await ExportarDimensoesAsync(dimensoesDto);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "ExportarDimensoesComLista" },
                { "TotalRegistros", dimensoes.Count.ToString() }
            });
            throw;
        }
    }

    public async Task<byte[]> ExportarDimensoesAsync(List<DimensaoExportDto> dimensoesDto)
    {
        try
        {
            var startTime = DateTimeOffset.UtcNow;
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Dimensões");
            
            // Cabeçalhos
            worksheet.Cells[1, 1].Value = "Código";
            worksheet.Cells[1, 2].Value = "Dimensão";
            worksheet.Cells[1, 3].Value = "Tipo Avaliação";
            worksheet.Cells[1, 4].Value = "Status";
            
            // Formatação do cabeçalho
            using (var range = worksheet.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }
            
            // Dados
            for (int i = 0; i < dimensoesDto.Count; i++)
            {
                var row = i + 2;
                var dimensao = dimensoesDto[i];
                
                worksheet.Cells[row, 1].Value = dimensao.IdDimensao;
                worksheet.Cells[row, 2].Value = dimensao.Dimensao;
                worksheet.Cells[row, 3].Value = dimensao.TipoAvaliacao;
                worksheet.Cells[row, 4].Value = dimensao.Status == 1 ? "Ativo" : "Inativo";
            }
            
            // Auto-fit colunas
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            
            var result = await package.GetAsByteArrayAsync();
            
            var duration = DateTimeOffset.UtcNow - startTime;
            _telemetryService.TrackDependency("Excel", "ExportarDimensoes", "EXPORT", startTime, duration, true);
            
            _telemetryService.TrackEvent("Dimensoes_Exportadas", new Dictionary<string, string>
            {
                { "TotalRegistros", dimensoesDto.Count.ToString() },
                { "TamanhoArquivo", result.Length.ToString() }
            });
            
            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "ExportarDimensoesDto" },
                { "TotalRegistros", dimensoesDto.Count.ToString() }
            });
            throw;
        }
    }
}