using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.FrentesInternas;

public interface IFrentesInternasService
{
    Task<List<PeriodoAvaliacaoDto>> ListaTodosPeriodosAsync(int empresaId);
    Task<PeriodoAvaliacaoDto?> ObterPeriodoUltimoAsync();
    Task<Associado?> ObterAssociadoAsync(int idAssociado);
    Task<List<FrenteInternaDto>> ObterLiderFrenteInternaAsync(int idAssociado);
    Task<List<FrenteInternaDto>> ObterFrenteInternaAsync();
    Task<List<Associado>> ObterParticipantesFrentesInternasAsync(int idFrenteInterna);
    Task<List<AvaliacaoAlocacaoInternaDto>> ObterAvaliacoesAlocacoesInternasAsync(int? idAvaliador = null, int? idPeriodo = null, int? idAlocacaoInterna = null, int? idAssociado = null, int? idAvaliacaoAlocacaoInterna = null);
    Task<List<NotaAlocacaoInternaDto>> ObterNotasAlocacoesInternasAsync(int? idNotaAlocacaoInterna = null);
    Task GerirAvaliacaoAlocacaoInternaAsync(AvaliacaoAlocacaoInternaDto avaliacao);
}
