using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Dashboard.Common;

public class PeriodoUtil : IPeriodoUtil
{
    public string FormatarPeriodo(Periodo periodo)
    {
        if (periodo?.DataInicio == null || periodo?.DataFim == null)
            return "Período não definido";

        return $"{periodo.DataInicio.Value:dd/MM/yyyy} - {periodo.DataFim.Value:dd/MM/yyyy}";
    }

    public string FormatarPeriodoCompleto(Periodo periodo)
    {
        if (periodo?.DataInicio == null || periodo?.DataFim == null)
            return "Período não definido";

        var status = PeriodoEstaAtivo(periodo) ? "Ativo" : 
                    PeriodoEstaVencido(periodo) ? "Vencido" : "Futuro";

        return $"{FormatarPeriodo(periodo)} ({status})";
    }

    public bool PeriodoEstaAtivo(Periodo periodo)
    {
        if (periodo?.DataInicio == null || periodo?.DataFim == null)
            return false;

        var hoje = DateTime.Now.Date;
        return periodo.DataInicio.Value.Date <= hoje && periodo.DataFim.Value.Date >= hoje;
    }

    public bool PeriodoEstaVencido(Periodo periodo)
    {
        if (periodo?.DataFim == null)
            return false;

        return periodo.DataFim.Value.Date < DateTime.Now.Date;
    }

    public TimeSpan TempoRestantePeriodo(Periodo periodo)
    {
        if (periodo?.DataFim == null)
            return TimeSpan.Zero;

        var diferenca = periodo.DataFim.Value.Date - DateTime.Now.Date;
        return diferenca > TimeSpan.Zero ? diferenca : TimeSpan.Zero;
    }
}