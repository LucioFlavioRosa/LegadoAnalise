using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Mentoria.Common.ComboHelperMentoria
{
    public static class ComboHelperMentoria
    {
        public static async Task<List<ComboItem>> GetProjetosComboAsync(ApplicationDbContext db, int mentorId, int? clienteId = null, int? status = null, int? periodoId = null)
        {
            var query = db.Projetos.AsQueryable();
            if (mentorId > 0)
                query = query.Where(p => p.AssociadoGestor.Id == mentorId);
            if (clienteId.HasValue)
                query = query.Where(p => p.IdCliente == clienteId.Value);
            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);
            if (periodoId.HasValue)
                query = query.Where(p => p.AssociadosProjeto.Any(ap => ap.IdPeriodo == periodoId.Value));
            var projetos = await query.OrderBy(p => p.Nome).ToListAsync();
            var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
            items.AddRange(projetos.Select(p => new ComboItem { Value = p.Id.ToString(), Text = p.Nome }));
            return items;
        }

        public static async Task<List<ComboItem>> GetClientesComboAsync(ApplicationDbContext db)
        {
            var clientes = await db.Clientes.OrderBy(c => c.Nome).ToListAsync();
            var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
            items.AddRange(clientes.Select(c => new ComboItem { Value = c.IdCliente.ToString(), Text = c.Nome }));
            return items;
        }

        public static async Task<List<ComboItem>> GetPeriodosComboAsync(ApplicationDbContext db, int empresaId)
        {
            var periodos = await db.PeriodosAvaliacoes.Where(p => p.IdEmpresa == empresaId).OrderByDescending(p => p.IdPeriodo).ToListAsync();
            var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
            items.AddRange(periodos.Select(p => new ComboItem { Value = p.IdPeriodo.ToString(), Text = p.Nome }));
            return items;
        }

        public static async Task<List<ComboItem>> GetStatusComboAsync(ApplicationDbContext db)
        {
            var statusList = new List<ComboItem>
            {
                new ComboItem { Value = "", Text = "[Selecionar]" },
                new ComboItem { Value = "1", Text = "Ativo" },
                new ComboItem { Value = "0", Text = "Inativo" }
            };
            return await Task.FromResult(statusList);
        }
    }
}
