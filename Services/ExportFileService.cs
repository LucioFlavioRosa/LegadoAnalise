using OfficeOpenXml;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class ExportFileService : IExportFileService
{
    public byte[] GenerateExcelCompetencias(string fileName, List<CompetenciaExportModel> competencias)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Competências");
        
        // Cabeçalhos
        worksheet.Cells[1, 1].Value = "IdCompetencia";
        worksheet.Cells[1, 2].Value = "IdCargo";
        worksheet.Cells[1, 3].Value = "Cargo";
        worksheet.Cells[1, 4].Value = "IdEixo";
        worksheet.Cells[1, 5].Value = "Eixo";
        worksheet.Cells[1, 6].Value = "IdSubCompetencia";
        worksheet.Cells[1, 7].Value = "SubCompetencia";
        worksheet.Cells[1, 8].Value = "IdDimensao";
        worksheet.Cells[1, 9].Value = "Dimensao";
        worksheet.Cells[1, 10].Value = "DetalheNivelAtual";
        worksheet.Cells[1, 11].Value = "CompetenciaAtual";
        worksheet.Cells[1, 12].Value = "PalavrasChave";
        worksheet.Cells[1, 13].Value = "TipoAvaliacao";
        worksheet.Cells[1, 14].Value = "Escopo";
        worksheet.Cells[1, 15].Value = "ATV";
        
        // Dados
        for (int i = 0; i < competencias.Count; i++)
        {
            var comp = competencias[i];
            int row = i + 2;
            
            worksheet.Cells[row, 1].Value = comp.IdCompetencia;
            worksheet.Cells[row, 2].Value = comp.IdCargo;
            worksheet.Cells[row, 3].Value = comp.Cargo;
            worksheet.Cells[row, 4].Value = comp.IdEixo;
            worksheet.Cells[row, 5].Value = comp.Eixo;
            worksheet.Cells[row, 6].Value = comp.IdSubCompetencia;
            worksheet.Cells[row, 7].Value = comp.SubCompetencia;
            worksheet.Cells[row, 8].Value = comp.IdDimensao;
            worksheet.Cells[row, 9].Value = comp.Dimensao;
            worksheet.Cells[row, 10].Value = comp.DetalheNivelAtual;
            worksheet.Cells[row, 11].Value = comp.CompetenciaAtual;
            worksheet.Cells[row, 12].Value = comp.PalavrasChave;
            worksheet.Cells[row, 13].Value = comp.TipoAvaliacao;
            worksheet.Cells[row, 14].Value = comp.Escopo;
            worksheet.Cells[row, 15].Value = comp.ATV;
        }
        
        worksheet.Cells.AutoFitColumns();
        
        return package.GetAsByteArray();
    }
}