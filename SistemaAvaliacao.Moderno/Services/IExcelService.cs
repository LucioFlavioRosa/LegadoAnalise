namespace SistemaAvaliacao.Moderno.Services;

public interface IExcelService
{
    Task<byte[]> ExportarAssociadosAsync();
    Task<byte[]> ExportarPromocoesAsync();
    Task<(int inseridos, int alterados, int desconsiderados)> ImportarAssociadosAsync(Stream fileStream);
    Task<(int inseridos, int alterados, int desconsiderados)> ImportarPromocoesAsync(Stream fileStream);
}