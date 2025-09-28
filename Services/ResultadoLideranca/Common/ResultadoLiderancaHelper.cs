using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.ResultadoLideranca.Common;

public static class ResultadoLiderancaHelper
{
    public static string FormatPercent(decimal nota)
    {
        if (nota > 0)
            return nota.ToString("##0") + "%";
        return "0%";
    }

    public static string FormatPercentNullable(decimal? nota)
    {
        if (nota.HasValue && nota.Value > 0)
            return nota.Value.ToString("##0") + "%";
        return "0%";
    }

    public static string FormatDecimal(decimal nota)
    {
        return Math.Round(nota, 2).ToString();
    }

    public static string FormatDecimalNullable(decimal? nota)
    {
        if (nota.HasValue)
            return Math.Round(nota.Value, 2).ToString();
        return string.Empty;
    }

    public static string TruncarTexto(string texto, int qtdCaracteres)
    {
        if (!string.IsNullOrEmpty(texto) && texto.Length > qtdCaracteres)
            return string.Format("{0}...", texto.Substring(0, qtdCaracteres));
        return texto;
    }

    public static List<string> GerarLabelsGraficoEixos(IEnumerable<dynamic> eixos)
    {
        return eixos.Select(e => e.eixo?.ToString() ?? string.Empty).ToList();
    }

    public static List<decimal> GerarValoresGraficoEixos(IEnumerable<dynamic> eixos)
    {
        return eixos.Select(e => e.resultado is decimal d ? d : 0m).ToList();
    }

    public static (List<string> Labels, List<decimal> Valores) GerarDadosGrafico(IEnumerable<dynamic> dados, string labelProp, string valorProp)
    {
        var labels = new List<string>();
        var valores = new List<decimal>();
        foreach (var item in dados)
        {
            var label = item.GetType().GetProperty(labelProp)?.GetValue(item)?.ToString() ?? string.Empty;
            var valorObj = item.GetType().GetProperty(valorProp)?.GetValue(item);
            decimal valor = 0;
            if (valorObj is decimal d)
                valor = d;
            else if (valorObj is int i)
                valor = i;
            labels.Add(label);
            valores.Add(valor);
        }
        return (labels, valores);
    }

    public static List<(string Subcompetencia, decimal? Valor)> GerarListaSubcompetencias(IEnumerable<dynamic> subcompetencias, string nomeProp, string valorProp)
    {
        var lista = new List<(string, decimal?)>();
        foreach (var item in subcompetencias)
        {
            var nome = item.GetType().GetProperty(nomeProp)?.GetValue(item)?.ToString() ?? string.Empty;
            var valorObj = item.GetType().GetProperty(valorProp)?.GetValue(item);
            decimal? valor = null;
            if (valorObj is decimal d)
                valor = d;
            else if (valorObj is int i)
                valor = i;
            lista.Add((nome, valor));
        }
        return lista;
    }

    public static List<string> FiltrarPalavrasIrrelevantes(IEnumerable<string> palavras, IEnumerable<string> irrelevantes)
    {
        var irrelevantesSet = new HashSet<string>(irrelevantes.Select(x => x.ToLower()));
        return palavras.Where(p => !irrelevantesSet.Contains(p.ToLower())).ToList();
    }

    public static Dictionary<string, int> ContarPalavras(IEnumerable<string> palavras)
    {
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var palavra in palavras)
        {
            if (string.IsNullOrWhiteSpace(palavra))
                continue;
            if (!dict.ContainsKey(palavra))
                dict[palavra] = 0;
            dict[palavra]++;
        }
        return dict;
    }
}