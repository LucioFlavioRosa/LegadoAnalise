using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class SubCompetenciasService : ISubCompetenciasService
{
    private readonly ApplicationDbContext _context;

    public SubCompetenciasService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubCompetencia>> ObterListaSubCompetenciasAsync(bool ativo = true)
    {
        return await _context.SubCompetencias
            .Where(s => s.ATV == (ativo ? 1 : 0))
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<SubCompetencia?> ObterSubCompetenciaAsync(int id)
    {
        return await _context.SubCompetencias.FindAsync(id);
    }
}