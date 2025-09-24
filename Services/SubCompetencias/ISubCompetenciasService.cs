using Peers.Moderno.Models;

namespace Peers.Moderno.Services.SubCompetencias;

public interface ISubCompetenciasService
{
    Task<List<SubCompetencia>> ListarSubCompetenciasAsync();
    Task<List<SubCompetencia>> ListarSubCompetenciasAtivasAsync();
    Task<SubCompetencia?> ObterSubCompetenciaAsync(int id);
    Task<bool> InserirSubCompetenciaAsync(SubCompetencia subCompetencia);
    Task<bool> AlterarSubCompetenciaAsync(SubCompetencia subCompetencia);
    Task<bool> ExcluirSubCompetenciaAsync(int id);
    Task<bool> InativarSubCompetenciaAsync(int id);
    Task<bool> ExisteSubCompetenciaAsync(string nome, int? idExcluir = null);
    Task<int> ContarSubCompetenciasAtivasAsync();
    Task<List<SubCompetencia>> BuscarSubCompetenciasPorNomeAsync(string nome);
}