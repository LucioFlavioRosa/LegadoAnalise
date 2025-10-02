using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services
{
    public class FotosAssociadosService : IFotosAssociadosService
    {
        private readonly ApplicationDbContext _context;
        public FotosAssociadosService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FOTOSASSOCIADOS> ObterFotoPorAssociadoAsync(int idAssociado)
        {
            return await _context.FotosAssociados.FirstOrDefaultAsync(f => f.IdAssociado == idAssociado);
        }

        public async Task<bool> AdicionarFotoAsync(FOTOSASSOCIADOS foto)
        {
            _context.FotosAssociados.Add(foto);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarFotoAsync(FOTOSASSOCIADOS foto)
        {
            var existente = await _context.FotosAssociados.FirstOrDefaultAsync(f => f.IdAssociado == foto.IdAssociado);
            if (existente == null) return false;
            _context.Entry(existente).CurrentValues.SetValues(foto);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}