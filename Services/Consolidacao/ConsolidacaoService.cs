using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Consolidacao.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Consolidacao;

public class ConsolidacaoService : IConsolidacaoService
{
    private readonly ApplicationDbContext _db;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;

    public ConsolidacaoService(ApplicationDbContext db, ITelemetryService telemetryService, IMessageBoxService messageBoxService)
    {
        _db = db;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
    }

    public async Task<List<ResultadoCompetenciaModel>> ObterCompetenciasConsolidacaoAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        // Exemplo de consulta, adaptar conforme modelo real
        return await _db.AvaliacoesCompetenciasNotas
            .Where(x => x.IdAssociado == idAssociado && x.IdProjeto == idProjeto && x.IdPeriodo == idPeriodo)
            .Select(x => new ResultadoCompetenciaModel
            {
                IdAvaliacaoCompetencia = x.IdNota,
                Eixo = x.Eixo,
                NotaNivel1AutoAvaliacao = x.NotaNivel1AutoAvaliacao,
                NotaNivel1Feedback = x.NotaNivel1Feedback,
                NotaNivel2AutoAvaliacao = x.NotaNivel2AutoAvaliacao,
                NotaNivel2Feedback = x.NotaNivel2Feedback,
                NotaSubcompetenciaAvaliadoN1 = x.NotaSubcompetenciaAvaliadoN1,
                NotaSubcompetenciaGestorN1 = x.NotaSubcompetenciaGestorN1,
                NotaSubcompetenciaAvaliadoN2 = x.NotaSubcompetenciaAvaliadoN2,
                NotaSubcompetenciaGestorN2 = x.NotaSubcompetenciaGestorN2,
                NotaCompetenciaAvaliado = x.NotaCompetenciaAvaliado,
                NotaCompetenciaGestor = x.NotaCompetenciaGestor,
                NotaFinalNivel1 = x.NotaFinalNivel1,
                NotaFinalNivel2 = x.NotaFinalNivel2,
                IdNotaNivel1Comite = x.IdNotaNivel1Comite,
                IdNotaNivel2Comite = x.IdNotaNivel2Comite,
                IdNotaNivel1Feedback = x.IdNotaNivel1Feedback,
                IdNotaNivel2Feedback = x.IdNotaNivel2Feedback,
                enableNivel1 = x.EnableNivel1,
                enableNivel2 = x.EnableNivel2,
                DetalheNivelAtual = x.DetalheNivelAtual,
                CompetenciaAtual = x.CompetenciaAtual,
                DetalheProximoNivel = x.DetalheProximoNivel,
                CompetenciaProximo = x.CompetenciaProximo,
                ComentarioAvaliado = x.ComentarioAvaliado,
                ComentarioFeedback = x.ComentarioFeedback
            })
            .ToListAsync();
    }

    public async Task<List<ResultadoPerfomanceModel>> ObterPerformanceConsolidacaoAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        return await _db.Performances
            .Where(x => x.IdAssociado == idAssociado && x.IdProjeto == idProjeto && x.IdPeriodo == idPeriodo)
            .Select(x => new ResultadoPerfomanceModel
            {
                IdAvaliacaoPerformance = x.IdPerformance,
                Perfomance = x.PerformanceNome,
                NotaNivel1AutoAvaliacao = x.NotaNivel1AutoAvaliacao,
                NotaNivel1Feedback = x.NotaNivel1Feedback,
                IdNotaComite = x.IdNotaComite,
                IdNotaNivel1Feedback = x.IdNotaNivel1Feedback,
                NotaPerfomancePonderada = x.NotaPerfomancePonderada,
                NotaPerfomance = x.NotaPerfomance,
                ComentariosAutoAvaliacao = x.ComentariosAutoAvaliacao,
                ComentarioFeedback = x.ComentarioFeedback
            })
            .ToListAsync();
    }

    public async Task<string> CalcularNotaCompetenciaComiteAsync(int idAvaliacaoCompetencia, int nivel, int nota)
    {
        try
        {
            // Exemplo de cálculo, adaptar conforme regra de negócio
            var competencia = await _db.AvaliacoesCompetenciasNotas.FindAsync(idAvaliacaoCompetencia);
            if (competencia == null)
                return JsonSerializer.Serialize(new { erro = "Competência não encontrada" });
            decimal? notaComite = (nota + nivel) / 2m;
            return JsonSerializer.Serialize(new { nota = notaComite?.ToString("F2") });
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return JsonSerializer.Serialize(new { erro = "Erro ao tentar Salvar a Nota Competência Comitê: " + ex.Message });
        }
    }

    public async Task<string> CalcularNotaPerformanceComiteAsync(int idAvaliacaoPerformance, int nota)
    {
        try
        {
            var performance = await _db.Performances.FindAsync(idAvaliacaoPerformance);
            if (performance == null)
                return JsonSerializer.Serialize(new { erro = "Performance não encontrada" });
            decimal? notaComite = (nota + 1) / 2m;
            return JsonSerializer.Serialize(new { nota = notaComite?.ToString("F2") });
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return JsonSerializer.Serialize(new { erro = "Erro ao tentar Salvar a Nota Performance Comitê: " + ex.Message });
        }
    }

    public async Task<ConsideracoesMentorDto> CarregarConsideracoesMentorAsync(int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo)
    {
        var consideracao = await _db.Set<ConsideracoesMentor>()
            .FirstOrDefaultAsync(x => x.IdAssociado == idAssociado && x.IdPeriodo == idPeriodo && x.TipoAvaliacao == tipoAvaliacao && x.Escopo == escopo);
        if (consideracao == null)
        {
            // Inicializa com valores padrão
            return new ConsideracoesMentorDto
            {
                IdAssociado = idAssociado,
                IdPeriodo = idPeriodo,
                TipoAvaliacao = tipoAvaliacao,
                Escopo = escopo,
                LiberadoRH = false,
                AcaoComite = "-",
                PontosFortesRH = "-",
                PontosFracosRH = "-",
                SalarioAtual = 1,
                SalarioNovo = 1,
                RegimeContratacaoAtual = "-",
                RegimeContratacaoNovo = "-",
                MentoriaRealizada = false
            };
        }
        return new ConsideracoesMentorDto
        {
            IdConsideracoesMentor = consideracao.IdConsideracoesMentor,
            IdMentor = consideracao.IdMentor,
            IdAssociado = consideracao.IdAssociado,
            IdPeriodo = consideracao.IdPeriodo,
            TipoAvaliacao = consideracao.TipoAvaliacao,
            Escopo = consideracao.Escopo,
            LiberadoRH = consideracao.LiberadoRH,
            AcaoComite = consideracao.AcaoComite,
            PontosFortesRH = consideracao.PontosFortesRH,
            PontosFracosRH = consideracao.PontosFracosRH,
            SalarioAtual = consideracao.SalarioAtual,
            SalarioNovo = consideracao.SalarioNovo,
            RegimeContratacaoAtual = consideracao.RegimeContratacaoAtual,
            RegimeContratacaoNovo = consideracao.RegimeContratacaoNovo,
            MentoriaRealizada = consideracao.MentoriaRealizada,
            ProximoCargo = consideracao.ProximoCargo,
            LabelIncremento = consideracao.LabelIncremento,
            PontosFortes = consideracao.PontosFortes,
            PontosFracos = consideracao.PontosFracos
        };
    }

    public async Task<bool> SalvarConsideracoesMentorAsync(ConsideracoesMentorDto consideracoesMentorDto)
    {
        try
        {
            var entity = await _db.Set<ConsideracoesMentor>().FindAsync(consideracoesMentorDto.IdConsideracoesMentor);
            if (entity == null)
            {
                entity = new ConsideracoesMentor
                {
                    IdMentor = consideracoesMentorDto.IdMentor,
                    IdAssociado = consideracoesMentorDto.IdAssociado,
                    IdPeriodo = consideracoesMentorDto.IdPeriodo,
                    TipoAvaliacao = consideracoesMentorDto.TipoAvaliacao,
                    Escopo = consideracoesMentorDto.Escopo
                };
                _db.Set<ConsideracoesMentor>().Add(entity);
            }
            entity.LiberadoRH = consideracoesMentorDto.LiberadoRH;
            entity.AcaoComite = consideracoesMentorDto.AcaoComite;
            entity.PontosFortesRH = consideracoesMentorDto.PontosFortesRH;
            entity.PontosFracosRH = consideracoesMentorDto.PontosFracosRH;
            entity.SalarioAtual = consideracoesMentorDto.SalarioAtual;
            entity.SalarioNovo = consideracoesMentorDto.SalarioNovo;
            entity.RegimeContratacaoAtual = consideracoesMentorDto.RegimeContratacaoAtual;
            entity.RegimeContratacaoNovo = consideracoesMentorDto.RegimeContratacaoNovo;
            entity.MentoriaRealizada = consideracoesMentorDto.MentoriaRealizada;
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return false;
        }
    }

    public async Task<string> GerarJsonRadarAsync(int idAssociado, int idProjeto, int idPeriodo, int idCargo)
    {
        // Exemplo de geração de dados para gráfico radar
        var competencias = await ObterCompetenciasConsolidacaoAsync(idAssociado, idProjeto, idPeriodo);
        var labels = competencias.Select(c => c.Eixo).ToList();
        var datasetCompetencias = competencias.Select(c => c.NotaCompetenciaAvaliado ?? 0).ToList();
        var nivelAtual = competencias.Any() ? Math.Ceiling((competencias.Sum(c => c.NotaCompetenciaAvaliado ?? 0) / competencias.Count) / 100m) * 100m : 0m;
        var datasetNivelAtual = competencias.Select(c => nivelAtual).ToList();
        var obj = new
        {
            labels,
            datasetcompetencias = datasetCompetencias,
            datasetnivelatual = datasetNivelAtual
        };
        return JsonSerializer.Serialize(new[] { obj });
    }
}
