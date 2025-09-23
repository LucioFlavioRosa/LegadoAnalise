using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class CargosService : ICargosService
{
    private readonly ApplicationDbContext _context;

    public CargosService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cargo>> ObterListaCargosAsync(bool ativo = true)
    {
        return await _context.Cargos
            .Where(c => c.ATV == (ativo ? 1 : 0))
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cargo?> ObterCargoAsync(int id)
    {
        return await _context.Cargos.FindAsync(id);
    }

    public async Task<RelacaoCargoSubcompetencia?> ObterRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubCompetencia)
    {
        return await _context.RelacoesCargosSubcompetencias
            .FirstOrDefaultAsync(r => r.IdCargo == idCargo && r.IdSubcompetencia == idSubCompetencia);
    }

    public async Task AtualizarRelacaoCargoSubcompetenciaAsync(RelacaoCargoSubcompetencia relacao)
    {
        var existente = await ObterRelacaoCargoSubcompetenciaAsync(relacao.IdCargo, relacao.IdSubcompetencia);
        
        if (existente != null)
        {
            existente.Descricao = relacao.Descricao;
            _context.RelacoesCargosSubcompetencias.Update(existente);
        }
        else
        {
            _context.RelacoesCargosSubcompetencias.Add(relacao);
        }
        
        await _context.SaveChangesAsync();
    }
}