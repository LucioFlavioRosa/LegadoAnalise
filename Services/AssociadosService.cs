using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Peers.Moderno.Services
{
    public class AssociadosService : IAssociadosService
    {
        private readonly ApplicationDbContext _context;

        public AssociadosService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ASSOCIADOS>> ObterAssociadosAsync()
        {
            return await _context.Associados
                .Include(a => a.CARGOS)
                .Include(a => a.PERFIS)
                .Include(a => a.EMPRESAS)
                .Include(a => a.ASSOCIADOS2)
                .Include(a => a.VERTICAL)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ASSOCIADOS?> ObterAssociadoAsync(int id)
        {
            return await _context.Associados
                .Include(a => a.CARGOS)
                .Include(a => a.PERFIS)
                .Include(a => a.EMPRESAS)
                .Include(a => a.ASSOCIADOS2)
                .Include(a => a.VERTICAL)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAssociado == id);
        }

        public async Task<bool> InserirAssociadoAsync(ASSOCIADOS associado)
        {
            _context.Associados.Add(associado);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AlterarAssociadoAsync(int id, ASSOCIADOS associado)
        {
            var existente = await _context.Associados.FindAsync(id);
            if (existente == null) return false;
            _context.Entry(existente).CurrentValues.SetValues(associado);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExcluirAssociadoAsync(int id)
        {
            var existente = await _context.Associados.FindAsync(id);
            if (existente == null) return false;
            _context.Associados.Remove(existente);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}