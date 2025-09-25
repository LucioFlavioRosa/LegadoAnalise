using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Microsoft.Extensions.Configuration;

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
    bool ValidarNotasConsistencia(int? notaNivel1, int? notaNivel2);
    bool ValidarNotasObrigatorias(List<int?> notas);
    bool ValidarPilarPreenchido(List<int?> notas);
    int GetNotaPadraoAutoAvaliacao();
    int GetNotaPadraoAvaliacaoAsCegas();
    int GetNotaPadraoAvaliacaoGestor();
    int GetIdNotaNaoSeAplica();
    int GetIdNotaSelecionar();
    void RemoverOpcaoSelecionar(List<ComboItem> items);
    bool ShouldRemoveSelecionar(int? notaAtual);
    Dictionary<string, object> GetNotaValidationResult(List<int?> notas, string contexto);
}

public class NotaHelper : INotaHelper
{
    private readonly IConfiguration _configuration;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;

    public NotaHelper(
        IConfiguration configuration,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService)
    {
        _configuration = configuration;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
    }

    public List<ComboItem> GetNotasPerformanceItems(bool includeSelecionar = true)
    {
        try
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
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetNotasPerformanceItems" },
                { "Component", "NotaHelper" }
            });
            return new List<ComboItem>();
        }
    }

    public List<ComboItem> GetNotasCompetenciaItems(bool includeSelecionar = true)
    {
        try
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
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetNotasCompetenciaItems" },
                { "Component", "NotaHelper" }
            });
            return new List<ComboItem>();
        }
    }

    public bool IsNotaValida(int? nota)
    {
        return nota.HasValue && nota.Value >= 1 && nota.Value <= 5;
    }

    public bool IsNotaNaoSeAplica(int? nota)
    {
        return nota.HasValue && nota.Value == GetIdNotaNaoSeAplica();
    }

    public bool IsNotaSelecionar(int? nota)
    {
        return !nota.HasValue || nota.Value == GetIdNotaSelecionar();
    }

    public string GetNotaTexto(int? nota)
    {
        if (!nota.HasValue || nota.Value == 0)
            return "[Selecionar]";

        return nota.Value switch
        {
            1 => "1",
            2 => "2",
            3 => "3",
            4 => "4",
            5 => "Não se aplica",
            _ => "[Selecionar]"
        };
    }

    public string GetNotaCssClass(int? nota)
    {
        if (!nota.HasValue || nota.Value == 0)
            return "form-control text-muted";

        return nota.Value switch
        {
            1 => "form-control text-danger",
            2 => "form-control text-warning",
            3 => "form-control text-info",
            4 => "form-control text-success",
            5 => "form-control text-secondary",
            _ => "form-control"
        };
    }

    public bool ValidarNotasConsistencia(int? notaNivel1, int? notaNivel2)
    {
        try
        {
            if (!notaNivel1.HasValue || !notaNivel2.HasValue)
                return true;

            if (IsNotaNaoSeAplica(notaNivel1) && !IsNotaNaoSeAplica(notaNivel2))
            {
                _messageBoxService.ShowWarning(_configuration["AutoAvaliacao:ValidationMessages:NotaNaoSeAplicaInconsistente"] ?? 
                    "ATENÇÃO ! Avaliação de Competência com inconsistência. (Detalhe: quando a nota de Competência do Nivel Atual for = Não Se Aplica, o Próximo Nivel deve ser Não Se Aplica). O Sistema ajustou sua avaliação. Favor revalidar sua avaliação !!");
                return false;
            }

            if (notaNivel2.Value > notaNivel1.Value && !IsNotaNaoSeAplica(notaNivel1) && !IsNotaNaoSeAplica(notaNivel2))
            {
                _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:NotasInconsistentes"] ?? 
                    "Existem Avaliações de Competências inconsistentes. A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarNotasConsistencia" },
                { "Component", "NotaHelper" },
                { "NotaNivel1", notaNivel1?.ToString() ?? "null" },
                { "NotaNivel2", notaNivel2?.ToString() ?? "null" }
            });
            return false;
        }
    }

    public bool ValidarNotasObrigatorias(List<int?> notas)
    {
        try
        {
            var notasInvalidas = notas.Where(n => IsNotaSelecionar(n)).Count();
            
            if (notasInvalidas > 0)
            {
                _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:NotasObrigatorias"] ?? 
                    "É obrigatório selecionar uma nota para cada nível.");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarNotasObrigatorias" },
                { "Component", "NotaHelper" },
                { "TotalNotas", notas.Count.ToString() }
            });
            return false;
        }
    }

    public bool ValidarPilarPreenchido(List<int?> notas)
    {
        try
        {
            var notasMensuráveis = notas.Where(n => IsNotaValida(n) && !IsNotaNaoSeAplica(n)).Count();
            
            if (notasMensuráveis == 0)
            {
                _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:PilarVazio"] ?? 
                    "Cada pilar precisa receber ao mínimo 1 nota mensurável em cada nível (diferente de não se aplica)");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPilarPreenchido" },
                { "Component", "NotaHelper" },
                { "TotalNotas", notas.Count.ToString() }
            });
            return false;
        }
    }

    public int GetNotaPadraoAutoAvaliacao()
    {
        return _configuration.GetValue<int>("AutoAvaliacao:NotasPadrao:NotaPadraoAutoAvaliacao", 1);
    }

    public int GetNotaPadraoAvaliacaoAsCegas()
    {
        return _configuration.GetValue<int>("AutoAvaliacao:NotasPadrao:NotaPadraoAvaliacaoAsCegas", 1);
    }

    public int GetNotaPadraoAvaliacaoGestor()
    {
        return _configuration.GetValue<int>("AutoAvaliacao:NotasPadrao:NotaPadraoAvaliacaoGestor", 1);
    }

    public int GetIdNotaNaoSeAplica()
    {
        return _configuration.GetValue<int>("AutoAvaliacao:NotasPadrao:IdNotaNaoSeAplica", 5);
    }

    public int GetIdNotaSelecionar()
    {
        return _configuration.GetValue<int>("AutoAvaliacao:NotasPadrao:IdNotaSelecionar", 0);
    }

    public void RemoverOpcaoSelecionar(List<ComboItem> items)
    {
        try
        {
            var selecionarItem = items.FirstOrDefault(i => i.Value == "0" && i.Text == "[Selecionar]");
            if (selecionarItem != null)
            {
                items.Remove(selecionarItem);
            }
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "RemoverOpcaoSelecionar" },
                { "Component", "NotaHelper" }
            });
        }
    }

    public bool ShouldRemoveSelecionar(int? notaAtual)
    {
        return IsNotaValida(notaAtual) && !IsNotaSelecionar(notaAtual);
    }

    public Dictionary<string, object> GetNotaValidationResult(List<int?> notas, string contexto)
    {
        try
        {
            var result = new Dictionary<string, object>
            {
                { "IsValid", true },
                { "Errors", new List<string>() },
                { "Warnings", new List<string>() },
                { "Context", contexto }
            };

            var errors = (List<string>)result["Errors"];
            var warnings = (List<string>)result["Warnings"];

            if (!ValidarNotasObrigatorias(notas))
            {
                result["IsValid"] = false;
                errors.Add("Notas obrigatórias não preenchidas");
            }

            if (!ValidarPilarPreenchido(notas))
            {
                result["IsValid"] = false;
                errors.Add("Pilar sem notas mensuráveis");
            }

            _telemetryService.TrackEvent("NotaValidationCompleted", new Dictionary<string, string>
            {
                { "Context", contexto },
                { "IsValid", result["IsValid"].ToString() },
                { "ErrorCount", errors.Count.ToString() },
                { "WarningCount", warnings.Count.ToString() }
            });

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetNotaValidationResult" },
                { "Component", "NotaHelper" },
                { "Context", contexto }
            });
            
            return new Dictionary<string, object>
            {
                { "IsValid", false },
                { "Errors", new List<string> { "Erro interno na validação" } },
                { "Warnings", new List<string>() },
                { "Context", contexto }
            };
        }
    }
}