using Peers.Moderno.Models;
using Microsoft.Extensions.AI;

namespace Peers.Moderno.Services.Avaliacoes.Common;

public interface IAvaliacaoIAService
{
    Task<AvaliacaoSugestaoIA> GerarSugestaoAvaliacaoAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao);
    Task<List<CompetenciaSugestaoIA>> SugerirCompetenciasAsync(int idCargo, string tipoAvaliacao);
    Task<List<PerformanceSugestaoIA>> SugerirPerformancesAsync(int idCargo, string contexto);
    Task<FeedbackSugestaoIA> GerarSugestaoFeedbackAsync(List<AvaliacaoCompetencia> competencias, List<AvaliacaoPerformance> performances);
    Task<string> AnalisarTendenciasAvaliacaoAsync(int idAssociado, List<int> periodosAnteriores);
    Task<RecomendacaoDesenvolvimentoIA> GerarRecomendacaoDesenvolvimentoAsync(int idAssociado, string pontosFracos);
    Task<bool> ValidarConsistenciaAvaliacaoAsync(List<AvaliacaoCompetencia> competencias, List<AvaliacaoPerformance> performances);
    Task<string> GerarRelatorioInsightsAsync(int idProjeto, int idPeriodo);
}

public class AvaliacaoSugestaoIA
{
    public string SugestaoGeral { get; set; } = string.Empty;
    public List<string> PontosFortes { get; set; } = new();
    public List<string> AreasDesenvolvimento { get; set; } = new();
    public decimal ConfiancaAnalise { get; set; }
    public DateTime DataAnalise { get; set; } = DateTime.UtcNow;
}

public class CompetenciaSugestaoIA
{
    public int IdCompetencia { get; set; }
    public string NomeCompetencia { get; set; } = string.Empty;
    public int NotaSugerida { get; set; }
    public string Justificativa { get; set; } = string.Empty;
    public decimal ConfiancaSugestao { get; set; }
}

public class PerformanceSugestaoIA
{
    public int IdPerformance { get; set; }
    public string NomePerformance { get; set; } = string.Empty;
    public int NotaSugerida { get; set; }
    public string Justificativa { get; set; } = string.Empty;
    public decimal ConfiancaSugestao { get; set; }
}

public class FeedbackSugestaoIA
{
    public string FeedbackPositivo { get; set; } = string.Empty;
    public string FeedbackDesenvolvimento { get; set; } = string.Empty;
    public List<string> AcoesRecomendadas { get; set; } = new();
    public string TomSugerido { get; set; } = string.Empty;
}

public class RecomendacaoDesenvolvimentoIA
{
    public List<string> CursosRecomendados { get; set; } = new();
    public List<string> LivrosRecomendados { get; set; } = new();
    public List<string> ProjetosRecomendados { get; set; } = new();
    public List<string> MentoriaRecomendada { get; set; } = new();
    public string PlanoDesenvolvimento { get; set; } = string.Empty;
}

public class AvaliacaoCompetencia
{
    public int IdCompetencia { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Nota { get; set; }
    public string Comentario { get; set; } = string.Empty;
}

public class AvaliacaoPerformance
{
    public int IdPerformance { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Nota { get; set; }
    public string Comentario { get; set; } = string.Empty;
}