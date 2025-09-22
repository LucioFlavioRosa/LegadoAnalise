using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public class EmpresasService : IEmpresasService
{
    private readonly ApplicationDbContext _context;
    
    public EmpresasService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Empresa>> ObterTodasAsync(bool apenasAtivas = false)
    {
        var query = _context.Empresas.AsQueryable();
        
        if (apenasAtivas)
        {
            query = query.Where(e => e.ATV == 1);
        }
        
        return await query.OrderBy(e => e.Nome).ToListAsync();
    }
    
    public async Task<Empresa?> ObterPorIdAsync(int id)
    {
        return await _context.Empresas.FindAsync(id);
    }
}