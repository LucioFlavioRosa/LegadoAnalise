using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public interface IExportFileService
{
    byte[] GenerateExcelCompetencias(string fileName, List<CompetenciaExportModel> competencias);
}