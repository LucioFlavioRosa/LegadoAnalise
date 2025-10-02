using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services
{
    public class PerfisService : IPerfisService
    {
        private readonly ApplicationDbContext _context;
        public PerfisService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PERFIS>> ObterListaPerfisAsync(bool ativo)
        {
            return await _context.Perfis
                .Where(p => !ativo || p.ATV == 1)
                .ToListAsync();
        }
    }
}