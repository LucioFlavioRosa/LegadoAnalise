using Peers.Moderno.Services.Common;
using Microsoft.Extensions.Configuration;

namespace Peers.Moderno.Services.AutoAvaliacao.Common;

public interface IAccordionHelper
{
    string TruncarTexto(string? texto, int tamanhoMaximo);
    string GetTextoExpandido(string? texto, int tamanhoMaximo, string textoVerMais = "Ver+");
    bool DeveExibirBotaoExpandir(string? texto, int tamanhoMaximo);
    string GetIdAccordion(string prefixo, int id);
    string GetClasseAccordion(bool isExpanded = false);
    string GetAriaExpanded(bool isExpanded = false);
    AccordionConfig GetAccordionConfig();
    Dictionary<string, object> GetAccordionAttributes(string targetId, bool isExpanded = false);
    string FormatarTextoComPrefixo(string? texto, string prefixo);
    bool IsTextoVazio(string? texto);
    int GetTamanhoMaximoPadrao();
    string GetTextoVerMaisPadrao();
}

public class AccordionHelper : IAccordionHelper
{
    private readonly IConfiguration _configuration;
    private readonly ITelemetryService _telemetryService;

    public AccordionHelper(
        IConfiguration configuration,
        ITelemetryService telemetryService)
    {
        _configuration = configuration;
        _telemetryService = telemetryService;
    }

    public string TruncarTexto(string? texto, int tamanhoMaximo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            if (texto.Length <= tamanhoMaximo)
                return texto;

            return texto.Substring(0, tamanhoMaximo) + "...";
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "TruncarTexto" },
                { "Component", "AccordionHelper" },
                { "TamanhoMaximo", tamanhoMaximo.ToString() }
            });
            return texto ?? string.Empty;
        }
    }

    public string GetTextoExpandido(string? texto, int tamanhoMaximo, string textoVerMais = "Ver+")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            if (texto.Length <= tamanhoMaximo)
                return texto;

            var textoTruncado = TruncarTexto(texto, tamanhoMaximo);
            return $"{textoTruncado} <span style='font-weight:bold'>{textoVerMais}</span>";
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetTextoExpandido" },
                { "Component", "AccordionHelper" },
                { "TamanhoMaximo", tamanhoMaximo.ToString() }
            });
            return texto ?? string.Empty;
        }
    }

    public bool DeveExibirBotaoExpandir(string? texto, int tamanhoMaximo)
    {
        return !string.IsNullOrWhiteSpace(texto) && texto.Length > tamanhoMaximo;
    }

    public string GetIdAccordion(string prefixo, int id)
    {
        return $"{prefixo}_{id}";
    }

    public string GetClasseAccordion(bool isExpanded = false)
    {
        return isExpanded ? "accordian-body collapse show" : "accordian-body collapse";
    }

    public string GetAriaExpanded(bool isExpanded = false)
    {
        return isExpanded.ToString().ToLower();
    }

    public AccordionConfig GetAccordionConfig()
    {
        try
        {
            return new AccordionConfig
            {
                TamanhoMaximo = _configuration.GetValue<int>("AutoAvaliacao:MaxCompetenciaLength", 70),
                TextoVerMais = _configuration.GetValue<string>("AutoAvaliacao:VerMaisTexto", "Ver+") ?? "Ver+",
                HabilitarAccordions = _configuration.GetValue<bool>("AutoAvaliacao:ComponenteConfig:CompetenciaTable:EnableAccordions", true),
                ExibirBotaoExpandir = _configuration.GetValue<bool>("AutoAvaliacao:ComponenteConfig:CompetenciaTable:ShowExpandButton", true),
                HabilitarCabecalhoFixo = _configuration.GetValue<bool>("AutoAvaliacao:ComponenteConfig:CompetenciaTable:EnableStickyHeaders", true)
            };
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetAccordionConfig" },
                { "Component", "AccordionHelper" }
            });
            
            return new AccordionConfig
            {
                TamanhoMaximo = 70,
                TextoVerMais = "Ver+",
                HabilitarAccordions = true,
                ExibirBotaoExpandir = true,
                HabilitarCabecalhoFixo = true
            };
        }
    }

    public Dictionary<string, object> GetAccordionAttributes(string targetId, bool isExpanded = false)
    {
        return new Dictionary<string, object>
        {
            { "data-toggle", "collapse" },
            { "data-target", $"#{targetId}" },
            { "aria-expanded", GetAriaExpanded(isExpanded) },
            { "aria-controls", targetId },
            { "class", "accordion-toggle" }
        };
    }

    public string FormatarTextoComPrefixo(string? texto, string prefixo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return $"[{prefixo}] {texto}";
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "FormatarTextoComPrefixo" },
                { "Component", "AccordionHelper" },
                { "Prefixo", prefixo }
            });
            return texto ?? string.Empty;
        }
    }

    public bool IsTextoVazio(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto);
    }

    public int GetTamanhoMaximoPadrao()
    {
        return _configuration.GetValue<int>("AutoAvaliacao:MaxCompetenciaLength", 70);
    }

    public string GetTextoVerMaisPadrao()
    {
        return _configuration.GetValue<string>("AutoAvaliacao:VerMaisTexto", "Ver+") ?? "Ver+";
    }
}

public class AccordionConfig
{
    public int TamanhoMaximo { get; set; } = 70;
    public string TextoVerMais { get; set; } = "Ver+";
    public bool HabilitarAccordions { get; set; } = true;
    public bool ExibirBotaoExpandir { get; set; } = true;
    public bool HabilitarCabecalhoFixo { get; set; } = true;
}

public static class AccordionExtensions
{
    public static string ToAccordionId(this string prefixo, int id)
    {
        return $"{prefixo}_{id}";
    }

    public static string ToAccordionTarget(this string id)
    {
        return $"#{id}";
    }

    public static bool ShouldTruncate(this string? texto, int tamanhoMaximo)
    {
        return !string.IsNullOrWhiteSpace(texto) && texto.Length > tamanhoMaximo;
    }

    public static string TruncateWithEllipsis(this string? texto, int tamanhoMaximo)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Length <= tamanhoMaximo)
            return texto ?? string.Empty;

        return texto.Substring(0, tamanhoMaximo) + "...";
    }

    public static Dictionary<string, object> ToBootstrapAccordionAttributes(this string targetId, bool isExpanded = false)
    {
        return new Dictionary<string, object>
        {
            { "data-toggle", "collapse" },
            { "data-target", targetId.StartsWith("#") ? targetId : $"#{targetId}" },
            { "aria-expanded", isExpanded.ToString().ToLower() },
            { "aria-controls", targetId.TrimStart('#') },
            { "class", "accordion-toggle" }
        };
    }
}