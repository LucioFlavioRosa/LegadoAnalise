using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Services
{
    public class ExportService
    {
        /// <summary>
        /// Exporta uma lista de cargos para CSV.
        /// </summary>
        /// <param name="cargos">Lista de cargos a exportar</param>
        /// <returns>Array de bytes representando o arquivo CSV</returns>
        public byte[] ExportCargosToCsv(IEnumerable<Cargo> cargos)
        {
            var sb = new StringBuilder();
            sb.AppendLine("IdCargo,NomeCargo,ProximoCargoId,TempoMinimoPromocao,Funcao,Autonomia,EscopoAtuacao,NivelInterlocucao,Status");
            foreach (var cargo in cargos)
            {
                sb.AppendLine($"{cargo.IdCargo},\"{EscapeCsv(cargo.NomeCargo)}\",{cargo.ProximoCargoId},{cargo.TempoMinimoPromocao},\"{EscapeCsv(cargo.Funcao)}\",\"{EscapeCsv(cargo.Autonomia)}\",\"{EscapeCsv(cargo.EscopoAtuacao)}\",\"{EscapeCsv(cargo.NivelInterlocucao)}\",{cargo.Status}");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Replace("\"", "\"\"");
        }
    }
}
