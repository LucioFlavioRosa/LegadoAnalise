using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface ICargosService
{
    Task<List<Cargo>> ObterListaCargosAsync(bool ativo = true);
    Task<Cargo?> ObterCargoAsync(int id);
    Task<RelacaoCargoSubcompetencia?> ObterRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubCompetencia);
    Task AtualizarRelacaoCargoSubcompetenciaAsync(RelacaoCargoSubcompetencia relacao);
}