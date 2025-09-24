using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Avaliacoes.Common;

public interface IAutoAvaliacaoService
{
    Task<List<ComboItem>> CarregarComboProjetosAsync(int usuarioId);
    Task<List<ComboItem>> CarregarComboClientesAsync();
    Task<List<ComboItem>> CarregarComboPeriodosAsync(int empresaId);
    Task<List<ComboItem>> CarregarComboStatusAsync();
    Task<List<ProjetoModel>> BuscarAvaliacoesFiltradasAsync(int usuarioId, FiltroAutoAvaliacaoModel filtro);
    Task<bool> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId);
    Task<AvaliacaoStatusModel> ObterStatusAvaliacaoAsync(int idAvaliacao);
    Task<bool> ValidarPermissaoFinalizacaoAsync(int idAvaliacao, int usuarioId);
}

public class FiltroAutoAvaliacaoModel
{
    public int? IdProjeto { get; set; }
    public int? IdCliente { get; set; }
    public int? IdPeriodo { get; set; }
    public int? IdStatus { get; set; }
    public int EmpresaId { get; set; }
}

public class AvaliacaoStatusModel
{
    public int IdAvaliacao { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Etapa { get; set; } = string.Empty;
    public bool PodeSerFinalizada { get; set; }
    public DateTime? DataFinalizacao { get; set; }
    public string RotuloBotao { get; set; } = string.Empty;
    public bool ExibirBotaoFinalizar { get; set; }
}