using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Associados.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Pages;

public partial class TotalizadorAvaliacao : ComponentBase
{
    [Inject] private ITotalizadorAvaliacaoService TotalizadorService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private ITelemetryService TelemetryService { get; set; } = default!;

    private AssociadoFormModel FormModel { get; set; } = new();
    private List<ComboItem> Mentores { get; set; } = new();
    private List<ComboItem> Cargos { get; set; } = new();
    private List<ComboItem> Perfis { get; set; } = new();
    private List<ComboItem> StatusOptions { get; set; } = new();
    private List<Associado> Associados { get; set; } = new();
    
    private bool IsLoading { get; set; } = true;
    private bool IsProcessing { get; set; } = false;
    private bool IsEditMode { get; set; } = false;
    private bool ShowMessageBox { get; set; } = false;
    private string MessageBoxText { get; set; } = string.Empty;
    private MessageBoxType MessageBoxType { get; set; } = MessageBoxType.Info;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            MessageBoxService.OnMessageReceived += HandleMessageReceived;
            await CarregarDadosIniciais();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "OnInitializedAsync" },
                { "Component", "TotalizadorAvaliacao" }
            });
            ShowMessage("Erro ao carregar a página", MessageBoxType.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CarregarDadosIniciais()
    {
        var tasks = new Task[]
        {
            CarregarMentores(),
            CarregarCargos(),
            CarregarPerfis(),
            CarregarStatus(),
            CarregarAssociados()
        };

        await Task.WhenAll(tasks);
    }

    private async Task CarregarMentores()
    {
        Mentores = await TotalizadorService.ObterMentoresAsync();
    }

    private async Task CarregarCargos()
    {
        Cargos = await TotalizadorService.ObterCargosAsync();
    }

    private async Task CarregarPerfis()
    {
        Perfis = await TotalizadorService.ObterPerfisAsync();
    }

    private async Task CarregarStatus()
    {
        StatusOptions = await TotalizadorService.ObterStatusAsync();
        if (!StatusOptions.Any())
        {
            StatusOptions = ComboHelper.GetStatusItems();
        }
    }

    private async Task CarregarAssociados()
    {
        Associados = await TotalizadorService.ListarAssociadosAsync();
    }

    private async Task HandleSubmit()
    {
        if (IsProcessing) return;

        try
        {
            IsProcessing = true;
            StateHasChanged();

            OperationResult result;
            
            if (IsEditMode)
            {
                result = await TotalizadorService.AtualizarAssociadoAsync(FormModel.Id, FormModel);
            }
            else
            {
                result = await TotalizadorService.CadastrarAssociadoAsync(FormModel);
            }

            if (result.IsSuccess)
            {
                ShowMessage(result.Message, MessageBoxType.Success);
                await CarregarAssociados();
                LimparFormulario();
            }
            else
            {
                ShowMessage(result.ErrorMessage, MessageBoxType.Error);
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "HandleSubmit" },
                { "Component", "TotalizadorAvaliacao" },
                { "IsEditMode", IsEditMode.ToString() }
            });
            ShowMessage("Erro interno ao processar solicitação", MessageBoxType.Error);
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    private async Task EditarAssociado(int id)
    {
        try
        {
            var associado = await TotalizadorService.ObterAssociadoParaEdicaoAsync(id);
            if (associado != null)
            {
                FormModel = associado;
                IsEditMode = true;
                StateHasChanged();
                
                TelemetryService.TrackEvent("AssociadoEditModeActivated", new Dictionary<string, string>
                {
                    { "AssociadoId", id.ToString() }
                });
            }
            else
            {
                ShowMessage("Associado não encontrado", MessageBoxType.Error);
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "EditarAssociado" },
                { "Component", "TotalizadorAvaliacao" },
                { "AssociadoId", id.ToString() }
            });
            ShowMessage("Erro ao carregar dados do associado", MessageBoxType.Error);
        }
    }

    private void LimparFormulario()
    {
        FormModel = new AssociadoFormModel();
        IsEditMode = false;
        StateHasChanged();
    }

    private void HandleMessageReceived(MessageBoxEventArgs args)
    {
        ShowMessage(args.Message, args.Type);
    }

    private void ShowMessage(string message, MessageBoxType type)
    {
        MessageBoxText = message;
        MessageBoxType = type;
        ShowMessageBox = true;
        StateHasChanged();
    }

    private void HideMessageBox()
    {
        ShowMessageBox = false;
        StateHasChanged();
    }

    private string GetAlertClass()
    {
        return MessageBoxType switch
        {
            MessageBoxType.Success => "alert-success",
            MessageBoxType.Error => "alert-danger",
            MessageBoxType.Warning => "alert-warning",
            MessageBoxType.Info => "alert-info",
            _ => "alert-info"
        };
    }

    public void Dispose()
    {
        if (MessageBoxService != null)
        {
            MessageBoxService.OnMessageReceived -= HandleMessageReceived;
        }
    }
}