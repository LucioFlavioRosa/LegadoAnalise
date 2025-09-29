using System;
using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;

namespace Services.Mentoria.Common;

public static class MentoriaHelper
{
    public static string FormatarNota(double? nota, double notaMaxima = 5)
    {
        if (!nota.HasValue || double.IsNaN(nota.Value))
            return "-";
        return $"{nota.Value:0.##} / {notaMaxima}";
    }

    public static double? CalcularMedia(IEnumerable<double?> valores)
    {
        var validos = valores.Where(v => v.HasValue && !double.IsNaN(v.Value)).Select(v => v.Value).ToList();
        if (validos.Count == 0)
            return null;
        return Math.Round(validos.Average(), 2);
    }

    public static List<IGrouping<int, MENTORADORESPOSTAS>> AgruparRespostasPorPeriodo(IEnumerable<MENTORADORESPOSTAS> respostas)
    {
        return respostas.GroupBy(r => r.idPeriodo).ToList();
    }

    public static string ObterStatusPeriodo(int countMentorados)
    {
        return countMentorados > 1 ? "Completo" : "Parcial";
    }

    public static string ObterClassePill(bool isUltimoPeriodo)
    {
        return isUltimoPeriodo ? "btn btn-danger" : "btn btn-facebook";
    }
}
