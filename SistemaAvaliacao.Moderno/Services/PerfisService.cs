using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public class PerfisService : IPerfisService
{
    private readonly ApplicationDbContext _context;
    
    public PerfisService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Perfil>> ObterTodosAsync(bool apenasAtivos = false)
    {
        var query = _context.Perfis.AsQueryable();
        
        if (apenasAtivos)
        {
            query = query.Where(p => p.ATV == 1);
        }
        
        return await query.OrderBy(p => p.Nome).ToListAsync();
    }
    
    public async Task<Perfil?> ObterPorIdAsync(int id)
    {
        return await _context.Perfis.FindAsync(id);
    }
    
    public async Task<bool> InserirAsync(Perfil perfil)
    {
        try
        {
            _context.Perfis.Add(perfil);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> AtualizarAsync(Perfil perfil)
    {
        try
        {
            _context.Perfis.Update(perfil);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}