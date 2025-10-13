using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SistemaAvaliacao.Services
{
    public interface IExportService
    {
        byte[] ExportToCsv<T>(IEnumerable<T> data, string[] columns, Func<T, object[]> rowSelector);
    }

    public class ExportService : IExportService
    {
        public byte[] ExportToCsv<T>(IEnumerable<T> data, string[] columns, Func<T, object[]> rowSelector)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", columns));
            foreach (var item in data)
            {
                var row = rowSelector(item);
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] != null)
                        row[i] = row[i].ToString().Replace(";", ",");
                }
                sb.AppendLine(string.Join(";", row));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}