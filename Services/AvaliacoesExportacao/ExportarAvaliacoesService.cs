using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.AvaliacoesExportacao.Common;

namespace Services.AvaliacoesExportacao;

public interface IExportarAvaliacoesService
{
    Task<ExportarAvaliacoesResult> BuscarAvaliacoesAsync(
        int? periodoId,
        int? projetoId,
        int? associadoId,
        int empresaId
    );
}

public class ExportarAvaliacoesService : IExportarAvaliacoesService
{
    private readonly ApplicationDbContext _db;
    private readonly ExportarAvaliacoesHelper _helper;

    public ExportarAvaliacoesService(ApplicationDbContext db, ExportarAvaliacoesHelper helper)
    {
        _db = db;
        _helper = helper;
    }

    public async Task<ExportarAvaliacoesResult> BuscarAvaliacoesAsync(
        int? periodoId,
        int? projetoId,
        int? associadoId,
        int empresaId
    )
    {
        var avaliacoes = new List<AvaliacaoResumoModel>();

        var query = _db.AvaliacoesCompetencias
            .Include(a => a.PERIODOSAVALIACOES)
            .Include(a => a.PROJETOS)
            .Include(a => a.ASSOCIADOS)
                .ThenInclude(ass => ass.Cargo)
            .Include(a => a.PROJETOS.AssociadoGestor)
            .Include(a => a.COMPETENCIAS)
                .ThenInclude(c => c.Eixo)
            .Include(a => a.COMPETENCIAS)
                .ThenInclude(c => c.SubCompetencia)
            .AsQueryable();

        if (periodoId.HasValue && periodoId.Value > 0)
            query = query.Where(a => a.IdPeriodo == periodoId.Value);
        if (projetoId.HasValue && projetoId.Value > 0)
            query = query.Where(a => a.IdProjeto == projetoId.Value);
        if (associadoId.HasValue && associadoId.Value > 0)
            query = query.Where(a => a.IdAssociado == associadoId.Value);
        if (empresaId > 0)
            query = query.Where(a => a.PERIODOSAVALIACOES.IdEmpresa == empresaId);

        var lista = await query.ToListAsync();

        foreach (var item in lista)
        {
            avaliacoes.Add(new AvaliacaoResumoModel
            {
                Periodo = item.PERIODOSAVALIACOES?.Periodo ?? string.Empty,
                Projeto = item.PROJETOS?.Nome ?? string.Empty,
                NomeAvaliado = item.ASSOCIADOS?.Nome ?? string.Empty,
                Cargo = item.ASSOCIADOS?.Cargo?.Nome ?? string.Empty,
                Gestor = item.PROJETOS?.AssociadoGestor?.Nome ?? string.Empty,
                Tipo = item.TipoAvaliacao ?? string.Empty,
                CompetenciaOuPerformance = _helper.ObterCompetenciaOuPerformance(item)
            });
        }

        return new ExportarAvaliacoesResult
        {
            Avaliacoes = avaliacoes
        };
    }
}

public class ExportarAvaliacoesResult
{
    public List<AvaliacaoResumoModel> Avaliacoes { get; set; } = new();
}

public class AvaliacaoResumoModel
{
    public string Periodo { get; set; } = string.Empty;
    public string Projeto { get; set; } = string.Empty;
    public string NomeAvaliado { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Gestor { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string CompetenciaOuPerformance { get; set; } = string.Empty;
}
