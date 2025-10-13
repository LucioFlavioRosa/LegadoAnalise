using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SistemaAvaliacao.Services;

namespace SistemaAvaliacao.Services
{
    public class ExportService : IExportService
    {
        public byte[] Export<T>(IEnumerable<T> data, string format)
        {
            if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
            {
                return ExportToCsv(data);
            }
            // Futuramente: outros formatos (xlsx, pdf, etc.)
            throw new NotSupportedException($"Formato de exportação '{format}' não suportado.");
        }

        private byte[] ExportToCsv<T>(IEnumerable<T> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var sb = new StringBuilder();
            var props = typeof(T).GetProperties();
            // Cabeçalho
            sb.AppendLine(string.Join(",", props.Select(p => p.Name)));
            // Dados
            foreach (var item in data)
            {
                var values = props.Select(p => EscapeCsv(p.GetValue(item, null)?.ToString() ?? ""));
                sb.AppendLine(string.Join(",", values));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string EscapeCsv(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
