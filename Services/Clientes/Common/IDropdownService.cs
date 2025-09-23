using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Clientes.Common;

public interface IDropdownService
{
    Task<List<DropdownItem>> ObterSociosAsync();
    Task<List<DropdownItem>> ObterStatusAsync();
}