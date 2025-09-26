using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.AvaliacoesGestor.Common;

public interface IAvaliacoesGestorService
{
    Task<AvaliacaoGestorPerformanceDto> ObterDadosAvaliacaoGestorPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
    Task<List<PerformanceModel>> ObterListaPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<bool> SalvarAvaliacoesGestorPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao, List<AvaliacaoGestorPerformanceInput> avaliacoes, bool finalizarAvaliacao = false);
    Task<bool> PodeEditarAvaliacaoGestorAsync(int idAssociado, int idProjeto, int idPeriodo, int idPerformance);
    Task<NotasGestorPerformanceDto> ObterNotasGestorPerformanceAsync(int idAssociado, int idProjeto, int idPeriodo, int idPerformance);
}

public class AvaliacaoGestorPerformanceDto
{
    public Projeto Projeto { get; set; }
    public Associado Associado { get; set; }
    public Periodo Periodo { get; set; }
    public Associado Gestor { get; set; }
    public Cliente Cliente { get; set; }
    public string TempoPeers { get; set; } = string.Empty;
    public string TempoCargo { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
    public List<PerformanceModel> Performances { get; set; } = new();
}

public class AvaliacaoGestorPerformanceInput
{
    public int IdPerformance { get; set; }
    public int IdNotaNivel1AvaliacaoGestor { get; set; }
    public string ComentariosAvaliacaoGestor { get; set; } = string.Empty;
}

public class NotasGestorPerformanceDto
{
    public string NotaAvaliado { get; set; } = string.Empty;
    public string ObservacaoAvaliado { get; set; } = string.Empty;
    public string NotaCegas { get; set; } = string.Empty;
    public string ObservacaoCegas { get; set; } = string.Empty;
    public string NotaGestor { get; set; } = string.Empty;
    public string ObservacaoGestor { get; set; } = string.Empty;
    public bool PodeEditar { get; set; }
}
