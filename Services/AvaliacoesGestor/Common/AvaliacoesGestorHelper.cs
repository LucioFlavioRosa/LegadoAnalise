using System;
using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Models;

namespace Services.AvaliacoesGestor.Common;

public static class AvaliacoesGestorHelper
{
    public static List<ComboItem> GetEtapasAvaliacaoItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "PreenchimentoTodas", Text = "Preenchimento Todas" },
            new ComboItem { Value = "PreenchimentoGestor", Text = "Preenchimento Gestor" },
            new ComboItem { Value = "PreenchimentoLiderado", Text = "Preenchimento Liderado" },
            new ComboItem { Value = "FinalizadasTodas", Text = "Finalizadas Todas" },
            new ComboItem { Value = "FinalizadasGestor", Text = "Finalizadas Gestor" },
            new ComboItem { Value = "FinalizadasLiderado", Text = "Finalizadas Liderado" }
        };
    }

    public static string GetStatusDescricao(string status)
    {
        switch (status?.Trim()?.ToLower())
        {
            case "1":
            case "ativo":
                return "Ativo";
            case "0":
            case "inativo":
                return "Inativo";
            default:
                return status ?? "";
        }
    }

    public static string GetEtapaDescricao(string etapaKey)
    {
        return etapaKey switch
        {
            "NaoIniciada" => "Não Iniciada",
            "AutoAvaliacao" => "Em Auto-avaliação",
            "AvaliacaoAsCegas" => "Em Av. às Cegas",
            "AvaliacaoGestor" => "Em Av. Gestor",
            "Feedback" => "Em Feedback",
            "AvaliacaoMentor" => "Em Cons. Mentor",
            "Finalizada" => "Finalizada",
            _ => etapaKey ?? ""
        };
    }

    public static List<ComboItem> GetProjetosCombo(List<Projeto> projetos)
    {
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        if (projetos != null)
        {
            items.AddRange(projetos.Select(p => new ComboItem { Value = p.Id.ToString(), Text = p.Nome }));
        }
        return items;
    }

    public static List<ComboItem> GetClientesCombo(List<Cliente> clientes)
    {
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        if (clientes != null)
        {
            items.AddRange(clientes.Select(c => new ComboItem { Value = c.IdCliente.ToString(), Text = c.Nome }));
        }
        return items;
    }

    public static List<ComboItem> GetPeriodosCombo(List<PERIODOSAVALIACOES> periodos)
    {
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        if (periodos != null)
        {
            items.AddRange(periodos.Select(p => new ComboItem { Value = p.IdPeriodo.ToString(), Text = p.Nome }));
        }
        return items;
    }

    public static List<ComboItem> GetStatusCombo(List<PROJETOSSTATUS> statusList)
    {
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        if (statusList != null)
        {
            items.AddRange(statusList.Select(s => new ComboItem { Value = s.IdStatus.ToString(), Text = s.Status }));
        }
        return items;
    }

    public static string FormatData(DateTime? data)
    {
        return data.HasValue ? data.Value.ToString("dd/MM/yyyy") : string.Empty;
    }

    public static string FormatDataPeriodo(PERIODOSAVALIACOES periodo)
    {
        return periodo?.Nome ?? string.Empty;
    }

    public static string FormatNomeAssociado(Associado associado)
    {
        return associado?.Nome ?? string.Empty;
    }

    public static string GetFotoAssociado(Associado associado)
    {
        return !string.IsNullOrEmpty(associado?.FotoNome) ? associado.FotoNome : "assets/images/users/usernophoto.jpg";
    }

    public static bool IsProjetoSelecionado(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "" && value != "0";
    }

    public static bool IsPeriodoSelecionado(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "" && value != "0";
    }

    public static bool IsClienteSelecionado(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "" && value != "0";
    }

    public static bool IsStatusSelecionado(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "" && value != "0";
    }
}
