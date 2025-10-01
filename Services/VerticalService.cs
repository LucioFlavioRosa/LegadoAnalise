using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Services
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
            return await _context.VERTICAL.ToListAsync();
        }
    }
}