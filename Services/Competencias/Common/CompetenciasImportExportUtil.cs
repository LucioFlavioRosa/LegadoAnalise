using OfficeOpenXml;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Competencias.Common.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Competencias.Common;

public class CompetenciasImportExportUtil
{
    private readonly ApplicationDbContext _context;

    public CompetenciasImportExportUtil(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportarParaExcelAsync(List<CompetenciaExportDto> competencias)
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
            var competencia = competencias[i];
            var row = i + 2;

            worksheet.Cells[row, 1].Value = competencia.IdCompetencia;
            worksheet.Cells[row, 2].Value = competencia.IdCargo;
            worksheet.Cells[row, 3].Value = competencia.Cargo;
            worksheet.Cells[row, 4].Value = competencia.IdEixo;
            worksheet.Cells[row, 5].Value = competencia.Eixo;
            worksheet.Cells[row, 6].Value = competencia.IdSubCompetencia;
            worksheet.Cells[row, 7].Value = competencia.SubCompetencia;
            worksheet.Cells[row, 8].Value = competencia.IdDimensao;
            worksheet.Cells[row, 9].Value = competencia.Dimensao;
            worksheet.Cells[row, 10].Value = competencia.DetalheNivelAtual;
            worksheet.Cells[row, 11].Value = competencia.CompetenciaAtual;
            worksheet.Cells[row, 12].Value = competencia.PalavrasChave;
            worksheet.Cells[row, 13].Value = competencia.TipoAvaliacao;
            worksheet.Cells[row, 14].Value = competencia.Escopo;
            worksheet.Cells[row, 15].Value = competencia.ATV;
        }

        // Auto-ajustar colunas
        worksheet.Cells.AutoFitColumns();

        return await Task.FromResult(package.GetAsByteArray());
    }

    public async Task<List<CompetenciaImportDto>> LerExcelAsync(Stream fileStream)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        var competencias = new List<CompetenciaImportDto>();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        
        if (worksheet == null)
            throw new InvalidOperationException("O arquivo Excel não possui nenhuma planilha.");

        int rowCount = worksheet.Dimension?.End.Row ?? 0;

        for (int row = 2; row <= rowCount; row++)
        {
            var competencia = new CompetenciaImportDto
            {
                IdCompetencia = worksheet.Cells[row, 1].GetValue<int?>() ?? 0,
                IdCargo = worksheet.Cells[row, 2].GetValue<int?>() ?? 0,
                IdEixo = worksheet.Cells[row, 4].GetValue<int?>() ?? 0,
                IdSubCompetencia = worksheet.Cells[row, 6].GetValue<int?>() ?? 0,
                IdDimensao = worksheet.Cells[row, 8].GetValue<int?>() ?? 0,
                DetalheNivelAtual = worksheet.Cells[row, 10].GetValue<string>() ?? "-",
                CompetenciaAtual = worksheet.Cells[row, 11].GetValue<string>() ?? "-",
                PalavrasChave = worksheet.Cells[row, 12].GetValue<string>() ?? "-",
                TipoAvaliacao = worksheet.Cells[row, 13].GetValue<string>() ?? "-",
                Escopo = worksheet.Cells[row, 14].GetValue<string>() ?? "-",
                ATV = worksheet.Cells[row, 15].GetValue<int?>() ?? 0
            };

            if (string.IsNullOrEmpty(competencia.DetalheNivelAtual))
                competencia.DetalheNivelAtual = "-";
            
            if (string.IsNullOrEmpty(competencia.CompetenciaAtual))
                competencia.CompetenciaAtual = "-";

            competencias.Add(competencia);
        }

        return await Task.FromResult(competencias);
    }

    public async Task<Competencia?> ObterCompetenciaExistenteAsync(int idEmpresa, int idCargo, int idNivel, int idEixo, int idSubCompetencia, int idDimensao, string detalhamento)
    {
        return await _context.Competencias
            .FirstOrDefaultAsync(c => 
                c.IdEmpresa == idEmpresa &&
                c.IdCargo == idCargo &&
                c.IdNivel == idNivel &&
                c.IdEixo == idEixo &&
                c.IdSubCompetencia == idSubCompetencia &&
                c.IdDimensao == idDimensao &&
                c.CompetenciaJRDetalhe == detalhamento);
    }
}