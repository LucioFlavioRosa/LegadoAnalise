using Peers.Moderno.Services.AutoAvaliacao.Common;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.AutoAvaliacao.Common;

public interface IAutoAvaliacaoService
{
    Task<AutoAvaliacaoDto> CarregarAutoAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor);
    Task<AutoAvaliacaoDto> CarregarCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao);
    Task<AutoAvaliacaoValidationResult> ValidarAvaliacaoAsync(AutoAvaliacaoDto avaliacao);
    Task<AutoAvaliacaoSaveResult> SalvarAvaliacaoAsync(AutoAvaliacaoDto avaliacao, bool finalizarAvaliacao = false);
    Task<AutoAvaliacaoDto> CarregarCabecalhoDescricoesAsync(int idCargo, List<Competencia> competencias, Associado associado);
    Task<bool> VerificarPermissaoEdicaoAsync(int idAvaliacao, string etapaAtual);
    Task<string> CalcularTempoRestanteAsync(int idAvaliacao);
    Task<AutoAvaliacaoStatusInfo> ObterStatusAvaliacaoAsync(int idAvaliacao);
    Task<bool> PodeFinalizarAvaliacaoAsync(AutoAvaliacaoDto avaliacao);
    Task<List<CompetenciaAvaliacaoDto>> ProcessarCompetenciasParaExibicaoAsync(List<Competencia> competencias, List<object> avaliacoesExistentes, Associado associado);
    Task<bool> AvancaProximaEtapaAsync(int idAvaliacao, string tipoAvaliacao);
    Task<AutoAvaliacaoConfigDto> ObterConfiguracaoAvaliacaoAsync();
}

public class AutoAvaliacaoValidationResult
{
    public bool IsValid { get; set; }
    public List<string> ErrorMessages { get; set; } = new List<string>();
    public List<CompetenciaValidationError> CompetenciaErrors { get; set; } = new List<CompetenciaValidationError>();

    public static AutoAvaliacaoValidationResult Success()
    {
        return new AutoAvaliacaoValidationResult { IsValid = true };
    }

    public static AutoAvaliacaoValidationResult Error(string message)
    {
        return new AutoAvaliacaoValidationResult 
        { 
            IsValid = false, 
            ErrorMessages = new List<string> { message } 
        };
    }

    public static AutoAvaliacaoValidationResult Error(List<string> messages)
    {
        return new AutoAvaliacaoValidationResult 
        { 
            IsValid = false, 
            ErrorMessages = messages 
        };
    }
}

public class AutoAvaliacaoSaveResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? IdAvaliacao { get; set; }
    public string NovaEtapa { get; set; } = string.Empty;

    public static AutoAvaliacaoSaveResult Success(string message, int? idAvaliacao = null, string novaEtapa = "")
    {
        return new AutoAvaliacaoSaveResult 
        { 
            IsSuccess = true, 
            Message = message,
            IdAvaliacao = idAvaliacao,
            NovaEtapa = novaEtapa
        };
    }

    public static AutoAvaliacaoSaveResult Error(string message)
    {
        return new AutoAvaliacaoSaveResult 
        { 
            IsSuccess = false, 
            Message = message 
        };
    }
}

public class CompetenciaValidationError
{
    public int IdCompetencia { get; set; }
    public string SubCompetencia { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty; // "nivel1", "nivel2", "consideracoes"
}

public class AutoAvaliacaoStatusInfo
{
    public string EtapaAtual { get; set; } = string.Empty;
    public string StatusAvaliacao { get; set; } = string.Empty;
    public bool PodeEditar { get; set; }
    public bool PodeFinalizar { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string TempoRestante { get; set; } = string.Empty;
}

public class AutoAvaliacaoConfigDto
{
    public string DefaultTipoAvaliacao { get; set; } = string.Empty;
    public string DefaultEscopo { get; set; } = string.Empty;
    public bool RequireAllCompetenciasPreenchidas { get; set; }
    public bool EnableFinalizacaoAutomatica { get; set; }
    public bool EnableNotificacaoFinalizacao { get; set; }
    public Dictionary<string, string> ValidationMessages { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> StatusOptions { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> EtapasAvaliacao { get; set; } = new Dictionary<string, string>();
}