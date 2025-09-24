using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Perfis;
using Peers.Moderno.Services.Perfis.Common;

namespace Peers.Moderno.Components.Pages;

public partial class Perfis : ComponentBase
{
    [Inject] private IPerfisService PerfisService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private ITelemetryService TelemetryService { get; set; } = default!;

    private List<PerfilDto> perfis = new();
    private PerfilDto perfilAtual = new();
    private List<StatusOption> statusOptions = new();
    private bool isLoading = true;
    private bool isProcessing = false;
    private bool isEditMode = false;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            statusOptions = StatusHelper.GetStatusOptions();
            await CarregarPerfis();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "OnInitializedAsync" },
                { "Component", "Perfis" }
            });
            MessageBoxService.ShowError("Erro ao carregar a página");
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task CarregarPerfis()
    {
        try
        {
            perfis = await PerfisService.ListarPerfisAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarPerfis" },
                { "Component", "Perfis" }
            });
            MessageBoxService.ShowError("Erro ao carregar lista de perfis");
        }
    }

    private async Task SalvarPerfil()
    {
        if (string.IsNullOrWhiteSpace(perfilAtual.Nome))
        {
            MessageBoxService.ShowWarning("Preencha o campo Perfil");
            return;
        }

        isProcessing = true;
        StateHasChanged();

        try
        {
            bool sucesso;
            string mensagem;

            if (isEditMode)
            {
                sucesso = await PerfisService.AlterarPerfilAsync(perfilAtual);
                mensagem = sucesso ? "Perfil alterado com sucesso!" : "Erro ao alterar perfil. Verifique se já existe um perfil com este nome.";
            }
            else
            {
                sucesso = await PerfisService.InserirPerfilAsync(perfilAtual);
                mensagem = sucesso ? "Perfil inserido com sucesso!" : "Erro ao inserir perfil. Verifique se já existe um perfil com este nome.";
            }

            if (sucesso)
            {
                MessageBoxService.ShowSuccess(mensagem);
                await CarregarPerfis();
                LimparFormulario();
            }
            else
            {
                MessageBoxService.ShowError(mensagem);
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SalvarPerfil" },
                { "Component", "Perfis" },
                { "IsEditMode", isEditMode.ToString() },
                { "PerfilId", perfilAtual.Id.ToString() }
            });
            MessageBoxService.ShowError("Erro interno ao salvar perfil");
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private async Task EditarPerfil(int id)
    {
        try
        {
            var perfil = await PerfisService.ObterPerfilAsync(id);
            if (perfil != null)
            {
                perfilAtual = new PerfilDto
                {
                    Id = perfil.Id,
                    Nome = perfil.Nome,
                    Ativo = perfil.Ativo
                };
                isEditMode = true;
                StateHasChanged();
            }
            else
            {
                MessageBoxService.ShowError("Perfil não encontrado");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "EditarPerfil" },
                { "Component", "Perfis" },
                { "PerfilId", id.ToString() }
            });
            MessageBoxService.ShowError("Erro ao carregar perfil para edição");
        }
    }

    private async Task InativarPerfil(int id)
    {
        try
        {
            var sucesso = await PerfisService.InativarPerfilAsync(id);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Perfil inativado com sucesso!");
                await CarregarPerfis();
            }
            else
            {
                MessageBoxService.ShowError("Erro ao inativar perfil");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InativarPerfil" },
                { "Component", "Perfis" },
                { "PerfilId", id.ToString() }
            });
            MessageBoxService.ShowError("Erro interno ao inativar perfil");
        }
    }

    private void CancelarEdicao()
    {
        LimparFormulario();
    }

    private void LimparFormulario()
    {
        perfilAtual = new PerfilDto { Ativo = true };
        isEditMode = false;
        StateHasChanged();
    }
}