using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.ResultadoLideranca.Common;

namespace Services.ResultadoLideranca;

public interface IResultadoLiderancaService
{
    Task<List<ResultadoLiderancaFiltroModel>> GetFiltrosAsync(int? idAssociado = null);
    Task<List<ResultadoLiderancaPainelModel>> GetResultadosAsync(int idLider, int? idCiclo = null);
    Task<List<ResultadoLiderancaGraficoModel>> GetGraficosAsync(int idLider, int? idCiclo = null);
    Task NotificarLideresAsync();
}

public class ResultadoLiderancaService : IResultadoLiderancaService
{
    private readonly ApplicationDbContext _db;

    public ResultadoLiderancaService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ResultadoLiderancaFiltroModel>> GetFiltrosAsync(int? idAssociado = null)
    {
        var periodos = await _db.PeriodosAvaliacoes.Where(x => x.fl_lib_res_lideranca).OrderByDescending(x => x.IdPeriodo).ToListAsync();
        var projetos = await _db.Projetos.OrderBy(x => x.Nome).ToListAsync();
        var cargos = await _db.Cargos.OrderBy(x => x.Nome).ToListAsync();
        var liderados = await _db.Associados.OrderBy(x => x.Nome).ToListAsync();

        return new List<ResultadoLiderancaFiltroModel>
        {
            new ResultadoLiderancaFiltroModel
            {
                Periodos = periodos.Select(p => new ComboItem { Value = p.IdPeriodo.ToString(), Text = p.Nome }).ToList(),
                Projetos = projetos.Select(p => new ComboItem { Value = p.Id.ToString(), Text = p.Nome }).ToList(),
                Cargos = cargos.Select(c => new ComboItem { Value = c.IdCargo.ToString(), Text = c.Nome }).ToList(),
                Liderados = liderados.Select(l => new ComboItem { Value = l.Id.ToString(), Text = l.Nome }).ToList()
            }
        };
    }

    public async Task<List<ResultadoLiderancaPainelModel>> GetResultadosAsync(int idLider, int? idCiclo = null)
    {
        var periodos = await _db.PeriodosAvaliacoes.Where(x => x.fl_lib_res_lideranca).OrderByDescending(x => x.IdPeriodo).ToListAsync();
        var cicloAvaliado = idCiclo ?? periodos.FirstOrDefault()?.IdPeriodo ?? 0;
        var cicloAnterior = periodos.SkipWhile(x => x.IdPeriodo != cicloAvaliado).Skip(1).FirstOrDefault()?.IdPeriodo;

        var lider = await _db.Associados.FirstOrDefaultAsync(x => x.Id == idLider);
        if (lider == null) return new List<ResultadoLiderancaPainelModel>();

        var respostas = await _db.AvaliacoesCompetencias.Where(x => x.IdAssociado == idLider && (x.IdPeriodo == cicloAvaliado || (cicloAnterior != null && x.IdPeriodo == cicloAnterior))).ToListAsync();

        var avaliacoes = await _db.AvaliacoesCompetencias.Where(x => x.IdAssociado == idLider).ToListAsync();

        var resultadoAtual = ResultadoLiderancaHelper.MontarPainelResultado(lider, cicloAvaliado, respostas, avaliacoes, periodos);
        var resultadoAnterior = cicloAnterior != null ? ResultadoLiderancaHelper.MontarPainelResultado(lider, cicloAnterior.Value, respostas, avaliacoes, periodos) : null;

        var lista = new List<ResultadoLiderancaPainelModel>();
        if (resultadoAnterior != null) lista.Add(resultadoAnterior);
        lista.Add(resultadoAtual);
        return lista;
    }

    public async Task<List<ResultadoLiderancaGraficoModel>> GetGraficosAsync(int idLider, int? idCiclo = null)
    {
        var periodos = await _db.PeriodosAvaliacoes.Where(x => x.fl_lib_res_lideranca).OrderByDescending(x => x.IdPeriodo).ToListAsync();
        var cicloAvaliado = idCiclo ?? periodos.FirstOrDefault()?.IdPeriodo ?? 0;
        var cicloAnterior = periodos.SkipWhile(x => x.IdPeriodo != cicloAvaliado).Skip(1).FirstOrDefault()?.IdPeriodo;

        var respostas = await _db.AvaliacoesCompetencias.Where(x => x.IdAssociado == idLider && (x.IdPeriodo == cicloAvaliado || (cicloAnterior != null && x.IdPeriodo == cicloAnterior))).ToListAsync();
        var avaliacoes = await _db.AvaliacoesCompetencias.Where(x => x.IdAssociado == idLider).ToListAsync();

        var graficos = new List<ResultadoLiderancaGraficoModel>();
        if (cicloAnterior != null)
            graficos.Add(ResultadoLiderancaHelper.MontarGraficoResultado(idLider, cicloAnterior.Value, respostas, avaliacoes));
        graficos.Add(ResultadoLiderancaHelper.MontarGraficoResultado(idLider, cicloAvaliado, respostas, avaliacoes));
        return graficos;
    }

    public async Task NotificarLideresAsync()
    {
        // Implementação de envio de e-mail para todos os líderes
        // (Stub: lógica real de envio deve ser implementada conforme integração de e-mail do sistema)
        await Task.CompletedTask;
    }
}

public class ResultadoLiderancaFiltroModel
{
    public List<ComboItem> Periodos { get; set; } = new();
    public List<ComboItem> Projetos { get; set; } = new();
    public List<ComboItem> Cargos { get; set; } = new();
    public List<ComboItem> Liderados { get; set; } = new();
}

public class ResultadoLiderancaPainelModel
{
    public string Lider { get; set; } = string.Empty;
    public string Ciclo { get; set; } = string.Empty;
    public List<ComboItem> Ciclos { get; set; } = new();
    public List<ResultadoLiderancaProjetoModel> Projetos { get; set; } = new();
    public List<Dictionary<object, object>> ResultadoTotal { get; set; } = new();
    public ResultadoLiderancaPilaresModel ResultadoPilares { get; set; } = new();
    public ResultadoLiderancaSubcompetenciasModel ResultadoSubcompetencias { get; set; } = new();
    public List<ResultadoLiderancaDeltaModel> DeltaMaior { get; set; } = new();
    public List<ResultadoLiderancaDeltaModel> DeltaMenor { get; set; } = new();
    public List<ResultadoLiderancaPalavraModel> Palavras { get; set; } = new();
}

public class ResultadoLiderancaProjetoModel
{
    public int IdProjeto { get; set; }
    public string Projeto { get; set; } = string.Empty;
}

public class ResultadoLiderancaPilaresModel
{
    public List<string> Pilares { get; set; } = new();
    public List<decimal?> ResultadoPilaresLider { get; set; } = new();
    public List<decimal?> ResultadoPilaresTodos { get; set; } = new();
}

public class ResultadoLiderancaSubcompetenciasModel
{
    public List<string> Subcompetencias { get; set; } = new();
    public List<decimal?> ResultadoSubcompetenciasLider { get; set; } = new();
    public List<decimal?> ResultadoSubcompetenciasTodos { get; set; } = new();
}

public class ResultadoLiderancaDeltaModel
{
    public string Subcompetencia { get; set; } = string.Empty;
    public decimal? Resultado { get; set; }
}

public class ResultadoLiderancaPalavraModel
{
    public string Palavra { get; set; } = string.Empty;
    public string DataWeight { get; set; } = string.Empty;
}

public class ResultadoLiderancaGraficoModel
{
    public string Tipo { get; set; } = string.Empty;
    public object Dados { get; set; } = new();
}

public class ComboItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsSelected { get; set; } = false;
    public bool IsDisabled { get; set; } = false;
    public string CssClass { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}
