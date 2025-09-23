using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Peers.Moderno.Data;
using Peers.Moderno.Services.Avaliacoes.Common;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Avaliacoes;

public class EvolucaoAssociadoService : IEvolucaoAssociadoService
{
    private readonly ApplicationDbContext _context;
    private readonly IAssociadosService _associadosService;
    private readonly ITelemetryService _telemetryService;

    public EvolucaoAssociadoService(
        ApplicationDbContext context,
        IAssociadosService associadosService,
        ITelemetryService telemetryService)
    {
        _context = context;
        _associadosService = associadosService;
        _telemetryService = telemetryService;
    }

    public async Task<EvolucaoAssociadoViewModel> ObterEvolucaoAssociadoAsync(int associadoId, string tipoAvaliacao, string escopo)
    {
        try
        {
            var associadoInfo = await ObterInfoAssociadoAsync(associadoId);
            var projetos = await ObterProjetosEvolucaoAsync(associadoId, tipoAvaliacao, escopo);
            var jsonRadar = await MontarJsonRadarAsync(projetos, associadoId);

            return new EvolucaoAssociadoViewModel
            {
                AssociadoInfo = associadoInfo,
                Projetos = projetos,
                JsonRadar = jsonRadar,
                HasData = projetos.Any()
            };
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "AssociadoId", associadoId.ToString() },
                { "TipoAvaliacao", tipoAvaliacao },
                { "Escopo", escopo }
            });

            return new EvolucaoAssociadoViewModel
            {
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<AssociadoInfoViewModel> ObterInfoAssociadoAsync(int associadoId)
    {
        var associado = await _context.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Mentor)
            .FirstOrDefaultAsync(a => a.Id == associadoId);

        if (associado == null)
        {
            return new AssociadoInfoViewModel();
        }

        var proximoCargo = associado.Cargo?.ProximoCargo;

        return new AssociadoInfoViewModel
        {
            Nome = associado.Nome,
            Mentor = associado.Mentor?.Nome ?? string.Empty,
            Cargo = associado.Cargo?.Nome ?? string.Empty,
            ProximoCargo = proximoCargo?.Nome ?? string.Empty
        };
    }

    public async Task<string> MontarJsonRadarAsync(List<EvolucaoAssociadoProjeto> projetos, int associadoId)
    {
        try
        {
            var associado = await _context.Associados.FindAsync(associadoId);
            var idCargo = associado?.IdCargo ?? 0;

            dynamic obj = new JObject();
            List<string> labels = new List<string>();
            List<JObject> datasets = new List<JObject>();

            int count = 1;

            foreach (var projeto in projetos.OrderByDescending(x => x.IdPeriodo))
            {
                dynamic itemProjeto = new JObject();
                itemProjeto.periodo = projeto.Periodo;
                List<decimal> dataset = new List<decimal>();

                foreach (var competencia in projeto.Competencias)
                {
                    if (count == 1)
                    {
                        labels.Add(competencia.Eixo);
                    }

                    dataset.Add(competencia.Nota.HasValue && competencia.Nota.Value > 0 ? competencia.Nota.Value : 0);
                }

                itemProjeto.dataset = new JArray(dataset);
                datasets.Add(itemProjeto);

                count++;
            }

            obj.labels = new JArray(labels);
            obj.datasets = new JArray(datasets);

            var premissaRadar = await _context.PremissasRadar
                .FirstOrDefaultAsync(p => p.IdCargo == idCargo);

            obj.stepsize = 200;

            if (premissaRadar != null && premissaRadar.ValorRadarPeers > 1)
            {
                decimal step = 200m / (premissaRadar.ValorRadarPeers + 1);
                obj.stepsize = Math.Round(step);
            }

            return JsonConvert.SerializeObject(obj);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return "{}";
        }
    }

    private async Task<List<EvolucaoAssociadoProjeto>> ObterProjetosEvolucaoAsync(int associadoId, string tipoAvaliacao, string escopo)
    {
        var projetos = new List<EvolucaoAssociadoProjeto>();

        try
        {
            var query = _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT 
                        e.IdPeriodo,
                        e.Cargo,
                        p.Periodo,
                        e.NotaCompetencia,
                        e.NotaPerfomance as NotaPerformance,
                        e.RatingPerformance,
                        e.TipoAvaliacao
                    FROM EVOLUCAOASSOCIADO e
                    INNER JOIN PERIODOSAVALIACOES p ON e.IdPeriodo = p.IdPeriodo
                    WHERE e.IdAssociado = {0} 
                    AND e.TipoAvaliacao = {1} 
                    AND e.Escopo = {2}
                    ORDER BY e.IdPeriodo DESC
                ", associadoId, tipoAvaliacao, escopo);

            await foreach (var item in query.AsAsyncEnumerable())
            {
                var projeto = new EvolucaoAssociadoProjeto
                {
                    IdPeriodo = item.IdPeriodo,
                    Cargo = item.Cargo ?? string.Empty,
                    Periodo = item.Periodo ?? string.Empty,
                    NotaCompetencia = item.NotaCompetencia ?? 0,
                    NotaPerformance = item.NotaPerformance,
                    RatingPerformance = item.RatingPerformance ?? string.Empty,
                    TipoAvaliacao = item.TipoAvaliacao ?? string.Empty,
                    ExibirPerformance = item.TipoAvaliacao != "lideranca"
                };

                projeto.Competencias = await ObterCompetenciasPorPeriodoAsync(associadoId, projeto.IdPeriodo, tipoAvaliacao, escopo);
                projeto.Performances = await ObterPerformancesPorPeriodoAsync(associadoId, projeto.IdPeriodo, tipoAvaliacao, escopo);

                projetos.Add(projeto);
            }
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
        }

        return projetos;
    }

    private async Task<List<EvolucaoCompetencia>> ObterCompetenciasPorPeriodoAsync(int associadoId, int idPeriodo, string tipoAvaliacao, string escopo)
    {
        var competencias = new List<EvolucaoCompetencia>();

        try
        {
            var query = _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT 
                        e.Eixo,
                        ec.NotaNeutra,
                        ec.Nota
                    FROM EVOLUCAOCOMPETENCIAS ec
                    INNER JOIN EIXOS e ON ec.IdEixo = e.IdEixo
                    WHERE ec.IdAssociado = {0} 
                    AND ec.IdPeriodo = {1}
                    AND ec.TipoAvaliacao = {2}
                    AND ec.Escopo = {3}
                    ORDER BY ec.IdEixo
                ", associadoId, idPeriodo, tipoAvaliacao, escopo);

            await foreach (var item in query.AsAsyncEnumerable())
            {
                competencias.Add(new EvolucaoCompetencia
                {
                    Eixo = item.Eixo ?? string.Empty,
                    NotaNeutra = item.NotaNeutra ?? 0,
                    Nota = item.Nota
                });
            }
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
        }

        return competencias;
    }

    private async Task<List<EvolucaoPerformance>> ObterPerformancesPorPeriodoAsync(int associadoId, int idPeriodo, string tipoAvaliacao, string escopo)
    {
        var performances = new List<EvolucaoPerformance>();

        try
        {
            var query = _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT 
                        p.Performance,
                        ep.Nota
                    FROM EVOLUCAOPERFORMANCE ep
                    INNER JOIN PERFORMANCES p ON ep.IdPerformance = p.IdPerformance
                    WHERE ep.IdAssociado = {0} 
                    AND ep.IdPeriodo = {1}
                    AND ep.TipoAvaliacao = {2}
                    AND ep.Escopo = {3}
                    ORDER BY ep.IdPerformance
                ", associadoId, idPeriodo, tipoAvaliacao, escopo);

            await foreach (var item in query.AsAsyncEnumerable())
            {
                performances.Add(new EvolucaoPerformance
                {
                    Performance = item.Performance ?? string.Empty,
                    Nota = item.Nota ?? 0
                });
            }
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
        }

        return performances;
    }
}