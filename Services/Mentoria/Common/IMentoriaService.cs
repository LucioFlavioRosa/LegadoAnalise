using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Mentoria.Common;

public interface IMentoriaService
{
    Task<List<PERIODOSAVALIACOES>> ListarPeriodosAsync(int tipo);
    Task<PERIODOSAVALIACOES?> ObterUltimoPeriodoAsync();
    Task<List<MENTORADORESPOSTAS>> ObterMentoradoRespostasAsync(int idMentorado, int? idPeriodo = null, int? idMentor = null, int? idPergunta = null);
    Task<List<MentorPergunta>> ObterMentorPerguntasAsync();
    Task<List<MentorNota>> ObterMentorNotasAsync();
    Task GerirMentoradoRespostasAsync(MENTORADORESPOSTAS resposta);
    Task<MENTORADORESPOSTAS?> ObterMentoradoRespostaPorIdAsync(int idResposta);
}
