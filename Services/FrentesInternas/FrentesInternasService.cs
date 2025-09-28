using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Services.FrentesInternas;

public class FrentesInternasService : IFrentesInternasService
{
    private readonly ApplicationDbContext _db;
    public FrentesInternasService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<PeriodoAvaliacaoDto>> ListaTodosPeriodosAsync(int empresaId)
    {
        return await _db.PeriodosAvaliacoes
            .Where(p => p.IdEmpresa == empresaId)
            .OrderByDescending(p => p.IdPeriodo)
            .Select(p => new PeriodoAvaliacaoDto { IdPeriodo = p.IdPeriodo, Periodo = p.Nome })
            .ToListAsync();
    }

    public async Task<PeriodoAvaliacaoDto?> ObterPeriodoUltimoAsync()
    {
        var periodo = await _db.PeriodosAvaliacoes.OrderByDescending(p => p.IdPeriodo).FirstOrDefaultAsync();
        if (periodo == null) return null;
        return new PeriodoAvaliacaoDto { IdPeriodo = periodo.IdPeriodo, Periodo = periodo.Nome };
    }

    public async Task<Associado?> ObterAssociadoAsync(int idAssociado)
    {
        return await _db.Associados.FirstOrDefaultAsync(a => a.Id == idAssociado);
    }

    public async Task<List<FrenteInternaDto>> ObterLiderFrenteInternaAsync(int idAssociado)
    {
        // Supondo que existe uma relação entre lider e frente interna
        return await _db.FrentesInternas
            .Where(f => f.LideresFrente.Any(l => l.IdAssociado == idAssociado))
            .Select(f => new FrenteInternaDto { IdFrenteInterna = f.IdFrenteInterna, FrenteInterna1 = f.Nome })
            .ToListAsync();
    }

    public async Task<List<FrenteInternaDto>> ObterFrenteInternaAsync()
    {
        return await _db.FrentesInternas
            .Select(f => new FrenteInternaDto { IdFrenteInterna = f.IdFrenteInterna, FrenteInterna1 = f.Nome })
            .ToListAsync();
    }

    public async Task<List<Associado>> ObterParticipantesFrentesInternasAsync(int idFrenteInterna)
    {
        return await _db.Associados
            .Where(a => a.FrentesInternasParticipantes.Any(f => f.IdFrenteInterna == idFrenteInterna))
            .ToListAsync();
    }

    public async Task<List<AvaliacaoAlocacaoInternaDto>> ObterAvaliacoesAlocacoesInternasAsync(int? idAvaliador = null, int? idPeriodo = null, int? idAlocacaoInterna = null, int? idAssociado = null, int? idAvaliacaoAlocacaoInterna = null)
    {
        var query = _db.AvaliacoesAlocacoesInternas.AsQueryable();
        if (idAvaliador.HasValue)
            query = query.Where(x => x.IdAvaliador == idAvaliador.Value);
        if (idPeriodo.HasValue)
            query = query.Where(x => x.IdPeriodo == idPeriodo.Value);
        if (idAlocacaoInterna.HasValue)
            query = query.Where(x => x.IdAlocacaoInterna == idAlocacaoInterna.Value);
        if (idAssociado.HasValue)
            query = query.Where(x => x.IdAssociado == idAssociado.Value);
        if (idAvaliacaoAlocacaoInterna.HasValue)
            query = query.Where(x => x.IdAvaliacaoAlocacaoInterna == idAvaliacaoAlocacaoInterna.Value);
        return await query.Select(x => new AvaliacaoAlocacaoInternaDto
        {
            IdAvaliacaoAlocacaoInterna = x.IdAvaliacaoAlocacaoInterna,
            IdAlocacaoInterna = x.IdAlocacaoInterna,
            IdAvaliador = x.IdAvaliador,
            IdAssociado = x.IdAssociado,
            IdPeriodo = x.IdPeriodo,
            IdNota = x.IdNota,
            Comentarios = x.Comentarios,
            DHC = x.DHC,
            ValidadoMD = x.ValidadoMD,
            DHCValidadoMD = x.DHCValidadoMD
        }).ToListAsync();
    }

    public async Task<List<NotaAlocacaoInternaDto>> ObterNotasAlocacoesInternasAsync(int? idNotaAlocacaoInterna = null)
    {
        var query = _db.NotasAlocacoesInternas.AsQueryable();
        if (idNotaAlocacaoInterna.HasValue)
            query = query.Where(x => x.IdNotaAlocacaoInterna == idNotaAlocacaoInterna.Value);
        return await query.Select(x => new NotaAlocacaoInternaDto
        {
            IdNotaAlocacaoInterna = x.IdNotaAlocacaoInterna,
            Descricao = x.Descricao
        }).ToListAsync();
    }

    public async Task GerirAvaliacaoAlocacaoInternaAsync(AvaliacaoAlocacaoInternaDto avaliacao)
    {
        var entity = await _db.AvaliacoesAlocacoesInternas.FirstOrDefaultAsync(x => x.IdAvaliacaoAlocacaoInterna == avaliacao.IdAvaliacaoAlocacaoInterna);
        if (entity != null)
        {
            entity.IdNota = avaliacao.IdNota;
            entity.Comentarios = avaliacao.Comentarios;
            entity.ValidadoMD = avaliacao.ValidadoMD;
            entity.DHC = DateTime.Now;
            entity.DHCValidadoMD = avaliacao.DHCValidadoMD;
            _db.AvaliacoesAlocacoesInternas.Update(entity);
        }
        else
        {
            entity = new AvaliacoesAlocacoesInternas
            {
                IdAlocacaoInterna = avaliacao.IdAlocacaoInterna,
                IdAvaliador = avaliacao.IdAvaliador,
                IdAssociado = avaliacao.IdAssociado,
                IdPeriodo = avaliacao.IdPeriodo,
                IdNota = avaliacao.IdNota,
                Comentarios = avaliacao.Comentarios,
                DHC = DateTime.Now,
                ValidadoMD = avaliacao.ValidadoMD,
                DHCValidadoMD = avaliacao.DHCValidadoMD
            };
            await _db.AvaliacoesAlocacoesInternas.AddAsync(entity);
        }
        await _db.SaveChangesAsync();
    }
}
