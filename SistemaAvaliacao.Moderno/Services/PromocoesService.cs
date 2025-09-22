using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public class PromocoesService : IPromocoesService
{
    private readonly ApplicationDbContext _context;
    
    public PromocoesService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Promocao>> ObterPorAssociadoAsync(int idAssociado)
    {
        return await _context.Promocoes
            .Include(p => p.CargoAnterior)
            .Include(p => p.CargoNovo)
            .Where(p => p.IdAssociado == idAssociado && p.ATV)
            .OrderByDescending(p => p.DataPromocao)
            .ToListAsync();
    }
    
    public async Task<List<Promocao>> ObterTodasAsync()
    {
        return await _context.Promocoes
            .Include(p => p.Associado)
            .Include(p => p.CargoAnterior)
            .Include(p => p.CargoNovo)
            .Where(p => p.ATV)
            .OrderByDescending(p => p.DataPromocao)
            .ToListAsync();
    }
    
    public async Task<bool> InserirAsync(Promocao promocao)
    {
        try
        {
            _context.Promocoes.Add(promocao);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> AtualizarAsync(Promocao promocao)
    {
        try
        {
            _context.Promocoes.Update(promocao);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> AtualizarComentarioAsync(int idPromocao, string comentario)
    {
        try
        {
            var promocao = await _context.Promocoes.FindAsync(idPromocao);
            if (promocao != null)
            {
                promocao.Comentarios = comentario;
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
    
    public async Task<Promocao?> ObterPorIdAsync(int idPromocao)
    {
        return await _context.Promocoes
            .Include(p => p.Associado)
            .Include(p => p.CargoAnterior)
            .Include(p => p.CargoNovo)
            .FirstOrDefaultAsync(p => p.IdPromocao == idPromocao);
    }
}