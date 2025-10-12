using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using SistemaAvaliacao.Services;
using SistemaAvaliacao.Models;

namespace SistemaAvaliacao.Services
{
    public class ExcelExportService : IExportService
    {
        public byte[] ExportCargosToExcel(IEnumerable<CargoViewModel> cargos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Cargos");
                // Cabeçalhos
                worksheet.Cells[1, 1].Value = "Código";
                worksheet.Cells[1, 2].Value = "Cargo";
                worksheet.Cells[1, 3].Value = "Próximo Cargo";
                worksheet.Cells[1, 4].Value = "Status";
                worksheet.Cells[1, 5].Value = "Tempo Mínimo (meses)";
                worksheet.Cells[1, 6].Value = "Função";
                worksheet.Cells[1, 7].Value = "Autonomia";
                worksheet.Cells[1, 8].Value = "Escopo de Atuação";
                worksheet.Cells[1, 9].Value = "Nível de Interlocução";
                int row = 2;
                foreach (var cargo in cargos)
                {
                    worksheet.Cells[row, 1].Value = cargo.IdCargo;
                    worksheet.Cells[row, 2].Value = cargo.Cargo;
                    worksheet.Cells[row, 3].Value = cargo.ProximoCargo;
                    worksheet.Cells[row, 4].Value = cargo.Status ? "Ativo" : "Inativo";
                    worksheet.Cells[row, 5].Value = cargo.TempoMinimo;
                    worksheet.Cells[row, 6].Value = cargo.Funcao;
                    worksheet.Cells[row, 7].Value = cargo.Autonomia;
                    worksheet.Cells[row, 8].Value = cargo.EscopoAtuacao;
                    worksheet.Cells[row, 9].Value = cargo.NivelInterlocucao;
                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
    }
}
