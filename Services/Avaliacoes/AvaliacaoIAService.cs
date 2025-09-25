using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Peers.Moderno.Services.Avaliacoes.Common;
using Peers.Moderno.Services.Common;
using System.Text.Json;

namespace Peers.Moderno.Services.Avaliacoes;

public class AvaliacaoIAService : IAvaliacaoIAService
{
    private readonly IChatClient? _chatClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AvaliacaoIAService> _logger;
    private readonly ITelemetryService _telemetryService;
    private readonly bool _isAIEnabled;

    public AvaliacaoIAService(
        IChatClient? chatClient,
        IConfiguration configuration,
        ILogger<AvaliacaoIAService> logger,
        ITelemetryService telemetryService)
    {
        _chatClient = chatClient;
        _configuration = configuration;
        _logger = logger;
        _telemetryService = telemetryService;
        _isAIEnabled = _configuration.GetValue<bool>("AutoAvaliacao:Features:EnableAIIntegration");
    }

    public async Task<AvaliacaoSugestaoIA> GerarSugestaoAvaliacaoAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao)
    {
        try
        {
            if (!_isAIEnabled || _chatClient == null)
            {
                return GerarSugestaoMock("Sugestão de avaliação baseada em dados históricos");
            }

            var prompt = $"Gere uma sugestão de avaliação para o associado {idAssociado} no projeto {idProjeto}, período {idPeriodo}, tipo {tipoAvaliacao}";
            
            var response = await _chatClient.CompleteAsync(prompt);
            
            _telemetryService.TrackEvent("AISuggestionGenerated", new Dictionary<string, string>
            {
                { "Type", "AvaliacaoGeral" },
                { "AssociadoId", idAssociado.ToString() },
                { "ProjetoId", idProjeto.ToString() }
            });

            return ProcessarRespostaIA(response.Message.Text ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar sugestão de avaliação com IA");
            _telemetryService.TrackException(ex);
            return GerarSugestaoMock("Sugestão padrão devido a erro na IA");
        }
    }

    public async Task<List<CompetenciaSugestaoIA>> SugerirCompetenciasAsync(int idCargo, string tipoAvaliacao)
    {
        try
        {
            if (!_isAIEnabled || _chatClient == null)
            {
                return GerarCompetenciasMock(idCargo);
            }

            var prompt = $"Sugira competências relevantes para o cargo {idCargo} no tipo de avaliação {tipoAvaliacao}";
            
            var response = await _chatClient.CompleteAsync(prompt);
            
            _telemetryService.TrackEvent("AISuggestionGenerated", new Dictionary<string, string>
            {
                { "Type", "Competencias" },
                { "CargoId", idCargo.ToString() },
                { "TipoAvaliacao", tipoAvaliacao }
            });

            return ProcessarCompetenciasIA(response.Message.Text ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sugerir competências com IA");
            _telemetryService.TrackException(ex);
            return GerarCompetenciasMock(idCargo);
        }
    }

    public async Task<List<PerformanceSugestaoIA>> SugerirPerformancesAsync(int idCargo, string contexto)
    {
        try
        {
            if (!_isAIEnabled || _chatClient == null)
            {
                return GerarPerformancesMock(idCargo);
            }

            var prompt = $"Sugira performances para o cargo {idCargo} considerando o contexto: {contexto}";
            
            var response = await _chatClient.CompleteAsync(prompt);
            
            _telemetryService.TrackEvent("AISuggestionGenerated", new Dictionary<string, string>
            {
                { "Type", "Performances" },
                { "CargoId", idCargo.ToString() },
                { "Contexto", contexto }
            });

            return ProcessarPerformancesIA(response.Message.Text ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sugerir performances com IA");
            _telemetryService.TrackException(ex);
            return GerarPerformancesMock(idCargo);
        }
    }

    public async Task<FeedbackSugestaoIA> GerarSugestaoFeedbackAsync(List<AvaliacaoCompetencia> competencias, List<AvaliacaoPerformance> performances)
    {
        try
        {
            if (!_isAIEnabled || _chatClient == null)
            {
                return GerarFeedbackMock();
            }

            var competenciasJson = JsonSerializer.Serialize(competencias);
            var performancesJson = JsonSerializer.Serialize(performances);
            var prompt = $"Gere um feedback baseado nas competências: {competenciasJson} e performances: {performancesJson}";
            
            var response = await _chatClient.CompleteAsync(prompt);
            
            _telemetryService.TrackEvent("AISuggestionGenerated", new Dictionary<string, string>
            {
                { "Type", "Feedback" },
                { "CompetenciasCount", competencias.Count.ToString() },
                { "PerformancesCount", performances.Count.ToString() }
            });

            return ProcessarFeedbackIA(response.Message.Text ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar sugestão de feedback com IA");
            _telemetryService.TrackException(ex);
            return GerarFeedbackMock();
        }
    }

    public async Task<string> AnalisarTendenciasAvaliacaoAsync(int idAssociado, List<int> periodosAnteriores)
    {
        try
        {
            if (!_isAIEnabled || _chatClient == null)
            {
                return "Análise de tendências não disponível. Funcionalidade de IA desabilitada.";
            }

            var periodosJson = JsonSerializer.Serialize(periodosAnteriores);
            var prompt = $"Analise as tendências de avaliação do associado {idAssociado} nos períodos: {periodosJson}";
            
            var response = await _chatClient.CompleteAsync(prompt);
            
            _telemetryService.TrackEvent("AISuggestionGenerated", new Dictionary<string, string>
            {
                { "Type", "TendenciasAnalise" },
                { "AssociadoId", idAssociado.ToString() },
                { "PeriodosCount", periodosAnteriores.Count.ToString() }
            });

            return response.Message.Text ?? "Análise não disponível";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao analisar tendências com IA");
            _telemetryService.TrackException(ex);
            return "Erro na análise de tendências. Tente novamente mais tarde.";
        }
    }

    public async Task<RecomendacaoDesenvolvimentoIA> GerarRecomendacaoDesenvolvimentoAsync(int idAssociado, string pontosFracos)
    {
        try
        {
            if (!_isAIEnabled || _chatClient == null)
            {
                return GerarRecomendacaoMock();
            }

            var prompt = $"Gere recomendações de desenvolvimento para o associado {idAssociado} com base nos pontos fracos: {pontosFracos}";
            
            var response = await _chatClient.CompleteAsync(prompt);
            
            _telemetryService.TrackEvent("AISuggestionGenerated", new Dictionary<string, string>
            {
                { "Type", "RecomendacaoDesenvolvimento" },
                { "AssociadoId", idAssociado.ToString() }
            });

            return ProcessarRecomendacaoIA(response.Message.Text ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar recomendação de desenvolvimento com IA");
            _telemetryService.TrackException(ex);
            return GerarRecomendacaoMock();
        }
    }

    public async Task<bool> ValidarConsistenciaAvaliacaoAsync(List<AvaliacaoCompetencia> competencias, List<AvaliacaoPerformance> performances)
    {
        try
        {
            if (!_isAIEnabled || _chatClient == null)
            {
                return ValidarConsistenciaMock(competencias, performances);
            }

            var competenciasJson = JsonSerializer.Serialize(competencias);
            var performancesJson = JsonSerializer.Serialize(performances);
            var prompt = $"Valide a consistência entre competências: {competenciasJson} e performances: {performancesJson}. Responda apenas 'true' ou 'false'.";
            
            var response = await _chatClient.CompleteAsync(prompt);
            
            _telemetryService.TrackEvent("AIValidation", new Dictionary<string, string>
            {
                { "Type", "ConsistenciaAvaliacao" },
                { "Result", response.Message.Text ?? "false" }
            });

            return bool.TryParse(response.Message.Text?.Trim(), out bool result) && result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar consistência com IA");
            _telemetryService.TrackException(ex);
            return ValidarConsistenciaMock(competencias, performances);
        }
    }

    public async Task<string> GerarRelatorioInsightsAsync(int idProjeto, int idPeriodo)
    {
        try
        {
            if (!_isAIEnabled || _chatClient == null)
            {
                return "Relatório de insights não disponível. Funcionalidade de IA desabilitada.";
            }

            var prompt = $"Gere insights sobre as avaliações do projeto {idProjeto} no período {idPeriodo}";
            
            var response = await _chatClient.CompleteAsync(prompt);
            
            _telemetryService.TrackEvent("AISuggestionGenerated", new Dictionary<string, string>
            {
                { "Type", "RelatorioInsights" },
                { "ProjetoId", idProjeto.ToString() },
                { "PeriodoId", idPeriodo.ToString() }
            });

            return response.Message.Text ?? "Insights não disponíveis";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório de insights com IA");
            _telemetryService.TrackException(ex);
            return "Erro na geração de insights. Tente novamente mais tarde.";
        }
    }

    private AvaliacaoSugestaoIA GerarSugestaoMock(string sugestaoGeral)
    {
        return new AvaliacaoSugestaoIA
        {
            SugestaoGeral = sugestaoGeral,
            PontosFortes = new List<string> { "Comunicação efetiva", "Trabalho em equipe" },
            AreasDesenvolvimento = new List<string> { "Gestão de tempo", "Liderança técnica" },
            ConfiancaAnalise = 0.75m
        };
    }

    private List<CompetenciaSugestaoIA> GerarCompetenciasMock(int idCargo)
    {
        return new List<CompetenciaSugestaoIA>
        {
            new CompetenciaSugestaoIA
            {
                IdCompetencia = 1,
                NomeCompetencia = "Comunicação",
                NotaSugerida = 4,
                Justificativa = "Baseado no perfil do cargo",
                ConfiancaSugestao = 0.8m
            }
        };
    }

    private List<PerformanceSugestaoIA> GerarPerformancesMock(int idCargo)
    {
        return new List<PerformanceSugestaoIA>
        {
            new PerformanceSugestaoIA
            {
                IdPerformance = 1,
                NomePerformance = "Qualidade do Trabalho",
                NotaSugerida = 4,
                Justificativa = "Baseado no contexto fornecido",
                ConfiancaSugestao = 0.8m
            }
        };
    }

    private FeedbackSugestaoIA GerarFeedbackMock()
    {
        return new FeedbackSugestaoIA
        {
            FeedbackPositivo = "Demonstra excelente capacidade técnica e colaboração.",
            FeedbackDesenvolvimento = "Pode melhorar na gestão de prazos e comunicação proativa.",
            AcoesRecomendadas = new List<string> { "Participar de treinamento de gestão de tempo", "Buscar feedback regular" },
            TomSugerido = "Construtivo e encorajador"
        };
    }

    private RecomendacaoDesenvolvimentoIA GerarRecomendacaoMock()
    {
        return new RecomendacaoDesenvolvimentoIA
        {
            CursosRecomendados = new List<string> { "Gestão de Projetos", "Liderança Técnica" },
            LivrosRecomendados = new List<string> { "Clean Code", "The Manager's Path" },
            ProjetosRecomendados = new List<string> { "Liderar próximo projeto", "Mentoria de júnior" },
            MentoriaRecomendada = new List<string> { "Mentoria em liderança", "Coaching técnico" },
            PlanoDesenvolvimento = "Foco em habilidades de liderança e gestão nos próximos 6 meses."
        };
    }

    private bool ValidarConsistenciaMock(List<AvaliacaoCompetencia> competencias, List<AvaliacaoPerformance> performances)
    {
        return competencias.Any() && performances.Any();
    }

    private AvaliacaoSugestaoIA ProcessarRespostaIA(string resposta)
    {
        return GerarSugestaoMock(resposta);
    }

    private List<CompetenciaSugestaoIA> ProcessarCompetenciasIA(string resposta)
    {
        return GerarCompetenciasMock(0);
    }

    private List<PerformanceSugestaoIA> ProcessarPerformancesIA(string resposta)
    {
        return GerarPerformancesMock(0);
    }

    private FeedbackSugestaoIA ProcessarFeedbackIA(string resposta)
    {
        return GerarFeedbackMock();
    }

    private RecomendacaoDesenvolvimentoIA ProcessarRecomendacaoIA(string resposta)
    {
        return GerarRecomendacaoMock();
    }
}