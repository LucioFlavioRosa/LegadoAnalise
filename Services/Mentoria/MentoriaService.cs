using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Microsoft.EntityFrameworkCore;
using Services.Mentoria.Common;

namespace Services.Mentoria;

public interface IMentoriaService
{
    Task<List<PERIODOSAVALIACOES>> GetPeriodosLiberadosMentoriaAsync(int empresaId = 1);
    Task<PERIODOSAVALIACOES?> GetUltimoPeriodoLiberadoMentoriaAsync();
    Task<ASSOCIADOS?> GetMentorAsync(int idAssociado);
    Task<List<MENTORPERGUNTAS>> GetMentorPerguntasAsync();
    Task<List<MENTORADORESPOSTAS>> GetMentoradoRespostasAsync(int? idPeriodo = null, int? idMentor = null, int? idResposta = null);
    double CalcularNota(IEnumerable<MENTORADORESPOSTAS> respostas);
}

public class MentoriaService : IMentoriaService
{
    private readonly ApplicationDbContext _db;
    private const double NOTA_MAXIMA = 5;

    public MentoriaService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<PERIODOSAVALIACOES>> GetPeriodosLiberadosMentoriaAsync(int empresaId = 1)
    {
        return await _db.PeriodosAvaliacoes
            .Where(p => p.EmpresaId == empresaId && p.LiberadoMentoria)
            .OrderByDescending(p => p.IdPeriodo)
            .ToListAsync();
    }

    public async Task<PERIODOSAVALIACOES?> GetUltimoPeriodoLiberadoMentoriaAsync()
    {
        return await _db.PeriodosAvaliacoes
            .Where(p => p.LiberadoMentoria)
            .OrderByDescending(p => p.IdPeriodo)
            .FirstOrDefaultAsync();
    }

    public async Task<ASSOCIADOS?> GetMentorAsync(int idAssociado)
    {
        return await _db.Associados
            .Include(a => a.Cargo)
            .FirstOrDefaultAsync(a => a.Id == idAssociado);
    }

    public async Task<List<MENTORPERGUNTAS>> GetMentorPerguntasAsync()
    {
        return await _db.Set<MENTORPERGUNTAS>().OrderBy(q => q.Ordem).ToListAsync();
    }

    public async Task<List<MENTORADORESPOSTAS>> GetMentoradoRespostasAsync(int? idPeriodo = null, int? idMentor = null, int? idResposta = null)
    {
        var query = _db.Set<MENTORADORESPOSTAS>().AsQueryable();
        if (idPeriodo.HasValue)
            query = query.Where(x => x.idPeriodo == idPeriodo.Value);
        if (idMentor.HasValue)
            query = query.Where(x => x.idMentor == idMentor.Value);
        if (idResposta.HasValue)
            query = query.Where(x => x.idResposta == idResposta.Value);
        return await query.ToListAsync();
    }

    public double CalcularNota(IEnumerable<MENTORADORESPOSTAS> respostas)
    {
        var validas = respostas.Where(x => x.idNota != null && x.MENTORPERGUNTASNOTAS != null && x.MENTORPERGUNTASNOTAS.Valor > 0).ToList();
        if (validas.Count > 0)
        {
            return Math.Round(validas.Select(x => x.MENTORPERGUNTASNOTAS.Valor).Average(), 2);
        }
        return double.NaN;
    }
}
