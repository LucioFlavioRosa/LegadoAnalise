using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Peers.Moderno.Services
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
            // Se ativo for usado, filtrar por campo ATV (se existir)
            return await _context.Perfis.AsNoTracking().ToListAsync();
        }
    }
}