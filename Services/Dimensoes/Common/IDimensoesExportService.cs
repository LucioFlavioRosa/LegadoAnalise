using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Dimensoes.Common;

public interface IDimensoesExportService
{
    Task<byte[]> ExportarDimensoesAsync();
    Task<byte[]> ExportarDimensoesAsync(List<Dimensao> dimensoes);
    Task<byte[]> ExportarDimensoesAsync(List<DimensaoExportDto> dimensoesDto);
}