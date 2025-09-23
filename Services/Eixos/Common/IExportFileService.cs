namespace Peers.Moderno.Services.Eixos.Common;

public interface IExportFileService
{
    Task<byte[]> GenerateExcelAsync<T>(string fileName, IEnumerable<T> data);
    Task<byte[]> GenerateExcelWithSheetsAsync(string fileName, Dictionary<string, object> sheets);
    string GetContentType();
}