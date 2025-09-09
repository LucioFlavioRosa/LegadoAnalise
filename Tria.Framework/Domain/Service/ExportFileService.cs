using CsvHelper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using TriaSoftware.Util.Framework.Domain.Interface;

namespace TriaSoftware.Util.Framework.Domain.Service
{
    public class ExportFileService : IExportFile
    {
        public byte[] GenerateCSV<T>(string delimiter, List<T> lstGeneric)
        {
            try
            {
                using (var mem = new MemoryStream())
                using (var writer = new StreamWriter(mem))
                using (var csvWriter = new CsvWriter(writer))
                {
                    csvWriter.Configuration.Delimiter = delimiter;
                    csvWriter.Configuration.HasHeaderRecord = true;
                    var type = lstGeneric[0].GetType();
                    csvWriter.Configuration.AutoMap(type);

                    csvWriter.WriteHeader(type);
                    csvWriter.WriteRecords(lstGeneric);

                    writer.Flush();
                    return mem.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GenerateExcel<T>(List<T> lstGeneric)
        {
            FileInfo targetFile = new FileInfo("Tria_");

            try
            {
                byte[] result;
                using (var excelFile = new ExcelPackage(targetFile))
                {
                    var worksheet = excelFile.Workbook.Worksheets.Add("Planilha");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstGeneric, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 100])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i < 100; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    result = excelFile.GetAsByteArray();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GenerateExcel<T, U>(string fileName, List<T> lstCompetencia, List<U> lstPerformance)
        {
            FileInfo targetFile = new FileInfo(fileName);

            try
            {
                byte[] result;
                using (var excelFile = new ExcelPackage(targetFile))
                {
                    var worksheet = excelFile.Workbook.Worksheets.Add("Consolidacao Competencias");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstCompetencia, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheet.Cells[2, 1, 1000, 20])
                    {
                        cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cells.Style.WrapText = false;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    worksheet = excelFile.Workbook.Worksheets.Add("Consolidacao Performance");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstPerformance, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    result = excelFile.GetAsByteArray();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GenerateExcel<T, U, W>(string fileName, List<T> lstCompetencia, List<U> lstPerformance, List<W> lstLideranca)
        {
            FileInfo targetFile = new FileInfo(fileName);

            try
            {
                byte[] result;
                using (var excelFile = new ExcelPackage(targetFile))
                {
                    var worksheet = excelFile.Workbook.Worksheets.Add("Consolidacao Competencias Desempenho");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstCompetencia, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheet.Cells[2, 1, 1000, 20])
                    {
                        cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cells.Style.WrapText = false;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    worksheet = excelFile.Workbook.Worksheets.Add("Consolidacao Performance Desempenho");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstPerformance, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    worksheet = excelFile.Workbook.Worksheets.Add("Consolidacao Liderança");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstLideranca, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    result = excelFile.GetAsByteArray();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GenerateExcel_ExportResultadoDesempenho<Z, X, Y, T, U, W>(string fileName, List<Z> lstTotal, List<X> lstTotalCompetencias, List<Y> lstTotalPerformances,
            List<T> lstProjetos, List<U> lstCompetencias, List<W> lstPerformances)
        {
            FileInfo targetFile = new FileInfo(fileName);

            try
            {
                byte[] result;
                using (var excelFile = new ExcelPackage(targetFile))
                {
                    //
                    var worksheet = excelFile.Workbook.Worksheets.Add("Resultado Total");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstTotal, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheet.Cells[2, 1, 1000, 20])
                    {
                        cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cells.Style.WrapText = false;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    //
                    worksheet = excelFile.Workbook.Worksheets.Add("Resultado Total Competencias");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstTotalCompetencias, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheet.Cells[2, 1, 1000, 20])
                    {
                        cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cells.Style.WrapText = false;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    //
                    worksheet = excelFile.Workbook.Worksheets.Add("Resultado Total Performances");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstTotalPerformances, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheet.Cells[2, 1, 1000, 20])
                    {
                        cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cells.Style.WrapText = false;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    //
                    worksheet = excelFile.Workbook.Worksheets.Add("Resultado Projetos");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstProjetos, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheet.Cells[2, 1, 1000, 20])
                    {
                        cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cells.Style.WrapText = false;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    //
                    worksheet = excelFile.Workbook.Worksheets.Add("Resultado Projetos Competencias");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstCompetencias, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    worksheet = excelFile.Workbook.Worksheets.Add("Resultado Projetos Performances");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstPerformances, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    result = excelFile.GetAsByteArray();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] GenerateExcelConsideracoesMentor<T>(string fileName, List<T> lstConsideracoesMentor)
        {
            FileInfo targetFile = new FileInfo(fileName);

            try
            {
                byte[] result;
                using (var excelFile = new ExcelPackage(targetFile))
                {
                    var worksheet = excelFile.Workbook.Worksheets.Add("Dados");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstConsideracoesMentor, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheet.Cells[2, 1, 1000, 20])
                    {
                        cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cells.Style.WrapText = false;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    result = excelFile.GetAsByteArray();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GenerateExcel_ResultadoLideranca<T, U, V, C>(string fileName, List<T> lstPeriodos, List<U> lstProjetos, List<V> lstPilares, List<C> lstSubcompetencias)
        {
            FileInfo targetFile = new FileInfo(fileName);

            try
            {
                byte[] result;
                using (var excelFile = new ExcelPackage(targetFile))
                {
                    // PERIODOS
                    var worksheet = excelFile.Workbook.Worksheets.Add("Períodos");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstPeriodos, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheet.Cells[2, 1, 1000, 20])
                    {
                        cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        cells.Style.WrapText = false;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    // PROJETOS
                    worksheet = excelFile.Workbook.Worksheets.Add("Projetos");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstProjetos, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    // PILARES
                    worksheet = excelFile.Workbook.Worksheets.Add("Pilares");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstPilares, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    // SUBCOMPETENCIAS
                    worksheet = excelFile.Workbook.Worksheets.Add("Subcompetencias");
                    worksheet.Cells["A1"].LoadFromCollection(Collection: lstSubcompetencias, PrintHeaders: true);

                    using (var cells = worksheet.Cells[1, 1, 1, 20])
                    {
                        cells.Style.Font.Bold = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    result = excelFile.GetAsByteArray();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
