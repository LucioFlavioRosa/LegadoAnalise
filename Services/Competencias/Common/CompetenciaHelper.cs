using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;
using Peers.Moderno.Services.Common;

namespace Services.Competencias.Common;

public static class CompetenciaHelper
{
    public static string TruncarTexto(string texto, int qtdCaracteres)
    {
        if (!string.IsNullOrEmpty(texto))
        {
            if (texto.Length > qtdCaracteres)
            {
                return string.Format("{0}...", texto.Substring(0, qtdCaracteres));
            }
        }
        return texto;
    }

    public static bool ValidarNotas(int notaNivel1, int notaNivel2, out string? mensagemErro)
    {
        mensagemErro = null;
        if (notaNivel1 == 5 && notaNivel2 != 5)
        {
            mensagemErro = "ATENÇÃO ! Avaliação de Competência com inconsistência. (Detalhe: quando a nota de Competência do Nivel Atual for = Não Se Aplica, o Próximo Nivel deve ser Não Se Aplica). O Sistema ajustou sua avaliação. Favor revalidar sua avaliação !!";
            return false;
        }
        if (notaNivel1 != 5 && notaNivel1 != 0 && notaNivel2 != 0 && notaNivel2 < notaNivel1)
        {
            mensagemErro = "Existem Avaliações de Competências inconsistentes. A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).";
            return false;
        }
        return true;
    }

    public static void CarregaCombosNota(ref List<ComboItem> combo, bool includeSelecionar = true)
    {
        combo.Clear();
        combo.AddRange(ComboHelper.GetNotasCompetenciaItems(includeSelecionar));
    }

    public static void PreencherNotasSelecionadas(
        int idNotaNivel1,
        int idNotaNivel2,
        List<ComboItem> comboNivel1,
        List<ComboItem> comboNivel2)
    {
        foreach (var item in comboNivel1)
        {
            item.IsSelected = item.Value == idNotaNivel1.ToString();
        }
        foreach (var item in comboNivel2)
        {
            item.IsSelected = item.Value == idNotaNivel2.ToString();
        }
    }

    public static void ResetarNotasSelecionadas(List<ComboItem> comboNivel1, List<ComboItem> comboNivel2)
    {
        foreach (var item in comboNivel1)
        {
            item.IsSelected = false;
        }
        foreach (var item in comboNivel2)
        {
            item.IsSelected = false;
        }
    }
}
