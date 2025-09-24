using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Performance.Common;

public interface IPerformanceComboHelper
{
    Task<List<ComboItem>> GetCargosAsync();
    Task<List<ComboItem>> GetStatusOptionsAsync();
    Task<List<ComboItem>> GetAbrangenciaOptionsAsync();
    Task<List<ComboItem>> GetNotasPadraoAsync();
    string FormatStatusDisplay(int status);
    string FormatAbrangenciaDisplay(string abrangencia);
}

public class PerformanceComboHelper : IPerformanceComboHelper
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public PerformanceComboHelper(
        ApplicationDbContext context,
        ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<List<ComboItem>> GetCargosAsync()
    {
        try
        {
            var cargos = await _context.Cargos
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new ComboItem
                {
                    Value = c.IdCargo.ToString(),
                    Text = c.Nome
                })
                .ToListAsync();

            cargos.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });

            _telemetryService.TrackEvent("PerformanceCargosLoaded", new Dictionary<string, string>
            {
                { "Count", (cargos.Count - 1).ToString() }
            });

            return cargos;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetCargosAsync" }
            });
            return new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        }
    }

    public async Task<List<ComboItem>> GetStatusOptionsAsync()
    {
        await Task.CompletedTask;
        
        return new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
    }

    public async Task<List<ComboItem>> GetAbrangenciaOptionsAsync()
    {
        await Task.CompletedTask;
        
        return new List<ComboItem>
        {
            new ComboItem { Value = "Individual", Text = "Individual" },
            new ComboItem { Value = "Coletivo", Text = "Coletivo" }
        };
    }

    public async Task<List<ComboItem>> GetNotasPadraoAsync()
    {
        try
        {
            var notas = await _context.Set<AvaliacaoCompetenciaNota>()
                .OrderBy(n => n.CodigoNota)
                .Select(n => new ComboItem
                {
                    Value = n.IdNota.ToString(),
                    Text = n.CodigoNota
                })
                .ToListAsync();

            notas.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });

            _telemetryService.TrackEvent("PerformanceNotasLoaded", new Dictionary<string, string>
            {
                { "Count", (notas.Count - 1).ToString() }
            });

            return notas;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetNotasPadraoAsync" }
            });
            return new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        }
    }

    public string FormatStatusDisplay(int status)
    {
        return status == 1 ? "Ativo" : "Inativo";
    }

    public string FormatAbrangenciaDisplay(string abrangencia)
    {
        return string.IsNullOrWhiteSpace(abrangencia) ? "-" : abrangencia;
    }
}

public class PerformanceComboOptions
{
    public static class Status
    {
        public const int Ativo = 1;
        public const int Inativo = 0;
        
        public static string GetDisplayText(int status)
        {
            return status == Ativo ? "Ativo" : "Inativo";
        }
    }

    public static class Abrangencia
    {
        public const string Individual = "Individual";
        public const string Coletivo = "Coletivo";
        
        public static List<string> GetAll()
        {
            return new List<string> { Individual, Coletivo };
        }
    }

    public static class InputTypes
    {
        public const bool Habilitado = true;
        public const bool Desabilitado = false;
        
        public static string GetDisplayText(bool input)
        {
            return input ? "Habilitado" : "Desabilitado";
        }
    }
}