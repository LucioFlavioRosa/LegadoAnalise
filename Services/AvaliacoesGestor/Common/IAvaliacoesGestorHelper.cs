using Peers.Moderno.Models;
using Peers.Moderno.Data;
using System.Threading.Tasks;

namespace Services.AvaliacoesGestor.Common;

public interface IAvaliacoesGestorHelper
{
    bool DeveIncluirAssociadoNoFluxo(AssociadoProjeto ap, int? idPeriodo);
    Task<ProjetosAssociadosModel?> MapearProjetosAssociadosModelAsync(AssociadoProjeto ap, int? idPeriodo, ApplicationDbContext db);
    string ObterEtapaAvaliacao(string? posicaoFluxo);
    string ObterStatusAvaliacao(object? status);
    string ObterRotuloBotao(AssociadoProjeto ap, string etapa);
    bool PodeExibirBotaoFinalizar(AssociadoProjeto ap, string etapa);
    Task<bool> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId, ApplicationDbContext db);
}
