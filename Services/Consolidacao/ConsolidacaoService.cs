using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Consolidacao;

public interface IConsolidacaoService
{
    Task<List<ConsolidacaoItem>> ObterListConsolidacaoAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo);
    Task CalculaNotaPerformanceComiteAsync(int idNotaPerformance, int idNotaComite);
}

public class ConsolidacaoService : IConsolidacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly IAssociadosService _associadosService;
    private readonly ITelemetryService _telemetryService;

    public ConsolidacaoService(
        ApplicationDbContext context,
        IAssociadosService associadosService,
        ITelemetryService telemetryService)
    {
        _context = context;
        _associadosService = associadosService;
        _telemetryService = telemetryService;
    }

    public async Task<List<ConsolidacaoItem>> ObterListConsolidacaoAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo)
    {
        try
        {
            _telemetryService.TrackEvent("ConsolidacaoService.ObterListConsolidacao", new Dictionary<string, string>
            {
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() },
                { "IdPeriodo", idPeriodo.ToString() },
                { "TipoAvaliacao", tipoAvaliacao },
                { "Escopo", escopo }
            });

            var query = from a in _context.Associados
                       join c in _context.Cargos on a.IdCargo equals c.IdCargo
                       select new ConsolidacaoItem
                       {
                           IdAssociado = a.Id,
                           Nome = a.Nome,
                           Cargo = c.Nome,
                           TipoAvaliacao = tipoAvaliacao,
                           Escopo = escopo
                       };

            if (idAssociado > 0)
                query = query.Where(x => x.IdAssociado == idAssociado);

            var result = await query.ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterListConsolidacaoAsync" },
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() },
                { "IdPeriodo", idPeriodo.ToString() }
            });
            throw;
        }
    }

    public async Task CalculaNotaPerformanceComiteAsync(int idNotaPerformance, int idNotaComite)
    {
        try
        {
            _telemetryService.TrackEvent("ConsolidacaoService.CalculaNotaPerformanceComite", new Dictionary<string, string>
            {
                { "IdNotaPerformance", idNotaPerformance.ToString() },
                { "IdNotaComite", idNotaComite.ToString() }
            });

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CalculaNotaPerformanceComiteAsync" },
                { "IdNotaPerformance", idNotaPerformance.ToString() },
                { "IdNotaComite", idNotaComite.ToString() }
            });
            throw;
        }
    }
}