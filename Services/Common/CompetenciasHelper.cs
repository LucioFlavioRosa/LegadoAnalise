using System;
using System.Collections.Generic;
using System.Linq;

namespace Peers.Moderno.Services.Common;

public static class CompetenciasHelper
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

    public static bool ValidarNotasNivel(int notaNivel1, int notaNivel2, List<ComboItem> statusList, out string? mensagemErro)
    {
        mensagemErro = null;
        if ((notaNivel1 == 0 && notaNivel2 > 0) || (notaNivel1 > 0 && notaNivel2 == 0))
        {
            mensagemErro = "É obrigatório selecionar uma nota para cada nível.";
            return false;
        }
        if (notaNivel1 > 0 && notaNivel2 > 0)
        {
            var pesoDdl1 = statusList.FirstOrDefault(x => x.Value == notaNivel1.ToString())?.AdditionalData.TryGetValue("Peso", out var peso1) == true ? Convert.ToInt32(peso1) : 0;
            var pesoDdl2 = statusList.FirstOrDefault(x => x.Value == notaNivel2.ToString())?.AdditionalData.TryGetValue("Peso", out var peso2) == true ? Convert.ToInt32(peso2) : 0;
            if (pesoDdl2 > pesoDdl1)
            {
                mensagemErro = "Existem Avaliações de Competências inconsistentes. A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).";
                return false;
            }
        }
        return true;
    }

    public static bool ValidarNotaNaoSeAplica(int notaNivel1, int notaNivel2, int idNotaNaoSeAplica, out string? mensagemErro)
    {
        mensagemErro = null;
        if (notaNivel1 == idNotaNaoSeAplica && notaNivel2 != idNotaNaoSeAplica)
        {
            mensagemErro = "ATENÇÃO ! Avaliação de Competência com inconsistência. (Detalhe: quando a nota de Competência do Nivel Atual for = Não Se Aplica, o Próximo Nivel deve ser Não Se Aplica). O Sistema ajustou sua avaliação. Favor revalidar sua avaliação !!";
            return false;
        }
        return true;
    }

    public static bool PilarVazioPorSubcompetencia(List<CompetenciaModel> listaRespostas, string subCompetencia, int idNotaNaoSeAplica)
    {
        var competenciasPilar = listaRespostas.Where(r => r.SubCompetencia == subCompetencia && r.IdModo != 2).ToList();
        var respostasVaziasNivel1 = competenciasPilar.Where(r => r.notaValidaNivel1 == idNotaNaoSeAplica).ToList();
        var respostasVaziasNivel2 = competenciasPilar.Where(r => r.notaValidaNivel2 == idNotaNaoSeAplica).ToList();
        if (competenciasPilar.Count == respostasVaziasNivel1.Count || competenciasPilar.Count == respostasVaziasNivel2.Count)
        {
            return true;
        }
        return false;
    }

    public static string NotaIndisponivelMsg(bool visivelEtapaAtual, bool visivelNivel)
    {
        return visivelEtapaAtual && visivelNivel ? string.Empty : "Nota não disponível nesta etapa.";
    }
}

public class CompetenciaModel
{
    public int IdCompetencia { get; set; }
    public string SubCompetencia { get; set; } = string.Empty;
    public int IdModo { get; set; }
    public int notaValidaNivel1 { get; set; }
    public int notaValidaNivel2 { get; set; }
    public object? ddl1 { get; set; }
    public object? ddl2 { get; set; }
}
