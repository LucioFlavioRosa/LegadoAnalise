using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Competencias.Common;

public interface ICompetenciasImportExportUtil
{
    Task<byte[]> ExportarParaExcelAsync(List<Competencia> competencias);
    Task<ImportResult> ImportarDeExcelAsync(Stream fileStream, int idEmpresa, int idUsuario);
}

public class CompetenciasImportExportUtil : ICompetenciasImportExportUtil
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public CompetenciasImportExportUtil(
        ApplicationDbContext context,
        ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<byte[]> ExportarParaExcelAsync(List<Competencia> competencias)
    {
        try
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Competências");

            // Cabeçalhos
            var headers = new string[]
            {
                "IdCompetencia", "IdCargo", "Cargo", "IdEixo", "Eixo",
                "IdSubCompetencia", "SubCompetencia", "IdDimensao", "Dimensao",
                "DetalheNivelAtual", "CompetenciaAtual", "PalavrasChave",
                "TipoAvaliacao", "Escopo", "ATV"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            // Dados
            for (int row = 0; row < competencias.Count; row++)
            {
                var competencia = competencias[row];
                var excelRow = row + 2;

                worksheet.Cells[excelRow, 1].Value = competencia.IdCompetencia;
                worksheet.Cells[excelRow, 2].Value = competencia.IdCargo;
                worksheet.Cells[excelRow, 3].Value = competencia.Cargo?.Nome ?? "";
                worksheet.Cells[excelRow, 4].Value = competencia.IdEixo;
                worksheet.Cells[excelRow, 5].Value = competencia.Eixo?.Nome ?? "";
                worksheet.Cells[excelRow, 6].Value = competencia.IdSubCompetencia;
                worksheet.Cells[excelRow, 7].Value = competencia.SubCompetencia?.Nome ?? "";
                worksheet.Cells[excelRow, 8].Value = competencia.IdDimensao;
                worksheet.Cells[excelRow, 9].Value = competencia.Dimensao?.Nome ?? "";
                worksheet.Cells[excelRow, 10].Value = competencia.CompetenciaJRDetalhe ?? "";
                worksheet.Cells[excelRow, 11].Value = competencia.CompetenciaJR ?? "";
                worksheet.Cells[excelRow, 12].Value = competencia.PalavrasChave ?? "";
                worksheet.Cells[excelRow, 13].Value = competencia.TipoAvaliacao ?? "";
                worksheet.Cells[excelRow, 14].Value = competencia.Escopo ?? "";
                worksheet.Cells[excelRow, 15].Value = competencia.ATV;
            }

            // Auto-ajustar colunas
            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao exportar competências para Excel");
            throw;
        }
    }

    public async Task<ImportResult> ImportarDeExcelAsync(Stream fileStream, int idEmpresa, int idUsuario)
    {
        var resultado = new ImportResult();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                resultado.Erros.Add("O arquivo Excel não possui nenhuma planilha.");
                return resultado;
            }

            int rowCount = worksheet.Dimension?.End.Row ?? 0;
            if (rowCount <= 1)
            {
                resultado.Erros.Add("O arquivo não possui dados para importar.");
                return resultado;
            }

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var idCompetencia = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                    var idCargo = worksheet.Cells[row, 2].GetValue<int?>() ?? 0;
                    var idEixo = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                    var idSubCompetencia = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                    var idDimensao = worksheet.Cells[row, 8].GetValue<int?>() ?? 0;
                    var detalheNivelAtual = worksheet.Cells[row, 10].GetValue<string>() ?? "-";
                    var competenciaAtual = worksheet.Cells[row, 11].GetValue<string>() ?? "-";
                    var palavrasChave = worksheet.Cells[row, 12].GetValue<string>() ?? "";
                    var tipoAvaliacao = worksheet.Cells[row, 13].GetValue<string>() ?? "desempenho";
                    var escopo = worksheet.Cells[row, 14].GetValue<string>() ?? "projeto";
                    var ativo = worksheet.Cells[row, 15].GetValue<int?>() ?? 1;

                    if (!ValidarDadosLinha(idCargo, idEixo, idSubCompetencia, idDimensao, tipoAvaliacao, escopo))
                    {
                        resultado.LinhasDesconsideradas++;
                        continue;
                    }

                    var competencia = new Competencia
                    {
                        IdCargo = idCargo,
                        IdEixo = idEixo,
                        IdSubCompetencia = idSubCompetencia,
                        IdDimensao = idDimensao,
                        IdEmpresa = idEmpresa,
                        IdNivel = 1,
                        CompetenciaJRDetalhe = detalheNivelAtual,
                        CompetenciaJR = competenciaAtual,
                        CompetenciaPLDetalhe = detalheNivelAtual,
                        CompetenciaPL = competenciaAtual,
                        CompetenciaSRDetalhe = detalheNivelAtual,
                        CompetenciaSR = competenciaAtual,
                        PalavrasChave = palavrasChave,
                        TipoAvaliacao = tipoAvaliacao,
                        Escopo = escopo,
                        ATV = ativo,
                        USR = idUsuario,
                        DHC = DateTime.Now,
                        IdModo = 1,
                        InputAutoAvaliacao = true,
                        InputAvaliacaoAsCegas = true,
                        InputAvaliacaoGestor = true,
                        InputFeedback = true,
                        InputNivel1 = true,
                        InputNivel2 = true,
                        VisivelAutoAvaliacao = true,
                        VisivelAvaliacaoAsCegas = true,
                        VisivelAvaliacaoGestor = true,
                        VisivelFeedback = true,
                        VisivelNivel1 = true,
                        VisivelNivel2 = true
                    };

                    var competenciaExistente = await _context.Competencias
                        .FirstOrDefaultAsync(c => c.IdCargo == idCargo &&
                                                 c.IdEixo == idEixo &&
                                                 c.IdSubCompetencia == idSubCompetencia &&
                                                 c.IdDimensao == idDimensao &&
                                                 c.CompetenciaJRDetalhe == detalheNivelAtual);

                    if (idCompetencia == 0 && competenciaExistente == null)
                    {
                        _context.Competencias.Add(competencia);
                        await _context.SaveChangesAsync();
                        resultado.LinhasInseridas++;
                    }
                    else if (competenciaExistente != null && competenciaExistente.ATV == 0)
                    {
                        resultado.LinhasDesconsideradas++;
                    }
                    else
                    {
                        var competenciaParaAlterar = competenciaExistente ?? await _context.Competencias
                            .FirstOrDefaultAsync(c => c.IdCompetencia == idCompetencia);

                        if (competenciaParaAlterar != null)
                        {
                            AtualizarCompetenciaExistente(competenciaParaAlterar, competencia);
                            await _context.SaveChangesAsync();
                            resultado.LinhasAlteradas++;
                        }
                        else
                        {
                            resultado.LinhasComErro++;
                        }
                    }

                    // Atualizar relação cargo-subcompetência
                    await AtualizarRelacaoCargoSubcompetenciaAsync(idCargo, idSubCompetencia, competenciaAtual);
                }
                catch (Exception ex)
                {
                    resultado.LinhasComErro++;
                    resultado.Erros.Add($"Erro na linha {row}: {ex.Message}");
                }
            }

            return resultado;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao importar competências do Excel");
            resultado.Erros.Add($"Erro geral na importação: {ex.Message}");
            return resultado;
        }
    }

    private bool ValidarDadosLinha(int idCargo, int idEixo, int idSubCompetencia, int idDimensao, string tipoAvaliacao, string escopo)
    {
        return idCargo > 0 && idEixo > 0 && idSubCompetencia > 0 && idDimensao > 0 &&
               !string.IsNullOrEmpty(tipoAvaliacao) && !string.IsNullOrEmpty(escopo);
    }

    private void AtualizarCompetenciaExistente(Competencia existente, Competencia nova)
    {
        existente.CompetenciaJR = nova.CompetenciaJR;
        existente.CompetenciaJRDetalhe = nova.CompetenciaJRDetalhe;
        existente.CompetenciaPL = nova.CompetenciaPL;
        existente.CompetenciaPLDetalhe = nova.CompetenciaPLDetalhe;
        existente.CompetenciaSR = nova.CompetenciaSR;
        existente.CompetenciaSRDetalhe = nova.CompetenciaSRDetalhe;
        existente.PalavrasChave = nova.PalavrasChave;
        existente.TipoAvaliacao = nova.TipoAvaliacao;
        existente.Escopo = nova.Escopo;
        existente.ATV = nova.ATV;
        existente.DHC = DateTime.Now;
    }

    private async Task AtualizarRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubcompetencia, string descricao)
    {
        var relacao = await _context.RelacoesCargosSubcompetencias
            .FirstOrDefaultAsync(r => r.IdCargo == idCargo && r.IdSubcompetencia == idSubcompetencia);

        if (relacao == null)
        {
            relacao = new RelacaoCargoSubcompetencia
            {
                IdCargo = idCargo,
                IdSubcompetencia = idSubcompetencia,
                Descricao = descricao
            };
            _context.RelacoesCargosSubcompetencias.Add(relacao);
        }
        else
        {
            relacao.Descricao = descricao;
        }

        await _context.SaveChangesAsync();
    }
}