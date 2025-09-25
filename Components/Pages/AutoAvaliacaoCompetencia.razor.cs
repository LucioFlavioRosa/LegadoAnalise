using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Peers.Moderno.Services.AutoAvaliacao;
using Peers.Moderno.Services.AutoAvaliacao.Common;
using Peers.Moderno.Services.Common;
using System.ComponentModel;

namespace Peers.Moderno.Components.Pages;

public partial class AutoAvaliacaoCompetencia : ComponentBase, IDisposable
{
    [Parameter, SupplyParameterFromQuery] public int? IdProjeto { get; set; }
    [Parameter, SupplyParameterFromQuery] public int? IdAssociado { get; set; }
    [Parameter, SupplyParameterFromQuery] public int? IdPeriodo { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? TipoAvaliacao { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? Escopo { get; set; }
    [Parameter, SupplyParameterFromQuery] public int? IdGestor { get; set; }

    private AutoAvaliacaoDto? avaliacaoData;
    private bool isLoading = true;
    private bool podeEditar = true;
    private bool messageBoxVisible = false;
    private string messageBoxMessage = string.Empty;
    private string messageBoxType = "info";
    private DotNetObjectReference<AutoAvaliacaoCompetencia>? dotNetRef;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            TelemetryService.TrackEvent("AutoAvaliacaoCompetencia_PageLoad", new Dictionary<string, string>
            {
                { "IdProjeto", IdProjeto?.ToString() ?? "null" },
                { "IdAssociado", IdAssociado?.ToString() ?? "null" },
                { "IdPeriodo", IdPeriodo?.ToString() ?? "null" },
                { "TipoAvaliacao", TipoAvaliacao ?? "null" },
                { "Escopo", Escopo ?? "null" }
            });

            await CarregarDadosAvaliacao();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "OnInitializedAsync" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
            
            ShowMessage("Erro ao carregar a página de avaliação.", "danger");
        }
        finally
        {
            isLoading = false;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && avaliacaoData != null && podeEditar)
        {
            try
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("initAutoSave", dotNetRef);
            }
            catch (Exception ex)
            {
                TelemetryService.TrackException(ex, new Dictionary<string, string>
                {
                    { "Method", "OnAfterRenderAsync" },
                    { "Component", "AutoAvaliacaoCompetencia" }
                });
            }
        }
    }

    private async Task CarregarDadosAvaliacao()
    {
        try
        {
            if (!IdProjeto.HasValue || !IdAssociado.HasValue || !IdPeriodo.HasValue || 
                string.IsNullOrEmpty(TipoAvaliacao) || string.IsNullOrEmpty(Escopo))
            {
                ShowMessage("Parâmetros obrigatórios não informados.", "warning");
                return;
            }

            var request = new CarregarAvaliacaoRequest
            {
                IdProjeto = IdProjeto.Value,
                IdAssociado = IdAssociado.Value,
                IdPeriodo = IdPeriodo.Value,
                TipoAvaliacao = TipoAvaliacao,
                Escopo = Escopo,
                IdGestor = IdGestor
            };

            var resultado = await AutoAvaliacaoService.CarregarAvaliacaoAsync(request);
            
            if (resultado.Sucesso)
            {
                avaliacaoData = resultado.Dados;
                podeEditar = avaliacaoData?.PodeEditar ?? false;
                
                TelemetryService.TrackEvent("AutoAvaliacaoCompetencia_DataLoaded", new Dictionary<string, string>
                {
                    { "CompetenciasCount", avaliacaoData?.Competencias?.Count.ToString() ?? "0" },
                    { "PodeEditar", podeEditar.ToString() }
                });
            }
            else
            {
                ShowMessage(resultado.MensagemErro ?? "Erro ao carregar dados da avaliação.", "danger");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarDadosAvaliacao" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
            
            ShowMessage("Erro interno ao carregar dados da avaliação.", "danger");
        }
    }

    private async Task SalvarAvaliacao()
    {
        if (avaliacaoData == null || !podeEditar)
            return;

        try
        {
            var request = new SalvarAvaliacaoRequest
            {
                IdProjeto = IdProjeto!.Value,
                IdAssociado = IdAssociado!.Value,
                IdPeriodo = IdPeriodo!.Value,
                TipoAvaliacao = TipoAvaliacao!,
                Escopo = Escopo!,
                IdGestor = IdGestor,
                Competencias = avaliacaoData.Competencias,
                FinalizarAvaliacao = false
            };

            var resultado = await AutoAvaliacaoService.SalvarAvaliacaoAsync(request);
            
            if (resultado.Sucesso)
            {
                ShowMessage("Avaliação salva com sucesso.", "success");
                
                TelemetryService.TrackEvent("AutoAvaliacaoCompetencia_Saved", new Dictionary<string, string>
                {
                    { "CompetenciasCount", avaliacaoData.Competencias?.Count.ToString() ?? "0" }
                });
            }
            else
            {
                ShowMessage(resultado.MensagemErro ?? "Erro ao salvar avaliação.", "danger");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SalvarAvaliacao" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
            
            ShowMessage("Erro interno ao salvar avaliação.", "danger");
        }
    }

    [JSInvokable]
    public async Task AutoSave()
    {
        if (avaliacaoData != null && podeEditar)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("showMessage", "success", "SALVANDO!!", 9000);
                await SalvarAvaliacao();
                
                TelemetryService.TrackEvent("AutoAvaliacaoCompetencia_AutoSaved");
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

    private async Task IrParaPerformance()
    {
        if (avaliacaoData == null)
            return;

        try
        {
            var request = new SalvarAvaliacaoRequest
            {
                IdProjeto = IdProjeto!.Value,
                IdAssociado = IdAssociado!.Value,
                IdPeriodo = IdPeriodo!.Value,
                TipoAvaliacao = TipoAvaliacao!,
                Escopo = Escopo!,
                IdGestor = IdGestor,
                Competencias = avaliacaoData.Competencias,
                FinalizarAvaliacao = false
            };

            var resultado = await AutoAvaliacaoService.SalvarAvaliacaoAsync(request);
            
            if (resultado.Sucesso)
            {
                var url = $"/autoavaliacao-performance?IdProjeto={IdProjeto}&IdAssociado={IdAssociado}&IdPeriodo={IdPeriodo}&TipoAvaliacao={TipoAvaliacao}&Escopo={Escopo}";
                if (IdGestor.HasValue)
                    url += $"&IdGestor={IdGestor}";
                    
                Navigation.NavigateTo(url);
            }
            else
            {
                ShowMessage(resultado.MensagemErro ?? "Erro ao salvar avaliação antes de navegar.", "danger");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "IrParaPerformance" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
            
            ShowMessage("Erro ao navegar para Performance.", "danger");
        }
    }

    private async Task IrParaFinalizacao()
    {
        if (avaliacaoData == null)
            return;

        try
        {
            var request = new SalvarAvaliacaoRequest
            {
                IdProjeto = IdProjeto!.Value,
                IdAssociado = IdAssociado!.Value,
                IdPeriodo = IdPeriodo!.Value,
                TipoAvaliacao = TipoAvaliacao!,
                Escopo = Escopo!,
                IdGestor = IdGestor,
                Competencias = avaliacaoData.Competencias,
                FinalizarAvaliacao = false
            };

            var resultado = await AutoAvaliacaoService.SalvarAvaliacaoAsync(request);
            
            if (resultado.Sucesso)
            {
                var url = $"/autoavaliacao?IdProjeto={IdProjeto}&IdAssociado={IdAssociado}&IdPeriodo={IdPeriodo}&TipoAvaliacao={TipoAvaliacao}&Escopo={Escopo}";
                if (IdGestor.HasValue)
                    url += $"&IdGestor={IdGestor}";
                    
                Navigation.NavigateTo(url);
            }
            else
            {
                ShowMessage(resultado.MensagemErro ?? "Erro ao salvar avaliação antes de navegar.", "danger");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "IrParaFinalizacao" },
                { "Component", "AutoAvaliacaoCompetencia" }
            });
            
            ShowMessage("Erro ao navegar para Finalização.", "danger");
        }
    }

    private void ShowMessage(string message, string type)
    {
        messageBoxMessage = message;
        messageBoxType = type;
        messageBoxVisible = true;
        StateHasChanged();
    }

    private static string TruncarTexto(string? texto, int qtdCaracteres)
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
        try
        {
            if (dotNetRef != null)
            {
                JSRuntime.InvokeVoidAsync("clearAutoSave");
                dotNetRef.Dispose();
            }
        }
        catch
        {
            // Ignore disposal errors
        }
    }
}