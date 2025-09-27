using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.Resultados;

public interface IResultadoService
{
    Task<List<ResultadoProjetosModel>> ObterResultadoAssociadoAsync(int idAssociado, int idPeriodo, int idCargo, string tipoAvaliacao, string escopo);
    Task<List<ResultadoSomaProjetosModel>> ObterSomaProjetosAsync(List<ResultadoProjetosModel> resultadosProjetos);
    string FormatPercentagem(object nota);
    string FormatDecimal(object nota);
    string TruncarTexto(string texto, int qtdCaracteres);
}
