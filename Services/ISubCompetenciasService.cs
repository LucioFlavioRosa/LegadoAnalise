using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface ISubCompetenciasService
{
    Task<List<SubCompetencia>> ObterListaSubCompetenciasAsync(bool ativo = true);
    Task<SubCompetencia?> ObterSubCompetenciaAsync(int id);
}