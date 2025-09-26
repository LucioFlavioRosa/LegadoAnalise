using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Mentoria.Common;

namespace Peers.Moderno.Services.Mentoria;

public class MentoriaService : IMentoriaService
{
    private readonly ApplicationDbContext _context;

    public MentoriaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PERIODOSAVALIACOES>> ListarPeriodosAsync(int tipo)
    {
        return await _context.PeriodosAvaliacoes
            .Where(p => p.Tipo == tipo)
            .OrderByDescending(p => p.IdPeriodo)
            .ToListAsync();
    }

    public async Task<PERIODOSAVALIACOES?> ObterUltimoPeriodoAsync()
    {
        return await _context.PeriodosAvaliacoes
            .OrderByDescending(p => p.IdPeriodo)
            .FirstOrDefaultAsync();
    }

    public async Task<List<MENTORADORESPOSTAS>> ObterMentoradoRespostasAsync(int idMentorado, int? idPeriodo = null, int? idMentor = null, int? idPergunta = null)
    {
        var query = _context.Set<MENTORADORESPOSTAS>().AsQueryable();
        query = query.Where(x => x.idMentorado == idMentorado);
        if (idPeriodo.HasValue)
            query = query.Where(x => x.idPeriodo == idPeriodo.Value);
        if (idMentor.HasValue)
            query = query.Where(x => x.idMentor == idMentor.Value);
        if (idPergunta.HasValue)
            query = query.Where(x => x.idPergunta == idPergunta.Value);
        return await query.ToListAsync();
    }

    public async Task<List<MentorPergunta>> ObterMentorPerguntasAsync()
    {
        // Supondo que MentorPergunta é uma entidade mapeada
        return await _context.Set<MentorPergunta>().OrderBy(x => x.Ordem).ToListAsync();
    }

    public async Task<List<MentorNota>> ObterMentorNotasAsync()
    {
        // Supondo que MentorNota é uma entidade mapeada
        return await _context.Set<MentorNota>().OrderBy(x => x.Ordem).ToListAsync();
    }

    public async Task GerirMentoradoRespostasAsync(MENTORADORESPOSTAS resposta)
    {
        var existente = await _context.Set<MENTORADORESPOSTAS>().FirstOrDefaultAsync(x => x.idResposta == resposta.idResposta);
        if (existente != null)
        {
            _context.Entry(existente).CurrentValues.SetValues(resposta);
        }
        else
        {
            await _context.Set<MENTORADORESPOSTAS>().AddAsync(resposta);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<MENTORADORESPOSTAS?> ObterMentoradoRespostaPorIdAsync(int idResposta)
    {
        return await _context.Set<MENTORADORESPOSTAS>().FirstOrDefaultAsync(x => x.idResposta == idResposta);
    }
}
