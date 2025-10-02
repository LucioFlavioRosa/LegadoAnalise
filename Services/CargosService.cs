using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services
{
    public class CargosService : ICargosService
    {
        private readonly ApplicationDbContext _context;
        public CargosService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CARGOS>> ObterListaCargosAsync(bool ativo)
        {
            return await _context.Cargos
                .Where(c => !ativo || c.ATV == 1)
                .ToListAsync();
        }

        public async Task<bool> AdicionarPromocaoAsync(PROMOCOES promocao)
        {
            _context.Promocoes.Add(promocao);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<PROMOCOES>> ObterTodasPromocoesAssociadoAsync(int idAssociado)
        {
            return await _context.Promocoes
                .Where(p => p.idAssociado == idAssociado)
                .Include(p => p.CARGOS)
                .Include(p => p.CARGOS1)
                .ToListAsync();
        }

        public async Task<bool> AlterarPromocaoComentarioAsync(int idPromocao, string comentario)
        {
            var promocao = await _context.Promocoes.FindAsync(idPromocao);
            if (promocao == null) return false;
            promocao.Comentarios = comentario;
            _context.Promocoes.Update(promocao);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<PROMOCOES>> ObterListaPromocoesAsync()
        {
            return await _context.Promocoes
                .Include(p => p.ASSOCIADOS)
                .Include(p => p.CARGOS)
                .Include(p => p.CARGOS1)
                .ToListAsync();
        }

        public async Task<PROMOCOES> ObterPromocaoAsync(int idAssociado, int idCargoAnterior, int idCargoNovo)
        {
            return await _context.Promocoes.FirstOrDefaultAsync(p =>
                p.idAssociado == idAssociado &&
                p.idCargoAnterior == idCargoAnterior &&
                p.idCargoNovo == idCargoNovo);
        }

        public async Task<bool> AlterarPromocaoAsync(PROMOCOES promocao)
        {
            var existente = await _context.Promocoes.FindAsync(promocao.idPromocao);
            if (existente == null) return false;
            _context.Entry(existente).CurrentValues.SetValues(promocao);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}