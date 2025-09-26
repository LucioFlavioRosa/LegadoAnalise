using System;
using System.Collections.Generic;
using System.Linq;

namespace Peers.Moderno.Services.Common;

public static class CompetenciaHelper
{
    public static string TruncarTexto(string texto, int qtdCaracteres)
    {
        if (string.IsNullOrEmpty(texto))
            return texto;
        if (texto.Length > qtdCaracteres)
            return string.Format("{0}...", texto.Substring(0, qtdCaracteres));
        return texto;
    }

    public static bool IsNotaNaoSeAplica(int nota, int idNotaNaoSeAplica = 5)
    {
        return nota == idNotaNaoSeAplica;
    }

    public static bool IsNotaSelecionar(int nota, int idNotaSelecionar = 0)
    {
        return nota == idNotaSelecionar;
    }

    public static bool IsNotaValida(int nota, int idNotaSelecionar = 0, int idNotaNaoSeAplica = 5)
    {
        return nota > idNotaSelecionar && nota <= idNotaNaoSeAplica;
    }

    public static bool IsNotaMaiorQue(int nota1, int nota2)
    {
        return nota2 > nota1;
    }

    public static bool IsNotaMenorQue(int nota1, int nota2)
    {
        return nota2 < nota1;
    }

    public static bool PilarPossuiNotaMensuravel(List<int> notasNivel, int idNotaNaoSeAplica = 5)
    {
        return notasNivel.Any(n => n != idNotaNaoSeAplica);
    }

    public static string GetNotaDescricao(int nota, List<ComboItem> notas)
    {
        var item = notas.FirstOrDefault(x => x.Value == nota.ToString());
        return item?.Text ?? string.Empty;
    }

    public static bool ValidarNotasConsistentes(int notaNivel1, int notaNivel2, List<ComboItem> notas, int idNotaNaoSeAplica = 5)
    {
        if (notaNivel1 == idNotaNaoSeAplica)
            return notaNivel2 == idNotaNaoSeAplica;
        var peso1 = GetNotaPeso(notaNivel1, notas);
        var peso2 = GetNotaPeso(notaNivel2, notas);
        return peso2 <= peso1;
    }

    public static int GetNotaPeso(int nota, List<ComboItem> notas)
    {
        // Supondo que ComboItem.AdditionalData["Peso"] armazene o peso
        var item = notas.FirstOrDefault(x => x.Value == nota.ToString());
        if (item != null && item.AdditionalData.ContainsKey("Peso"))
        {
            if (int.TryParse(item.AdditionalData["Peso"].ToString(), out int peso))
                return peso;
        }
        return 0;
    }

    public static bool PilarEstaVazio(List<int> notasNivel, int idNotaNaoSeAplica = 5)
    {
        return notasNivel.All(n => n == idNotaNaoSeAplica);
    }
}
