using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Associados;

public interface IAssociadosService
{
    Task<Associado?> ObterAssociadoAsync(int id);
    Task<Associado?> ObterAssociadoPorEmailAsync(string email);
    Task<IEnumerable<Associado>> ObterMentoradosAsync(int mentorId);
    Task<IEnumerable<dynamic>> ObterProjetosGestorAsync(int gestorId);
    Task<bool> AlteraAssociadoSenhaAsync(int id, Associado associado);
    Task<IEnumerable<Associado>> ObterTodosAssociadosAsync();
    Task<Associado> CriarAssociadoAsync(Associado associado);
    Task<bool> AtualizarAssociadoAsync(Associado associado);
    Task<bool> ExcluirAssociadoAsync(int id);
}