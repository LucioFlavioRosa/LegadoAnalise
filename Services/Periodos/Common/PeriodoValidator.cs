using System;
using Peers.Moderno.Models;

namespace Services.Periodos.Common;

public interface IPeriodoValidator
{
    bool ValidarPeriodo(PERIODOSAVALIACOES periodo, out string mensagemErro);
}

public class PeriodoValidator : IPeriodoValidator
{
    public bool ValidarPeriodo(PERIODOSAVALIACOES periodo, out string mensagemErro)
    {
        mensagemErro = string.Empty;
        if (periodo.IdEmpresa == 0)
            mensagemErro += "Selecione a Empresa, ";
        if (!periodo.DataInicio.HasValue)
            mensagemErro += "Informe o Campo Data de Início, ";
        if (!periodo.DataFim.HasValue)
            mensagemErro += "Informe o Campo Data de Término, ";
        if (string.IsNullOrWhiteSpace(periodo.Codigo))
            mensagemErro += "Informe o Campo Código NSEM-YYYY, ";
        if (periodo.DataInicio.HasValue && periodo.DataFim.HasValue && periodo.DataInicio > periodo.DataFim)
            mensagemErro += "A Data Início é maior que a Data Término, ";
        if (!string.IsNullOrEmpty(mensagemErro))
            return false;
        return true;
    }
}
