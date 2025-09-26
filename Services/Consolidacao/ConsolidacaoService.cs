using Peers.Moderno.Services.Consolidacao.Common;
using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Peers.Moderno.Services.Consolidacao;

public class ConsolidacaoService : IConsolidacaoService
{
    private readonly ApplicationDbContext _db;

    public ConsolidacaoService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ResultadoCompetenciaModel>> ObterCompetenciasConsolidacaoAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        // Exemplo de consulta simplificada, ajustar conforme modelo real
        var competencias = await _db.Competencias
            .Where(c => c.Cargo.Associados.Any(a => a.Id == idAssociado))
            .Select(c => new ResultadoCompetenciaModel
            {
                IdAvaliacaoCompetencia = c.IdCompetencia,
                Eixo = c.Eixo.Nome,
                NotaNivel1AutoAvaliacao = 0, // Preencher conforme regra
                NotaNivel1Feedback = 0,
                NotaNivel2AutoAvaliacao = 0,
                NotaNivel2Feedback = 0,
                NotaSubcompetenciaAvaliadoN1 = 0,
                NotaSubcompetenciaGestorN1 = 0,
                NotaSubcompetenciaAvaliadoN2 = 0,
                NotaSubcompetenciaGestorN2 = 0,
                NotaCompetenciaAvaliado = 0,
                NotaCompetenciaGestor = 0,
                NotaFinalNivel1 = 0,
                NotaFinalNivel2 = 0,
                ComentarioAvaliado = "",
                ComentarioFeedback = "",
                DetalheNivelAtual = "",
                CompetenciaAtual = c.Nome,
                DetalheProximoNivel = "",
                CompetenciaProximo = "",
                IdNotaNivel1Comite = null,
                IdNotaNivel2Comite = null,
                IdNotaNivel1Feedback = null,
                IdNotaNivel2Feedback = null,
                enableNivel1 = true,
                enableNivel2 = true
            })
            .ToListAsync();
        return competencias;
    }

    public async Task<List<ResultadoPerfomanceModel>> ObterPerformanceConsolidacaoAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        var performances = await _db.Performances
            .Select(p => new ResultadoPerfomanceModel
            {
                IdAvaliacaoPerformance = p.IdPerformance,
                Perfomance = p.Nome,
                NotaNivel1AutoAvaliacao = 0, // Preencher conforme regra
                NotaNivel1Feedback = 0,
                NotaPerfomancePonderada = 0,
                NotaPerfomance = 0,
                ComentariosAutoAvaliacao = "",
                ComentarioFeedback = "",
                IdNotaComite = null,
                IdNotaNivel1Feedback = null
            })
            .ToListAsync();
        return performances;
    }

    public async Task<string> CalcularNotaCompetenciaComiteAsync(int idAvaliacaoCompetencia, int nivel, int nota)
    {
        // Simulação de cálculo, ajustar conforme regra de negócio real
        decimal? notaComite = nota * 1.0m;
        notaComite = Math.Round(notaComite.Value, 2);
        string strNota = notaComite.HasValue ? notaComite.Value.ToString().Replace(".", ",") : "0";
        return JsonConvert.SerializeObject(new { nota = strNota });
    }

    public async Task<string> CalcularNotaPerformanceComiteAsync(int idAvaliacaoPerformance, int nota)
    {
        decimal? notaComite = nota * 1.0m;
        string strNota = notaComite.HasValue ? notaComite.Value.ToString().Replace(".", ",") : "0";
        return JsonConvert.SerializeObject(new { nota = strNota });
    }

    public async Task<ConsideracoesMentorModel?> CarregarConsideracoesMentorAsync(int idMentor, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo)
    {
        // Simulação de busca, ajustar conforme modelo real
        var consideracoes = await _db.Set<ConsideracoesMentorModel>()
            .FirstOrDefaultAsync();
        return consideracoes;
    }

    public async Task<bool> SalvarConsideracoesMentorAsync(ConsideracoesMentorModel consideracoes)
    {
        // Simulação de update, ajustar conforme modelo real
        _db.Update(consideracoes);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<string> ObterJsonRadarAsync(int idAssociado, int idProjeto, int idPeriodo, int idCargo)
    {
        // Simulação de geração de radar, ajustar conforme regra real
        var labels = new List<string> { "Competência 1", "Competência 2" };
        var datasetCompetencia = new List<decimal> { 80, 90 };
        var datasetNivelAtual = new List<decimal> { 100, 100 };
        dynamic obj = new JObject();
        obj.labels = new JArray(labels);
        obj.datasetcompetencias = new JArray(datasetCompetencia);
        obj.datasetnivelatual = new JArray(datasetNivelAtual);
        var listradar = new List<dynamic> { obj };
        return JsonConvert.SerializeObject(listradar);
    }
}
