using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Common.Avaliacoes;

public class TotalAvaliacaoService : ITotalAvaliacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public TotalAvaliacaoService(
        ApplicationDbContext context,
        ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<int> ObterTotalAvaliacoesAsync()
    {
        try
        {
            var total = await _context.AvaliacoesCompetenciasNotas.CountAsync();
            
            _telemetryService.TrackEvent("TotalAvaliacaoConsultado", new Dictionary<string, string>
            {
                { "Total", total.ToString() },
                { "Method", "ObterTotalAvaliacoesAsync" }
            });
            
            return total;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterTotalAvaliacoesAsync" },
                { "Component", "TotalAvaliacaoService" }
            });
            throw;
        }
    }

    public async Task<int> ObterTotalAvaliacoesPorTipoAsync(string tipoAvaliacao)
    {
        try
        {
            var total = await _context.AvaliacoesCompetenciasNotas
                .Where(a => a.TipoAvaliacao == tipoAvaliacao)
                .CountAsync();
            
            _telemetryService.TrackEvent("TotalAvaliacaoPorTipoConsultado", new Dictionary<string, string>
            {
                { "TipoAvaliacao", tipoAvaliacao },
                { "Total", total.ToString() }
            });
            
            return total;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterTotalAvaliacoesPorTipoAsync" },
                { "Component", "TotalAvaliacaoService" },
                { "TipoAvaliacao", tipoAvaliacao }
            });
            throw;
        }
    }

    public async Task<int> ObterTotalAvaliacoesPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        try
        {
            var total = await _context.AvaliacoesCompetenciasNotas
                .Where(a => a.DataAvaliacao >= dataInicio && a.DataAvaliacao <= dataFim)
                .CountAsync();
            
            _telemetryService.TrackEvent("TotalAvaliacaoPorPeriodoConsultado", new Dictionary<string, string>
            {
                { "DataInicio", dataInicio.ToString("yyyy-MM-dd") },
                { "DataFim", dataFim.ToString("yyyy-MM-dd") },
                { "Total", total.ToString() }
            });
            
            return total;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterTotalAvaliacoesPorPeriodoAsync" },
                { "Component", "TotalAvaliacaoService" }
            });
            throw;
        }
    }

    public async Task<int> ObterTotalAvaliacoesPorAssociadoAsync(int idAssociado)
    {
        try
        {
            var total = await _context.AvaliacoesCompetenciasNotas
                .Where(a => a.IdAssociado == idAssociado)
                .CountAsync();
            
            _telemetryService.TrackEvent("TotalAvaliacaoPorAssociadoConsultado", new Dictionary<string, string>
            {
                { "IdAssociado", idAssociado.ToString() },
                { "Total", total.ToString() }
            });
            
            return total;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterTotalAvaliacoesPorAssociadoAsync" },
                { "Component", "TotalAvaliacaoService" },
                { "IdAssociado", idAssociado.ToString() }
            });
            throw;
        }
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasAvaliacoesAsync()
    {
        try
        {
            var estatisticas = new Dictionary<string, int>();
            
            estatisticas["Total"] = await _context.AvaliacoesCompetenciasNotas.CountAsync();
            estatisticas["AutoAvaliacao"] = await _context.AvaliacoesCompetenciasNotas
                .Where(a => a.TipoAvaliacao == "autoavaliacao")
                .CountAsync();
            estatisticas["AvaliacaoGestor"] = await _context.AvaliacoesCompetenciasNotas
                .Where(a => a.TipoAvaliacao == "gestor")
                .CountAsync();
            estatisticas["AvaliacaoAsCegas"] = await _context.AvaliacoesCompetenciasNotas
                .Where(a => a.TipoAvaliacao == "ascegas")
                .CountAsync();
            
            _telemetryService.TrackEvent("EstatisticasAvaliacoesConsultadas", new Dictionary<string, string>
            {
                { "TotalGeral", estatisticas["Total"].ToString() }
            });
            
            return estatisticas;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterEstatisticasAvaliacoesAsync" },
                { "Component", "TotalAvaliacaoService" }
            });
            throw;
        }
    }

    public async Task<bool> ExistemAvaliacoesAsync()
    {
        try
        {
            var existem = await _context.AvaliacoesCompetenciasNotas.AnyAsync();
            
            _telemetryService.TrackEvent("VerificacaoExistenciaAvaliacoes", new Dictionary<string, string>
            {
                { "ExistemAvaliacoes", existem.ToString() }
            });
            
            return existem;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExistemAvaliacoesAsync" },
                { "Component", "TotalAvaliacaoService" }
            });
            throw;
        }
    }
}