using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class AvaliacoesService : IAvaliacoesService
{
    private readonly ApplicationDbContext _context;

    public AvaliacoesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AvaliacaoCompetenciaNota>> ObterAvaliacaoCompetenciasNotasAsync()
    {
        return await _context.AvaliacoesCompetenciasNotas
            .Where(n => !n.IndFeedback && n.ATV == 1)
            .OrderBy(n => n.CodigoNota)
            .ToListAsync();
    }

    public async Task<List<ModoCalculoCompetencia>> ObterModosCalculosCompetenciasAsync()
    {
        return await _context.ModosCalculosCompetencias
            .OrderBy(m => m.ModoDesc)
            .ToListAsync();
    }
}