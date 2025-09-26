using System;
using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Avaliacoes.Common;

public static class AvaliacaoUtils
{
    public static string GetEtapaDescricao(string? posicaoFluxo, string tipoAvaliacao)
    {
        return posicaoFluxo switch
        {
            "AVM" => "Não iniciada",
            "AAV" => "Autoavaliação",
            "ACE" => "Av. as Cegas",
            "AGE" => tipoAvaliacao == "desempenho" ? "Av. Gestor" : "Av. Liderança",
            "FED" => "Feedback",
            "AME" => "Mentoria",
            "AFI" => "Finalizada",
            _ => "Não iniciada"
        };
    }

    public static (string rotulo, string exibirBotao) GetRotuloBotao(string posicaoFluxo, int idAssociado, int idAvaliador, int idGestor, int associadoLogado, string tipoAvaliacao)
    {
        string rotulo = "Responder";
        string exibirBotao = "hidden";
        switch (posicaoFluxo)
        {
            case "AVM":
            case "AAV":
                if (idAssociado == associadoLogado)
                    exibirBotao = "";
                break;
            case "ACE":
                if (idAvaliador == associadoLogado || idGestor == associadoLogado)
                    exibirBotao = "";
                else if (idAssociado == associadoLogado)
                {
                    rotulo = "Ver respostas";
                    exibirBotao = "";
                }
                break;
            case "AGE":
                if (tipoAvaliacao == "lideranca")
                {
                    if (idAssociado == associadoLogado || idGestor != associadoLogado)
                        break;
                }
                if (idAvaliador == associadoLogado || idGestor == associadoLogado)
                    exibirBotao = "";
                else if (idAssociado == associadoLogado)
                {
                    rotulo = "Ver respostas";
                    exibirBotao = "";
                }
                break;
            case "FED":
                if (idAvaliador == associadoLogado || idGestor == associadoLogado)
                    exibirBotao = "";
                else if (idAssociado == associadoLogado)
                {
                    rotulo = "Ver respostas";
                    exibirBotao = "";
                }
                break;
            case "AME":
                if (idAvaliador == associadoLogado)
                {
                    exibirBotao = "";
                    rotulo = "Mentoria";
                }
                else if (idAssociado == associadoLogado || idAvaliador == associadoLogado || idGestor == associadoLogado)
                {
                    rotulo = "Ver respostas";
                    exibirBotao = "";
                }
                break;
            case "AFI":
                if (tipoAvaliacao == "lideranca")
                {
                    if (idAssociado == associadoLogado || idGestor != associadoLogado)
                        break;
                }
                rotulo = "Ver respostas";
                exibirBotao = "";
                break;
        }
        return (rotulo, exibirBotao);
    }

    public static bool ExibirBotaoFinalizar(string posicaoFluxo, int idAssociado, int associadoLogado)
    {
        return (posicaoFluxo == "AAV" || posicaoFluxo == "ACE") && idAssociado == associadoLogado;
    }

    public static string GetStatusProjetoText(int status)
    {
        return status == 1 ? "Ativo" : "Inativo";
    }

    public static string GetDataFormatada(DateTime? data)
    {
        return data.HasValue ? data.Value.ToString("dd/MM/yyyy") : string.Empty;
    }
}