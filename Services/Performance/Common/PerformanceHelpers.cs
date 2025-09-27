using System;
using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;

namespace Services.Performance.Common
{
    public static class PerformanceHelpers
    {
        public static string TruncateText(string texto, int maxLength)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;
            return texto.Length > maxLength ? texto.Substring(0, maxLength) + "..." : texto;
        }

        public static List<PerformanceModel> OrganizeByAbrangencia(List<PerformanceModel> performances)
        {
            var result = new List<PerformanceModel>();
            var abrangenciasContadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var perf in performances)
            {
                if (!abrangenciasContadas.Contains(perf.Abrangencia))
                {
                    abrangenciasContadas.Add(perf.Abrangencia);
                    perf.SeparadorAbrangencia = "";
                }
                else
                {
                    perf.SeparadorAbrangencia = "hidden";
                }
                result.Add(perf);
            }
            return result;
        }
    }
}
