using Peers.Moderno.Services.Common;
using Microsoft.Extensions.Configuration;

namespace Peers.Moderno.Services.AutoAvaliacao.Common;

public interface IValidationHelper
{
    ValidationResult ValidarProjetoObrigatorio(int? idProjeto);
    ValidationResult ValidarAssociadoObrigatorio(int? idAssociado);
    ValidationResult ValidarPeriodoObrigatorio(int? idPeriodo);
    ValidationResult ValidarGestorObrigatorio(int? idGestor);
    ValidationResult ValidarTipoAvaliacaoObrigatorio(string? tipoAvaliacao);
    ValidationResult ValidarEscopoObrigatorio(string? escopo);
    ValidationResult ValidarCompetenciasPreenchidas(List<int?> notas);
    ValidationResult ValidarPerformancesPreenchidas(List<int?> notas);
    ValidationResult ValidarPermissaoUsuario(int userProfileId, int minProfileRequired);
    ValidationResult ValidarAvaliacaoFinalizada(bool isFinalizada);
    ValidationResult ValidarPrazosAvaliacao(DateTime? dataInicio, DateTime? dataFim);
    ValidationResult ValidarContextoAvaliacao(int? idProjeto, int? idAssociado, int? idPeriodo);
    List<ValidationResult> ValidarTodosRequisitos(ValidationContext context);
    string GetMensagemValidacao(string chave, params object[] parametros);
}

public class ValidationHelper : IValidationHelper
{
    private readonly IConfiguration _configuration;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IUserContextService _userContextService;

    public ValidationHelper(
        IConfiguration configuration,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService,
        IUserContextService userContextService)
    {
        _configuration = configuration;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
        _userContextService = userContextService;
    }

    public ValidationResult ValidarProjetoObrigatorio(int? idProjeto)
    {
        try
        {
            if (!idProjeto.HasValue || idProjeto.Value <= 0)
            {
                var mensagem = GetMensagemValidacao("ProjetoObrigatorio");
                _messageBoxService.ShowInfo(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarProjetoObrigatorio" },
                { "Component", "ValidationHelper" },
                { "IdProjeto", idProjeto?.ToString() ?? "null" }
            });
            return ValidationResult.Error("Erro interno na validação do projeto");
        }
    }

    public ValidationResult ValidarAssociadoObrigatorio(int? idAssociado)
    {
        try
        {
            if (!idAssociado.HasValue || idAssociado.Value <= 0)
            {
                var mensagem = GetMensagemValidacao("AssociadoObrigatorio");
                _messageBoxService.ShowInfo(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarAssociadoObrigatorio" },
                { "Component", "ValidationHelper" },
                { "IdAssociado", idAssociado?.ToString() ?? "null" }
            });
            return ValidationResult.Error("Erro interno na validação do associado");
        }
    }

    public ValidationResult ValidarPeriodoObrigatorio(int? idPeriodo)
    {
        try
        {
            if (!idPeriodo.HasValue || idPeriodo.Value <= 0)
            {
                var mensagem = GetMensagemValidacao("PeriodoObrigatorio");
                _messageBoxService.ShowInfo(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPeriodoObrigatorio" },
                { "Component", "ValidationHelper" },
                { "IdPeriodo", idPeriodo?.ToString() ?? "null" }
            });
            return ValidationResult.Error("Erro interno na validação do período");
        }
    }

    public ValidationResult ValidarGestorObrigatorio(int? idGestor)
    {
        try
        {
            if (!idGestor.HasValue || idGestor.Value <= 0)
            {
                var mensagem = GetMensagemValidacao("GestorObrigatorio");
                _messageBoxService.ShowInfo(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarGestorObrigatorio" },
                { "Component", "ValidationHelper" },
                { "IdGestor", idGestor?.ToString() ?? "null" }
            });
            return ValidationResult.Error("Erro interno na validação do gestor");
        }
    }

    public ValidationResult ValidarTipoAvaliacaoObrigatorio(string? tipoAvaliacao)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tipoAvaliacao))
            {
                var mensagem = GetMensagemValidacao("TipoAvaliacaoObrigatorio");
                _messageBoxService.ShowInfo(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarTipoAvaliacaoObrigatorio" },
                { "Component", "ValidationHelper" },
                { "TipoAvaliacao", tipoAvaliacao ?? "null" }
            });
            return ValidationResult.Error("Erro interno na validação do tipo de avaliação");
        }
    }

    public ValidationResult ValidarEscopoObrigatorio(string? escopo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(escopo))
            {
                var mensagem = GetMensagemValidacao("EscopoObrigatorio");
                _messageBoxService.ShowInfo(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarEscopoObrigatorio" },
                { "Component", "ValidationHelper" },
                { "Escopo", escopo ?? "null" }
            });
            return ValidationResult.Error("Erro interno na validação do escopo");
        }
    }

    public ValidationResult ValidarCompetenciasPreenchidas(List<int?> notas)
    {
        try
        {
            var notasVazias = notas.Where(n => !n.HasValue || n.Value == 0).Count();
            
            if (notasVazias > 0)
            {
                var mensagem = GetMensagemValidacao("CompetenciasNaoPreenchidas");
                _messageBoxService.ShowError(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarCompetenciasPreenchidas" },
                { "Component", "ValidationHelper" },
                { "TotalNotas", notas.Count.ToString() }
            });
            return ValidationResult.Error("Erro interno na validação das competências");
        }
    }

    public ValidationResult ValidarPerformancesPreenchidas(List<int?> notas)
    {
        try
        {
            var notasVazias = notas.Where(n => !n.HasValue || n.Value == 0).Count();
            
            if (notasVazias > 0)
            {
                var mensagem = GetMensagemValidacao("PerformancesNaoPreenchidas");
                _messageBoxService.ShowError(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPerformancesPreenchidas" },
                { "Component", "ValidationHelper" },
                { "TotalNotas", notas.Count.ToString() }
            });
            return ValidationResult.Error("Erro interno na validação das performances");
        }
    }

    public ValidationResult ValidarPermissaoUsuario(int userProfileId, int minProfileRequired)
    {
        try
        {
            if (userProfileId < minProfileRequired)
            {
                var mensagem = GetMensagemValidacao("PermissaoNegada");
                _messageBoxService.ShowError(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPermissaoUsuario" },
                { "Component", "ValidationHelper" },
                { "UserProfileId", userProfileId.ToString() },
                { "MinProfileRequired", minProfileRequired.ToString() }
            });
            return ValidationResult.Error("Erro interno na validação de permissão");
        }
    }

    public ValidationResult ValidarAvaliacaoFinalizada(bool isFinalizada)
    {
        try
        {
            if (isFinalizada)
            {
                var mensagem = "Avaliação já foi finalizada e não pode ser alterada";
                _messageBoxService.ShowWarning(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarAvaliacaoFinalizada" },
                { "Component", "ValidationHelper" },
                { "IsFinalizada", isFinalizada.ToString() }
            });
            return ValidationResult.Error("Erro interno na validação do status da avaliação");
        }
    }

    public ValidationResult ValidarPrazosAvaliacao(DateTime? dataInicio, DateTime? dataFim)
    {
        try
        {
            var agora = DateTime.Now;

            if (dataInicio.HasValue && agora < dataInicio.Value)
            {
                var mensagem = "Avaliação ainda não foi liberada";
                _messageBoxService.ShowWarning(mensagem);
                return ValidationResult.Error(mensagem);
            }

            if (dataFim.HasValue && agora > dataFim.Value)
            {
                var mensagem = "Prazo para avaliação expirado";
                _messageBoxService.ShowWarning(mensagem);
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPrazosAvaliacao" },
                { "Component", "ValidationHelper" },
                { "DataInicio", dataInicio?.ToString() ?? "null" },
                { "DataFim", dataFim?.ToString() ?? "null" }
            });
            return ValidationResult.Error("Erro interno na validação dos prazos");
        }
    }

    public ValidationResult ValidarContextoAvaliacao(int? idProjeto, int? idAssociado, int? idPeriodo)
    {
        try
        {
            var resultados = new List<ValidationResult>
            {
                ValidarProjetoObrigatorio(idProjeto),
                ValidarAssociadoObrigatorio(idAssociado),
                ValidarPeriodoObrigatorio(idPeriodo)
            };

            var erros = resultados.Where(r => !r.IsValid).ToList();
            
            if (erros.Any())
            {
                var mensagensErro = string.Join("; ", erros.Select(e => e.ErrorMessage));
                return ValidationResult.Error(mensagensErro);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarContextoAvaliacao" },
                { "Component", "ValidationHelper" }
            });
            return ValidationResult.Error("Erro interno na validação do contexto");
        }
    }

    public List<ValidationResult> ValidarTodosRequisitos(ValidationContext context)
    {
        try
        {
            var resultados = new List<ValidationResult>();

            resultados.Add(ValidarContextoAvaliacao(context.IdProjeto, context.IdAssociado, context.IdPeriodo));
            
            if (context.ValidarCompetencias && context.NotasCompetencias != null)
            {
                resultados.Add(ValidarCompetenciasPreenchidas(context.NotasCompetencias));
            }
            
            if (context.ValidarPerformances && context.NotasPerformances != null)
            {
                resultados.Add(ValidarPerformancesPreenchidas(context.NotasPerformances));
            }
            
            if (context.ValidarPermissoes)
            {
                resultados.Add(ValidarPermissaoUsuario(context.UserProfileId, context.MinProfileRequired));
            }
            
            if (context.ValidarPrazos)
            {
                resultados.Add(ValidarPrazosAvaliacao(context.DataInicio, context.DataFim));
            }

            _telemetryService.TrackEvent("ValidationCompleted", new Dictionary<string, string>
            {
                { "TotalValidations", resultados.Count.ToString() },
                { "FailedValidations", resultados.Count(r => !r.IsValid).ToString() },
                { "Context", context.ToString() }
            });

            return resultados;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarTodosRequisitos" },
                { "Component", "ValidationHelper" }
            });
            return new List<ValidationResult> { ValidationResult.Error("Erro interno na validação geral") };
        }
    }

    public string GetMensagemValidacao(string chave, params object[] parametros)
    {
        try
        {
            var mensagem = _configuration[$"AutoAvaliacao:ValidationMessages:{chave}"];
            
            if (string.IsNullOrEmpty(mensagem))
            {
                return GetMensagemPadrao(chave);
            }

            if (parametros.Length > 0)
            {
                return string.Format(mensagem, parametros);
            }

            return mensagem;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetMensagemValidacao" },
                { "Component", "ValidationHelper" },
                { "Chave", chave }
            });
            return GetMensagemPadrao(chave);
        }
    }

    private string GetMensagemPadrao(string chave)
    {
        return chave switch
        {
            "ProjetoObrigatorio" => "É obrigatório a seleção de um Projeto.",
            "AssociadoObrigatorio" => "É obrigatório a seleção de um Associado.",
            "PeriodoObrigatorio" => "É obrigatório a seleção de um Período.",
            "GestorObrigatorio" => "É obrigatório a seleção de um Gestor.",
            "TipoAvaliacaoObrigatorio" => "É obrigatório a seleção de um tipo de avaliação.",
            "EscopoObrigatorio" => "É obrigatório a seleção de um escopo.",
            "CompetenciasNaoPreenchidas" => "É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação",
            "PerformancesNaoPreenchidas" => "É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação",
            "PermissaoNegada" => "Você não tem permissão para esta operação",
            _ => "Erro de validação"
        };
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public string WarningMessage { get; private set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; private set; } = new();

    private ValidationResult() { }

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Error(string errorMessage)
    {
        return new ValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }

    public static ValidationResult Warning(string warningMessage)
    {
        return new ValidationResult
        {
            IsValid = true,
            WarningMessage = warningMessage
        };
    }

    public ValidationResult WithData(string key, object value)
    {
        AdditionalData[key] = value;
        return this;
    }
}

public class ValidationContext
{
    public int? IdProjeto { get; set; }
    public int? IdAssociado { get; set; }
    public int? IdPeriodo { get; set; }
    public int? IdGestor { get; set; }
    public string? TipoAvaliacao { get; set; }
    public string? Escopo { get; set; }
    public List<int?>? NotasCompetencias { get; set; }
    public List<int?>? NotasPerformances { get; set; }
    public int UserProfileId { get; set; }
    public int MinProfileRequired { get; set; } = 1;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool ValidarCompetencias { get; set; } = false;
    public bool ValidarPerformances { get; set; } = false;
    public bool ValidarPermissoes { get; set; } = false;
    public bool ValidarPrazos { get; set; } = false;

    public override string ToString()
    {
        return $"ValidationContext[Projeto:{IdProjeto}, Associado:{IdAssociado}, Periodo:{IdPeriodo}]";
    }
}