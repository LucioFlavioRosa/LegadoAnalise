using System;
using System.Collections.Generic;
using System.Linq;

namespace Peers.Moderno.Services.Common;

public static class ValidationHelper
{
    public static bool IsRequired(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsRequired(int? value)
    {
        return value.HasValue && value.Value > 0;
    }

    public static bool IsRequired<T>(IEnumerable<T>? collection)
    {
        return collection != null && collection.Any();
    }

    public static bool IsValidSelection(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "";
    }

    public static bool ValidarNotasObrigatorias(int notaNivel1, int notaNivel2, int idNotaSelecionar = 0)
    {
        return (notaNivel1 > idNotaSelecionar && notaNivel2 > idNotaSelecionar) || (notaNivel1 == idNotaSelecionar && notaNivel2 == idNotaSelecionar);
    }

    public static bool ValidarNotasConsistentes(int notaNivel1, int notaNivel2, List<ComboItem> notas, int idNotaNaoSeAplica = 5)
    {
        return CompetenciaHelper.ValidarNotasConsistentes(notaNivel1, notaNivel2, notas, idNotaNaoSeAplica);
    }

    public static bool ValidarPilarVazio(List<int> notasNivel, int idNotaNaoSeAplica = 5)
    {
        return CompetenciaHelper.PilarEstaVazio(notasNivel, idNotaNaoSeAplica);
    }

    public static bool ValidarPilarPossuiNotaMensuravel(List<int> notasNivel, int idNotaNaoSeAplica = 5)
    {
        return CompetenciaHelper.PilarPossuiNotaMensuravel(notasNivel, idNotaNaoSeAplica);
    }

    public static string? GetValidationMessage(string key, IDictionary<string, string>? messages)
    {
        if (messages == null) return null;
        if (messages.ContainsKey(key)) return messages[key];
        return null;
    }
}
