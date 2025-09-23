using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Dashboard.Common;

public interface IPeriodoUtil
{
    string FormatarPeriodo(Periodo periodo);
    string FormatarPeriodoCompleto(Periodo periodo);
    bool PeriodoEstaAtivo(Periodo periodo);
    bool PeriodoEstaVencido(Periodo periodo);
    TimeSpan TempoRestantePeriodo(Periodo periodo);
}