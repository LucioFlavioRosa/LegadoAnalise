namespace Services.Resultados.Common;

public class ResultadoHelper
{
    public string FormatPercentagem(object nota)
    {
        if (nota != null)
        {
            if (decimal.TryParse(nota.ToString(), out var notaFormatted))
            {
                return notaFormatted.ToString("##0") + "%";
            }
        }
        return "0%";
    }

    public string FormatDecimal(object nota)
    {
        if (nota != null)
        {
            if (decimal.TryParse(nota.ToString(), out var notaFormatted))
            {
                return Math.Round(notaFormatted, 2).ToString();
            }
        }
        return "0";
    }

    public string TruncarTexto(string texto, int qtdCaracteres)
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
