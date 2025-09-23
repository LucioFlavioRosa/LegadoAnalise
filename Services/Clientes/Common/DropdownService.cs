using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Clientes.Common;

public class DropdownService : IDropdownService
{
    private readonly ApplicationDbContext _context;

    public DropdownService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DropdownItem>> ObterSociosAsync()
    {
        return await _context.Associados
            .Where(a => a.Ativo)
            .Select(a => new DropdownItem
            {
                Id = a.Id,
                Nome = a.Nome
            })
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<List<DropdownItem>> ObterStatusAsync()
    {
        return new List<DropdownItem>
        {
            new DropdownItem { Id = 1, Nome = "Ativo" },
            new DropdownItem { Id = 0, Nome = "Inativo" }
        };
    }
}