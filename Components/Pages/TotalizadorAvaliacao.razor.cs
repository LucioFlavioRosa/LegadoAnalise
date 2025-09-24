using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Associados.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Pages;

public partial class TotalizadorAvaliacao : ComponentBase
{
    [Inject] private ITotalizadorAvaliacaoService TotalizadorService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ITelemetryService TelemetryService { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery] public int? Id { get; set; }

    private AssociadoFormModel model = new();
    private List<ComboItem> mentores = new();
    private List<ComboItem> cargos = new();
    private List<ComboItem> perfis = new();
    private List<ComboItem> statusOptions = new();
    private List<string> validationErrors = new();
    
    private bool isLoading = true;
    private bool isProcessing = false;
    private bool showValidationErrors = false;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await CarregarDadosIniciais();
            
            if (Id.HasValue && Id.Value > 0)
            {
                await CarregarAssociadoParaEdicao(Id.Value);
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "OnInitializedAsync" },
                { "Component", "TotalizadorAvaliacao" }
            });
            MessageBoxService.ShowError("Erro ao carregar dados iniciais");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task CarregarDadosIniciais()
    {
        var tasks = new List<Task>
        {
            CarregarMentores(),
            CarregarCargos(),
            CarregarPerfis(),
            CarregarStatus()
        };

        await Task.WhenAll(tasks);
    }

    private async Task CarregarMentores()
    {
        try
        {
            mentores = await TotalizadorService.ObterMentoresAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarMentores" },
                { "Component", "TotalizadorAvaliacao" }
            });
            mentores = ComboHelper.CreateComboFromList(new List<ComboItem>(), m => m.Value, m => m.Text, true);
        }
    }

    private async Task CarregarCargos()
    {
        try
        {
            cargos = await TotalizadorService.ObterCargosAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarCargos" },
                { "Component", "TotalizadorAvaliacao" }
            });
            cargos = ComboHelper.CreateComboFromList(new List<ComboItem>(), c => c.Value, c => c.Text, true);
        }
    }

    private async Task CarregarPerfis()
    {
        try
        {
            perfis = await TotalizadorService.ObterPerfisAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarPerfis" },
                { "Component", "TotalizadorAvaliacao" }
            });
            perfis = ComboHelper.CreateComboFromList(new List<ComboItem>(), p => p.Value, p => p.Text, true);
        }
    }

    private async Task CarregarStatus()
    {
        try
        {
            statusOptions = await TotalizadorService.ObterStatusAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarStatus" },
                { "Component", "TotalizadorAvaliacao" }
            });
            statusOptions = ComboHelper.GetStatusItems();
        }
    }

    private async Task CarregarAssociadoParaEdicao(int id)
    {
        try
        {
            model = await TotalizadorService.ObterAssociadoParaEdicaoAsync(id);
            if (model.Id == 0)
            {
                MessageBoxService.ShowWarning("Associado não encontrado");
                Navigation.NavigateTo("/totalizador-avaliacao");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarAssociadoParaEdicao" },
                { "Component", "TotalizadorAvaliacao" },
                { "Id", id.ToString() }
            });
            MessageBoxService.ShowError("Erro ao carregar dados do associado");
        }
    }

    private async Task HandleValidSubmit()
    {
        if (isProcessing) return;

        try
        {
            isProcessing = true;
            showValidationErrors = false;
            validationErrors.Clear();
            StateHasChanged();

            var result = await TotalizadorService.SalvarAssociadoAsync(model);

            if (result.IsSuccess)
            {
                MessageBoxService.ShowSuccess(result.Message);
                
                TelemetryService.TrackEvent("AssociadoSalvo", new Dictionary<string, string>
                {
                    { "Id", model.Id.ToString() },
                    { "IsEdicao", model.IsEdicao.ToString() },
                    { "Nome", model.Nome }
                });

                if (!model.IsEdicao)
                {
                    LimparFormulario();
                }
            }
            else
            {
                if (result.Errors.Any())
                {
                    validationErrors = result.Errors;
                    showValidationErrors = true;
                }
                else
                {
                    MessageBoxService.ShowError(result.Message);
                }
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "HandleValidSubmit" },
                { "Component", "TotalizadorAvaliacao" }
            });
            MessageBoxService.ShowError("Erro interno ao salvar associado");
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private void LimparFormulario()
    {
        model = new AssociadoFormModel();
        validationErrors.Clear();
        showValidationErrors = false;
        StateHasChanged();
        
        TelemetryService.TrackEvent("FormularioLimpo", new Dictionary<string, string>
        {
            { "Component", "TotalizadorAvaliacao" }
        });
    }

    protected override void OnParametersSet()
    {
        if (Id.HasValue && Id.Value > 0 && model.Id != Id.Value)
        {
            InvokeAsync(async () => await CarregarAssociadoParaEdicao(Id.Value));
        }
    }
}