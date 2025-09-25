using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.AutoAvaliacao.Common;

public interface INotaHelper
{
    List<ComboItem> GetNotasPerformanceItems(bool includeSelecionar = true);
    List<ComboItem> GetNotasCompetenciaItems(bool includeSelecionar = true);
    bool IsNotaValida(int? nota);
    bool IsNotaNaoSeAplica(int? nota);
    bool IsNotaSelecionar(int? nota);
    string GetNotaTexto(int? nota);
    string GetNotaCssClass(int? nota);
    bool ValidarNotaObrigatoria(int? nota, bool isRequired = true);
    bool ValidarNotasConsistencia(int? notaNivel1, int? notaNivel2);
    void AjustarNotaNaoSeAplica(ref int? notaNivel1, ref int? notaNivel2);
    bool HasNotasMensuravelPorPilar(List<int?> notasNivel1, List<int?> notasNivel2);
    bool ValidarPreenchimentoCompleto(List<int?> notas, bool allowNaoSeAplica = true);
    int GetNotaPadraoAutoAvaliacao();
    int GetNotaPadraoAvaliacaoAsCegas();
    int GetNotaPadraoAvaliacaoGestor();
    int GetIdNotaNaoSeAplica();
    int GetIdNotaSelecionar();
}

public class NotaHelper : INotaHelper
{
    private readonly ITelemetryService _telemetryService;
    private readonly IConfiguration _configuration;
    
    private const int ID_NOTA_NAO_SE_APLICA = 5;
    private const int ID_NOTA_SELECIONAR = 0;
    private const int NOTA_PADRAO_AUTO_AVALIACAO = 1;
    private const int NOTA_PADRAO_AVALIACAO_AS_CEGAS = 1;
    private const int NOTA_PADRAO_AVALIACAO_GESTOR = 1;

    public NotaHelper(ITelemetryService telemetryService, IConfiguration configuration)
    {
        _telemetryService = telemetryService;
        _configuration = configuration;
    }

    public List<ComboItem> GetNotasPerformanceItems(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "0", Text = "[Selecionar]", IsDisabled = true });
        }
        
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "1" },
            new ComboItem { Value = "2", Text = "2" },
            new ComboItem { Value = "3", Text = "3" },
            new ComboItem { Value = "4", Text = "4" },
            new ComboItem { Value = "5", Text = "Não se aplica" }
        });
        
        return items;
    }

    public List<ComboItem> GetNotasCompetenciaItems(bool includeSelecionar = true)
    {
        var items = new List<ComboItem>();
        
        if (includeSelecionar)
        {
            items.Add(new ComboItem { Value = "0", Text = "[Selecionar]", IsDisabled = true });
        }
        
        items.AddRange(new List<ComboItem>
        {
            new ComboItem { Value = "1", Text = "1" },
            new ComboItem { Value = "2", Text = "2" },
            new ComboItem { Value = "3", Text = "3" },
            new ComboItem { Value = "4", Text = "4" },
            new ComboItem { Value = "5", Text = "Não se aplica" }
        });
        
        return items;
    }

    public bool IsNotaValida(int? nota)
    {
        return nota.HasValue && nota.Value >= 1 && nota.Value <= 5;
    }

    public bool IsNotaNaoSeAplica(int? nota)
    {
        return nota.HasValue && nota.Value == ID_NOTA_NAO_SE_APLICA;
    }

    public bool IsNotaSelecionar(int? nota)
    {
        return !nota.HasValue || nota.Value == ID_NOTA_SELECIONAR;
    }

    public string GetNotaTexto(int? nota)
    {
        if (!nota.HasValue || nota.Value == ID_NOTA_SELECIONAR)
            return "[Selecionar]";
            
        return nota.Value == ID_NOTA_NAO_SE_APLICA ? "Não se aplica" : nota.Value.ToString();
    }

    public string GetNotaCssClass(int? nota)
    {
        if (!IsNotaValida(nota))
            return "text-muted";
            
        return nota.Value switch
        {
            1 => "text-danger",
            2 => "text-warning",
            3 => "text-info",
            4 => "text-success",
            5 => "text-secondary",
            _ => "text-muted"
        };
    }

    public bool ValidarNotaObrigatoria(int? nota, bool isRequired = true)
    {
        if (!isRequired)
            return true;
            
        return IsNotaValida(nota);
    }

    public bool ValidarNotasConsistencia(int? notaNivel1, int? notaNivel2)
    {
        if (!IsNotaValida(notaNivel1) || !IsNotaValida(notaNivel2))
            return true;
            
        if (IsNotaNaoSeAplica(notaNivel1))
            return IsNotaNaoSeAplica(notaNivel2);
            
        return notaNivel2.Value <= notaNivel1.Value;
    }

    public void AjustarNotaNaoSeAplica(ref int? notaNivel1, ref int? notaNivel2)
    {
        if (IsNotaNaoSeAplica(notaNivel1) && !IsNotaNaoSeAplica(notaNivel2))
        {
            notaNivel2 = ID_NOTA_NAO_SE_APLICA;
            
            _telemetryService.TrackEvent("NotaAjustadaNaoSeAplica", new Dictionary<string, string>
            {
                { "NotaNivel1", notaNivel1.ToString() },
                { "NotaNivel2Original", notaNivel2.ToString() },
                { "NotaNivel2Ajustada", ID_NOTA_NAO_SE_APLICA.ToString() }
            });
        }
    }

    public bool HasNotasMensuravelPorPilar(List<int?> notasNivel1, List<int?> notasNivel2)
    {
        var notasMensuravelNivel1 = notasNivel1.Count(n => IsNotaValida(n) && !IsNotaNaoSeAplica(n));
        var notasMensuravelNivel2 = notasNivel2.Count(n => IsNotaValida(n) && !IsNotaNaoSeAplica(n));
        
        return notasMensuravelNivel1 > 0 && notasMensuravelNivel2 > 0;
    }

    public bool ValidarPreenchimentoCompleto(List<int?> notas, bool allowNaoSeAplica = true)
    {
        foreach (var nota in notas)
        {
            if (IsNotaSelecionar(nota))
                return false;
                
            if (!allowNaoSeAplica && IsNotaNaoSeAplica(nota))
                return false;
        }
        
        return true;
    }

    public int GetNotaPadraoAutoAvaliacao()
    {
        return _configuration.GetValue("AutoAvaliacao:NotasPadrao:AutoAvaliacao", NOTA_PADRAO_AUTO_AVALIACAO);
    }

    public int GetNotaPadraoAvaliacaoAsCegas()
    {
        return _configuration.GetValue("AutoAvaliacao:NotasPadrao:AvaliacaoAsCegas", NOTA_PADRAO_AVALIACAO_AS_CEGAS);
    }

    public int GetNotaPadraoAvaliacaoGestor()
    {
        return _configuration.GetValue("AutoAvaliacao:NotasPadrao:AvaliacaoGestor", NOTA_PADRAO_AVALIACAO_GESTOR);
    }

    public int GetIdNotaNaoSeAplica()
    {
        return _configuration.GetValue("AutoAvaliacao:NotasPadrao:IdNotaNaoSeAplica", ID_NOTA_NAO_SE_APLICA);
    }

    public int GetIdNotaSelecionar()
    {
        return _configuration.GetValue("AutoAvaliacao:NotasPadrao:IdNotaSelecionar", ID_NOTA_SELECIONAR);
    }
}