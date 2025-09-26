using System;
using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;

namespace Services.AvaliacoesGestor.Common;

public class AvaliacoesGestorHelper
{
    public static string TruncarTexto(string texto, int qtdCaracter)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;
        if (texto.Length > qtdCaracter)
            return texto.Substring(0, qtdCaracter) + "...";
        return texto;
    }

    public static List<string> ObterAbrangencias(List<PerformanceModel> performances)
    {
        return performances.Select(p => p.Abrangencia).Distinct().ToList();
    }

    public static List<PerformanceModel> OrganizarPorAbrangencia(List<PerformanceModel> performances)
    {
        var result = new List<PerformanceModel>();
        var abrangencias = ObterAbrangencias(performances);
        foreach (var abrangencia in abrangencias)
        {
            var abrangenciaItems = performances.Where(p => p.Abrangencia == abrangencia).ToList();
            if (abrangenciaItems.Any())
            {
                abrangenciaItems.First().SeparadorAbrangencia = string.Empty;
                foreach (var item in abrangenciaItems.Skip(1))
                {
                    item.SeparadorAbrangencia = "hidden";
                }
                result.AddRange(abrangenciaItems);
            }
        }
        return result;
    }

    public static bool ValidarNotasPreenchidas(List<PerformanceModel> performances)
    {
        return performances.All(p => p.NotaGestor > 0);
    }
}
