using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Services.Workflow;

public class WorkflowService : IWorkflowService
{
    private readonly ApplicationDbContext _db;

    public WorkflowService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<WORKFLOW>> ObterListaAsync()
    {
        return await _db.Set<WORKFLOW>()
            .Include(w => w.PERIODOSAVALIACOES)
            .OrderByDescending(w => w.IdWorkflow)
            .ToListAsync();
    }

    public async Task<WORKFLOW?> ObterByPeriodoAsync(int idPeriodo)
    {
        return await _db.Set<WORKFLOW>()
            .FirstOrDefaultAsync(w => w.IdPeriodo == idPeriodo);
    }

    public async Task<WORKFLOW?> ObterAsync(int idWorkflow)
    {
        return await _db.Set<WORKFLOW>()
            .FirstOrDefaultAsync(w => w.IdWorkflow == idWorkflow);
    }

    public async Task<bool> InserirAsync(WORKFLOW workflow)
    {
        _db.Set<WORKFLOW>().Add(workflow);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> AlterarAsync(int idWorkflow, WORKFLOW workflow)
    {
        var entity = await _db.Set<WORKFLOW>().FirstOrDefaultAsync(w => w.IdWorkflow == idWorkflow);
        if (entity == null)
            return false;
        entity.DataInicio = workflow.DataInicio;
        entity.DiasAutoAvaliacao = workflow.DiasAutoAvaliacao;
        entity.DiasAvaliacaoCegas = workflow.DiasAvaliacaoCegas;
        entity.DiasAvaliacaoGestor = workflow.DiasAvaliacaoGestor;
        entity.DiasFeedback = workflow.DiasFeedback;
        entity.DiasAlertaSemAlteracao = workflow.DiasAlertaSemAlteracao;
        entity.IdPeriodo = workflow.IdPeriodo;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeletarAsync(int idWorkflow)
    {
        var entity = await _db.Set<WORKFLOW>().FirstOrDefaultAsync(w => w.IdWorkflow == idWorkflow);
        if (entity == null)
            return false;
        _db.Set<WORKFLOW>().Remove(entity);
        return await _db.SaveChangesAsync() > 0;
    }
}