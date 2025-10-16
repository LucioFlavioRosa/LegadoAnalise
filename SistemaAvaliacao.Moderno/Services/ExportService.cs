using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services
{
    public class ExportService
    {
        public Task<byte[]> ExportarCargosAsync(List<Cargo> cargos)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Cargos");

            worksheet.Cell(1, 1).Value = "Código";
            worksheet.Cell(1, 2).Value = "Cargo";
            worksheet.Cell(1, 3).Value = "Próximo Cargo";
            worksheet.Cell(1, 4).Value = "Tempo Mínimo Promoção (meses)";
            worksheet.Cell(1, 5).Value = "Função";
            worksheet.Cell(1, 6).Value = "Autonomia";
            worksheet.Cell(1, 7).Value = "Escopo de Atuação";
            worksheet.Cell(1, 8).Value = "Nível de Interlocução";
            worksheet.Cell(1, 9).Value = "Status";

            int row = 2;
            foreach (var cargo in cargos)
            {
                worksheet.Cell(row, 1).Value = cargo.IdCargo;
                worksheet.Cell(row, 2).Value = cargo.NomeCargo;
                worksheet.Cell(row, 3).Value = cargo.ProximoCargoId.HasValue ? cargo.ProximoCargoId.Value.ToString() : string.Empty;
                worksheet.Cell(row, 4).Value = cargo.TempoMinimoPromocao;
                worksheet.Cell(row, 5).Value = cargo.Funcao;
                worksheet.Cell(row, 6).Value = cargo.Autonomia;
                worksheet.Cell(row, 7).Value = cargo.EscopoAtuacao;
                worksheet.Cell(row, 8).Value = cargo.NivelInterlocucao;
                worksheet.Cell(row, 9).Value = cargo.Ativo ? "Ativo" : "Inativo";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return Task.FromResult(stream.ToArray());
        }
    }
}
