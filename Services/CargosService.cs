using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Peers.Moderno.Services
{
    public class CargosService : ICargosService
    {
        private readonly ApplicationDbContext _context;

        public CargosService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CARGOS>> ObterListaCargosAsync(bool? ativo = null)
        {
            // Se ativo for usado, filtrar por campo ATV (se existir)
            return await _context.Cargos.AsNoTracking().ToListAsync();
        }

        public async Task<bool> AdicionarPromocaoAsync(PROMOCOES promocao)
        {
            _context.Promocoes.Add(promocao);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<PROMOCOES>> ObterPromocoesAssociadoAsync(int idAssociado)
        {
            return await _context.Promocoes
                .Where(p => p.idAssociado == idAssociado)
                .Include(p => p.ASSOCIADOS)
                .Include(p => p.CARGOS)
                .Include(p => p.CARGOS1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AlterarPromocaoComentarioAsync(int id, string comentario)
        {
            var promocao = await _context.Promocoes.FindAsync(id);
            if (promocao == null) return false;
            promocao.Comentarios = comentario;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}