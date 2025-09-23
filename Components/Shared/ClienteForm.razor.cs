using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Clientes.Common;

namespace Peers.Moderno.Components.Shared;

public partial class ClienteForm
{
    [Parameter] public ClienteFormDto FormModel { get; set; } = new();
    [Parameter] public EventCallback<ClienteFormDto> OnClienteSalvo { get; set; }
    [Parameter] public EventCallback OnCancelado { get; set; }
    [Parameter] public int IdEmpresa { get; set; }
    [Parameter] public int IdUsuario { get; set; }

    private List<SocioDto> Socios { get; set; } = new();
    private bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CarregarSociosAsync();
    }

    private async Task CarregarSociosAsync()
    {
        var socios = await ClientesService.ObterSociosAsync();
        Socios = socios.ToList();
    }

    private async Task OnSubmitAsync()
    {
        IsLoading = true;
        StateHasChanged();

        try
        {
            var clienteDto = new ClienteDto
            {
                IdCliente = FormModel.IdCliente,
                Cliente = FormModel.Cliente,
                Telefones = FormModel.Telefone,
                IdAssociacoResponsavel = FormModel.IdSocio,
                GestorCliente = FormModel.GestorCliente,
                Email = FormModel.Email,
                ATV = FormModel.Status,
                IdEmpresa = IdEmpresa,
                USR = IdUsuario,
                DHC = DateTime.Now
            };

            bool sucesso;
            if (FormModel.IdCliente == 0)
            {
                sucesso = await ClientesService.InserirClienteAsync(clienteDto);
            }
            else
            {
                sucesso = await ClientesService.AlterarClienteAsync(clienteDto);
            }

            if (sucesso)
            {
                await OnClienteSalvo.InvokeAsync(FormModel);
            }
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task OnCancelar()
    {
        await OnCancelado.InvokeAsync();
    }

    public void LimparFormulario()
    {
        FormModel = new ClienteFormDto();
        StateHasChanged();
    }

    public void CarregarCliente(ClienteDto cliente)
    {
        FormModel = new ClienteFormDto
        {
            IdCliente = cliente.IdCliente,
            Cliente = cliente.Cliente,
            Telefone = cliente.Telefones,
            IdSocio = cliente.IdAssociacoResponsavel,
            GestorCliente = cliente.GestorCliente,
            Email = cliente.Email,
            Status = cliente.ATV
        };
        StateHasChanged();
    }
}