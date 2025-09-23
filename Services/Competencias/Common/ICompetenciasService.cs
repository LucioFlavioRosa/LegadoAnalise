using Peers.Moderno.Models;
using Peers.Moderno.Services.Competencias.Common.DTOs;

namespace Peers.Moderno.Services.Competencias.Common;

public interface ICompetenciasService
{
    Task<List<Competencia>> ListarAsync(bool? ativo = null);
    Task<Competencia?> ObterPorIdAsync(int id);
    Task<Competencia?> ObterCompetenciaAsync(int idEmpresa, int idCargo, int idNivel, int idEixo, int idSubCompetencia, int idDimensao, string detalhamento);
    Task<bool> CadastrarAsync(CompetenciaDto competenciaDto);
    Task<bool> AlterarAsync(int id, CompetenciaDto competenciaDto);
    Task<bool> ExcluirAsync(int id);
    Task<List<CompetenciaExportDto>> ExportarAsync();
    Task<ImportResultDto> ImportarAsync(Stream fileStream);
    Task<int> AgregarCompetenciaAsync(int idCompetencia);
}