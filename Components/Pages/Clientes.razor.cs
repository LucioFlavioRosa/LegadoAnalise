using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Clientes.Common;
using Peers.Moderno.Components.Shared;

namespace Peers.Moderno.Components.Pages;

public partial class Clientes
{
    private ClienteForm? clienteForm;
    private List<ClienteDto> clientes = new();
    private ClienteFormDto formModel = new();
    private bool isLoadingList = false;
    
    private int idEmpresa = 1;
    private int idUsuario = 1;

    protected override async Task OnInitializedAsync()
    {
        await CarregarClientesAsync();
    }

    private async Task CarregarClientesAsync()
    {
        isLoadingList = true;
        StateHasChanged();

        try
        {
            var clientesResult = await ClientesService.ObterListaClientesAsync();
            clientes = clientesResult.ToList();
        }
        finally
        {
            isLoadingList = false;
            StateHasChanged();
        }
    }

    private async Task OnClienteSalvo(ClienteFormDto formDto)
    {
        clienteForm?.LimparFormulario();
        formModel = new ClienteFormDto();
        await CarregarClientesAsync();
    }

    private async Task OnCancelado()
    {
        clienteForm?.LimparFormulario();
        formModel = new ClienteFormDto();
        StateHasChanged();
    }

    private async Task OnClienteAlterado(ClienteDto cliente)
    {
        clienteForm?.CarregarCliente(cliente);
        
        await JSRuntime.InvokeVoidAsync("scrollTo", 0, 0);
    }

    private async Task OnClienteInativado(ClienteDto cliente)
    {
        var confirmacao = await JSRuntime.InvokeAsync<bool>("confirm", $"Deseja realmente inativar o cliente '{cliente.Cliente}'?");
        
        if (confirmacao)
        {
            var sucesso = await ClientesService.ExcluirClienteAsync(cliente.IdCliente);
            if (sucesso)
            {
                await CarregarClientesAsync();
            }
        }
    }
}