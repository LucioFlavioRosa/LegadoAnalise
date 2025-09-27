namespace Peers.Moderno.Services.Common;

public static class FormatHelper
{
    public static string FormatPercent(decimal nota)
    {
        if (nota > 0)
        {
            return nota.ToString("##0") + "%";
        }
        return "0%";
    }

    public static string FormatDecimal(decimal nota)
    {
        return Math.Round(nota, 2).ToString();
    }

    public static string FormatPercentNullable(decimal? nota)
    {
        if (nota.HasValue && nota.Value > 0)
        {
            return nota.Value.ToString("##0") + "%";
        }
        return "0%";
    }

    public static string FormatDecimalNullable(decimal? nota)
    {
        if (nota.HasValue)
        {
            return Math.Round(nota.Value, 2).ToString();
        }
        return "";
    }

    public static string FormatCurrency(decimal value)
    {
        return value.ToString("C2");
    }

    public static string FormatNumber(decimal value, int decimals = 2)
    {
        return Math.Round(value, decimals).ToString($"N{decimals}");
    }

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
}
