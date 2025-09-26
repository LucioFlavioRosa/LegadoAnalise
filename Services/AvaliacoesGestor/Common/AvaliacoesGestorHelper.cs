using System;
using System.Collections.Generic;
using System.Linq;
using Services.AvaliacoesGestor.Common;

namespace Services.AvaliacoesGestor.Common;

public class AvaliacoesGestorHelper
{
    public string TruncaTexto(string texto, int qtdCaracter)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;
        if (texto.Length > qtdCaracter)
            return texto.Substring(0, qtdCaracter) + "...";
        return texto;
    }

    public bool ValidarAvaliacoesPreenchidas(List<AvaliacaoGestorPerformanceInput> avaliacoes)
    {
        return avaliacoes.All(a => a.IdNotaNivel1AvaliacaoGestor > 0);
    }

    public List<string> ObterAbrangenciasUnicas(List<PerformanceModel> performances)
    {
        return performances.Select(p => p.Abrangencia).Distinct().ToList();
    }
}
