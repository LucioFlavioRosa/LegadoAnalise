using System.Threading.Tasks;
using System.Collections.Generic;
using Peers.Moderno.Models;
using Microsoft.AspNetCore.Http;

namespace Services.Resultados;

public interface IResultadoService
{
    Task<List<ResultadoProjetosModel>> ListarResultadosAsync(int? idProjeto, int? idAssociado, int? idPeriodo, string? tipoAvaliacao = null);
    Task<byte[]> ExportarResultadosLiderancaAsync(int? idPeriodo);
    Task<byte[]> ExportarResultadosDesempenhoAsync(int idPeriodo);
    Task<bool> LiberarLiderancaAsync(int idPeriodo);
    Task<bool> LiberarMentoriaAsync(int idPeriodo);
}