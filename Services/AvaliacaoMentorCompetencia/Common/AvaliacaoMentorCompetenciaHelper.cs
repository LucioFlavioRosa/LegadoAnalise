namespace Services.AvaliacaoMentorCompetencia.Common
{
    public class AvaliacaoMentorCompetenciaHelper
    {
        public string FormatPercentagem(decimal nota)
        {
            if (nota > 0)
            {
                return nota.ToString("##0") + "%";
            }
            return "0%";
        }

        public string FormatDecimal(decimal nota)
        {
            return System.Math.Round(nota, 2).ToString();
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
}
