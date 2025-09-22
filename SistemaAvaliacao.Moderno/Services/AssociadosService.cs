using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public class AssociadosService : IAssociadosService
{
    private readonly ApplicationDbContext _context;
    
    public AssociadosService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Associado>> ObterTodosAsync(bool apenasAtivos = false)
    {
        var query = _context.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Perfil)
            .Include(a => a.Mentor)
            .Include(a => a.VerticalNavigation)
            .AsQueryable();
            
        if (apenasAtivos)
        {
            query = query.Where(a => a.ATV == 1);
        }
        
        return await query.OrderBy(a => a.Nome).ToListAsync();
    }
    
    public async Task<Associado?> ObterPorIdAsync(int id)
    {
        return await _context.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Perfil)
            .Include(a => a.Mentor)
            .Include(a => a.VerticalNavigation)
            .FirstOrDefaultAsync(a => a.IdAssociado == id);
    }
    
    public async Task<Associado?> ObterPorEmailAsync(string email)
    {
        return await _context.Associados
            .FirstOrDefaultAsync(a => a.Email == email);
    }
    
    public async Task<bool> InserirAsync(Associado associado)
    {
        try
        {
            _context.Associados.Add(associado);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> AtualizarAsync(Associado associado)
    {
        try
        {
            _context.Associados.Update(associado);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> InativarAsync(int id)
    {
        try
        {
            var associado = await _context.Associados.FindAsync(id);
            if (associado != null)
            {
                associado.ATV = 0;
                associado.IdStatus = 0;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<Associado?> ObterUltimoAsync()
    {
        return await _context.Associados
            .OrderByDescending(a => a.IdAssociado)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<Associado>> ObterMentoresAsync()
    {
        return await _context.Associados
            .Where(a => a.ATV == 1)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }
}