using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Clientes;
using Peers.Moderno.Services.Clientes.Common;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Models;

namespace Peers.Moderno.Components.Clientes;

public partial class Clientes : ComponentBase
{
    [Inject] private IClienteService ClienteService { get; set; } = default!;
    [Inject] private IDropdownService DropdownService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private ClienteDto clienteDto = new();
    private List<ClienteDto>? clientes;
    private List<DropdownItem> socios = new();
    private bool isProcessing = false;
    private string mensagem = string.Empty;
    private TipoMensagem tipoMensagem = TipoMensagem.Info;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        try
        {
            var tasks = new Task[]
            {
                CarregarClientes(),
                CarregarSocios()
            };

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            await ExibirMensagem($"Erro ao carregar dados: {ex.Message}", TipoMensagem.Error);
        }
    }

    private async Task CarregarClientes()
    {
        clientes = await ClienteService.ObterClientesAsync();
    }

    private async Task CarregarSocios()
    {
        socios = await DropdownService.ObterSociosAsync();
    }

    private async Task CadastrarCliente()
    {
        if (isProcessing) return;

        try
        {
            isProcessing = true;
            
            if (clienteDto.Id > 0)
            {
                var sucesso = await ClienteService.AlterarClienteAsync(clienteDto);
                if (sucesso)
                {
                    await ExibirMensagem("Cliente alterado com sucesso!", TipoMensagem.Success);
                    LimparFormulario();
                    await CarregarClientes();
                }
                else
                {
                    await ExibirMensagem("Erro ao alterar cliente.", TipoMensagem.Error);
                }
            }
            else
            {
                var sucesso = await ClienteService.InserirClienteAsync(clienteDto);
                if (sucesso)
                {
                    await ExibirMensagem("Cliente inserido com sucesso!", TipoMensagem.Success);
                    LimparFormulario();
                    await CarregarClientes();
                }
                else
                {
                    await ExibirMensagem("Erro ao inserir cliente.", TipoMensagem.Error);
                }
            }
        }
        catch (Exception ex)
        {
            await ExibirMensagem($"Erro: {ex.Message}", TipoMensagem.Error);
        }
        finally
        {
            isProcessing = false;
        }
    }

    private async Task EditarCliente(int clienteId)
    {
        try
        {
            var cliente = await ClienteService.ObterClientePorIdAsync(clienteId);
            if (cliente != null)
            {
                clienteDto = cliente;
                StateHasChanged();
            }
            else
            {
                await ExibirMensagem("Cliente não encontrado.", TipoMensagem.Warning);
            }
        }
        catch (Exception ex)
        {
            await ExibirMensagem($"Erro ao carregar cliente: {ex.Message}", TipoMensagem.Error);
        }
    }

    private async Task InativarCliente(int clienteId)
    {
        try
        {
            var confirmacao = await JSRuntime.InvokeAsync<bool>("confirm", "Deseja realmente inativar este cliente?");
            if (confirmacao)
            {
                var sucesso = await ClienteService.ExcluirClienteAsync(clienteId);
                if (sucesso)
                {
                    await ExibirMensagem("Cliente inativado com sucesso!", TipoMensagem.Success);
                    await CarregarClientes();
                }
                else
                {
                    await ExibirMensagem("Erro ao inativar cliente.", TipoMensagem.Error);
                }
            }
        }
        catch (Exception ex)
        {
            await ExibirMensagem($"Erro ao inativar cliente: {ex.Message}", TipoMensagem.Error);
        }
    }

    private void LimparFormulario()
    {
        clienteDto = new ClienteDto();
        StateHasChanged();
    }

    private async Task ExibirMensagem(string msg, TipoMensagem tipo)
    {
        mensagem = msg;
        tipoMensagem = tipo;
        StateHasChanged();
        
        await Task.Delay(5000);
        mensagem = string.Empty;
        StateHasChanged();
    }

    private string GetAlertClass(TipoMensagem tipo)
    {
        return tipo switch
        {
            TipoMensagem.Success => "alert-success",
            TipoMensagem.Warning => "alert-warning",
            TipoMensagem.Error => "alert-danger",
            _ => "alert-info"
        };
    }
}