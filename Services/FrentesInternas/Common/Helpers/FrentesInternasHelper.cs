using Services.FrentesInternas.Common.Models;
using Services.Common;

namespace Services.FrentesInternas.Common.Helpers;

public static class FrentesInternasHelper
{
    public static string GetStatusBadgeClass(bool validado)
    {
        return validado ? "badge-success" : "badge-secondary";
    }

    public static string GetNotaDescricao(List<ComboItem> notas, int idNota)
    {
        var nota = notas.FirstOrDefault(x => x.Value == idNota.ToString());
        return nota?.Text ?? "[Selecionar]";
    }

    public static bool IsNotaSelecionada(int idNota)
    {
        return idNota > 0;
    }

    public static string GetComentariosPlaceholder(bool enabled)
    {
        return enabled ? "Digite seu comentário" : "Comentários bloqueados para este período";
    }
}