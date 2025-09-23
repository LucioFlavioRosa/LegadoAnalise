using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Associados.Common;

public interface IDropdownService
{
    Task<List<DropdownItem>> ObterCargosAsync();
    Task<List<DropdownItem>> ObterPerfisAsync();
    Task<List<DropdownItem>> ObterMentoresAsync();
    Task<List<DropdownItem>> ObterVerticaisAsync();
}

public class DropdownService : IDropdownService
{
    private readonly ApplicationDbContext _context;

    public DropdownService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DropdownItem>> ObterCargosAsync()
    {
        return await _context.Cargos
            .Where(c => c.ATV == 1)
            .Select(c => new DropdownItem { Id = c.IdCargo, Nome = c.Nome })
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<List<DropdownItem>> ObterPerfisAsync()
    {
        return await _context.Perfis
            .Where(p => p.Ativo)
            .Select(p => new DropdownItem { Id = p.Id, Nome = p.Nome })
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<List<DropdownItem>> ObterMentoresAsync()
    {
        return await _context.Associados
            .Where(a => a.Ativo)
            .Select(a => new DropdownItem { Id = a.Id, Nome = a.Nome })
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<List<DropdownItem>> ObterVerticaisAsync()
    {
        return await _context.Verticais
            .Where(v => v.Ativo)
            .Select(v => new DropdownItem { Id = v.Id, Nome = v.Nome })
            .OrderBy(v => v.Nome)
            .ToListAsync();
    }
}