using System.Collections.Generic;
using SistemaAvaliacao.Models;

namespace SistemaAvaliacao.Services
{
    public interface IExportService
    {
        /// <summary>
        /// Exporta a lista de cargos para um arquivo no formato especificado.
        /// </summary>
        /// <param name="cargos">Lista de cargos a exportar.</param>
        /// <returns>Array de bytes representando o arquivo exportado.</returns>
        byte[] ExportCargos(IEnumerable<Cargo> cargos);

        /// <summary>
        /// Retorna o Content-Type apropriado para o arquivo exportado.
        /// </summary>
        /// <returns>Content-Type (MIME type).</returns>
        string GetContentType();

        /// <summary>
        /// Retorna a extensão padrão do arquivo exportado (ex: ".xlsx", ".csv").
        /// </summary>
        /// <returns>Extensão do arquivo.</returns>
        string GetFileExtension();
    }
}