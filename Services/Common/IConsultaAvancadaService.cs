using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.Common;

public interface IConsultaAvancadaService
{
    Task<List<ComboItem>> CarregarProjetosAsync(int? gestorId = null, int? clienteId = null, int? status = null, int? periodoId = null);
    Task<List<ComboItem>> CarregarClientesAsync();
    Task<List<ComboItem>> CarregarProfissionaisAsync(bool apenasAtivos = true);
    Task<List<ComboItem>> CarregarPeriodosAsync(int? empresaId = null);
    Task<List<AvaliacaoEmailModel>> ConsultarAvaliacoesAsync(
        int? projetoId,
        int? associadoId,
        int? periodoId,
        int? clienteId,
        string? fase
    );
}
