using Peers.Moderno.Services.Common;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Avaliacoes.Common;

public static class ComboHelperAvaliacao
{
    public static List<ComboItem> CreateProjetosCombo(List<Projeto> projetos, bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(projetos.Select(p => new ComboItem
        {
            Value = p.Id.ToString(),
            Text = p.Nome
        }));
        
        return items;
    }

    public static List<ComboItem> CreateClientesCombo(List<Cliente> clientes, bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(clientes.Select(c => new ComboItem
        {
            Value = c.IdCliente.ToString(),
            Text = c.Nome
        }));
        
        return items;
    }

    public static List<ComboItem> CreatePeriodosCombo(List<PeriodoAvaliacao> periodos, bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(periodos.Select(p => new ComboItem
        {
            Value = p.Id.ToString(),
            Text = p.Nome
        }));
        
        return items;
    }

    public static List<ComboItem> CreateStatusAvaliacaoCombo(List<StatusAvaliacao> statusList, bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(statusList.Select(s => new ComboItem
        {
            Value = s.Id.ToString(),
            Text = s.Nome
        }));
        
        return items;
    }

    public static List<ComboItem> CreateStatusProjetoCombo(List<ProjetoStatus> statusList, bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(statusList.Select(s => new ComboItem
        {
            Value = s.Id.ToString(),
            Text = s.Nome
        }));
        
        return items;
    }

    public static List<ComboItem> CreateAssociadosCombo(List<Associado> associados, bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(associados.Select(a => new ComboItem
        {
            Value = a.Id.ToString(),
            Text = a.Nome
        }));
        
        return items;
    }

    public static List<ComboItem> CreateCargosCombo(List<Cargo> cargos, bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(cargos.Select(c => new ComboItem
        {
            Value = c.IdCargo.ToString(),
            Text = c.Nome
        }));
        
        return items;
    }

    public static List<ComboItem> CreateTiposAvaliacaoCombo(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "desempenho", Text = "Desempenho" },
            new ComboItem { Value = "lideranca", Text = "Liderança" }
        });
        
        return items;
    }

    public static List<ComboItem> CreateEscoposCombo(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "projeto", Text = "Projeto" },
            new ComboItem { Value = "lider", Text = "Líder" },
            new ComboItem { Value = "backoffice", Text = "Back Office" }
        });
        
        return items;
    }

    public static List<ComboItem> CreateEtapasAvaliacaoCombo(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "nao_iniciada", Text = "Não Iniciada" },
            new ComboItem { Value = "auto_avaliacao", Text = "Auto Avaliação" },
            new ComboItem { Value = "avaliacao_cegas", Text = "Avaliação às Cegas" },
            new ComboItem { Value = "avaliacao_gestor", Text = "Avaliação do Gestor" },
            new ComboItem { Value = "feedback", Text = "Feedback" },
            new ComboItem { Value = "avaliacao_mentor", Text = "Avaliação do Mentor" },
            new ComboItem { Value = "finalizada", Text = "Finalizada" }
        });
        
        return items;
    }

    public static string GetEtapaDescricao(string etapa)
    {
        return etapa switch
        {
            "nao_iniciada" => "Não Iniciada",
            "auto_avaliacao" => "Em Auto-avaliação",
            "avaliacao_cegas" => "Em Avaliação às Cegas",
            "avaliacao_gestor" => "Em Avaliação do Gestor",
            "feedback" => "Em Feedback",
            "avaliacao_mentor" => "Em Avaliação do Mentor",
            "finalizada" => "Finalizada",
            _ => "Desconhecida"
        };
    }

    public static string GetStatusBadgeClass(string status)
    {
        return status?.ToLower() switch
        {
            "não iniciado" => "badge badge-secondary",
            "em andamento" => "badge badge-warning",
            "concluído" => "badge badge-success",
            "finalizada" => "badge badge-success",
            "pausado" => "badge badge-info",
            "cancelado" => "badge badge-danger",
            _ => "badge badge-light"
        };
    }

    public static string GetEtapaBadgeClass(string etapa)
    {
        return etapa?.ToLower() switch
        {
            "não iniciada" or "nao_iniciada" => "badge badge-secondary",
            "auto_avaliacao" or "em auto-avaliação" => "badge badge-primary",
            "avaliacao_cegas" or "em avaliação às cegas" => "badge badge-info",
            "avaliacao_gestor" or "em avaliação do gestor" => "badge badge-warning",
            "feedback" or "em feedback" => "badge badge-dark",
            "avaliacao_mentor" or "em avaliação do mentor" => "badge badge-purple",
            "finalizada" => "badge badge-success",
            _ => "badge badge-light"
        };
    }

    public static bool IsEtapaEditavel(string etapa)
    {
        var etapasEditaveis = new[] { "nao_iniciada", "auto_avaliacao", "avaliacao_cegas", "avaliacao_gestor" };
        return etapasEditaveis.Contains(etapa?.ToLower());
    }

    public static bool IsAvaliacaoFinalizavel(string etapa, bool temCompetencias, bool temPerformances)
    {
        return etapa?.ToLower() == "auto_avaliacao" && temCompetencias && temPerformances;
    }
}