using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Models;
using System.Globalization;
using System.Collections.Generic;

namespace Services
{
    public class ImportFileService : IImportFileService
    {
        private readonly IAssociadosService _associadosService;
        private readonly ICargosService _cargosService;
        public ImportFileService(IAssociadosService associadosService, ICargosService cargosService)
        {
            _associadosService = associadosService;
            _cargosService = cargosService;
        }

        public async Task<(int inseridos, int alterados, int desconsiderados)> ImportarAssociadosExcelAsync(Stream fileStream)
        {
            int inseridos = 0, alterados = 0, desconsiderados = 0;
            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return (0, 0, 0);
                int rowCount = worksheet.Dimension.End.Row;
                for (int row = 2; row <= rowCount; row++)
                {
                    int idAssociado = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                    string nome = worksheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                    int idCargo = worksheet.Cells[row, 3].GetValue<int?>() ?? 0;
                    int idEmpresa = worksheet.Cells[row, 5].GetValue<int?>() ?? 1;
                    int idPerfil = worksheet.Cells[row, 7].GetValue<int?>() ?? 0;
                    int idMentor = worksheet.Cells[row, 9].GetValue<int?>() ?? 0;
                    string email = worksheet.Cells[row, 11].GetValue<string>() ?? string.Empty;
                    DateTime? dataAdmissao = worksheet.Cells[row, 12].GetValue<DateTime?>();
                    string vertical = worksheet.Cells[row, 13].GetValue<string>() ?? string.Empty;
                    int idVertical = worksheet.Cells[row, 14].GetValue<int?>() ?? 1;
                    int atv = worksheet.Cells[row, 15].GetValue<int?>() ?? 0;

                    if (!string.IsNullOrEmpty(nome) && idEmpresa > 0 && idPerfil > 0 && idMentor > 0 && !string.IsNullOrEmpty(email) && dataAdmissao != null)
                    {
                        var importItem = new ASSOCIADOS
                        {
                            IdAssociado = idAssociado,
                            Nome = nome,
                            IdCargo = idCargo,
                            IdEmpresa = idEmpresa,
                            IdPerfil = idPerfil,
                            IdNivel = 1,
                            IdStatus = 1,
                            IdAssociadoMentor = idMentor,
                            DataAdmissao = dataAdmissao.Value,
                            Email = email,
                            Senha = "avaliacao",
                            Vertical = vertical,
                            FotoNome = null,
                            ATV = atv,
                            USR = 0,
                            DHC = DateTime.Now,
                            IdVertical = idVertical
                        };
                        var existeItem = idAssociado != 0 ? await _associadosService.ObterAssociadoAsync(idAssociado) : null;
                        if (existeItem == null)
                        {
                            await _associadosService.InserirAssociadoAsync(importItem);
                            inseridos++;
                        }
                        else
                        {
                            importItem.IdAssociado = idAssociado;
                            importItem.Senha = existeItem.Senha;
                            importItem.FotoNome = existeItem.FotoNome;
                            await _associadosService.AlterarAssociadoAsync(idAssociado, importItem);
                            alterados++;
                        }
                    }
                    else
                    {
                        desconsiderados++;
                    }
                }
            }
            return (inseridos, alterados, desconsiderados);
        }

        public async Task<(int inseridos, int alterados, int desconsiderados)> ImportarPromocoesExcelAsync(Stream fileStream)
        {
            int inseridos = 0, alterados = 0, desconsiderados = 0;
            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return (0, 0, 0);
                int rowCount = worksheet.Dimension.End.Row;
                for (int row = 2; row <= rowCount; row++)
                {
                    int idPromocao = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                    int idAssociado = worksheet.Cells[row, 2].GetValue<int?>() ?? 0;
                    int idCargoAnterior = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                    int idCargoNovo = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                    DateTime? dataPromocao = worksheet.Cells[row, 8].GetValue<DateTime?>();
                    string comentarios = worksheet.Cells[row, 9].GetValue<string>() ?? "Import";
                    bool atv = worksheet.Cells[row, 10].GetValue<bool?>() ?? true;

                    if (idAssociado > 0 && idCargoAnterior > 0 && idCargoNovo > 0 && dataPromocao != null)
                    {
                        var importItem = new PROMOCOES
                        {
                            idAssociado = idAssociado,
                            idCargoAnterior = idCargoAnterior,
                            idCargoNovo = idCargoNovo,
                            DataPromocao = dataPromocao.Value,
                            Comentarios = comentarios,
                            ATV = atv,
                            DHC = DateTime.Now
                        };
                        var existeItem = idPromocao != 0 ? await _cargosService.ObterPromocaoAsync(idAssociado, idCargoAnterior, idCargoNovo) : null;
                        if (existeItem == null)
                        {
                            await _cargosService.AdicionarPromocaoAsync(importItem);
                            inseridos++;
                        }
                        else
                        {
                            importItem.idPromocao = idPromocao;
                            await _cargosService.AlterarPromocaoAsync(importItem);
                            alterados++;
                        }
                    }
                    else
                    {
                        desconsiderados++;
                    }
                }
            }
            return (inseridos, alterados, desconsiderados);
        }
    }
}