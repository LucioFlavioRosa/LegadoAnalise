using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class PremissasService : IPremissasService
{
    private readonly ApplicationDbContext _context;

    public PremissasService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PremissasRadar>> ObterListaAsync()
    {
        return await _context.PremissasRadar
            .Include(p => p.Eixo)
            .Include(p => p.Cargo)
            .Include(p => p.CargoNivel)
            .OrderBy(p => p.IdPremissa)
            .ToListAsync();
    }

    public async Task<PremissasRadar?> ObterAsync(int id)
    {
        return await _context.PremissasRadar
            .Include(p => p.Eixo)
            .Include(p => p.Cargo)
            .Include(p => p.CargoNivel)
            .FirstOrDefaultAsync(p => p.IdPremissa == id);
    }

    public async Task<bool> InserirAsync(PremissasRadar premissa)
    {
        try
        {
            _context.PremissasRadar.Add(premissa);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AlterarAsync(PremissasRadar premissa)
    {
        try
        {
            _context.PremissasRadar.Update(premissa);
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
            var premissa = await _context.PremissasRadar.FindAsync(id);
            if (premissa != null)
            {
                premissa.ATV = premissa.ATV == 1 ? 0 : 1;
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

    public async Task<List<Eixo>> ObterEixosAsync()
    {
        return await _context.Eixos
            .Where(e => e.ATV == 1)
            .OrderBy(e => e.Nome)
            .ToListAsync();
    }

    public async Task<List<Cargo>> ObterCargosAsync()
    {
        return await _context.Cargos
            .Where(c => c.ATV == 1)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<List<CargoNivel>> ObterNiveisAsync()
    {
        return await _context.CargosNiveis
            .Where(n => n.ATV == 1)
            .OrderBy(n => n.Nivel)
            .ToListAsync();
    }
}