using Services.FrentesInternas.Common.Models;
using Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Services.FrentesInternas.Common.Helpers;

public static class FrentesInternasHelper
{
    public static List<ComboItem> GetNotasCombo(bool includeSelecionar = true)
    {
        return ComboHelper.GetNotasPerformanceItems(includeSelecionar);
    }

    public static string GetStatusValidado(bool? validado)
    {
        return validado == true ? "checked" : string.Empty;
    }

    public static string GetNotaDescricao(List<ComboItem> notas, int idNota)
    {
        var item = notas.FirstOrDefault(n => n.Value == idNota.ToString());
        return item?.Text ?? "[Selecionar]";
    }

    public static bool IsEditablePeriodo(int periodoId, int ultimoPeriodoId)
    {
        return periodoId == ultimoPeriodoId;
    }
}