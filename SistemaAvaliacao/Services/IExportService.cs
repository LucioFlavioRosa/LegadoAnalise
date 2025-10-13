using System.Collections.Generic;

namespace SistemaAvaliacao.Services
{
    public interface IExportService
    {
        /// <summary>
        /// Exporta uma lista de objetos para um formato específico.
        /// </summary>
        /// <typeparam name="T">Tipo dos dados a exportar.</typeparam>
        /// <param name="data">Lista de dados.</param>
        /// <param name="format">Formato de exportação (ex: "csv", "xlsx").</param>
        /// <returns>Array de bytes do arquivo exportado.</returns>
        byte[] Export<T>(IEnumerable<T> data, string format);
    }
}
