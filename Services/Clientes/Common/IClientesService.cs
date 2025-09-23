using Peers.Moderno.Services.Clientes.Common;

namespace Peers.Moderno.Services.Clientes.Common;

public interface IClientesService
{
    Task<IEnumerable<ClienteDto>> ObterListaClientesAsync();
    Task<ClienteDto?> ObterClienteAsync(int idCliente);
    Task<bool> InserirClienteAsync(ClienteDto cliente);
    Task<bool> AlterarClienteAsync(ClienteDto cliente);
    Task<bool> ExcluirClienteAsync(int idCliente);
    Task<IEnumerable<SocioDto>> ObterSociosAsync();
    Task<bool> ValidarClienteAsync(ClienteDto cliente);
}