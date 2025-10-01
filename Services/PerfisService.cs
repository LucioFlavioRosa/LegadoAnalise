using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Data;

namespace Services
{
    public class PerfisService : IPerfisService
    {
        private readonly ApplicationDbContext _context;

        public PerfisService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PERFIS>> ObterListaPerfisAsync(bool? ativo = null)
        {
            return await _context.PERFIS
                .Where(p => ativo == null || p.ATV == (ativo.Value ? 1 : 0))
                .ToListAsync();
        }
    }
}