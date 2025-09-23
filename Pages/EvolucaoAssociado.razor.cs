using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Peers.Moderno.Models;
using Peers.Moderno.Services;

namespace Peers.Moderno.Pages;

public partial class EvolucaoAssociado : ComponentBase
{
    [Inject] private IEvolucaoAssociadoService EvolucaoService { get; set; } = default!;
    [Inject] private IWebStorageService WebStorageService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private string tipoAvaliacaoSelecionado = string.Empty;
    private string escopoSelecionado = string.Empty;
    private bool isLoading = false;
    private string mensagemErro = string.Empty;
    private AssociadoInfo associadoInfo = new();
    private List<EVOLUCAOASSOCIADO> resultadoAssociado = new();
    private UsuarioLogado usuarioLogado = new();

    protected override async Task OnInitializedAsync()
    {
        usuarioLogado = WebStorageService.GetUsuarioLogado();
        await Task.CompletedTask;
    }

    private async Task GerarEvolucao()
    {
        mensagemErro = string.Empty;

        if (string.IsNullOrEmpty(tipoAvaliacaoSelecionado) || tipoAvaliacaoSelecionado == "" ||
            string.IsNullOrEmpty(escopoSelecionado) || escopoSelecionado == "")
        {
            mensagemErro = "Selecione o Tipo de Avaliações e Escopo";
            return;
        }

        isLoading = true;
        StateHasChanged();

        try
        {
            associadoInfo = await EvolucaoService.ObterAssociadoMentorCargoAsync(usuarioLogado.Id);
            resultadoAssociado = await EvolucaoService.ObterEvolucaoAsync(usuarioLogado.Id, tipoAvaliacaoSelecionado, escopoSelecionado);

            if (resultadoAssociado.Any())
            {
                var jsonRadar = EvolucaoService.GerarJsonRadar(resultadoAssociado, usuarioLogado.IdCargo);
                await JSRuntime.InvokeVoidAsync("renderizarRadar", jsonRadar);
            }
        }
        catch (Exception ex)
        {
            mensagemErro = $"Erro: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}