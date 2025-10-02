using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

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