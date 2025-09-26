using System;
using System.Collections.Generic;

namespace Services.Common;

public interface IPerformanceHelper
{
    string TruncaTexto(string texto, int qtdCaracter);
    string ObterTempoAssociado(int idAssociado, PerformanceTempoTipo tipo);
    List<NotaItemDto> GetNotasPerformanceItems(bool includeSelecionar = true);
}

public class PerformanceHelper : IPerformanceHelper
{
    public string TruncaTexto(string texto, int qtdCaracter)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;
        return texto.Length > qtdCaracter ? texto.Substring(0, qtdCaracter) + "..." : texto;
    }

    public string ObterTempoAssociado(int idAssociado, PerformanceTempoTipo tipo)
    {
        // Implementação mockada, substituir por lógica real se necessário
        return tipo == PerformanceTempoTipo.Cargo ? "2 anos" : "1 ano";
    }

    public List<NotaItemDto> GetNotasPerformanceItems(bool includeSelecionar = true)
    {
        var items = new List<NotaItemDto>();
        if (includeSelecionar)
        {
            items.Add(new NotaItemDto { Value = "0", Text = "[Selecionar]", IsDisabled = true, AdditionalData = new Dictionary<string, object> { ["Peso"] = 0 } });
        }
        items.AddRange(new List<NotaItemDto>
        {
            new NotaItemDto { Value = "1", Text = "1", AdditionalData = new Dictionary<string, object> { ["Peso"] = 1 } },
            new NotaItemDto { Value = "2", Text = "2", AdditionalData = new Dictionary<string, object> { ["Peso"] = 2 } },
            new NotaItemDto { Value = "3", Text = "3", AdditionalData = new Dictionary<string, object> { ["Peso"] = 3 } },
            new NotaItemDto { Value = "4", Text = "4", AdditionalData = new Dictionary<string, object> { ["Peso"] = 4 } },
            new NotaItemDto { Value = "5", Text = "N/A", AdditionalData = new Dictionary<string, object> { ["Peso"] = 5 } }
        });
        return items;
    }
}

public enum PerformanceTempoTipo
{
    Cargo,
    Peers
}
