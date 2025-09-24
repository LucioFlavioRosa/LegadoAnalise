using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Cargos;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Performance.Common;

public interface IPerformanceComboHelper
{
    Task<List<ComboItem>> GetCargosItemsAsync();
    List<ComboItem> GetStatusItems();
    List<ComboItem> GetAbrangenciaItems();
    Task<List<ComboItem>> GetNotasPadraoItemsAsync();
    List<ComboItem> GetInputCheckboxItems();
    Task<List<ComboItem>> GetCargosItemsAsync(bool includeInactive);
    Task<List<ComboItem>> GetNotasPadraoItemsAsync(bool includeEmpty);
    string GetStatusText(int status);
    string GetAbrangenciaText(string abrangencia);
    string GetInputText(bool input);
}

public class PerformanceComboHelper : IPerformanceComboHelper
{
    private readonly ICargosService _cargosService;
    private readonly IAvaliacoesService _avaliacoesService;
    private readonly ITelemetryService _telemetryService;

    public PerformanceComboHelper(
        ICargosService cargosService,
        IAvaliacoesService avaliacoesService,
        ITelemetryService telemetryService)
    {
        _cargosService = cargosService;
        _avaliacoesService = avaliacoesService;
        _telemetryService = telemetryService;
    }

    public async Task<List<ComboItem>> GetCargosItemsAsync()
    {
        return await GetCargosItemsAsync(false);
    }

    public async Task<List<ComboItem>> GetCargosItemsAsync(bool includeInactive)
    {
        try
        {
            var cargos = await _cargosService.ObterListaCargosAsync(includeInactive);
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "0", Text = "[Selecionar]" }
            };

            items.AddRange(cargos.Select(c => new ComboItem
            {
                Value = c.IdCargo.ToString(),
                Text = c.Nome
            }));

            _telemetryService.TrackEvent("PerformanceCargosComboLoaded", new Dictionary<string, string>
            {
                { "ItemCount", (items.Count - 1).ToString() },
                { "IncludeInactive", includeInactive.ToString() }
            });

            return items;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetCargosItemsAsync" },
                { "Component", "PerformanceComboHelper" }
            });
            return new List<ComboItem> { new ComboItem { Value = "0", Text = "[Erro ao carregar]" } };
        }
    }

    public List<ComboItem> GetStatusItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "Ativo" },
            new ComboItem { Value = "0", Text = "Inativo" }
        };
    }

    public List<ComboItem> GetAbrangenciaItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "Individual", Text = "Individual" },
            new ComboItem { Value = "Coletivo", Text = "Coletivo" }
        };
    }

    public async Task<List<ComboItem>> GetNotasPadraoItemsAsync()
    {
        return await GetNotasPadraoItemsAsync(true);
    }

    public async Task<List<ComboItem>> GetNotasPadraoItemsAsync(bool includeEmpty)
    {
        try
        {
            var notas = await _avaliacoesService.ObterNotasPerformanceAsync();
            var items = new List<ComboItem>();

            if (includeEmpty)
            {
                items.Add(new ComboItem { Value = "0", Text = "[Selecionar]" });
            }

            items.AddRange(notas.Select(n => new ComboItem
            {
                Value = n.IdNota.ToString(),
                Text = n.CodigoNota
            }));

            _telemetryService.TrackEvent("PerformanceNotasComboLoaded", new Dictionary<string, string>
            {
                { "ItemCount", (items.Count - (includeEmpty ? 1 : 0)).ToString() },
                { "IncludeEmpty", includeEmpty.ToString() }
            });

            return items;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetNotasPadraoItemsAsync" },
                { "Component", "PerformanceComboHelper" }
            });
            return new List<ComboItem> { new ComboItem { Value = "0", Text = "[Erro ao carregar]" } };
        }
    }

    public List<ComboItem> GetInputCheckboxItems()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "true", Text = "Habilitado" },
            new ComboItem { Value = "false", Text = "Desabilitado" }
        };
    }

    public string GetStatusText(int status)
    {
        return status switch
        {
            1 => "Ativo",
            0 => "Inativo",
            _ => "Desconhecido"
        };
    }

    public string GetAbrangenciaText(string abrangencia)
    {
        if (string.IsNullOrWhiteSpace(abrangencia))
            return "Não informado";

        return abrangencia switch
        {
            "Individual" => "Individual",
            "Coletivo" => "Coletivo",
            _ => abrangencia
        };
    }

    public string GetInputText(bool input)
    {
        return input ? "Habilitado" : "Desabilitado";
    }

    public static class PerformanceComboConstants
    {
        public static class Status
        {
            public const int Ativo = 1;
            public const int Inativo = 0;
        }

        public static class Abrangencia
        {
            public const string Individual = "Individual";
            public const string Coletivo = "Coletivo";
        }

        public static class Input
        {
            public const bool Habilitado = true;
            public const bool Desabilitado = false;
        }

        public static class DefaultValues
        {
            public const int DefaultStatus = Status.Ativo;
            public const string DefaultAbrangencia = Abrangencia.Individual;
            public const bool DefaultInputAutoAvaliacao = Input.Habilitado;
            public const bool DefaultInputAvaliacaoAsCegas = Input.Habilitado;
            public const bool DefaultInputAvaliacaoGestor = Input.Habilitado;
        }
    }

    public static class PerformanceComboExtensions
    {
        public static ComboItem ToComboItem(this Cargo cargo)
        {
            return new ComboItem
            {
                Value = cargo.IdCargo.ToString(),
                Text = cargo.Nome
            };
        }

        public static ComboItem ToComboItem(this AvaliacaoPerformanceNota nota)
        {
            return new ComboItem
            {
                Value = nota.IdNota.ToString(),
                Text = nota.CodigoNota
            };
        }

        public static List<ComboItem> ToComboItems<T>(this IEnumerable<T> items, Func<T, string> valueSelector, Func<T, string> textSelector)
        {
            return items.Select(item => new ComboItem
            {
                Value = valueSelector(item),
                Text = textSelector(item)
            }).ToList();
        }
    }
}