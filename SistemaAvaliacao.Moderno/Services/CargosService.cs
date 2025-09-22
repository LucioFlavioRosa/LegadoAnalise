using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public class CargosService : ICargosService
{
    private readonly ApplicationDbContext _context;
    
    public CargosService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Cargo>> ObterTodosAsync(bool apenasAtivos = false)
    {
        var query = _context.Cargos.AsQueryable();
        
        if (apenasAtivos)
        {
            query = query.Where(c => c.ATV == 1);
        }
        
        return await query.OrderBy(c => c.Nome).ToListAsync();
    }
    
    public async Task<Cargo?> ObterPorIdAsync(int id)
    {
        return await _context.Cargos.FindAsync(id);
    }
    
    public async Task<bool> InserirAsync(Cargo cargo)
    {
        try
        {
            _context.Cargos.Add(cargo);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> AtualizarAsync(Cargo cargo)
    {
        try
        {
            _context.Cargos.Update(cargo);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}