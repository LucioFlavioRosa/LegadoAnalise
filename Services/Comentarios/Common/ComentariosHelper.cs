using System.Threading.Tasks;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Comentarios.Common;

public static class ComentariosHelper
{
    public static async Task<COMENTARIOS> GarantirComentarioAsync(ApplicationDbContext db, int idAssociado, int idPeriodo)
    {
        var comentario = await db.Set<COMENTARIOS>()
            .FirstOrDefaultAsync(c => c.idAssociado == idAssociado && c.idPeriodo == idPeriodo);
        if (comentario == null)
        {
            comentario = new COMENTARIOS
            {
                idAssociado = idAssociado,
                idPeriodo = idPeriodo,
                Comentario = string.Empty,
                DHC = DateTime.Now
            };
            db.Set<COMENTARIOS>().Add(comentario);
            await db.SaveChangesAsync();
        }
        return comentario;
    }

    public static bool ComentarioEstaPreenchido(COMENTARIOS comentario)
    {
        return !string.IsNullOrWhiteSpace(comentario?.Comentario);
    }
}