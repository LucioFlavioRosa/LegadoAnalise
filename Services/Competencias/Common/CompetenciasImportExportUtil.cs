using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Competencias.Common;

namespace Peers.Moderno.Services.Competencias.Common;

public class CompetenciasImportExportUtil
{
    private readonly ApplicationDbContext _context;

    public CompetenciasImportExportUtil(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GerarArquivoExcelAsync(List<CompetenciaExportModel> competencias)
    {
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
            int row = i + 2;

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

        return await package.GetAsByteArrayAsync();
    }

    public async Task<ImportResult> ProcessarImportacaoAsync(ExcelPackage package, int idEmpresa, int idUsuario)
    {
        var result = new ImportResult();
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        
        if (worksheet == null)
        {
            throw new InvalidOperationException("O arquivo Excel não possui nenhuma planilha.");
        }

        int rowCount = worksheet.Dimension?.End.Row ?? 0;
        if (rowCount <= 1) return result;

        // Obter modo de cálculo padrão
        var modoCalculoPadrao = await _context.ModosCalculosCompetencias.FirstOrDefaultAsync();
        if (modoCalculoPadrao == null)
        {
            throw new InvalidOperationException("Nenhum modo de cálculo encontrado no sistema.");
        }

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var dadosLinha = ExtrairDadosLinha(worksheet, row);
                
                if (!ValidarDadosObrigatorios(dadosLinha))
                {
                    result.LinhasDesconsideradas++;
                    continue;
                }

                var competencia = await CriarCompetenciaFromDados(dadosLinha, idEmpresa, idUsuario, modoCalculoPadrao.IdModo);
                var competenciaExistente = await ObterCompetenciaExistente(dadosLinha);

                if (dadosLinha.IdCompetencia == 0 && competenciaExistente == null)
                {
                    // Inserir nova competência
                    _context.Competencias.Add(competencia);
                    await _context.SaveChangesAsync();
                    
                    await AtualizarRelacaoCargoSubcompetencia(dadosLinha.IdCargo, dadosLinha.IdSubCompetencia, dadosLinha.CompetenciaAtual);
                    result.LinhasInseridas++;
                }
                else if (competenciaExistente != null && competenciaExistente.Ativo == false)
                {
                    result.LinhasDesconsideradas++;
                }
                else
                {
                    // Alterar competência existente
                    var idParaAlterar = dadosLinha.IdCompetencia > 0 ? dadosLinha.IdCompetencia : (competenciaExistente?.IdCompetencia ?? 0);
                    
                    if (idParaAlterar > 0)
                    {
                        competencia.IdCompetencia = idParaAlterar;
                        _context.Entry(competencia).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        
                        await AtualizarRelacaoCargoSubcompetencia(dadosLinha.IdCargo, dadosLinha.IdSubCompetencia, dadosLinha.CompetenciaAtual);
                        result.LinhasAlteradas++;
                    }
                    else
                    {
                        result.LinhasComErro++;
                    }
                }
            }
            catch
            {
                result.LinhasComErro++;
            }
        }

        return result;
    }

    private DadosImportacao ExtrairDadosLinha(ExcelWorksheet worksheet, int row)
    {
        return new DadosImportacao
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
    }

    private bool ValidarDadosObrigatorios(DadosImportacao dados)
    {
        return dados.IdCargo > 0 && 
               dados.IdEixo > 0 && 
               dados.IdSubCompetencia > 0 && 
               dados.IdDimensao > 0 && 
               !string.IsNullOrEmpty(dados.TipoAvaliacao) && 
               !string.IsNullOrEmpty(dados.Escopo);
    }

    private async Task<Competencia> CriarCompetenciaFromDados(DadosImportacao dados, int idEmpresa, int idUsuario, int idModoCalculo)
    {
        return new Competencia
        {
            IdEmpresa = idEmpresa,
            IdCargo = dados.IdCargo,
            IdNivel = 1,
            IdEixo = dados.IdEixo,
            IdSubCompetencia = dados.IdSubCompetencia,
            IdDimensao = dados.IdDimensao,
            CompetenciaJR = dados.CompetenciaAtual,
            CompetenciaPL = dados.CompetenciaAtual,
            CompetenciaSR = dados.CompetenciaAtual,
            CompetenciaJRDetalhe = dados.DetalheNivelAtual,
            CompetenciaPLDetalhe = dados.DetalheNivelAtual,
            CompetenciaSRDetalhe = dados.DetalheNivelAtual,
            PalavrasChave = dados.PalavrasChave,
            TipoAvaliacao = dados.TipoAvaliacao,
            Escopo = dados.Escopo,
            Ativo = dados.ATV == 1,
            UsuarioCriacao = idUsuario,
            DataCriacao = DateTime.Now,
            IdModoCalculo = idModoCalculo,
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
    }

    private async Task<Competencia?> ObterCompetenciaExistente(DadosImportacao dados)
    {
        return await _context.Competencias
            .FirstOrDefaultAsync(c => 
                c.IdCargo == dados.IdCargo &&
                c.IdNivel == 1 &&
                c.IdEixo == dados.IdEixo &&
                c.IdSubCompetencia == dados.IdSubCompetencia &&
                c.IdDimensao == dados.IdDimensao &&
                c.CompetenciaJRDetalhe == dados.DetalheNivelAtual);
    }

    private async Task AtualizarRelacaoCargoSubcompetencia(int idCargo, int idSubcompetencia, string descricao)
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

    private class DadosImportacao
    {
        public int IdCompetencia { get; set; }
        public int IdCargo { get; set; }
        public int IdEixo { get; set; }
        public int IdSubCompetencia { get; set; }
        public int IdDimensao { get; set; }
        public string DetalheNivelAtual { get; set; } = string.Empty;
        public string CompetenciaAtual { get; set; } = string.Empty;
        public string PalavrasChave { get; set; } = string.Empty;
        public string TipoAvaliacao { get; set; } = string.Empty;
        public string Escopo { get; set; } = string.Empty;
        public int ATV { get; set; }
    }
}