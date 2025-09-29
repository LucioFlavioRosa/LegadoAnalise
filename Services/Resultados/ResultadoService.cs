using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Resultados;

public class ResultadoService : IResultadoService
{
    private readonly ApplicationDbContext _db;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IExportFileService _exportFileService;

    public ResultadoService(
        ApplicationDbContext db,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService,
        IExportFileService exportFileService)
    {
        _db = db;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
        _exportFileService = exportFileService;
    }

    public async Task<List<ResultadoProjetosModel>> ListarResultadosAsync(int? idProjeto, int? idAssociado, int? idPeriodo, string? tipoAvaliacao = null)
    {
        try
        {
            var query = _db.ResultadoProjetos.AsQueryable();
            if (idProjeto.HasValue && idProjeto.Value > 0)
                query = query.Where(r => r.IdProjeto == idProjeto.Value);
            if (idAssociado.HasValue && idAssociado.Value > 0)
                query = query.Where(r => r.IdAssociado == idAssociado.Value);
            if (idPeriodo.HasValue && idPeriodo.Value > 0)
                query = query.Where(r => r.IdPeriodo == idPeriodo.Value);
            if (!string.IsNullOrEmpty(tipoAvaliacao))
                query = query.Where(r => r.TipoAvaliacao == tipoAvaliacao);
            var result = await query.OrderByDescending(r => r.IdPeriodo).ToListAsync();
            _telemetryService.TrackEvent("ListarResultados", new Dictionary<string, string> {
                { "IdProjeto", idProjeto?.ToString() ?? "" },
                { "IdAssociado", idAssociado?.ToString() ?? "" },
                { "IdPeriodo", idPeriodo?.ToString() ?? "" },
                { "TipoAvaliacao", tipoAvaliacao ?? "" },
                { "Count", result.Count.ToString() }
            });
            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Method", "ListarResultadosAsync" }
            });
            _messageBoxService.ShowError("Erro ao listar resultados.");
            return new List<ResultadoProjetosModel>();
        }
    }

    public async Task<byte[]> ExportarResultadosLiderancaAsync(int? idPeriodo)
    {
        try
        {
            var periodos = _db.PeriodosAvaliacoes.AsQueryable();
            if (idPeriodo.HasValue && idPeriodo.Value > 0)
                periodos = periodos.Where(p => p.IdPeriodo == idPeriodo.Value);
            var periodosList = await periodos.ToListAsync();
            var avaliacoes = await _db.AvaliacoesCompetencias.Where(a => a.TipoAvaliacao == "lideranca").ToListAsync();
            var associados = await _db.Associados.ToListAsync();
            var avaliacoesCompetencias = await _db.AvaliacoesCompetenciasNotas.ToListAsync();
            // Simulação de exportação (deve ser adaptado ao serviço real de exportação)
            var bytes = await _exportFileService.GerarExcelResultadoLiderancaAsync(periodosList, avaliacoes, associados, avaliacoesCompetencias);
            _telemetryService.TrackEvent("ExportarResultadosLideranca", new Dictionary<string, string> {
                { "IdPeriodo", idPeriodo?.ToString() ?? "" },
                { "Size", bytes.Length.ToString() }
            });
            return bytes;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Method", "ExportarResultadosLiderancaAsync" }
            });
            _messageBoxService.ShowError("Erro ao exportar resultados de liderança.");
            return Array.Empty<byte>();
        }
    }

    public async Task<byte[]> ExportarResultadosDesempenhoAsync(int idPeriodo)
    {
        try
        {
            var resultado = await _db.ResultadoProjetos.Where(r => r.IdPeriodo == idPeriodo && r.TipoAvaliacao == "desempenho").ToListAsync();
            var bytes = await _exportFileService.GerarExcelResultadoDesempenhoAsync(resultado);
            _telemetryService.TrackEvent("ExportarResultadosDesempenho", new Dictionary<string, string> {
                { "IdPeriodo", idPeriodo.ToString() },
                { "Size", bytes.Length.ToString() }
            });
            return bytes;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Method", "ExportarResultadosDesempenhoAsync" }
            });
            _messageBoxService.ShowError("Erro ao exportar resultados de desempenho.");
            return Array.Empty<byte>();
        }
    }

    public async Task<bool> LiberarLiderancaAsync(int idPeriodo)
    {
        try
        {
            var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
            if (periodo == null)
                return false;
            periodo.fl_lib_res_lideranca = true;
            _db.PeriodosAvaliacoes.Update(periodo);
            await _db.SaveChangesAsync();
            _telemetryService.TrackEvent("LiberarLideranca", new Dictionary<string, string> {
                { "IdPeriodo", idPeriodo.ToString() }
            });
            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Method", "LiberarLiderancaAsync" }
            });
            _messageBoxService.ShowError("Erro ao liberar liderança.");
            return false;
        }
    }

    public async Task<bool> LiberarMentoriaAsync(int idPeriodo)
    {
        try
        {
            var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
            if (periodo == null)
                return false;
            periodo.fl_lib_res_mentoria = true;
            _db.PeriodosAvaliacoes.Update(periodo);
            await _db.SaveChangesAsync();
            _telemetryService.TrackEvent("LiberarMentoria", new Dictionary<string, string> {
                { "IdPeriodo", idPeriodo.ToString() }
            });
            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Method", "LiberarMentoriaAsync" }
            });
            _messageBoxService.ShowError("Erro ao liberar mentoria.");
            return false;
        }
    }
}