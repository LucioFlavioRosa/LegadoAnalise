using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Services.AvaliacoesExportacao.Common;

public interface IExportarAvaliacoesExcelService
{
    byte[] GerarExcel(List<ExportarAvaliacoesService.AvaliacaoResumoModel> avaliacoes);
}

public class ExportarAvaliacoesExcelService : IExportarAvaliacoesExcelService
{
    public byte[] GerarExcel(List<ExportarAvaliacoesService.AvaliacaoResumoModel> avaliacoes)
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Avaliacoes");

        // Cabeçalhos
        ws.Cells[1, 1].Value = "Período";
        ws.Cells[1, 2].Value = "Projeto";
        ws.Cells[1, 3].Value = "Avaliado";
        ws.Cells[1, 4].Value = "Cargo";
        ws.Cells[1, 5].Value = "Gestor";
        ws.Cells[1, 6].Value = "Tipo";
        ws.Cells[1, 7].Value = "Competência/Performance Avaliada";

        int row = 2;
        foreach (var av in avaliacoes)
        {
            ws.Cells[row, 1].Value = av.Periodo;
            ws.Cells[row, 2].Value = av.Projeto;
            ws.Cells[row, 3].Value = av.NomeAvaliado;
            ws.Cells[row, 4].Value = av.Cargo;
            ws.Cells[row, 5].Value = av.Gestor;
            ws.Cells[row, 6].Value = av.Tipo;
            ws.Cells[row, 7].Value = av.CompetenciaOuPerformance;
            row++;
        }
        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }
}
