using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Data;

namespace Services
{
    public class AssociadosService : IAssociadosService
    {
        private readonly ApplicationDbContext _context;

        public AssociadosService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ASSOCIADOS>> ObterAssociadosAsync(bool? ativo = null)
        {
            return await _context.ASSOCIADOS
                .Include(a => a.CARGOS)
                .Include(a => a.PERFIS)
                .Include(a => a.ASSOCIADOS2)
                .Include(a => a.EMPRESAS)
                .Include(a => a.VERTICAL)
                .Where(a => ativo == null || a.ATV == (ativo.Value ? 1 : 0))
                .ToListAsync();
        }

        public async Task<ASSOCIADOS?> ObterAssociadoAsync(int id)
        {
            return await _context.ASSOCIADOS
                .Include(a => a.CARGOS)
                .Include(a => a.PERFIS)
                .Include(a => a.ASSOCIADOS2)
                .Include(a => a.EMPRESAS)
                .Include(a => a.VERTICAL)
                .FirstOrDefaultAsync(a => a.IdAssociado == id);
        }

        public async Task<ASSOCIADOS?> ObterAssociadoPorEmailAsync(string email)
        {
            return await _context.ASSOCIADOS
                .Include(a => a.CARGOS)
                .Include(a => a.PERFIS)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<bool> InserirAssociadoAsync(ASSOCIADOS associado)
        {
            _context.ASSOCIADOS.Add(associado);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AlterarAssociadoAsync(int id, ASSOCIADOS associado)
        {
            var existente = await _context.ASSOCIADOS.FindAsync(id);
            if (existente == null) return false;
            _context.Entry(existente).CurrentValues.SetValues(associado);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> InativarAssociadoAsync(int id)
        {
            var existente = await _context.ASSOCIADOS.FindAsync(id);
            if (existente == null) return false;
            existente.ATV = 0;
            existente.IdStatus = 0;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ASSOCIADOS?> ObterUltimoAssociadoAsync()
        {
            return await _context.ASSOCIADOS
                .OrderByDescending(a => a.IdAssociado)
                .FirstOrDefaultAsync();
        }
    }
}