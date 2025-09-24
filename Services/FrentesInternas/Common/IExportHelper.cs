using Peers.Moderno.Models;

namespace Peers.Moderno.Services.FrentesInternas.Common;

public interface IExportHelper
{
    Task<byte[]> ExportarAvaliacoesAlocacaoAsync(List<AlocacaoExportModel> avaliacoes, string fileName);
    Task<byte[]> ExportarFrentesInternasAsync(List<FrenteInternaModel> frentes, string fileName);
    string GerarNomeArquivo(string prefixo, string extensao = ".xlsx");
    Task<bool> ValidarDadosExportacaoAsync<T>(List<T> dados) where T : class;
}