using System.Threading.Tasks;
using Services.AvaliacaoMentorCompetencia;

namespace Services.AvaliacaoMentorCompetencia.Common
{
    public interface IAvaliacaoMentorCompetenciaService
    {
        Task<AvaliacaoMentorCompetenciaService.AvaliacaoMentorCompetenciaDto> ObterAvaliacaoMentorCompetenciaAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor);
        Task<string> ObterRadarAsync(int idAssociado, int idProjeto, int idPeriodo, int idCargo);
        Task<bool> SalvarConsideracoesMentorAsync(AvaliacaoMentorCompetenciaService.ConsideracoesMentorInput input);
        string FormatPercentagem(decimal nota);
        string FormatDecimal(decimal nota);
        string TruncarTexto(string texto, int qtdCaracteres);
    }
}
