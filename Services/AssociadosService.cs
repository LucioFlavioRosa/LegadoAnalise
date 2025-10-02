using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

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
                .ToListAsync();
        }

        public async Task<ASSOCIADOS> ObterAssociadoAsync(int id)
        {
            return await _context.Associados
                .Include(a => a.CARGOS)
                .Include(a => a.PERFIS)
                .Include(a => a.EMPRESAS)
                .Include(a => a.ASSOCIADOS2)
                .FirstOrDefaultAsync(a => a.IdAssociado == id);
        }

        public async Task<ASSOCIADOS> ObterAssociadoPorEmailAsync(string email)
        {
            return await _context.Associados
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<bool> InserirAssociadoAsync(ASSOCIADOS associado)
        {
            _context.Associados.Add(associado);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AlteraAssociadoAsync(int id, ASSOCIADOS associado)
        {
            var existente = await _context.Associados.FindAsync(id);
            if (existente == null) return false;
            _context.Entry(existente).CurrentValues.SetValues(associado);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExcluiAssociadoAsync(int id, ASSOCIADOS associado)
        {
            var existente = await _context.Associados.FindAsync(id);
            if (existente == null) return false;
            existente.ATV = 0;
            existente.IdStatus = 0;
            _context.Associados.Update(existente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ASSOCIADOS> ObterUltimoAssociadoAsync()
        {
            return await _context.Associados.OrderByDescending(a => a.IdAssociado).FirstOrDefaultAsync();
        }
    }
}