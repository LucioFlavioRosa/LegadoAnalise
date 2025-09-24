using Peers.Moderno.Models;

namespace Peers.Moderno.Services.FrentesInternas;

public interface IFrentesInternasService
{
    Task<List<FrenteInternaModel>> ObterFrentesInternasAsync();
    Task<FrenteInternaModel?> ObterFrenteInternaAsync(int id);
    Task<bool> GerirFrenteInternaAsync(FrenteInternaModel frenteInterna);
    Task<bool> ExcluirFrenteInternaAsync(int id);
    
    Task<List<LiderFrenteInternaModel>> ObterLideresFrenteInternaAsync(int idFrenteInterna);
    Task<bool> GerirLiderFrenteInternaAsync(LiderFrenteInternaModel lider);
    Task<bool> ExcluirLiderFrenteInternaAsync(int id);
    
    Task<List<ParticipanteFrenteInternaModel>> ObterParticipantesFrenteInternaAsync(int idFrenteInterna);
    Task<bool> GerirParticipanteFrenteInternaAsync(ParticipanteFrenteInternaModel participante);
    Task<bool> ExcluirParticipanteFrenteInternaAsync(int id);
    
    Task<List<AlocacaoExportModel>> ObterAvaliacoesAlocacaoAsync();
    Task<List<int>> ObterFrentesLideradasPorAssociadoAsync(int idAssociado);
    Task<bool> ValidarPermissaoEdicaoAsync(int idAssociado, int? idFrenteInterna = null);
}