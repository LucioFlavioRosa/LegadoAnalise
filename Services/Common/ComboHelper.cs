namespace Peers.Moderno.Services.Common;

public static class ComboHelper
{
    public static List<string> GetTiposAvaliacao()
    {
        return new List<string> { "[Selecionar]", "desempenho" };
    }

    public static List<string> GetEscopos()
    {
        return new List<string> { "[Selecionar]", "projeto" };
    }

    public static List<ComboItem> GetTiposAvaliacaoItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "desempenho", Text = "desempenho" }
        };
    }

    public static List<ComboItem> GetEscoposItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "projeto", Text = "projeto" }
        };
    }

    public static List<ComboItem> GetStatusItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
    }

    public static List<ComboItem> GetStatusProjetoItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
    }

    public static List<ComboItem> GetGenericStatusItems(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        });
        
        return items;
    }

    public static List<ComboItem> CreateComboFromList<T>(IEnumerable<T> items, Func<T, string> valueSelector, Func<T, string> textSelector, bool includeSelecionar = true)
    {
        var comboItems = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            comboItems.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        comboItems.AddRange(items.Select(item => new ComboItem
        {
            Value = valueSelector(item),
            Text = textSelector(item)
        }));
        
        return comboItems;
    }

    public static List<ComboItem> CreateComboFromDictionary(Dictionary<string, string> items, bool includeSelecionar = true)
    {
        var comboItems = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            comboItems.Add(new ComboItem { Value = "", Text = "[Selecionar]" });
        }
        
        comboItems.AddRange(items.Select(kvp => new ComboItem
        {
            Value = kvp.Key,
            Text = kvp.Value
        }));
        
        return comboItems;
    }

    public static ComboItem GetDefaultSelectionItem()
    {
        return new ComboItem { Value = "", Text = "[Selecionar]" };
    }

    public static bool IsValidSelection(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "";
    }

    public static string GetSelectedText(List<ComboItem> items, string? selectedValue)
    {
        if (string.IsNullOrEmpty(selectedValue))
            return "[Selecionar]";
            
        var item = items.FirstOrDefault(i => i.Value == selectedValue);
        return item?.Text ?? "[Selecionar]";
    }

    // Métodos específicos para SubCompetências
    public static List<ComboItem> GetSubCompetenciasStatusItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
    }

    public static List<ComboItem> GetTiposAvaliacaoSubCompetencias()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "desempenho", Text = "Desempenho" },
            new ComboItem { Value = "lideranca", Text = "Liderança" }
        };
    }

    public static string GetStatusText(bool ativo)
    {
        return ativo ? "Ativo" : "Inativo";
    }

    public static string GetStatusBadgeClass(bool ativo)
    {
        return ativo ? "badge-success" : "badge-secondary";
    }

    // Novos métodos específicos para AutoAvaliação
    public static List<ComboItem> GetStatusAvaliacaoItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "nao_iniciado", Text = "Não iniciado" },
            new ComboItem { Value = "em_andamento", Text = "Em andamento" },
            new ComboItem { Value = "concluido", Text = "Concluído" },
            new ComboItem { Value = "finalizado", Text = "Finalizado" }
        };
    }

    public static List<ComboItem> CreateProjetosCombo<T>(IEnumerable<T> projetos, Func<T, string> idSelector, Func<T, string> nomeSelector)
    {
        return CreateComboFromList(projetos, idSelector, nomeSelector, true);
    }

    public static List<ComboItem> CreateClientesCombo<T>(IEnumerable<T> clientes, Func<T, string> idSelector, Func<T, string> nomeSelector)
    {
        return CreateComboFromList(clientes, idSelector, nomeSelector, true);
    }

    public static List<ComboItem> CreatePeriodosCombo<T>(IEnumerable<T> periodos, Func<T, string> idSelector, Func<T, string> periodoSelector)
    {
        return CreateComboFromList(periodos, idSelector, periodoSelector, true);
    }

    public static List<ComboItem> GetEtapasAvaliacaoItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "nao_iniciada", Text = "Não Iniciada" },
            new ComboItem { Value = "auto_avaliacao", Text = "Em Auto-avaliação" },
            new ComboItem { Value = "avaliacao_cegas", Text = "Em Av. às Cegas" },
            new ComboItem { Value = "avaliacao_gestor", Text = "Em Av. Gestor" },
            new ComboItem { Value = "feedback", Text = "Em Feedback" },
            new ComboItem { Value = "avaliacao_mentor", Text = "Em Cons. Mentor" },
            new ComboItem { Value = "finalizada", Text = "Finalizada" }
        };
    }

    public static string GetEtapaAvaliacaoText(string? etapa)
    {
        return etapa switch
        {
            "nao_iniciada" => "Não Iniciada",
            "auto_avaliacao" => "Em Auto-avaliação",
            "avaliacao_cegas" => "Em Av. às Cegas",
            "avaliacao_gestor" => "Em Av. Gestor",
            "feedback" => "Em Feedback",
            "avaliacao_mentor" => "Em Cons. Mentor",
            "finalizada" => "Finalizada",
            _ => "Não Iniciada"
        };
    }

    public static string GetStatusAvaliacaoText(string? status)
    {
        return status switch
        {
            "nao_iniciado" => "Não iniciado",
            "em_andamento" => "Em andamento",
            "concluido" => "Concluído",
            "finalizado" => "Finalizado",
            _ => "Não iniciado"
        };
    }

    public static string GetStatusAvaliacaoBadgeClass(string? status)
    {
        return status switch
        {
            "nao_iniciado" => "badge-secondary",
            "em_andamento" => "badge-warning",
            "concluido" => "badge-success",
            "finalizado" => "badge-primary",
            _ => "badge-secondary"
        };
    }

    public static List<ComboItem> GetRotuloBotaoItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "iniciar", Text = "Iniciar Avaliação" },
            new ComboItem { Value = "continuar", Text = "Continuar Avaliação" },
            new ComboItem { Value = "ver", Text = "Ver Avaliação" },
            new ComboItem { Value = "finalizar", Text = "Finalizar Avaliação" }
        };
    }

    public static string GetRotuloBotaoText(string? rotulo)
    {
        return rotulo switch
        {
            "iniciar" => "Iniciar Avaliação",
            "continuar" => "Continuar Avaliação",
            "ver" => "Ver Avaliação",
            "finalizar" => "Finalizar Avaliação",
            _ => "Iniciar Avaliação"
        };
    }

    public static string GetRotuloBotaoCssClass(string? rotulo)
    {
        return rotulo switch
        {
            "iniciar" => "btn btn-success",
            "continuar" => "btn btn-warning",
            "ver" => "btn btn-info",
            "finalizar" => "btn btn-danger",
            _ => "btn btn-success"
        };
    }

    public static bool ShouldShowFinalizarButton(string? status, string? etapa)
    {
        return status == "em_andamento" && 
               (etapa == "auto_avaliacao" || etapa == "avaliacao_cegas");
    }

    public static bool IsAvaliacaoLiberada(string? status)
    {
        return !string.IsNullOrEmpty(status) && status != "nao_iniciado";
    }

    public static List<ComboItem> GetTiposAvaliacaoAutoAvaliacao()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "desempenho", Text = "Desempenho" },
            new ComboItem { Value = "lideranca", Text = "Liderança" }
        };
    }

    public static List<ComboItem> GetEscoposAutoAvaliacao()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "projeto", Text = "Projeto" },
            new ComboItem { Value = "lider", Text = "Líder" },
            new ComboItem { Value = "backoffice", Text = "Backoffice" }
        };
    }
}

public class ComboItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsSelected { get; set; } = false;
    public bool IsDisabled { get; set; } = false;
    public string CssClass { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
}