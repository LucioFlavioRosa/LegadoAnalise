using Peers.Moderno.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Services.AvaliacoesGestorasCegas;

public interface IAvaliacaoGestorasCegasPerformanceService
{
    Task<CabecalhoAvaliacaoGestorasCegasPerformanceDto> GetDadosCabecalhoAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
    Task<List<PerformanceModel>> GetListaPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<List<NotaItemDto>> GetNotasAsync();
    Task<bool> SalvarAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, List<AvaliacaoPerformanceInputDto> avaliacoes, bool finalizar = false);
    Task<bool> FinalizarAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, List<AvaliacaoPerformanceInputDto> avaliacoes);
}

public class CabecalhoAvaliacaoGestorasCegasPerformanceDto
{
    public string NomeAssociado { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public string Projeto { get; set; } = string.Empty;
    public string Gestor { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string TempoCargo { get; set; } = string.Empty;
    public string TempoPeers { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
}

public class NotaItemDto
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsDisabled { get; set; } = false;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class AvaliacaoPerformanceInputDto
{
    public int IdPerformance { get; set; }
    public int Nota { get; set; }
    public string Comentario { get; set; } = string.Empty;
}
