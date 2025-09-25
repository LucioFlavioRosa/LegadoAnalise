using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Avaliacoes;
using Microsoft.Extensions.Configuration;

namespace Peers.Moderno.Services.AutoAvaliacao;

public interface IAutoAvaliacaoValidator
{
    Task<ValidationResult> ValidarAvaliacaoAsync(List<CompetenciaAvaliacaoDto> competencias);
    Task<ValidationResult> ValidarCompetenciaAsync(CompetenciaAvaliacaoDto competencia);
    Task<ValidationResult> ValidarConsistenciaNiveisAsync(int notaNivel1, int notaNivel2);
    Task<ValidationResult> ValidarPilaresAsync(List<CompetenciaAvaliacaoDto> competencias);
    Task<bool> ValidarEtapaPermitidaAsync(string etapaAtual);
}

public class AutoAvaliacaoValidator : IAutoAvaliacaoValidator
{
    private readonly INotasAvaliacaoService _notasAvaliacaoService;
    private readonly ITelemetryService _telemetryService;
    private readonly IConfiguration _configuration;

    public AutoAvaliacaoValidator(
        INotasAvaliacaoService notasAvaliacaoService,
        ITelemetryService telemetryService,
        IConfiguration configuration)
    {
        _notasAvaliacaoService = notasAvaliacaoService;
        _telemetryService = telemetryService;
        _configuration = configuration;
    }

    public async Task<ValidationResult> ValidarAvaliacaoAsync(List<CompetenciaAvaliacaoDto> competencias)
    {
        try
        {
            if (competencias == null || !competencias.Any())
            {
                return ValidationResult.Error("Nenhuma competência encontrada para validação");
            }

            var competenciasParaValidar = new List<CompetenciaValidacaoDto>();

            foreach (var competencia in competencias)
            {
                var validacaoCompetencia = await ValidarCompetenciaAsync(competencia);
                if (!validacaoCompetencia.IsValid)
                {
                    return validacaoCompetencia;
                }

                var competenciaValidacao = new CompetenciaValidacaoDto
                {
                    IdCompetencia = competencia.IdCompetencia,
                    SubCompetencia = competencia.SubCompetencia,
                    NotaNivel1 = competencia.NotaNivel1,
                    NotaNivel2 = competencia.NotaNivel2,
                    IdModo = competencia.IdModo
                };

                if (competencia.IdModo == 1)
                {
                    var validacaoConsistencia = await ValidarConsistenciaNiveisAsync(competencia.NotaNivel1, competencia.NotaNivel2);
                    if (!validacaoConsistencia.IsValid)
                    {
                        return validacaoConsistencia;
                    }
                }
                else if (competencia.IdModo == 2)
                {
                    competenciaValidacao.NotaNivel1 = 5;
                    competenciaValidacao.NotaNivel2 = 5;
                }

                competenciasParaValidar.Add(competenciaValidacao);
            }

            var validacaoPilares = await ValidarPilaresAsync(competencias);
            if (!validacaoPilares.IsValid)
            {
                return validacaoPilares;
            }

            _telemetryService.TrackEvent("AutoAvaliacaoValidada", new Dictionary<string, string>
            {
                { "TotalCompetencias", competencias.Count.ToString() },
                { "CompetenciasPadrao", competenciasParaValidar.Count(c => c.IdModo == 1).ToString() },
                { "CompetenciasAuto", competenciasParaValidar.Count(c => c.IdModo == 2).ToString() }
            });

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarAvaliacaoAsync" },
                { "Component", "AutoAvaliacaoValidator" }
            });
            return ValidationResult.Error("Erro interno durante validação");
        }
    }

    public async Task<ValidationResult> ValidarCompetenciaAsync(CompetenciaAvaliacaoDto competencia)
    {
        if (competencia == null)
        {
            return ValidationResult.Error("Competência não pode ser nula");
        }

        if (competencia.IdModo == 1)
        {
            if ((competencia.NotaNivel1 == 0 && competencia.NotaNivel2 > 0) || 
                (competencia.NotaNivel1 > 0 && competencia.NotaNivel2 == 0))
            {
                return ValidationResult.Error("É obrigatório selecionar uma nota para cada nível.");
            }
        }

        return ValidationResult.Success();
    }

    public async Task<ValidationResult> ValidarConsistenciaNiveisAsync(int notaNivel1, int notaNivel2)
    {
        try
        {
            if (notaNivel1 == 5 && notaNivel2 != 5)
            {
                return ValidationResult.Error("Quando a nota de Competência do Nível Atual for = Não Se Aplica, o Próximo Nível deve ser Não Se Aplica");
            }

            if (notaNivel1 > 0 && notaNivel2 > 0)
            {
                var statusList = await _notasAvaliacaoService.ListaNotasCompetenciasAsync(false);
                
                var pesoDdl1 = statusList.FirstOrDefault(x => x.IdNota == notaNivel1)?.Peso ?? 0;
                var pesoDdl2 = statusList.FirstOrDefault(x => x.IdNota == notaNivel2)?.Peso ?? 0;

                if (pesoDdl2 > pesoDdl1)
                {
                    return ValidationResult.Error("A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).");
                }
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarConsistenciaNiveisAsync" },
                { "NotaNivel1", notaNivel1.ToString() },
                { "NotaNivel2", notaNivel2.ToString() }
            });
            return ValidationResult.Error("Erro ao validar consistência entre níveis");
        }
    }

    public async Task<ValidationResult> ValidarPilaresAsync(List<CompetenciaAvaliacaoDto> competencias)
    {
        try
        {
            var competenciasParaValidar = competencias.Select(c => new CompetenciaValidacaoDto
            {
                SubCompetencia = c.SubCompetencia,
                NotaNivel1 = c.NotaNivel1,
                NotaNivel2 = c.NotaNivel2,
                IdModo = c.IdModo
            }).ToList();

            var listPilares = competenciasParaValidar.Select(r => r.SubCompetencia).Distinct().ToList();
            var pilarVazio = false;
            var pilaresProblemaNivel1 = new List<string>();
            var pilaresProblemaNivel2 = new List<string>();

            foreach (var pilar in listPilares)
            {
                var countCompetenciasPilar = competenciasParaValidar.Where(r => r.SubCompetencia == pilar).ToList();
                var countCompetenciasPadrao = countCompetenciasPilar.Where(r => r.IdModo != 2).ToList();
                var countCompetenciasAuto = countCompetenciasPilar.Where(r => r.IdModo == 2).ToList();
                var countRespostasVaziasNivel1 = countCompetenciasPadrao.Where(r => r.NotaNivel1 == 5).ToList();
                var countRespostasVaziasNivel2 = countCompetenciasPadrao.Where(r => r.NotaNivel2 == 5).ToList();

                if (countCompetenciasPilar.Count != countCompetenciasAuto.Count)
                {
                    if (countCompetenciasPadrao.Count == countRespostasVaziasNivel1.Count)
                    {
                        pilarVazio = true;
                        pilaresProblemaNivel1.Add(pilar);
                    }
                    if (countCompetenciasPadrao.Count == countRespostasVaziasNivel2.Count)
                    {
                        pilarVazio = true;
                        pilaresProblemaNivel2.Add(pilar);
                    }
                }
            }

            if (pilarVazio)
            {
                var mensagem = "Cada pilar precisa receber ao mínimo 1 nota mensurável em cada nível (diferente de não se aplica)";
                if (pilaresProblemaNivel1.Any() || pilaresProblemaNivel2.Any())
                {
                    mensagem += "\nPilares com problema: " + string.Join(", ", pilaresProblemaNivel1.Union(pilaresProblemaNivel2).Distinct());
                }
                return ValidationResult.Error(mensagem);
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPilaresAsync" },
                { "Component", "AutoAvaliacaoValidator" }
            });
            return ValidationResult.Error("Erro ao validar pilares");
        }
    }

    public async Task<bool> ValidarEtapaPermitidaAsync(string etapaAtual)
    {
        var etapasPermitidas = new[] 
        {
            "avaliacao_as_cegas",
            "auto_avaliacao", 
            "em_paralelo",
            "nao_iniciada"
        };

        return etapasPermitidas.Contains(etapaAtual?.ToLower());
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public List<string> ErrorMessages { get; private set; } = new();

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
            ErrorMessage = errorMessage,
            ErrorMessages = new List<string> { errorMessage }
        };
    }

    public static ValidationResult Error(List<string> errorMessages)
    {
        return new ValidationResult
        {
            IsValid = false,
            ErrorMessage = string.Join("; ", errorMessages),
            ErrorMessages = errorMessages
        };
    }
}

public class CompetenciaValidacaoDto
{
    public int IdCompetencia { get; set; }
    public string SubCompetencia { get; set; } = string.Empty;
    public int NotaNivel1 { get; set; }
    public int NotaNivel2 { get; set; }
    public int IdModo { get; set; }
}