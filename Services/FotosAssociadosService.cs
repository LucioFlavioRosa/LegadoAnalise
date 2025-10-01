using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Services
{
    public class FotosAssociadosService : IFotosAssociadosService
    {
        private readonly ApplicationDbContext _context;
        public FotosAssociadosService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FOTOSASSOCIADOS?> ObterFotoPorAssociadoAsync(int idAssociado)
        {
            return await _context.FOTOSASSOCIADOS.FirstOrDefaultAsync(f => f.IdAssociado == idAssociado);
        }

        public async Task<bool> AdicionarFotoAsync(FOTOSASSOCIADOS foto)
        {
            _context.FOTOSASSOCIADOS.Add(foto);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarFotoAsync(FOTOSASSOCIADOS foto)
        {
            _context.FOTOSASSOCIADOS.Update(foto);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}