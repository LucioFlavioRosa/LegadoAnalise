using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class DimensoesService : IDimensoesService
{
    private readonly ApplicationDbContext _context;

    public DimensoesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dimensao>> ObterListaDimensoesAsync(bool ativo = true, string? tipoAvaliacao = null)
    {
        var query = _context.Dimensoes
            .Where(d => d.ATV == (ativo ? 1 : 0))
            .AsQueryable();

        if (!string.IsNullOrEmpty(tipoAvaliacao))
        {
            query = query.Where(d => d.TipoAvaliacao == tipoAvaliacao);
        }

        return await query.OrderBy(d => d.Nome).ToListAsync();
    }

    public async Task<Dimensao?> ObterDimensaoAsync(int id)
    {
        return await _context.Dimensoes.FindAsync(id);
    }
}