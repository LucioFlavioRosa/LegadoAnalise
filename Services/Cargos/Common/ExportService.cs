using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Cargos.Common;

public interface IExportService
{
    Task<(byte[] bytes, string fileName)> ExportarCargosAsync();
}

public class ExportService : IExportService
{
    private readonly ApplicationDbContext _context;

    public ExportService(ApplicationDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<(byte[] bytes, string fileName)> ExportarCargosAsync()
    {
        var cargos = await _context.Cargos
            .Include(c => c.ProximoCargo)
            .Select(c => new CargoExportModel
            {
                IdCargo = c.IdCargo,
                Cargo = c.Nome,
                IdProximoCargo = c.IdProximoCargo,
                ProximoCargo = c.ProximoCargo != null ? c.ProximoCargo.Nome : string.Empty,
                TempoMinimoPromocao = c.TempoMinimoPromocao,
                Funcao = c.Funcao,
                Autonomia = c.Autonomia,
                EscopoDeAtuacao = c.EscopoDeAtuacao,
                NivelInterlocucao = c.NivelInterlocucao,
                ATV = c.ATV
            })
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Cargos");

        // Cabeçalhos
        worksheet.Cells[1, 1].Value = "IdCargo";
        worksheet.Cells[1, 2].Value = "Cargo";
        worksheet.Cells[1, 3].Value = "IdProximoCargo";
        worksheet.Cells[1, 4].Value = "ProximoCargo";
        worksheet.Cells[1, 5].Value = "TempoMinimoPromocao";
        worksheet.Cells[1, 6].Value = "Funcao";
        worksheet.Cells[1, 7].Value = "Autonomia";
        worksheet.Cells[1, 8].Value = "EscopoDeAtuacao";
        worksheet.Cells[1, 9].Value = "NivelInterlocucao";
        worksheet.Cells[1, 10].Value = "Status";

        // Dados
        for (int i = 0; i < cargos.Count; i++)
        {
            var cargo = cargos[i];
            var row = i + 2;

            worksheet.Cells[row, 1].Value = cargo.IdCargo;
            worksheet.Cells[row, 2].Value = cargo.Cargo;
            worksheet.Cells[row, 3].Value = cargo.IdProximoCargo;
            worksheet.Cells[row, 4].Value = cargo.ProximoCargo;
            worksheet.Cells[row, 5].Value = cargo.TempoMinimoPromocao;
            worksheet.Cells[row, 6].Value = cargo.Funcao;
            worksheet.Cells[row, 7].Value = cargo.Autonomia;
            worksheet.Cells[row, 8].Value = cargo.EscopoDeAtuacao;
            worksheet.Cells[row, 9].Value = cargo.NivelInterlocucao;
            worksheet.Cells[row, 10].Value = cargo.ATV == 1 ? "Ativo" : "Inativo";
        }

        worksheet.Cells.AutoFitColumns();
        
        var fileName = $"Cargos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return (package.GetAsByteArray(), fileName);
    }
}