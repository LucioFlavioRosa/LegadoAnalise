using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Peers.Moderno.Services
{
    public class VerticalService : IVerticalService
    {
        private readonly ApplicationDbContext _context;

        public VerticalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VERTICAL>> ListarVerticaisAsync()
        {
            return await _context.Verticais.AsNoTracking().ToListAsync();
        }
    }
}