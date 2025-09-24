using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Performance.Common;

public interface IPerformanceValidationUtil
{
    Task<PerformanceValidationResult> ValidatePerformanceAsync(Performance performance);
    Task<PerformanceValidationResult> ValidatePerformanceForInsertAsync(Performance performance);
    Task<PerformanceValidationResult> ValidatePerformanceForUpdateAsync(Performance performance);
    Task<PerformanceValidationResult> ValidatePerformanceForDeleteAsync(int performanceId);
    PerformanceValidationResult ValidateRequiredFields(Performance performance);
    PerformanceValidationResult ValidateBusinessRules(Performance performance);
    PerformanceValidationResult ValidateNotasConsistency(Performance performance);
    PerformanceValidationResult ValidateInputsAndNotas(Performance performance);
    bool IsValidAbrangencia(string abrangencia);
    bool IsValidStatus(int status);
}

public class PerformanceValidationUtil : IPerformanceValidationUtil
{
    private readonly ITelemetryService _telemetryService;
    private readonly IPerformanceComboHelper _comboHelper;
    private const int MAX_PERFORMANCE_LENGTH = 500;
    private const int MAX_DESCRICAO_LENGTH = 2000;

    public PerformanceValidationUtil(
        ITelemetryService telemetryService,
        IPerformanceComboHelper comboHelper)
    {
        _telemetryService = telemetryService;
        _comboHelper = comboHelper;
    }

    public async Task<PerformanceValidationResult> ValidatePerformanceAsync(Performance performance)
    {
        try
        {
            var requiredFieldsResult = ValidateRequiredFields(performance);
            if (!requiredFieldsResult.IsValid)
                return requiredFieldsResult;

            var businessRulesResult = ValidateBusinessRules(performance);
            if (!businessRulesResult.IsValid)
                return businessRulesResult;

            var notasConsistencyResult = ValidateNotasConsistency(performance);
            if (!notasConsistencyResult.IsValid)
                return notasConsistencyResult;

            var inputsNotasResult = ValidateInputsAndNotas(performance);
            if (!inputsNotasResult.IsValid)
                return inputsNotasResult;

            return await Task.FromResult(PerformanceValidationResult.Success());
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidatePerformanceAsync" },
                { "Component", "PerformanceValidationUtil" },
                { "PerformanceId", performance?.IdPerformance.ToString() ?? "0" }
            });
            return PerformanceValidationResult.Error("Erro interno na validação");
        }
    }

    public async Task<PerformanceValidationResult> ValidatePerformanceForInsertAsync(Performance performance)
    {
        var baseValidation = await ValidatePerformanceAsync(performance);
        if (!baseValidation.IsValid)
            return baseValidation;

        if (performance.IdPerformance > 0)
        {
            return PerformanceValidationResult.Error("ID deve ser zero para inserção");
        }

        return PerformanceValidationResult.Success();
    }

    public async Task<PerformanceValidationResult> ValidatePerformanceForUpdateAsync(Performance performance)
    {
        var baseValidation = await ValidatePerformanceAsync(performance);
        if (!baseValidation.IsValid)
            return baseValidation;

        if (performance.IdPerformance <= 0)
        {
            return PerformanceValidationResult.Error("ID deve ser maior que zero para alteração");
        }

        return PerformanceValidationResult.Success();
    }

    public async Task<PerformanceValidationResult> ValidatePerformanceForDeleteAsync(int performanceId)
    {
        if (performanceId <= 0)
        {
            return PerformanceValidationResult.Error("ID inválido para exclusão");
        }

        return await Task.FromResult(PerformanceValidationResult.Success());
    }

    public PerformanceValidationResult ValidateRequiredFields(Performance performance)
    {
        if (performance == null)
        {
            return PerformanceValidationResult.Error("Performance não pode ser nula");
        }

        if (performance.IdCargo <= 0)
        {
            return PerformanceValidationResult.Error("Selecione o campo Cargo");
        }

        if (string.IsNullOrWhiteSpace(performance.Nome))
        {
            return PerformanceValidationResult.Error("Preencha o campo Performance");
        }

        if (string.IsNullOrWhiteSpace(performance.PerformanceAbaixo))
        {
            return PerformanceValidationResult.Error("Preencha o campo Performance Abaixo");
        }

        if (string.IsNullOrWhiteSpace(performance.PerformanceEsperado))
        {
            return PerformanceValidationResult.Error("Preencha o campo Performance Esperado");
        }

        if (string.IsNullOrWhiteSpace(performance.PerformanceAcima))
        {
            return PerformanceValidationResult.Error("Preencha o campo Performance Acima");
        }

        if (string.IsNullOrWhiteSpace(performance.Abrangencia))
        {
            return PerformanceValidationResult.Error("Selecione a Abrangência");
        }

        return PerformanceValidationResult.Success();
    }

    public PerformanceValidationResult ValidateBusinessRules(Performance performance)
    {
        if (performance.Nome.Length > MAX_PERFORMANCE_LENGTH)
        {
            return PerformanceValidationResult.Error($"Performance deve ter no máximo {MAX_PERFORMANCE_LENGTH} caracteres");
        }

        if (performance.PerformanceAbaixo.Length > MAX_DESCRICAO_LENGTH)
        {
            return PerformanceValidationResult.Error($"Descrição Abaixo deve ter no máximo {MAX_DESCRICAO_LENGTH} caracteres");
        }

        if (performance.PerformanceEsperado.Length > MAX_DESCRICAO_LENGTH)
        {
            return PerformanceValidationResult.Error($"Descrição Esperado deve ter no máximo {MAX_DESCRICAO_LENGTH} caracteres");
        }

        if (performance.PerformanceAcima.Length > MAX_DESCRICAO_LENGTH)
        {
            return PerformanceValidationResult.Error($"Descrição Acima deve ter no máximo {MAX_DESCRICAO_LENGTH} caracteres");
        }

        if (!IsValidAbrangencia(performance.Abrangencia))
        {
            return PerformanceValidationResult.Error("Abrangência inválida");
        }

        return PerformanceValidationResult.Success();
    }

    public PerformanceValidationResult ValidateNotasConsistency(Performance performance)
    {
        // Validar consistência entre inputs e notas padrão
        if (!performance.InputAutoavaliacao && !performance.NotaPadraoAutoAvaliacao.HasValue)
        {
            return PerformanceValidationResult.Error("Selecione a nota padrão para auto avaliação");
        }

        if (!performance.InputAvaliacaoAsCegas && !performance.NotaPadraoAvaliacaoAsCegas.HasValue)
        {
            return PerformanceValidationResult.Error("Selecione a nota padrão para avaliação às cegas");
        }

        if (!performance.InputAvaliacaoGestor && !performance.NotaPadraoAvaliacaoGestor.HasValue)
        {
            return PerformanceValidationResult.Error("Selecione a nota padrão para avaliação do gestor");
        }

        return PerformanceValidationResult.Success();
    }

    public PerformanceValidationResult ValidateInputsAndNotas(Performance performance)
    {
        // Se input está habilitado, não deve ter nota padrão
        if (performance.InputAutoavaliacao && performance.NotaPadraoAutoAvaliacao.HasValue)
        {
            return PerformanceValidationResult.Error("Auto avaliação com input habilitado não deve ter nota padrão");
        }

        if (performance.InputAvaliacaoAsCegas && performance.NotaPadraoAvaliacaoAsCegas.HasValue)
        {
            return PerformanceValidationResult.Error("Avaliação às cegas com input habilitado não deve ter nota padrão");
        }

        if (performance.InputAvaliacaoGestor && performance.NotaPadraoAvaliacaoGestor.HasValue)
        {
            return PerformanceValidationResult.Error("Avaliação do gestor com input habilitado não deve ter nota padrão");
        }

        // Validar se as notas padrão existem (quando especificadas)
        if (performance.NotaPadraoAutoAvaliacao.HasValue && performance.NotaPadraoAutoAvaliacao.Value <= 0)
        {
            return PerformanceValidationResult.Error("Nota padrão para auto avaliação inválida");
        }

        if (performance.NotaPadraoAvaliacaoAsCegas.HasValue && performance.NotaPadraoAvaliacaoAsCegas.Value <= 0)
        {
            return PerformanceValidationResult.Error("Nota padrão para avaliação às cegas inválida");
        }

        if (performance.NotaPadraoAvaliacaoGestor.HasValue && performance.NotaPadraoAvaliacaoGestor.Value <= 0)
        {
            return PerformanceValidationResult.Error("Nota padrão para avaliação do gestor inválida");
        }

        return PerformanceValidationResult.Success();
    }

    public bool IsValidAbrangencia(string abrangencia)
    {
        if (string.IsNullOrWhiteSpace(abrangencia))
            return false;

        var validAbrangencias = _comboHelper.GetAbrangenciaItems().Select(x => x.Value).ToList();
        return validAbrangencias.Contains(abrangencia, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsValidStatus(int status)
    {
        return status == 0 || status == 1;
    }
}

public class PerformanceValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public List<string> Warnings { get; private set; } = new();

    private PerformanceValidationResult() { }

    public static PerformanceValidationResult Success()
    {
        return new PerformanceValidationResult { IsValid = true };
    }

    public static PerformanceValidationResult Error(string errorMessage)
    {
        return new PerformanceValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }

    public static PerformanceValidationResult SuccessWithWarnings(List<string> warnings)
    {
        return new PerformanceValidationResult
        {
            IsValid = true,
            Warnings = warnings
        };
    }

    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
}