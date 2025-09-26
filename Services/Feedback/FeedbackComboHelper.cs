using Peers.Moderno.Services.Feedback.Common;
using Peers.Moderno.Services.Clientes;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Services.Projetos;
using Peers.Moderno.Services.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peers.Moderno.Services.Feedback;

public class FeedbackComboHelper : IFeedbackComboHelper
{
    private readonly IClientesService _clientesService;
    private readonly IProjetosService _projetosService;
    private readonly IAvaliacoesService _avaliacoesService;

    public FeedbackComboHelper(IClientesService clientesService, IProjetosService projetosService, IAvaliacoesService avaliacoesService)
    {
        _clientesService = clientesService;
        _projetosService = projetosService;
        _avaliacoesService = avaliacoesService;
    }

    public async Task<List<ClienteFeedbackModel>> ListarClientesAsync()
    {
        var clientes = await _clientesService.ListaClientesAtivosAsync();
        return clientes.Select(c => new ClienteFeedbackModel
        {
            IdCliente = c.IdCliente,
            Cliente = c.Cliente
        }).ToList();
    }

    public async Task<List<PeriodoFeedbackModel>> ListarPeriodosAsync(int empresaId)
    {
        var periodos = await _avaliacoesService.ListaPeriodosAsync(empresaId);
        return periodos.Select(p => new PeriodoFeedbackModel
        {
            IdPeriodo = p.IdPeriodo,
            Periodo = p.Periodo
        }).ToList();
    }

    public async Task<List<StatusFeedbackModel>> ListarStatusAsync()
    {
        var statusList = await _avaliacoesService.ListaStatusAvaliacoesAsync();
        return statusList.Select(s => new StatusFeedbackModel
        {
            IdStatus = s.IdStatus,
            Status = s.Status
        }).ToList();
    }
}
