using System;

namespace Services.Periodos.Common;

public static class PeriodoFormatHelper
{
    public static string FormatStatus(int? atv)
    {
        return atv.HasValue && atv.Value == 1 ? "Ativo" : "Inativo";
    }

    public static string FormatDate(DateTime? date)
    {
        return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : string.Empty;
    }

    public static string FormatDateIso(DateTime? date)
    {
        return date.HasValue ? date.Value.ToString("yyyy-MM-dd") : string.Empty;
    }
}
