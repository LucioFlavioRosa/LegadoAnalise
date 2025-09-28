using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Comentarios.Common;

namespace Services.Comentarios;

public class ComentariosService : IComentariosService
{
    private readonly ApplicationDbContext _db;

    public ComentariosService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<COMENTARIOS?> ObterComentarioAsync(int idAssociado, int idPeriodo)
    {
        return await _db.Set<COMENTARIOS>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.idAssociado == idAssociado && c.idPeriodo == idPeriodo);
    }

    public async Task<COMENTARIOS> CriarOuAtualizarComentarioAsync(int idAssociado, int idPeriodo, string comentario)
    {
        var comentarioExistente = await _db.Set<COMENTARIOS>()
            .FirstOrDefaultAsync(c => c.idAssociado == idAssociado && c.idPeriodo == idPeriodo);

        if (comentarioExistente == null)
        {
            var novoComentario = new COMENTARIOS
            {
                idAssociado = idAssociado,
                idPeriodo = idPeriodo,
                Comentario = comentario ?? string.Empty,
                DHC = DateTime.Now
            };
            _db.Set<COMENTARIOS>().Add(novoComentario);
            await _db.SaveChangesAsync();
            return novoComentario;
        }
        else
        {
            comentarioExistente.Comentario = comentario ?? string.Empty;
            comentarioExistente.DHC = DateTime.Now;
            _db.Set<COMENTARIOS>().Update(comentarioExistente);
            await _db.SaveChangesAsync();
            return comentarioExistente;
        }
    }

    public async Task<List<COMENTARIOS>> ObterComentariosPorAssociadoAsync(int idAssociado)
    {
        return await _db.Set<COMENTARIOS>()
            .Where(c => c.idAssociado == idAssociado)
            .OrderByDescending(c => c.DHC)
            .ToListAsync();
    }

    public async Task<List<COMENTARIOS>> ObterComentariosPorPeriodoAsync(int idPeriodo)
    {
        return await _db.Set<COMENTARIOS>()
            .Where(c => c.idPeriodo == idPeriodo)
            .OrderByDescending(c => c.DHC)
            .ToListAsync();
    }
}