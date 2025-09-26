using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.FeedbackPerformance.Common;

public static class FeedbackPerformanceComboHelper
{
    public static List<ComboItem> GetNotasFeedbackItems(bool includeSelecionar = true)
    {
        // Reutiliza lógica de ComboHelper global, mas permite customização futura
        return ComboHelper.GetNotasPerformanceItems(includeSelecionar);
    }

    public static List<ComboItem> GetAbrangenciaItems()
    {
        // Reutiliza abrangências do ComboHelper global
        return ComboHelper.GetAbrangenciaPerformanceItems();
    }

    public static ComboItem GetDefaultSelectionItem()
    {
        return ComboHelper.GetDefaultSelectionItem();
    }

    public static string GetSelectedNotaText(List<ComboItem> items, string? selectedValue)
    {
        return ComboHelper.GetSelectedText(items, selectedValue);
    }

    public static string GetStatusText(int status)
    {
        return ComboHelper.GetStatusText(status);
    }
}
