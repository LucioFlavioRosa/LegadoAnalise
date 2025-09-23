using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class EixosService : IEixosService
{
    private readonly ApplicationDbContext _context;

    public EixosService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Eixo>> ObterListaEixosAsync(bool ativo = true, string? tipoAvaliacao = null)
    {
        var query = _context.Eixos
            .Where(e => e.ATV == (ativo ? 1 : 0))
            .AsQueryable();

        if (!string.IsNullOrEmpty(tipoAvaliacao))
        {
            query = query.Where(e => e.TipoAvaliacao == tipoAvaliacao);
        }

        return await query.OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<Eixo?> ObterEixoAsync(int id)
    {
        return await _context.Eixos.FindAsync(id);
    }
}