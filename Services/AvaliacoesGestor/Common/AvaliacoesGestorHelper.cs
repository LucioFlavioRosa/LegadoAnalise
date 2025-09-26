using System;
using System.Collections.Generic;
using System.Linq;
using Services.AvaliacoesGestor.Common;

public static class AvaliacoesGestorHelper
{
    public static string TruncaTexto(string texto, int qtdCaracter)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;
        return texto.Length > qtdCaracter ? texto.Substring(0, qtdCaracter) + "..." : texto;
    }

    public static List<PerformanceModel> OrganizarAbrangencias(List<PerformanceModel> performances)
    {
        var abrangenciasContadas = new HashSet<string>();
        var lista = new List<PerformanceModel>();
        foreach (var item in performances)
        {
            var clone = new PerformanceModel
            {
                IdPerformance = item.IdPerformance,
                Descricao = item.Descricao,
                Abaixo = item.Abaixo,
                Esperado = item.Esperado,
                Acima = item.Acima,
                Abrangencia = item.Abrangencia,
                SeparadorAbrangencia = abrangenciasContadas.Add(item.Abrangencia) ? string.Empty : "hidden",
                Input = item.Input,
                DisclaimerInput = item.DisclaimerInput,
                NotaAvaliado = item.NotaAvaliado,
                NotaCegas = item.NotaCegas,
                NotaGestor = item.NotaGestor,
                ObservacaoAvaliado = item.ObservacaoAvaliado,
                ObservacaoCegas = item.ObservacaoCegas,
                ObservacaoGestor = item.ObservacaoGestor,
                PodeEditar = item.PodeEditar,
                InputAvaliacaoGestor = item.InputAvaliacaoGestor,
                NotaPadraoAvaliacaoGestor = item.NotaPadraoAvaliacaoGestor
            };
            lista.Add(clone);
        }
        return lista;
    }

    public static bool ValidarNotasPreenchidas(List<PerformanceModel> performances)
    {
        return performances.All(p => !string.IsNullOrEmpty(p.NotaGestor) && p.NotaGestor != "0");
    }
}
