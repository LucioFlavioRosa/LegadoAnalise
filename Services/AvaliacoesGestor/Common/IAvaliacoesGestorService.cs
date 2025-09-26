using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.AvaliacoesGestor.Common;

public interface IAvaliacoesGestorService
{
    Task<AvaliacaoGestorPerformanceDto> CarregarDadosAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
    Task<List<PerformanceModel>> ObterListaPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<bool> SalvarAvaliacoesAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao, List<AvaliacaoGestorPerformanceInput> avaliacoes, bool finalizarAvaliacao = false);
    Task<bool> ValidarPreenchimentoAsync(int idProjeto, int idAssociado, int idPeriodo);
    Task<bool> PodeEditarAsync(int idAssociado, int idProjeto, int idPeriodo, int idPerformance);
    Task<PerformanceNotasDto> ObterNotasAsync(int idAssociado, int idProjeto, int idPeriodo, int idPerformance);
    Task<PerformanceComboDto> ObterCombosAsync();
}

public class AvaliacaoGestorPerformanceDto
{
    public ProjetoDto Projeto { get; set; } = new();
    public AssociadoDto Associado { get; set; } = new();
    public PeriodoDto Periodo { get; set; } = new();
    public AssociadoDto Gestor { get; set; } = new();
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

public class PerformanceNotasDto
{
    public string NotaAvaliado { get; set; } = string.Empty;
    public string ObservacaoAvaliado { get; set; } = string.Empty;
    public string NotaCegas { get; set; } = string.Empty;
    public string ObservacaoCegas { get; set; } = string.Empty;
    public string NotaGestor { get; set; } = string.Empty;
    public string ObservacaoGestor { get; set; } = string.Empty;
}

public class PerformanceComboDto
{
    public List<ComboItem> Notas { get; set; } = new();
}

public class ProjetoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
}

public class AssociadoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
}

public class PeriodoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
