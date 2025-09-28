using Peers.Moderno.Models;

namespace Services.AvaliacoesExportacao.Common;

public class ExportarAvaliacoesHelper
{
    public string ObterCompetenciaOuPerformance(AvaliacaoCompetencia avaliacao)
    {
        if (avaliacao?.COMPETENCIAS?.Eixo != null && avaliacao.COMPETENCIAS?.SubCompetencia != null)
        {
            return $"{avaliacao.COMPETENCIAS.Eixo.Nome} / {avaliacao.COMPETENCIAS.SubCompetencia.Nome}";
        }
        return string.Empty;
    }

    public string TruncarTexto(string texto, int maxLen)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;
        if (texto.Length > maxLen)
            return texto.Substring(0, maxLen) + "...";
        return texto;
    }

    public decimal CalcularPercentual(decimal valor, decimal total)
    {
        if (total == 0) return 0;
        return Math.Round((valor / total) * 100, 2);
    }
}
