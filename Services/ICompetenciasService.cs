using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface ICompetenciasService
{
    Task<List<Competencia>> ObterListaCompetenciasAsync(bool? ativo = null);
    Task<Competencia?> ObterCompetenciaAsync(int id);
    Task<Competencia?> ObterCompetenciaAsync(int idEmpresa, int idCargo, int idNivel, int idEixo, int idSubCompetencia, int idDimensao, string detalhe);
    Task<bool> InserirCompetenciaAsync(Competencia competencia);
    Task<bool> AlterarCompetenciaAsync(Competencia competencia);
    Task<bool> ExcluirCompetenciaAsync(int id);
    Task<List<CompetenciaExportModel>> ObterCompetenciasParaExportAsync();
}