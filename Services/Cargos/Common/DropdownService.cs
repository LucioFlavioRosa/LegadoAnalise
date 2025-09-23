using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Cargos.Common;

public interface IDropdownService
{
    Task<List<DropdownItem>> ObterCargosAtivosAsync();
    Task<List<DropdownItem>> ObterTodosCargosAsync();
    Task<List<DropdownItem>> ObterStatusOptionsAsync();
}

public class DropdownService : IDropdownService
{
    private readonly ApplicationDbContext _context;

    public DropdownService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DropdownItem>> ObterCargosAtivosAsync()
    {
        return await _context.Cargos
            .Where(c => c.ATV == 1)
            .Select(c => new DropdownItem { Id = c.IdCargo, Nome = c.Nome })
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<List<DropdownItem>> ObterTodosCargosAsync()
    {
        return await _context.Cargos
            .Select(c => new DropdownItem { Id = c.IdCargo, Nome = c.Nome })
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<List<DropdownItem>> ObterStatusOptionsAsync()
    {
        return new List<DropdownItem>
        {
            new DropdownItem { Id = 1, Nome = "Ativo" },
            new DropdownItem { Id = 0, Nome = "Inativo" }
        };
    }
}