using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Data;

namespace Services
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
            return await _context.CARGOS
                .Where(c => ativo == null || c.ATV == (ativo.Value ? 1 : 0))
                .ToListAsync();
        }

        public async Task<List<PROMOCOES>> ObterPromocoesAssociadoAsync(int idAssociado)
        {
            return await _context.PROMOCOES
                .Include(p => p.CARGOS)
                .Include(p => p.CARGOS1)
                .Where(p => p.idAssociado == idAssociado && p.ATV == true)
                .OrderByDescending(p => p.DataPromocao)
                .ToListAsync();
        }

        public async Task<bool> AdicionarPromocaoAsync(PROMOCOES promocao)
        {
            _context.PROMOCOES.Add(promocao);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AlterarPromocaoComentarioAsync(int idPromocao, string comentario)
        {
            var promocao = await _context.PROMOCOES.FindAsync(idPromocao);
            if (promocao == null) return false;
            promocao.Comentarios = comentario;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<PROMOCOES>> ObterListaPromocoesAsync()
        {
            return await _context.PROMOCOES
                .Include(p => p.ASSOCIADOS)
                .Include(p => p.CARGOS)
                .Include(p => p.CARGOS1)
                .ToListAsync();
        }

        public async Task<PROMOCOES?> ObterPromocaoAsync(int idAssociado, int idCargoAnterior, int idCargoNovo)
        {
            return await _context.PROMOCOES
                .FirstOrDefaultAsync(p => p.idAssociado == idAssociado && p.idCargoAnterior == idCargoAnterior && p.idCargoNovo == idCargoNovo);
        }

        public async Task<bool> AlterarPromocaoAsync(PROMOCOES promocao)
        {
            var existente = await _context.PROMOCOES.FindAsync(promocao.idPromocao);
            if (existente == null) return false;
            _context.Entry(existente).CurrentValues.SetValues(promocao);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}