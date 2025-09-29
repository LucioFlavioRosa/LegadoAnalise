using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Services.Common;

namespace Services.Workflow;

public class WorkflowComboHelper : IWorkflowComboHelper
{
    private readonly ApplicationDbContext _db;

    public WorkflowComboHelper(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ComboItem>> GetPeriodosComboAsync(int empresaId)
    {
        var query = _db.PeriodosAvaliacoes.AsQueryable();
        query = query.Where(p => p.IdEmpresa == empresaId);
        var periodos = await query.OrderByDescending(p => p.IdPeriodo).ToListAsync();
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        items.AddRange(periodos.Select(p => new ComboItem { Value = p.IdPeriodo.ToString(), Text = p.Nome }));
        return items;
    }
}