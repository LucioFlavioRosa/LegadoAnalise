using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Peers.Moderno.Services.Consolidacao.Common;
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

    public async Task<List<ResultadoCompetenciaModel>> ObterCompetenciasProjetoAssociadoAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        // Implementação simplificada: buscar no banco as competências relacionadas ao projeto/associado/período
        return await _db.Competencias
            .Where(c => c.Cargo.Associados.Any(a => a.Id == idAssociado) && c.Cargo.Projetos.Any(p => p.Id == idProjeto))
            .Select(c => new ResultadoCompetenciaModel
            {
                IdAvaliacaoCompetencia = c.IdCompetencia,
                Eixo = c.Eixo.Nome,
                NotaNivel1AutoAvaliacao = 0, // Preencher conforme lógica de negócio
                NotaNivel1Feedback = 0,
                NotaSubcompetenciaAvaliadoN1 = 0,
                NotaSubcompetenciaGestorN1 = 0,
                NotaCompetenciaAvaliado = 0,
                NotaCompetenciaGestor = 0,
                NotaFinalNivel1 = 0,
                NotaNivel2AutoAvaliacao = 0,
                NotaNivel2Feedback = 0,
                NotaSubcompetenciaAvaliadoN2 = 0,
                NotaSubcompetenciaGestorN2 = 0,
                NotaFinalNivel2 = 0,
                DetalheNivelAtual = c.Dimensao.Nome,
                CompetenciaAtual = c.Nome,
                DetalheProximoNivel = c.Dimensao.Nome,
                CompetenciaProximo = c.Nome,
                ComentarioAvaliado = "",
                ComentarioFeedback = "",
                IdNotaNivel1Comite = null,
                IdNotaNivel1Feedback = null,
                IdNotaNivel2Comite = null,
                IdNotaNivel2Feedback = null,
                enableNivel1 = true,
                enableNivel2 = true
            })
            .ToListAsync();
    }

    public async Task<List<ResultadoPerfomanceModel>> ObterPerformanceProjetoAssociadoAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        // Implementação simplificada: buscar no banco as performances relacionadas ao projeto/associado/período
        return await _db.Performances
            .Where(p => p.Cargo.Associados.Any(a => a.Id == idAssociado))
            .Select(p => new ResultadoPerfomanceModel
            {
                IdAvaliacaoPerformance = p.IdPerformance,
                Perfomance = p.Nome,
                NotaNivel1AutoAvaliacao = 0,
                NotaNivel1Feedback = 0,
                NotaPerfomancePonderada = 0,
                NotaPerfomance = 0,
                ComentariosAutoAvaliacao = "",
                ComentarioFeedback = "",
                IdNotaComite = null,
                IdNotaNivel1Feedback = null
            })
            .ToListAsync();
    }

    public async Task<string> CalcularNotaCompetenciaComiteAsync(int id, int nivel, int nota)
    {
        // Simulação de cálculo, pode ser substituído por lógica real
        decimal? notacomite = nota * 1.0m;
        notacomite = Math.Round(notacomite.Value, 2);
        string strNota = notacomite.HasValue ? notacomite.Value.ToString().Replace(".", ",") : "0";
        return JsonConvert.SerializeObject(new { nota = strNota });
    }

    public async Task<string> CalcularNotaPerformanceComiteAsync(int id, int nota)
    {
        decimal? notacomite = nota * 1.0m;
        string strNota = notacomite.HasValue ? notacomite.Value.ToString().Replace(".", ",") : "0";
        return JsonConvert.SerializeObject(new { nota = strNota });
    }

    public async Task<ConsideracoesMentorModel?> ObterConsideracoesMentorAsync(int idMentor, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo)
    {
        // Simulação de busca
        return await Task.FromResult<ConsideracoesMentorModel?>(null);
    }

    public async Task AtualizarConsideracoesMentorAsync(ConsideracoesMentorModel consideracoesMentor)
    {
        // Simulação de atualização
        await Task.CompletedTask;
    }

    public async Task<AssociadoMentorCargoModel?> ObterAssociadoMentorCargoAsync(int idAssociado)
    {
        // Simulação de busca
        return await Task.FromResult<AssociadoMentorCargoModel?>(null);
    }

    public async Task<PeriodoModel?> ObterPeriodoAsync(int idPeriodo)
    {
        // Simulação de busca
        return await Task.FromResult<PeriodoModel?>(null);
    }

    public async Task<string> ObterFotoAssociadoAsync(int idAssociado)
    {
        // Simulação de busca de foto
        return await Task.FromResult("assets/images/users/usernophoto.jpg");
    }

    public async Task<string> GerarJsonRadarAsync(ResultadoProjetosModel projeto)
    {
        var listradar = new List<dynamic>();
        dynamic obj = new JObject();
        List<string> labels = new List<string>();
        List<decimal> datasetCompetencia = new List<decimal>();
        List<decimal> datasetNivelAtual = new List<decimal>();
        var nivelAtual = Math.Ceiling((projeto.SomaNotaCompetenciaRadar ?? 0) / 100m) * 100m;
        foreach (var item in projeto.ListSomaCompetenciasN1N2)
        {
            labels.Add(item.Eixo);
            datasetCompetencia.Add(item.NotaCompetenciaRadar ?? 0);
            datasetNivelAtual.Add(nivelAtual);
        }
        obj.labels = new JArray(labels);
        obj.datasetcompetencias = new JArray(datasetCompetencia);
        obj.datasetnivelatual = new JArray(datasetNivelAtual);
        listradar.Add(obj);
        return JsonConvert.SerializeObject(listradar);
    }
}