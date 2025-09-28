using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Periodos.Common;

namespace Services.Periodos;

public interface IPeriodoService
{
    Task<List<PERIODOSAVALIACOES>> ListarPeriodosAsync(int idEmpresa);
    Task<PERIODOSAVALIACOES?> ObterPeriodoAsync(int idPeriodo);
    Task InserirPeriodoAsync(PERIODOSAVALIACOES periodo);
    Task AlterarPeriodoAsync(PERIODOSAVALIACOES periodo);
    Task<PERIODOSAVALIACOES?> VerificaExistenciaPeriodoAsync(DateTime dataInicio, DateTime dataFim, int idEmpresa);
}

public class PeriodoService : IPeriodoService
{
    private readonly ApplicationDbContext _db;
    private readonly IPeriodoValidator _validator;

    public PeriodoService(ApplicationDbContext db, IPeriodoValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<List<PERIODOSAVALIACOES>> ListarPeriodosAsync(int idEmpresa)
    {
        return await _db.PeriodosAvaliacoes
            .Where(p => p.IdEmpresa == idEmpresa)
            .OrderByDescending(p => p.IdPeriodo)
            .ToListAsync();
    }

    public async Task<PERIODOSAVALIACOES?> ObterPeriodoAsync(int idPeriodo)
    {
        return await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
    }

    public async Task InserirPeriodoAsync(PERIODOSAVALIACOES periodo)
    {
        if (!_validator.ValidarPeriodo(periodo, out var mensagemErro))
            throw new InvalidOperationException(mensagemErro);
        _db.PeriodosAvaliacoes.Add(periodo);
        await _db.SaveChangesAsync();
    }

    public async Task AlterarPeriodoAsync(PERIODOSAVALIACOES periodo)
    {
        if (!_validator.ValidarPeriodo(periodo, out var mensagemErro))
            throw new InvalidOperationException(mensagemErro);
        _db.PeriodosAvaliacoes.Update(periodo);
        await _db.SaveChangesAsync();
    }

    public async Task<PERIODOSAVALIACOES?> VerificaExistenciaPeriodoAsync(DateTime dataInicio, DateTime dataFim, int idEmpresa)
    {
        return await _db.PeriodosAvaliacoes
            .Where(p => p.IdEmpresa == idEmpresa &&
                        ((p.DataInicio <= dataFim && p.DataFim >= dataInicio)))
            .OrderBy(p => p.DataInicio)
            .FirstOrDefaultAsync();
    }
}
