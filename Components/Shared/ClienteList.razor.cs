using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Clientes.Common;

namespace Peers.Moderno.Components.Shared;

public partial class ClienteList
{
    [Parameter] public IEnumerable<ClienteDto>? Clientes { get; set; }
    [Parameter] public EventCallback<ClienteDto> OnClienteAlterado { get; set; }
    [Parameter] public EventCallback<ClienteDto> OnClienteInativado { get; set; }
    [Parameter] public bool IsLoading { get; set; }

    private async Task OnAlterar(ClienteDto cliente)
    {
        await OnClienteAlterado.InvokeAsync(cliente);
    }

    private async Task OnInativar(ClienteDto cliente)
    {
        await OnClienteInativado.InvokeAsync(cliente);
    }
}