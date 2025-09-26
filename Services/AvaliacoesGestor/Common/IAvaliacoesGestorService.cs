using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.AvaliacoesGestor.Common;

public interface IAvaliacoesGestorService
{
    Task<AvaliacaoGestorPerformanceDto> CarregarDadosAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
    Task<List<PerformanceModel>> CarregarListaPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<bool> SalvarAvaliacoesAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao, List<PerformanceGestorInputDto> avaliacoes, bool finalizarAvaliacao);
    Task<bool> ValidarPreenchimentoAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
    Task<bool> PodeEditarAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
}

public class AvaliacaoGestorPerformanceDto
{
    public ProjetoDto Projeto { get; set; } = new();
    public AssociadoDto Associado { get; set; } = new();
    public PeriodoDto Periodo { get; set; } = new();
    public AssociadoDto Gestor { get; set; } = new();
    public string Cliente { get; set; } = string.Empty;
    public string TempoPeers { get; set; } = string.Empty;
    public string TempoCargo { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
    public List<PerformanceModel> Performances { get; set; } = new();
}

public class ProjetoDto
{
    public int IdProjeto { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class AssociadoDto
{
    public int IdAssociado { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
}

public class PeriodoDto
{
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

public class PerformanceGestorInputDto
{
    public int IdPerformance { get; set; }
    public int IdNotaNivel1AvaliacaoGestor { get; set; }
    public string ComentariosAvaliacaoGestor { get; set; } = string.Empty;
}

public class PerformanceModel
{
    public int IdPerformance { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Abaixo { get; set; } = string.Empty;
    public string Esperado { get; set; } = string.Empty;
    public string Acima { get; set; } = string.Empty;
    public string Abrangencia { get; set; } = string.Empty;
    public string SeparadorAbrangencia { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string DisclaimerInput { get; set; } = string.Empty;
    public string NotaAvaliado { get; set; } = string.Empty;
    public string NotaCegas { get; set; } = string.Empty;
    public string NotaGestor { get; set; } = string.Empty;
    public string ObservacaoAvaliado { get; set; } = string.Empty;
    public string ObservacaoCegas { get; set; } = string.Empty;
    public string ObservacaoGestor { get; set; } = string.Empty;
    public bool PodeEditar { get; set; } = false;
    public bool InputAvaliacaoGestor { get; set; } = false;
    public int NotaPadraoAvaliacaoGestor { get; set; } = 0;
}
