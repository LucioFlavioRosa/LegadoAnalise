using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public class VerticaisService : IVerticaisService
{
    private readonly ApplicationDbContext _context;
    
    public VerticaisService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Vertical>> ObterTodasAsync(bool apenasAtivas = false)
    {
        var query = _context.Verticais.AsQueryable();
        
        if (apenasAtivas)
        {
            query = query.Where(v => v.ATV == 1);
        }
        
        return await query.OrderBy(v => v.Descricao).ToListAsync();
    }
    
    public async Task<Vertical?> ObterPorIdAsync(int id)
    {
        return await _context.Verticais.FindAsync(id);
    }
}