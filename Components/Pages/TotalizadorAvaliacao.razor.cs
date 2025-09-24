using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Associados;

namespace Peers.Moderno.Components.Pages;

public partial class TotalizadorAvaliacao : ComponentBase
{
    [Inject] private IAssociadosService AssociadosService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private ITelemetryService TelemetryService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private Associado associado = new();
    private List<ComboItem> mentoresCombo = new();
    private List<ComboItem> cargosCombo = new();
    private List<ComboItem> perfisCombo = new();
    private List<ComboItem> statusCombo = new();
    private string statusSelecionado = "1";
    private bool isProcessing = false;
    private bool showMessageBox = false;
    private string messageBoxText = string.Empty;
    private MessageBoxType messageBoxType = MessageBoxType.Info;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await CarregarCombosAsync();
            MessageBoxService.OnMessageReceived += HandleMessageReceived;
            
            TelemetryService.TrackPageView("TotalizadorAvaliacao", new Dictionary<string, string>
            {
                { "Component", "TotalizadorAvaliacao" },
                { "Action", "PageLoad" }
            });
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
    }

    private async Task CarregarCombosAsync()
    {
        try
        {
            var mentores = await AssociadosService.ObterMentoresAsync();
            mentoresCombo = ComboHelper.CreateComboFromList(
                mentores, 
                m => m.Id.ToString(), 
                m => m.Nome, 
                includeSelecionar: true
            );

            var cargos = await AssociadosService.ObterCargosAsync();
            cargosCombo = ComboHelper.CreateComboFromList(
                cargos, 
                c => c.IdCargo.ToString(), 
                c => c.Nome, 
                includeSelecionar: true
            );

            var perfis = await AssociadosService.ObterPerfisAsync();
            perfisCombo = ComboHelper.CreateComboFromList(
                perfis, 
                p => p.Id.ToString(), 
                p => p.Nome, 
                includeSelecionar: true
            );

            statusCombo = ComboHelper.GetStatusItems();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarCombosAsync" },
                { "Component", "TotalizadorAvaliacao" }
            });
            throw;
        }
    }

    private async Task CadastrarSalvar()
    {
        if (isProcessing) return;

        try
        {
            isProcessing = true;
            StateHasChanged();

            var validationResult = ValidationHelper.ValidateAssociado(associado, statusSelecionado);
            if (!validationResult.IsValid)
            {
                ShowMessage(validationResult.ErrorMessage, MessageBoxType.Error);
                return;
            }

            associado.Ativo = statusSelecionado == "1";

            bool success;
            if (associado.Id > 0)
            {
                success = await AssociadosService.AlterarAssociadoAsync(associado);
                if (success)
                {
                    ShowMessage("Associado alterado com sucesso!", MessageBoxType.Success);
                    TelemetryService.TrackEvent("AssociadoAlterado", new Dictionary<string, string>
                    {
                        { "AssociadoId", associado.Id.ToString() },
                        { "Component", "TotalizadorAvaliacao" }
                    });
                }
                else
                {
                    ShowMessage("Erro ao alterar associado", MessageBoxType.Error);
                }
            }
            else
            {
                success = await AssociadosService.InserirAssociadoAsync(associado);
                if (success)
                {
                    ShowMessage("Associado cadastrado com sucesso!", MessageBoxType.Success);
                    LimparFormulario();
                    TelemetryService.TrackEvent("AssociadoCadastrado", new Dictionary<string, string>
                    {
                        { "Component", "TotalizadorAvaliacao" }
                    });
                }
                else
                {
                    ShowMessage("Erro ao cadastrar associado", MessageBoxType.Error);
                }
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CadastrarSalvar" },
                { "Component", "TotalizadorAvaliacao" },
                { "AssociadoId", associado?.Id.ToString() ?? "0" }
            });
            ShowMessage("Erro interno ao processar solicitação", MessageBoxType.Error);
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private void LimparFormulario()
    {
        associado = new Associado();
        statusSelecionado = "1";
        StateHasChanged();
    }

    private void HandleMessageReceived(MessageBoxEventArgs args)
    {
        ShowMessage(args.Message, args.Type);
    }

    private void ShowMessage(string message, MessageBoxType type)
    {
        messageBoxText = message;
        messageBoxType = type;
        showMessageBox = true;
        StateHasChanged();
    }

    private string GetAlertClass()
    {
        return messageBoxType switch
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