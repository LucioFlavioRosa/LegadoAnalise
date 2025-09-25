using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.AutoAvaliacao.Common;

public interface IValidationHelper
{
    ValidationResult ValidarParametrosObrigatorios(int? idProjeto, int? idAssociado, int? idPeriodo);
    ValidationResult ValidarAvaliacaoCompleta(List<int?> notasCompetencia, List<int?> notasPerformance, bool requireAllFilled = true);
    ValidationResult ValidarConsistenciaNotas(List<(int? nivel1, int? nivel2)> paresNotas);
    ValidationResult ValidarPilaresPreenchidos(Dictionary<string, List<int?>> notasPorPilar);
    ValidationResult ValidarPermissaoEdicao(bool isFinalized, bool hasPermission);
    ValidationResult ValidarTempoRestante(DateTime? dataLimite);
    string GetMensagemValidacao(string chave, params object[] parametros);
    bool IsValidationEnabled();
}

public class ValidationHelper : IValidationHelper
{
    private readonly IConfiguration _configuration;
    private readonly ITelemetryService _telemetryService;
    private readonly INotaHelper _notaHelper;

    public ValidationHelper(
        IConfiguration configuration,
        ITelemetryService telemetryService,
        INotaHelper notaHelper)
    {
        _configuration = configuration;
        _telemetryService = telemetryService;
        _notaHelper = notaHelper;
    }

    public ValidationResult ValidarParametrosObrigatorios(int? idProjeto, int? idAssociado, int? idPeriodo)
    {
        var errors = new List<string>();

        if (!idProjeto.HasValue || idProjeto.Value <= 0)
        {
            errors.Add(GetMensagemValidacao("ProjetoObrigatorio"));
        }

        if (!idAssociado.HasValue || idAssociado.Value <= 0)
        {
            errors.Add(GetMensagemValidacao("AssociadoObrigatorio"));
        }

        if (!idPeriodo.HasValue || idPeriodo.Value <= 0)
        {
            errors.Add(GetMensagemValidacao("PeriodoObrigatorio"));
        }

        if (errors.Any())
        {
            _telemetryService.TrackEvent("ValidationError_ParametrosObrigatorios", new Dictionary<string, string>
            {
                { "ErrorCount", errors.Count.ToString() },
                { "IdProjeto", idProjeto?.ToString() ?? "null" },
                { "IdAssociado", idAssociado?.ToString() ?? "null" },
                { "IdPeriodo", idPeriodo?.ToString() ?? "null" }
            });

            return ValidationResult.Error(string.Join("; ", errors));
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidarAvaliacaoCompleta(List<int?> notasCompetencia, List<int?> notasPerformance, bool requireAllFilled = true)
    {
        var errors = new List<string>();

        if (requireAllFilled)
        {
            if (!_notaHelper.ValidarPreenchimentoCompleto(notasCompetencia, true))
            {
                errors.Add(GetMensagemValidacao("CompetenciasNaoPreenchidas"));
            }

            if (!_notaHelper.ValidarPreenchimentoCompleto(notasPerformance, true))
            {
                errors.Add(GetMensagemValidacao("PerformancesNaoPreenchidas"));
            }
        }

        if (!notasCompetencia.Any())
        {
            errors.Add(GetMensagemValidacao("CompetenciasNaoParametrizadas"));
        }

        if (!notasPerformance.Any())
        {
            errors.Add(GetMensagemValidacao("PerformancesNaoParametrizadas"));
        }

        if (errors.Any())
        {
            _telemetryService.TrackEvent("ValidationError_AvaliacaoIncompleta", new Dictionary<string, string>
            {
                { "ErrorCount", errors.Count.ToString() },
                { "CompetenciasCount", notasCompetencia.Count.ToString() },
                { "PerformancesCount", notasPerformance.Count.ToString() }
            });

            return ValidationResult.Error(string.Join("; ", errors));
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidarConsistenciaNotas(List<(int? nivel1, int? nivel2)> paresNotas)
    {
        var inconsistencias = new List<string>();
        var ajustesNecessarios = new List<string>();

        foreach (var (nivel1, nivel2) in paresNotas)
        {
            if (!_notaHelper.ValidarNotasConsistencia(nivel1, nivel2))
            {
                inconsistencias.Add($"Nota Nível 1: {_notaHelper.GetNotaTexto(nivel1)}, Nível 2: {_notaHelper.GetNotaTexto(nivel2)}");
            }

            if (_notaHelper.IsNotaNaoSeAplica(nivel1) && !_notaHelper.IsNotaNaoSeAplica(nivel2))
            {
                ajustesNecessarios.Add($"Ajuste necessário: Nível 1 = Não se aplica, Nível 2 deve ser Não se aplica");
            }
        }

        if (inconsistencias.Any())
        {
            _telemetryService.TrackEvent("ValidationError_NotasInconsistentes", new Dictionary<string, string>
            {
                { "InconsistenciasCount", inconsistencias.Count.ToString() },
                { "AjustesCount", ajustesNecessarios.Count.ToString() }
            });

            var errorMessage = GetMensagemValidacao("NotasInconsistentes");
            if (ajustesNecessarios.Any())
            {
                errorMessage += " " + GetMensagemValidacao("NotaNaoSeAplicaInconsistente");
            }

            return ValidationResult.Error(errorMessage);
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidarPilaresPreenchidos(Dictionary<string, List<int?>> notasPorPilar)
    {
        var pilaresVazios = new List<string>();

        foreach (var pilar in notasPorPilar)
        {
            var notasMensuravelNivel1 = pilar.Value.Count(n => _notaHelper.IsNotaValida(n) && !_notaHelper.IsNotaNaoSeAplica(n));
            
            if (notasMensuravelNivel1 == 0)
            {
                pilaresVazios.Add(pilar.Key);
            }
        }

        if (pilaresVazios.Any())
        {
            _telemetryService.TrackEvent("ValidationError_PilaresVazios", new Dictionary<string, string>
            {
                { "PilaresVaziosCount", pilaresVazios.Count.ToString() },
                { "PilaresVazios", string.Join(", ", pilaresVazios) }
            });

            return ValidationResult.Error(GetMensagemValidacao("PilarVazio"));
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidarPermissaoEdicao(bool isFinalized, bool hasPermission)
    {
        if (isFinalized)
        {
            return ValidationResult.Error("Avaliação já foi finalizada e não pode ser editada.");
        }

        if (!hasPermission)
        {
            return ValidationResult.Error(GetMensagemValidacao("PermissaoNegada"));
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidarTempoRestante(DateTime? dataLimite)
    {
        if (!dataLimite.HasValue)
        {
            return ValidationResult.Success();
        }

        if (DateTime.Now > dataLimite.Value)
        {
            return ValidationResult.Warning("Prazo da avaliação expirado. Utilize o período de compensação se disponível.");
        }

        var tempoRestante = dataLimite.Value - DateTime.Now;
        if (tempoRestante.TotalHours < 24)
        {
            return ValidationResult.Warning($"Atenção: Restam apenas {tempoRestante.TotalHours:F1} horas para finalizar a avaliação.");
        }

        return ValidationResult.Success();
    }

    public string GetMensagemValidacao(string chave, params object[] parametros)
    {
        var mensagem = _configuration[$"AutoAvaliacao:ValidationMessages:{chave}"] ?? chave;
        
        if (parametros.Any())
        {
            try
            {
                return string.Format(mensagem, parametros);
            }
            catch
            {
                return mensagem;
            }
        }
        
        return mensagem;
    }

    public bool IsValidationEnabled()
    {
        return _configuration.GetValue("AutoAvaliacao:EnableValidation", true);
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public bool IsWarning { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new List<string>();
    public List<string> Warnings { get; private set; } = new List<string>();

    private ValidationResult() { }

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Error(string message)
    {
        return new ValidationResult
        {
            IsValid = false,
            Message = message,
            Errors = new List<string> { message }
        };
    }

    public static ValidationResult Warning(string message)
    {
        return new ValidationResult
        {
            IsValid = true,
            IsWarning = true,
            Message = message,
            Warnings = new List<string> { message }
        };
    }

    public static ValidationResult Multiple(List<string> errors, List<string> warnings = null)
    {
        return new ValidationResult
        {
            IsValid = !errors.Any(),
            IsWarning = warnings?.Any() == true,
            Message = errors.Any() ? string.Join("; ", errors) : (warnings?.Any() == true ? string.Join("; ", warnings) : string.Empty),
            Errors = errors ?? new List<string>(),
            Warnings = warnings ?? new List<string>()
        };
    }
}