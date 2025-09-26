using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Consolidacao.Common;

public interface IConsolidacaoService
{
    Task<List<ResultadoCompetenciaModel>> ObterCompetenciasConsolidacaoAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<List<ResultadoPerfomanceModel>> ObterPerformanceConsolidacaoAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<string> CalcularNotaCompetenciaComiteAsync(int idAvaliacaoCompetencia, int nivel, int nota);
    Task<string> CalcularNotaPerformanceComiteAsync(int idAvaliacaoPerformance, int nota);
    Task<ConsideracoesMentorDto> CarregarConsideracoesMentorAsync(int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo);
    Task<bool> SalvarConsideracoesMentorAsync(ConsideracoesMentorDto consideracoesMentorDto);
    Task<string> GerarJsonRadarAsync(int idAssociado, int idProjeto, int idPeriodo, int idCargo);
}

public class ResultadoCompetenciaModel
{
    public int IdAvaliacaoCompetencia { get; set; }
    public string Eixo { get; set; } = string.Empty;
    public decimal? NotaNivel1AutoAvaliacao { get; set; }
    public decimal? NotaNivel1Feedback { get; set; }
    public decimal? NotaNivel2AutoAvaliacao { get; set; }
    public decimal? NotaNivel2Feedback { get; set; }
    public decimal? NotaSubcompetenciaAvaliadoN1 { get; set; }
    public decimal? NotaSubcompetenciaGestorN1 { get; set; }
    public decimal? NotaSubcompetenciaAvaliadoN2 { get; set; }
    public decimal? NotaSubcompetenciaGestorN2 { get; set; }
    public decimal? NotaCompetenciaAvaliado { get; set; }
    public decimal? NotaCompetenciaGestor { get; set; }
    public decimal? NotaFinalNivel1 { get; set; }
    public decimal? NotaFinalNivel2 { get; set; }
    public int? IdNotaNivel1Comite { get; set; }
    public int? IdNotaNivel2Comite { get; set; }
    public int? IdNotaNivel1Feedback { get; set; }
    public int? IdNotaNivel2Feedback { get; set; }
    public bool enableNivel1 { get; set; }
    public bool enableNivel2 { get; set; }
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string CompetenciaAtual { get; set; } = string.Empty;
    public string DetalheProximoNivel { get; set; } = string.Empty;
    public string CompetenciaProximo { get; set; } = string.Empty;
    public string ComentarioAvaliado { get; set; } = string.Empty;
    public string ComentarioFeedback { get; set; } = string.Empty;
}

public class ResultadoPerfomanceModel
{
    public int IdAvaliacaoPerformance { get; set; }
    public string Perfomance { get; set; } = string.Empty;
    public decimal? NotaNivel1AutoAvaliacao { get; set; }
    public decimal? NotaNivel1Feedback { get; set; }
    public int? IdNotaComite { get; set; }
    public int? IdNotaNivel1Feedback { get; set; }
    public decimal? NotaPerfomancePonderada { get; set; }
    public decimal? NotaPerfomance { get; set; }
    public string ComentariosAutoAvaliacao { get; set; } = string.Empty;
    public string ComentarioFeedback { get; set; } = string.Empty;
}

public class ConsideracoesMentorDto
{
    public int IdConsideracoesMentor { get; set; }
    public int IdMentor { get; set; }
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public bool LiberadoRH { get; set; }
    public string AcaoComite { get; set; } = string.Empty;
    public string PontosFortesRH { get; set; } = string.Empty;
    public string PontosFracosRH { get; set; } = string.Empty;
    public decimal SalarioAtual { get; set; }
    public decimal SalarioNovo { get; set; }
    public string RegimeContratacaoAtual { get; set; } = string.Empty;
    public string RegimeContratacaoNovo { get; set; } = string.Empty;
    public bool MentoriaRealizada { get; set; }
    public string ProximoCargo { get; set; } = string.Empty;
    public string LabelIncremento { get; set; } = string.Empty;
    public string PontosFortes { get; set; } = string.Empty;
    public string PontosFracos { get; set; } = string.Empty;
}
