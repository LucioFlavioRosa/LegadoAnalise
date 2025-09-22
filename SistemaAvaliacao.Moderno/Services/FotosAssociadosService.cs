using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public class FotosAssociadosService : IFotosAssociadosService
{
    private readonly ApplicationDbContext _context;
    
    public FotosAssociadosService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<FotoAssociado?> ObterPorAssociadoAsync(int idAssociado)
    {
        return await _context.FotosAssociados
            .FirstOrDefaultAsync(f => f.IdAssociado == idAssociado);
    }
    
    public async Task<bool> InserirAsync(FotoAssociado foto)
    {
        try
        {
            _context.FotosAssociados.Add(foto);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> AtualizarAsync(FotoAssociado foto)
    {
        try
        {
            _context.FotosAssociados.Update(foto);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> RemoverAsync(int idAssociado)
    {
        try
        {
            var foto = await ObterPorAssociadoAsync(idAssociado);
            if (foto != null)
            {
                _context.FotosAssociados.Remove(foto);
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
}