using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Peers.Moderno.Services.AutoAvaliacao;
using Peers.Moderno.Services.AutoAvaliacao.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Pages;

public partial class AutoAvaliacaoCompetencia : ComponentBase, IDisposable
{
    [Inject] private IAutoAvaliacaoService AutoAvaliacaoService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private ITelemetryService TelemetryService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public string? IdProjeto { get; set; }
    [Parameter] public string? IdAssociado { get; set; }
    [Parameter] public string? IdPeriodo { get; set; }
    [Parameter] public string? TipoAvaliacao { get; set; }
    [Parameter] public string? Escopo { get; set; }
    [Parameter] public string? IdGestor { get; set; }

    private AutoAvaliacaoDto? AvaliacaoData;
    private bool IsLoading = true;
    private bool IsProcessing = false;
    private DotNetObjectReference<AutoAvaliacaoCompetencia>? objRef;

    protected override async Task OnInitializedAsync()
    {
        objRef = DotNetObjectReference.Create(this);
        await CarregarDadosAvaliacao();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ConfigurarAutoSave();
            await ConfigurarScripts();
        }
    }

    private async Task CarregarDadosAvaliacao()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            var parametros = ObterParametrosUrl();
            
            if (!ValidarParametrosObrigatorios(parametros))
            {
                MessageBoxService.ShowError("Parâmetros obrigatórios não informados (Projeto, Associado, Período).");
                return;
            }

            AvaliacaoData = await AutoAvaliacaoService.CarregarDadosAvaliacaoAsync(parametros);

            if (AvaliacaoData == null)
            {
                MessageBoxService.ShowError("Não foi possível carregar os dados da avaliação.");
                return;
            }

            TelemetryService.TrackEvent("AutoAvaliacaoCompetencia_Loaded", new Dictionary<string, string>
            {
                { "IdProjeto", parametros.IdProjeto.ToString() },
                { "IdAssociado", parametros.IdAssociado.ToString() },
                { "IdPeriodo", parametros.IdPeriodo.ToString() },
                { "CompetenciasCount", AvaliacaoData.Competencias?.Count.ToString() ?? "0" },
                { "TipoAvaliacao", parametros.TipoAvaliacao ?? "" },
                { "Escopo", parametros.Escopo ?? "" }
            });
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarDadosAvaliacao" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
            MessageBoxService.ShowError($"Erro ao carregar dados da avaliação: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task SalvarAvaliacao()
    {
        if (AvaliacaoData == null || IsProcessing) return;

        try
        {
            IsProcessing = true;
            StateHasChanged();

            var validacao = await AutoAvaliacaoService.ValidarAvaliacaoAsync(AvaliacaoData);
            if (!validacao.Valida)
            {
                MessageBoxService.ShowError(validacao.MensagemErro ?? "Erro na validação da avaliação.");
                return;
            }

            var resultado = await AutoAvaliacaoService.SalvarAvaliacaoAsync(AvaliacaoData, false);

            if (resultado.Sucesso)
            {
                MessageBoxService.ShowSuccess("Avaliação salva com sucesso.");
                
                TelemetryService.TrackEvent("AutoAvaliacaoCompetencia_Saved", new Dictionary<string, string>
                {
                    { "IdAvaliacao", AvaliacaoData.IdAvaliacao.ToString() },
                    { "CompetenciasPreenchidas", resultado.CompetenciasPreenchidas.ToString() },
                    { "CompetenciasTotal", AvaliacaoData.Competencias?.Count.ToString() ?? "0" }
                });

                // Recarrega os dados para refletir o estado atual
                await CarregarDadosAvaliacao();
            }
            else
            {
                MessageBoxService.ShowError(resultado.MensagemErro ?? "Erro ao salvar avaliação.");
                
                TelemetryService.TrackEvent("AutoAvaliacaoCompetencia_SaveError", new Dictionary<string, string>
                {
                    { "IdAvaliacao", AvaliacaoData.IdAvaliacao.ToString() },
                    { "ErrorMessage", resultado.MensagemErro ?? "Erro desconhecido" }
                });
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SalvarAvaliacao" },
                { "Component", "AutoAvaliacaoCompetencia" },
                { "IdAvaliacao", AvaliacaoData?.IdAvaliacao.ToString() ?? "Unknown" }
            });
            MessageBoxService.ShowError("Erro interno ao salvar avaliação.");
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    private async Task IrParaPerformance()
    {
        if (AvaliacaoData == null) return;

        try
        {
            var validacao = await AutoAvaliacaoService.ValidarAvaliacaoAsync(AvaliacaoData);
            if (!validacao.Valida)
            {
                MessageBoxService.ShowError(validacao.MensagemErro ?? "Erro na validação da avaliação.");
                return;
            }

            // Salva antes de navegar
            var resultadoSalvar = await AutoAvaliacaoService.SalvarAvaliacaoAsync(AvaliacaoData, false);
            if (!resultadoSalvar.Sucesso)
            {
                MessageBoxService.ShowError("Erro ao salvar avaliação antes de navegar para Performance.");
                return;
            }

            var parametros = ObterParametrosUrl();
            var urlPerformance = $"/autoavaliacao-performance?IdProjeto={parametros.IdProjeto}&IdAssociado={parametros.IdAssociado}&IdPeriodo={parametros.IdPeriodo}&TipoAvaliacao={parametros.TipoAvaliacao}&Escopo={parametros.Escopo}&IdGestor={parametros.IdGestor}";
            
            Navigation.NavigateTo(urlPerformance);
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "IrParaPerformance" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
            MessageBoxService.ShowError("Erro ao navegar para Performance.");
        }
    }

    private async Task IrParaFinalizacao()
    {
        if (AvaliacaoData == null) return;

        try
        {
            var validacao = await AutoAvaliacaoService.ValidarAvaliacaoAsync(AvaliacaoData);
            if (!validacao.Valida)
            {
                MessageBoxService.ShowError(validacao.MensagemErro ?? "Erro na validação da avaliação.");
                return;
            }

            // Salva antes de navegar
            var resultadoSalvar = await AutoAvaliacaoService.SalvarAvaliacaoAsync(AvaliacaoData, false);
            if (!resultadoSalvar.Sucesso)
            {
                MessageBoxService.ShowError("Erro ao salvar avaliação antes de finalizar.");
                return;
            }

            var parametros = ObterParametrosUrl();
            var urlFinalizacao = $"/autoavaliacao?IdProjeto={parametros.IdProjeto}&IdAssociado={parametros.IdAssociado}&IdPeriodo={parametros.IdPeriodo}&TipoAvaliacao={parametros.TipoAvaliacao}&Escopo={parametros.Escopo}&IdGestor={parametros.IdGestor}";
            
            Navigation.NavigateTo(urlFinalizacao);
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "IrParaFinalizacao" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
            MessageBoxService.ShowError("Erro ao navegar para Finalização.");
        }
    }

    private AutoAvaliacaoParametrosDto ObterParametrosUrl()
    {
        var uri = new Uri(Navigation.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        return new AutoAvaliacaoParametrosDto
        {
            IdProjeto = int.TryParse(query.TryGetValue("IdProjeto", out var idProjeto) ? idProjeto.FirstOrDefault() : IdProjeto, out var projeto) ? projeto : 0,
            IdAssociado = int.TryParse(query.TryGetValue("IdAssociado", out var idAssociado) ? idAssociado.FirstOrDefault() : IdAssociado, out var associado) ? associado : 0,
            IdPeriodo = int.TryParse(query.TryGetValue("IdPeriodo", out var idPeriodo) ? idPeriodo.FirstOrDefault() : IdPeriodo, out var periodo) ? periodo : 0,
            TipoAvaliacao = query.TryGetValue("TipoAvaliacao", out var tipoAvaliacao) ? tipoAvaliacao.FirstOrDefault() : TipoAvaliacao ?? "desempenho",
            Escopo = query.TryGetValue("Escopo", out var escopo) ? escopo.FirstOrDefault() : Escopo ?? "projeto",
            IdGestor = int.TryParse(query.TryGetValue("IdGestor", out var idGestor) ? idGestor.FirstOrDefault() : IdGestor, out var gestor) ? gestor : 0
        };
    }

    private bool ValidarParametrosObrigatorios(AutoAvaliacaoParametrosDto parametros)
    {
        return parametros.IdProjeto > 0 && 
               parametros.IdAssociado > 0 && 
               parametros.IdPeriodo > 0 &&
               !string.IsNullOrEmpty(parametros.TipoAvaliacao) &&
               !string.IsNullOrEmpty(parametros.Escopo);
    }

    private async Task ConfigurarAutoSave()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("configureAutoSave", objRef);
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ConfigurarAutoSave" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
        }
    }

    private async Task ConfigurarScripts()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("initializeBootstrapComponents");
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ConfigurarScripts" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
        }
    }

    [JSInvokable]
    public async Task AutoSave()
    {
        if (!IsProcessing && AvaliacaoData != null)
        {
            try
            {
                await SalvarAvaliacao();
                TelemetryService.TrackEvent("AutoAvaliacaoCompetencia_AutoSaved", new Dictionary<string, string>
                {
                    { "IdAvaliacao", AvaliacaoData.IdAvaliacao.ToString() }
                });
            }
            catch (Exception ex)
            {
                TelemetryService.TrackException(ex, new Dictionary<string, string>
                {
                    { "Method", "AutoSave" },
                    { "Component", "AutoAvaliacaoCompetencia" }
                });
            }
        }
    }

    private string TruncarTexto(string? texto, int qtdCaracteres)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;

        if (texto.Length > qtdCaracteres)
        {
            return $"{texto.Substring(0, qtdCaracteres)}...";
        }

        return texto;
    }

    public void Dispose()
    {
        objRef?.Dispose();
    }
}