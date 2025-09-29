using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Common
{
    public interface IExportFileService
    {
        Task<byte[]> GerarExcelResultadoLiderancaAsync(List<PERIODOSAVALIACOES> periodos, List<AvaliacaoCompetencia> avaliacoes, List<Associado> associados, List<AvaliacaoCompetenciaNota> avaliacoesNotas);
        Task<byte[]> GerarExcelResultadoDesempenhoAsync(List<ResultadoProjetosModel> resultados);
        Task<byte[]> GerarExcelResultadoMentoriaAsync(List<ConsideracoesMentor> consideracoes);
    }
}