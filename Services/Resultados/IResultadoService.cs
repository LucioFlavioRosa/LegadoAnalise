using Peers.Moderno.Models;

namespace Services.Resultados;

public interface IResultadoService
{
    Task<List<ResultadoProjetosModel>> ObterResultadosProjetosAsync(int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo);
    Task<ResultadoSomaProjetosModel?> ObterSomaResultadosProjetosAsync(List<ResultadoProjetosModel> resultadosProjetos);
    Task<Associado?> ObterAssociadoMentorCargoAsync(int idAssociado);
    Task<PERIODOSAVALIACOES?> ObterPeriodoAsync(int idPeriodo);
}
