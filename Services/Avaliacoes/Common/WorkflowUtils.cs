using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Avaliacoes.Common;

public interface IWorkflowUtils
{
    Task<List<Prazo>> ObterTodosPrazosAtivosAsync();
    Task<Prazo?> ObterPrazoAsync(int idPrazo);
    Task<Workflow?> ObterWorkflowPorPeriodoAsync(int idPeriodo);
    string ObterEtapaNaoIniciada();
    string ObterEtapaAutoAvaliacao();
    string ObterEtapaAvaliacaoAsCegas();
    string ObterEtapaAvaliacaoGestor();
    string ObterEtapaFeedback();
    string ObterEtapaAvaliacaoMentor();
    string ObterEtapaFinalizada();
}

public class WorkflowUtils : IWorkflowUtils
{
    private readonly ApplicationDbContext _context;

    public WorkflowUtils(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Prazo>> ObterTodosPrazosAtivosAsync()
    {
        return await _context.Set<Prazo>()
            .Where(p => p.Ativo)
            .OrderBy(p => p.NomeDisparo)
            .ToListAsync();
    }

    public async Task<Prazo?> ObterPrazoAsync(int idPrazo)
    {
        return await _context.Set<Prazo>()
            .FirstOrDefaultAsync(p => p.IdPrazo == idPrazo);
    }

    public async Task<Workflow?> ObterWorkflowPorPeriodoAsync(int idPeriodo)
    {
        return await _context.Set<Workflow>()
            .FirstOrDefaultAsync(w => w.IdPeriodo == idPeriodo && w.Ativo);
    }

    public string ObterEtapaNaoIniciada() => "NI";
    public string ObterEtapaAutoAvaliacao() => "AA";
    public string ObterEtapaAvaliacaoAsCegas() => "AC";
    public string ObterEtapaAvaliacaoGestor() => "AG";
    public string ObterEtapaFeedback() => "FB";
    public string ObterEtapaAvaliacaoMentor() => "AM";
    public string ObterEtapaFinalizada() => "AFI";
}

public class Workflow
{
    public int IdWorkflow { get; set; }
    public int IdPeriodo { get; set; }
    public int DiasAutoAvaliacao { get; set; }
    public int DiasAvaliacaoAsCegas { get; set; }
    public int DiasAvaliacaoGestor { get; set; }
    public int DiasFeedback { get; set; }
    public int DiasMentor { get; set; }
    public bool Ativo { get; set; }
    public DateTime DHC { get; set; }
    public int USR { get; set; }
}