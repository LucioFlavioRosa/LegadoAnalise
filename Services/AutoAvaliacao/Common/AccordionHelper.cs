using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.AutoAvaliacao.Common;

public interface IAccordionHelper
{
    string TruncarTexto(string texto, int maxLength);
    string GerarIdAccordion(string prefixo, int id);
    string GerarLinkAccordion(string texto, string targetId, int maxLength);
    AccordionConfig GetAccordionConfig();
    bool ShouldShowExpandButton(string texto, int maxLength);
    string GetExpandButtonText();
    string FormatarTextoCompleto(string prefixo, string texto);
    AccordionItem CriarAccordionItem(string id, string titulo, string conteudo, bool isExpanded = false);
    List<AccordionItem> CriarAccordionsPerformance(int idPerformance, string abaixo, string esperado, string acima);
}

public class AccordionHelper : IAccordionHelper
{
    private readonly IConfiguration _configuration;
    private readonly ITelemetryService _telemetryService;

    public AccordionHelper(IConfiguration configuration, ITelemetryService telemetryService)
    {
        _configuration = configuration;
        _telemetryService = telemetryService;
    }

    public string TruncarTexto(string texto, int maxLength)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;

        if (texto.Length <= maxLength)
            return texto;

        return texto.Substring(0, maxLength) + "...";
    }

    public string GerarIdAccordion(string prefixo, int id)
    {
        return $"{prefixo}_{id}";
    }

    public string GerarLinkAccordion(string texto, string targetId, int maxLength)
    {
        var textoTruncado = TruncarTexto(texto, maxLength);
        var showExpandButton = ShouldShowExpandButton(texto, maxLength);
        
        if (showExpandButton)
        {
            return $"{textoTruncado} <span style='font-weight:bold'>{GetExpandButtonText()}</span>";
        }
        
        return textoTruncado;
    }

    public AccordionConfig GetAccordionConfig()
    {
        return new AccordionConfig
        {
            MaxTextLength = _configuration.GetValue("AutoAvaliacao:MaxCompetenciaLength", 70),
            ShowExpandButton = _configuration.GetValue("AutoAvaliacao:ShowVerMaisTexto", true),
            ExpandButtonText = _configuration.GetValue("AutoAvaliacao:VerMaisTexto", "Ver+"),
            EnableAccordions = _configuration.GetValue("AutoAvaliacao:ComponenteConfig:CompetenciaTable:EnableAccordions", true),
            EnableStickyHeaders = _configuration.GetValue("AutoAvaliacao:ComponenteConfig:CompetenciaTable:EnableStickyHeaders", true)
        };
    }

    public bool ShouldShowExpandButton(string texto, int maxLength)
    {
        if (string.IsNullOrEmpty(texto))
            return false;
            
        return texto.Length > maxLength && GetAccordionConfig().ShowExpandButton;
    }

    public string GetExpandButtonText()
    {
        return GetAccordionConfig().ExpandButtonText;
    }

    public string FormatarTextoCompleto(string prefixo, string texto)
    {
        if (string.IsNullOrEmpty(prefixo) || string.IsNullOrEmpty(texto))
            return texto ?? string.Empty;
            
        return $"[{prefixo}] {texto}";
    }

    public AccordionItem CriarAccordionItem(string id, string titulo, string conteudo, bool isExpanded = false)
    {
        var config = GetAccordionConfig();
        
        return new AccordionItem
        {
            Id = id,
            Titulo = titulo,
            TituloTruncado = TruncarTexto(titulo, config.MaxTextLength),
            Conteudo = conteudo,
            IsExpanded = isExpanded,
            ShowExpandButton = ShouldShowExpandButton(titulo, config.MaxTextLength),
            ExpandButtonText = GetExpandButtonText()
        };
    }

    public List<AccordionItem> CriarAccordionsPerformance(int idPerformance, string abaixo, string esperado, string acima)
    {
        var items = new List<AccordionItem>();
        var config = GetAccordionConfig();

        if (!string.IsNullOrEmpty(abaixo))
        {
            items.Add(new AccordionItem
            {
                Id = GerarIdAccordion("abaixo", idPerformance),
                Titulo = TruncarTexto(abaixo, config.MaxTextLength),
                Conteudo = FormatarTextoCompleto("Abaixo", abaixo),
                ShowExpandButton = ShouldShowExpandButton(abaixo, config.MaxTextLength),
                ExpandButtonText = GetExpandButtonText()
            });
        }

        if (!string.IsNullOrEmpty(esperado))
        {
            items.Add(new AccordionItem
            {
                Id = GerarIdAccordion("esperado", idPerformance),
                Titulo = TruncarTexto(esperado, config.MaxTextLength),
                Conteudo = FormatarTextoCompleto("Esperado", esperado),
                ShowExpandButton = ShouldShowExpandButton(esperado, config.MaxTextLength),
                ExpandButtonText = GetExpandButtonText()
            });
        }

        if (!string.IsNullOrEmpty(acima))
        {
            items.Add(new AccordionItem
            {
                Id = GerarIdAccordion("acima", idPerformance),
                Titulo = TruncarTexto(acima, config.MaxTextLength),
                Conteudo = FormatarTextoCompleto("Acima", acima),
                ShowExpandButton = ShouldShowExpandButton(acima, config.MaxTextLength),
                ExpandButtonText = GetExpandButtonText()
            });
        }

        _telemetryService.TrackEvent("AccordionItemsCreated", new Dictionary<string, string>
        {
            { "IdPerformance", idPerformance.ToString() },
            { "ItemsCount", items.Count.ToString() }
        });

        return items;
    }
}

public class AccordionConfig
{
    public int MaxTextLength { get; set; } = 70;
    public bool ShowExpandButton { get; set; } = true;
    public string ExpandButtonText { get; set; } = "Ver+";
    public bool EnableAccordions { get; set; } = true;
    public bool EnableStickyHeaders { get; set; } = true;
}

public class AccordionItem
{
    public string Id { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string TituloTruncado { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public bool IsExpanded { get; set; } = false;
    public bool ShowExpandButton { get; set; } = false;
    public string ExpandButtonText { get; set; } = "Ver+";
    public string CssClass { get; set; } = string.Empty;
    public string TargetId => $"#{Id}";
    public string DataTarget => $"#{Id}";
    public string AriaExpanded => IsExpanded ? "true" : "false";
    public string CollapseClass => IsExpanded ? "collapse show" : "collapse";
}