using System;
using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;

namespace Services.Resultados.Common;

public static class ResultadoHelper
{
    public static decimal CalcularMediaPercentual(IEnumerable<decimal?> valores)
    {
        var lista = valores.Where(v => v.HasValue).Select(v => v.Value).ToList();
        if (!lista.Any()) return 0;
        return Math.Round(lista.Average(), 2);
    }

    public static decimal CalcularSoma(IEnumerable<decimal?> valores)
    {
        return valores.Where(v => v.HasValue).Sum(v => v.Value);
    }

    public static string ObterRatingPerformance(decimal? valor)
    {
        if (!valor.HasValue) return "-";
        if (valor >= 90) return "Excelente";
        if (valor >= 75) return "Muito Bom";
        if (valor >= 60) return "Bom";
        if (valor >= 40) return "Regular";
        return "Insuficiente";
    }

    public static IEnumerable<ResultadoProjetosModel> FiltrarProjetosPorAssociadoPeriodo(IEnumerable<ResultadoProjetosModel> projetos, int idAssociado, int idPeriodo)
    {
        return projetos.Where(p => p.IdAssociado == idAssociado && p.IdPeriodo == idPeriodo);
    }

    public static IEnumerable<ResultadoProjetosModel> OrdenarProjetosPorData(IEnumerable<ResultadoProjetosModel> projetos)
    {
        return projetos.OrderBy(p => p.DataInicioAlocado);
    }

    public static string TruncarTexto(string texto, int qtdCaracteres)
    {
        if (!string.IsNullOrEmpty(texto) && texto.Length > qtdCaracteres)
        {
            return string.Format("{0}...", texto.Substring(0, qtdCaracteres));
        }
        return texto;
    }

    public static List<string> ObterEixosDasCompetencias(IEnumerable<Competencia> competencias)
    {
        return competencias.Select(c => c.Eixo?.Nome ?? "").Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
    }

    public static decimal? ObterNotaCompetenciaPorEixo(IEnumerable<ResultadoProjetosModel> projetos, string eixo)
    {
        var notas = projetos.SelectMany(p => p.ListCompetenciasNivel1)
            .Where(c => c.Eixo == eixo)
            .Select(c => c.PercentualNotaFinalNivel1)
            .Where(v => v.HasValue)
            .Select(v => v.Value);
        if (!notas.Any()) return null;
        return Math.Round(notas.Average(), 2);
    }

    public static decimal? ObterNotaPerformancePorProjeto(ResultadoProjetosModel projeto)
    {
        return projeto.SomaPerfomance;
    }

    public static string FormatPercentagem(decimal? nota)
    {
        if (nota.HasValue)
        {
            return nota.Value.ToString("##0") + "%";
        }
        return "0%";
    }

    public static string FormatDecimal(decimal? nota)
    {
        if (nota.HasValue)
        {
            return Math.Round(nota.Value, 2).ToString();
        }
        return "0";
    }
}
