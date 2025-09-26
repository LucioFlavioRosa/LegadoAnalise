using System;
using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;

namespace Services.AvaliacoesGestor.Common;

public static class CompetenciaHelper
{
    public static bool IsNotaNaoSeAplica(int nota, int idNotaNaoSeAplica = 5)
    {
        return nota == idNotaNaoSeAplica;
    }

    public static bool IsNotaSelecionar(int nota, int idNotaSelecionar = 0)
    {
        return nota == idNotaSelecionar;
    }

    public static bool IsNotaValida(int nota, int idNotaSelecionar = 0)
    {
        return nota > idNotaSelecionar;
    }

    public static bool IsNotaNivel2MaiorQueNivel1(int notaNivel1, int notaNivel2, Dictionary<int, int> pesos)
    {
        if (pesos.TryGetValue(notaNivel1, out int peso1) && pesos.TryGetValue(notaNivel2, out int peso2))
        {
            return peso2 > peso1;
        }
        return false;
    }

    public static bool PilarVazio(List<CompetenciaModel> respostasPilar, int idNotaNaoSeAplica = 5)
    {
        return respostasPilar.All(r => r.notaValidaNivel1 == idNotaNaoSeAplica) || respostasPilar.All(r => r.notaValidaNivel2 == idNotaNaoSeAplica);
    }

    public static IEnumerable<IGrouping<string, CompetenciaModel>> AgruparPorSubCompetencia(IEnumerable<CompetenciaModel> competencias)
    {
        return competencias.GroupBy(c => c.SubCompetencia);
    }

    public static List<string> ValidarNotas(List<CompetenciaModel> competencias, Dictionary<int, int> pesos, int idNotaNaoSeAplica = 5, int idNotaSelecionar = 0)
    {
        var mensagens = new List<string>();
        foreach (var comp in competencias)
        {
            if (comp.IdModo == 1)
            {
                if (IsNotaNaoSeAplica(comp.notaValidaNivel1, idNotaNaoSeAplica))
                {
                    if (!IsNotaNaoSeAplica(comp.notaValidaNivel2, idNotaNaoSeAplica))
                    {
                        mensagens.Add($"Quando a nota de Competência do Nível Atual for = Não Se Aplica, o Próximo Nível deve ser Não Se Aplica. SubCompetência: {comp.SubCompetencia}");
                    }
                }
                if ((IsNotaSelecionar(comp.notaValidaNivel1, idNotaSelecionar) && IsNotaValida(comp.notaValidaNivel2, idNotaSelecionar)) ||
                    (IsNotaValida(comp.notaValidaNivel1, idNotaSelecionar) && IsNotaSelecionar(comp.notaValidaNivel2, idNotaSelecionar)))
                {
                    mensagens.Add($"É obrigatório selecionar uma nota para cada nível. SubCompetência: {comp.SubCompetencia}");
                }
                if (IsNotaValida(comp.notaValidaNivel1, idNotaSelecionar) && IsNotaValida(comp.notaValidaNivel2, idNotaSelecionar))
                {
                    if (IsNotaNivel2MaiorQueNivel1(comp.notaValidaNivel1, comp.notaValidaNivel2, pesos))
                    {
                        mensagens.Add($"A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual). SubCompetência: {comp.SubCompetencia}");
                    }
                }
            }
        }
        return mensagens;
    }

    public static List<string> ValidarPilares(List<CompetenciaModel> competencias, int idNotaNaoSeAplica = 5, string tipoAvaliacao = "")
    {
        var mensagens = new List<string>();
        var grupos = AgruparPorSubCompetencia(competencias);
        foreach (var grupo in grupos)
        {
            var padrao = grupo.Where(r => r.IdModo != 2).ToList();
            var auto = grupo.Where(r => r.IdModo == 2).ToList();
            var vaziasNivel1 = padrao.Where(r => r.notaValidaNivel1 == idNotaNaoSeAplica).ToList();
            var vaziasNivel2 = padrao.Where(r => r.notaValidaNivel2 == idNotaNaoSeAplica).ToList();

            if (padrao.Count > 0 && padrao.Count == vaziasNivel1.Count)
            {
                mensagens.Add($"Pilar '{grupo.Key}' precisa receber ao mínimo 1 nota mensurável em cada nível (diferente de não se aplica)");
            }
            if (padrao.Count > 0 && padrao.Count == vaziasNivel2.Count)
            {
                mensagens.Add($"Pilar '{grupo.Key}' precisa receber ao mínimo 1 nota mensurável em cada nível (diferente de não se aplica)");
            }
        }
        return mensagens;
    }

    public static string TruncarTexto(string texto, int qtdCaracteres)
    {
        if (!string.IsNullOrEmpty(texto) && texto.Length > qtdCaracteres)
            return $"{texto.Substring(0, qtdCaracteres)}...";
        return texto;
    }

    public static void PreencherNotasPadrao(CompetenciaModel competencia, int? notaPadraoNivel1, int? notaPadraoNivel2, int idNotaNaoSeAplica = 5)
    {
        competencia.notaValidaNivel1 = notaPadraoNivel1 ?? idNotaNaoSeAplica;
        competencia.notaValidaNivel2 = notaPadraoNivel2 ?? idNotaNaoSeAplica;
    }

    public static void AplicarVisibilidade(CompetenciaModel competencia, bool inputEtapaAtual, bool inputNivel1, bool inputNivel2, bool visivelEtapaAtual, bool visivelNivel1, bool visivelNivel2)
    {
        competencia.disableSelectNivel1 = inputEtapaAtual && inputNivel1 ? "" : "disabled";
        competencia.disableSelectNivel2 = inputEtapaAtual && inputNivel2 ? "" : "disabled";
        competencia.hiddenSelectNivel1 = inputEtapaAtual && inputNivel1 ? "" : "hidden";
        competencia.hiddenSelectNivel2 = inputEtapaAtual && inputNivel2 ? "" : "hidden";
        competencia.hiddenTextBoxNivel1 = !inputEtapaAtual || !inputNivel1 ? "" : "hidden";
        competencia.hiddenTextBoxNivel2 = !inputEtapaAtual || !inputNivel2 ? "" : "hidden";
        competencia.textoNotaNivel1 = visivelEtapaAtual && visivelNivel1 ? competencia.textoNotaNivel1 : "Nota não disponível nesta etapa.";
        competencia.textoNotaNivel2 = visivelEtapaAtual && visivelNivel2 ? competencia.textoNotaNivel2 : "Nota não disponível nesta etapa.";
    }
}