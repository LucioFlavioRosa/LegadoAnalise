using Peers.Moderno.Services.Clientes.Common;

namespace Peers.Moderno.Services.Clientes;

public interface IClienteService
{
    Task<List<ClienteDto>> ObterClientesAsync();
    Task<ClienteDto?> ObterClientePorIdAsync(int id);
    Task<bool> InserirClienteAsync(ClienteDto clienteDto);
    Task<bool> AlterarClienteAsync(ClienteDto clienteDto);
    Task<bool> ExcluirClienteAsync(int id);
    Task<List<ClienteDto>> ObterClientesAtivosAsync();
    Task<List<ClienteDto>> ObterClientesPorSocioAsync(int idSocio);
}