using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Performance.Common;
using System.Data;
using OfficeOpenXml;

namespace Peers.Moderno.Services.Performance;

public interface IPerformanceService
{
    Task<List<Models.Performance>> ObterListaPerformancesAsync();
    Task<Models.Performance?> ObterPerformanceAsync(int id);
    Task<bool> InserirPerformanceAsync(Models.Performance performance);
    Task<bool> AlterarPerformanceAsync(Models.Performance performance);
    Task<bool> ExcluirPerformanceAsync(int id);
    Task<byte[]> ExportarPerformancesAsync();
    Task<PerformanceImportResult> ImportarPerformancesAsync(Stream fileStream, int userId);
}

public class PerformanceService : IPerformanceService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IPerformanceImportExportUtil _importExportUtil;
    private readonly IPerformanceValidationUtil _validationUtil;

    public PerformanceService(
        ApplicationDbContext context,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService,
        IPerformanceImportExportUtil importExportUtil,
        IPerformanceValidationUtil validationUtil)
    {
        _context = context;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
        _importExportUtil = importExportUtil;
        _validationUtil = validationUtil;
    }

    public async Task<List<Models.Performance>> ObterListaPerformancesAsync()
    {
        try
        {
            var performances = await _context.Set<Models.Performance>()
                .Include(p => p.Cargo)
                .Include(p => p.CargoNivel)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            _telemetryService.TrackEvent("PerformanceListLoaded", new Dictionary<string, string>
            {
                { "Count", performances.Count.ToString() }
            });

            return performances;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterListaPerformancesAsync" }
            });
            throw;
        }
    }

    public async Task<Models.Performance?> ObterPerformanceAsync(int id)
    {
        try
        {
            var performance = await _context.Set<Models.Performance>()
                .Include(p => p.Cargo)
                .Include(p => p.CargoNivel)
                .FirstOrDefaultAsync(p => p.IdPerformance == id);

            if (performance != null)
            {
                _telemetryService.TrackEvent("PerformanceLoaded", new Dictionary<string, string>
                {
                    { "PerformanceId", id.ToString() }
                });
            }

            return performance;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPerformanceAsync" },
                { "PerformanceId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InserirPerformanceAsync(Models.Performance performance)
    {
        try
        {
            var validationResult = await _validationUtil.ValidatePerformanceAsync(performance);
            if (!validationResult.IsValid)
            {
                _messageBoxService.ShowWarning(validationResult.ErrorMessage);
                return false;
            }

            performance.DHC = DateTime.Now;
            _context.Set<Models.Performance>().Add(performance);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerformanceCreated", new Dictionary<string, string>
                {
                    { "PerformanceId", performance.IdPerformance.ToString() },
                    { "UserId", performance.USR.ToString() }
                });
                _messageBoxService.ShowSuccess("Performance inserida com sucesso!");
            }
            else
            {
                _messageBoxService.ShowError("Erro ao inserir performance");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InserirPerformanceAsync" },
                { "UserId", performance.USR.ToString() }
            });
            _messageBoxService.ShowError("Erro interno ao inserir performance");
            return false;
        }
    }

    public async Task<bool> AlterarPerformanceAsync(Models.Performance performance)
    {
        try
        {
            var validationResult = await _validationUtil.ValidatePerformanceAsync(performance);
            if (!validationResult.IsValid)
            {
                _messageBoxService.ShowWarning(validationResult.ErrorMessage);
                return false;
            }

            var existingPerformance = await _context.Set<Models.Performance>()
                .FirstOrDefaultAsync(p => p.IdPerformance == performance.IdPerformance);

            if (existingPerformance == null)
            {
                _messageBoxService.ShowWarning("Performance não encontrada");
                return false;
            }

            existingPerformance.Nome = performance.Nome;
            existingPerformance.PerformanceAbaixo = performance.PerformanceAbaixo;
            existingPerformance.PerformanceEsperado = performance.PerformanceEsperado;
            existingPerformance.PerformanceAcima = performance.PerformanceAcima;
            existingPerformance.Abrangencia = performance.Abrangencia;
            existingPerformance.IdCargo = performance.IdCargo;
            existingPerformance.ATV = performance.ATV;
            existingPerformance.InputAutoavaliacao = performance.InputAutoavaliacao;
            existingPerformance.NotaPadraoAutoAvaliacao = performance.NotaPadraoAutoAvaliacao;
            existingPerformance.InputAvaliacaoAsCegas = performance.InputAvaliacaoAsCegas;
            existingPerformance.NotaPadraoAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas;
            existingPerformance.InputAvaliacaoGestor = performance.InputAvaliacaoGestor;
            existingPerformance.NotaPadraoAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor;
            existingPerformance.USR = performance.USR;
            existingPerformance.DHC = DateTime.Now;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerformanceUpdated", new Dictionary<string, string>
                {
                    { "PerformanceId", performance.IdPerformance.ToString() },
                    { "UserId", performance.USR.ToString() }
                });
                _messageBoxService.ShowSuccess("Performance alterada com sucesso!");
            }
            else
            {
                _messageBoxService.ShowError("Erro ao alterar performance");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AlterarPerformanceAsync" },
                { "PerformanceId", performance.IdPerformance.ToString() }
            });
            _messageBoxService.ShowError("Erro interno ao alterar performance");
            return false;
        }
    }

    public async Task<bool> ExcluirPerformanceAsync(int id)
    {
        try
        {
            var performance = await _context.Set<Models.Performance>()
                .FirstOrDefaultAsync(p => p.IdPerformance == id);

            if (performance == null)
            {
                _messageBoxService.ShowWarning("Performance não encontrada");
                return false;
            }

            performance.ATV = 0;
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerformanceDeactivated", new Dictionary<string, string>
                {
                    { "PerformanceId", id.ToString() }
                });
                _messageBoxService.ShowSuccess("Performance inativada com sucesso!");
            }
            else
            {
                _messageBoxService.ShowError("Erro ao inativar performance");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExcluirPerformanceAsync" },
                { "PerformanceId", id.ToString() }
            });
            _messageBoxService.ShowError("Erro interno ao inativar performance");
            return false;
        }
    }

    public async Task<byte[]> ExportarPerformancesAsync()
    {
        try
        {
            var performances = await ObterListaPerformancesAsync();
            var exportData = await _importExportUtil.PrepareExportDataAsync(performances);
            var result = _importExportUtil.GenerateExcelFile(exportData);

            _telemetryService.TrackEvent("PerformanceExported", new Dictionary<string, string>
            {
                { "Count", performances.Count.ToString() }
            });

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExportarPerformancesAsync" }
            });
            throw;
        }
    }

    public async Task<PerformanceImportResult> ImportarPerformancesAsync(Stream fileStream, int userId)
    {
        try
        {
            var importData = _importExportUtil.ReadExcelFile(fileStream);
            var result = await _importExportUtil.ProcessImportDataAsync(importData, userId);

            _telemetryService.TrackEvent("PerformanceImported", new Dictionary<string, string>
            {
                { "LinhasInseridas", result.LinhasInseridas.ToString() },
                { "LinhasAlteradas", result.LinhasAlteradas.ToString() },
                { "LinhasDesconsideradas", result.LinhasDesconsideradas.ToString() },
                { "UserId", userId.ToString() }
            });

            if (result.Sucesso)
            {
                _messageBoxService.ShowSuccess($"Performances importadas com sucesso<br>Inseridas: {result.LinhasInseridas}<br>Alteradas: {result.LinhasAlteradas}<br>Desconsideradas: {result.LinhasDesconsideradas}");
            }
            else
            {
                _messageBoxService.ShowError($"Erro na importação: {result.ErrorMessage}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ImportarPerformancesAsync" },
                { "UserId", userId.ToString() }
            });
            
            var errorResult = new PerformanceImportResult();
            errorResult.Erros.Add($"Erro interno na importação: {ex.Message}");
            _messageBoxService.ShowError(errorResult.ErrorMessage);
            return errorResult;
        }
    }
}