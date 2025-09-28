using System.Threading.Tasks;
using System.Collections.Generic;
using Peers.Moderno.Models;

namespace Services.Comentarios.Common;

public interface IComentariosService
{
    Task<COMENTARIOS?> ObterComentarioAsync(int idAssociado, int idPeriodo);
    Task<COMENTARIOS> CriarOuAtualizarComentarioAsync(int idAssociado, int idPeriodo, string comentario);
    Task<List<COMENTARIOS>> ObterComentariosPorAssociadoAsync(int idAssociado);
    Task<List<COMENTARIOS>> ObterComentariosPorPeriodoAsync(int idPeriodo);
}