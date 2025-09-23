using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Dashboard.Common;

namespace Peers.Moderno.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly IChartJsonUtil _chartJsonUtil;
    private readonly IPeriodoUtil _periodoUtil;

    public DashboardService(
        ApplicationDbContext context,
        IChartJsonUtil chartJsonUtil,
        IPeriodoUtil periodoUtil)
    {
        _context = context;
        _chartJsonUtil = chartJsonUtil;
        _periodoUtil = periodoUtil;
    }

    public async Task<DashboardModel> ObterNumerosAsync(int idPeriodo)
    {
        var associados = await _context.Associados
            .Where(a => a.Ativo)
            .CountAsync();

        var mentores = await _context.Associados
            .Where(a => a.Ativo && a.Mentorados.Any())
            .CountAsync();

        var gestores = await _context.Associados
            .Where(a => a.Ativo && a.IdPerfil >= 3)
            .CountAsync();

        var avaliacoes = await _context.AvaliacoesCompetenciasNotas
            .Where(a => a.IdPeriodo == idPeriodo)
            .Select(a => a.IdAvaliado)
            .Distinct()
            .CountAsync();

        var avaliadores = await _context.AvaliacoesCompetenciasNotas
            .Where(a => a.IdPeriodo == idPeriodo)
            .Select(a => a.IdAvaliador)
            .Distinct()
            .CountAsync();

        return new DashboardModel
        {
            QtdAssociados = associados,
            QtdMentores = mentores,
            QtdGestores = gestores,
            QtdAvaliacoes = avaliacoes,
            QtdAvaliadores = avaliadores
        };
    }

    public async Task<DashboardModel> ObterAndamentoAvaliacoesAsync(int idPeriodo)
    {
        var andamento = await _context.AvaliacoesCompetenciasNotas
            .Where(a => a.IdPeriodo == idPeriodo)
            .GroupBy(a => a.Status)
            .Select(g => new DashboardItemModel
            {
                Status = g.Key,
                QtdStatus = g.Count()
            })
            .ToListAsync();

        return new DashboardModel
        {
            ListAndamento = andamento
        };
    }

    public async Task<List<DashboardModel>> ObterAvaliadosPorPeriodoAsync()
    {
        var periodos = await _context.Set<Periodo>()
            .OrderByDescending(p => p.DataInicio)
            .Take(12)
            .ToListAsync();

        var resultado = new List<DashboardModel>();

        foreach (var periodo in periodos)
        {
            var qtdAvaliados = await _context.AvaliacoesCompetenciasNotas
                .Where(a => a.IdPeriodo == periodo.IdPeriodo)
                .Select(a => a.IdAvaliado)
                .Distinct()
                .CountAsync();

            resultado.Add(new DashboardModel
            {
                Periodo = _periodoUtil.FormatarPeriodo(periodo),
                QtdAvaliacoes = qtdAvaliados
            });
        }

        return resultado;
    }

    public async Task<List<DashboardModel>> ObterAvaliadosPorProjetoAsync(int idPeriodo)
    {
        var projetos = await _context.AvaliacoesCompetenciasNotas
            .Where(a => a.IdPeriodo == idPeriodo)
            .Join(_context.Set<Projeto>(),
                avaliacao => avaliacao.IdProjeto,
                projeto => projeto.IdProjeto,
                (avaliacao, projeto) => new { avaliacao, projeto })
            .GroupBy(x => x.projeto.Nome)
            .Select(g => new DashboardItemModel
            {
                Status = g.Key,
                QtdStatus = g.Select(x => x.avaliacao.IdAvaliado).Distinct().Count()
            })
            .OrderByDescending(x => x.QtdStatus)
            .ToListAsync();

        return new List<DashboardModel>
        {
            new DashboardModel { ListAndamento = projetos }
        };
    }

    public async Task<ResultadoProjetosModel> ObterRadarConsolidadoCompetenciaAsync(int idPeriodo)
    {
        var competencias = await _context.AvaliacoesCompetenciasNotas
            .Where(a => a.IdPeriodo == idPeriodo)
            .Join(_context.Competencias,
                avaliacao => avaliacao.IdCompetencia,
                competencia => competencia.IdCompetencia,
                (avaliacao, competencia) => new { avaliacao, competencia })
            .Join(_context.Eixos,
                x => x.competencia.IdEixo,
                eixo => eixo.IdEixo,
                (x, eixo) => new { x.avaliacao, x.competencia, eixo })
            .GroupBy(x => x.eixo.Nome)
            .Select(g => new SomaCompetenciasModel
            {
                Eixo = g.Key,
                PercentualSomaNotaFinalN1N2 = g.Average(x => x.avaliacao.NotaFinal) ?? 0
            })
            .ToListAsync();

        return new ResultadoProjetosModel
        {
            ListSomaCompetenciasN1N2 = competencias
        };
    }

    public async Task<List<EvolucaoPerformanceModel>> ObterChartConsolidadoPerformanceAsync(int idPeriodo)
    {
        var performance = await _context.AvaliacoesCompetenciasNotas
            .Where(a => a.IdPeriodo == idPeriodo)
            .GroupBy(a => a.TipoAvaliacao)
            .Select(g => new EvolucaoPerformanceModel
            {
                Performance = g.Key,
                Nota = g.Average(x => x.NotaFinal)
            })
            .ToListAsync();

        return performance;
    }

    public async Task<List<Periodo>> ObterPeriodosAsync(int idEmpresa)
    {
        return await _context.Set<Periodo>()
            .Where(p => p.IdEmpresa == idEmpresa)
            .OrderByDescending(p => p.DataInicio)
            .ToListAsync();
    }

    public async Task<Periodo> ObterPeriodoAtualAsync()
    {
        var hoje = DateTime.Now;
        return await _context.Set<Periodo>()
            .Where(p => p.DataInicio <= hoje && p.DataFim >= hoje)
            .FirstOrDefaultAsync() ?? await _context.Set<Periodo>()
            .OrderByDescending(p => p.DataInicio)
            .FirstAsync();
    }

    public async Task<Periodo> ObterPeriodoAsync(int idPeriodo)
    {
        return await _context.Set<Periodo>()
            .FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo) ?? throw new InvalidOperationException($"Período {idPeriodo} não encontrado");
    }
}