using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Models;
using Peers.Moderno.Data;

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
            return await _context.Verticais.ToListAsync();
        }
    }
}