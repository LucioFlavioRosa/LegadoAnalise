using Peers.Moderno.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Services.AvaliacoesGestor.Common;

public interface IAvaliacoesGestorService
{
    Task<List<ProjetoModel>> BuscarProjetosAvaliacoesAsync(int idGestor, int? idProjeto = null, int? idStatus = null, int? idPeriodo = null, int? idCliente = null);
    Task<List<ComboItem>> ObterProjetosComboAsync(int idGestor);
    Task<List<ComboItem>> ObterClientesComboAsync();
    Task<List<ComboItem>> ObterPeriodosComboAsync(int idEmpresa);
    Task<List<ComboItem>> ObterStatusComboAsync();
    Task<bool> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId);
    Task<ProjetoModel?> ObterProjetoDetalhadoAsync(int idProjeto, int idGestor, int? idPeriodo = null);
}