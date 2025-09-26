using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Services.PDI.Common;
using Peers.Moderno.Services.PDI.Common.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.PDI;

public class PDIService : IPDIService
{
    private readonly ApplicationDbContext _db;
    private readonly ITelemetryService _telemetry;

    public PDIService(ApplicationDbContext db, ITelemetryService telemetry)
    {
        _db = db;
        _telemetry = telemetry;
    }

    public async Task<List<PDIPeriodoModel>> GetPdiPeriodosAsync(int idAssociado)
    {
        var respostas = await _db.Set<PDI_RESPOSTAS>()
            .Include(r => r.PERIODOSAVALIACOES)
            .Include(r => r.PDI_QUESTOES)
            .Where(r => r.idAssociado == idAssociado)
            .ToListAsync();

        var periodoUltimo = await _db.Set<PERIODOSAVALIACOES>().OrderByDescending(p => p.IdPeriodo).FirstOrDefaultAsync();
        var periodos = respostas.Select(x => x.PERIODOSAVALIACOES).Distinct().OrderBy(p => p.IdPeriodo).ToList();
        var pdiQuestoesAll = await _db.Set<PDI_QUESTOES>().ToListAsync();

        var periodosModel = new List<PDIPeriodoModel>();
        for (int j = 0; j < periodos.Count; j++)
        {
            var periodo = periodos[j];
            var periodoModel = new PDIPeriodoModel
            {
                id = $"tab-{j}",
                active = j == periodos.Count - 1 ? "active in show" : string.Empty,
                arialabelled = $"tab-{j}-tab",
                Periodo = periodo.Periodo,
                PDIColunas = new List<PDIColunasModel>()
            };

            var pdiQuestoes = pdiQuestoesAll;
            var respostasPeriodo = respostas.Where(x => x.idPeriodo == periodo.IdPeriodo).ToList();
            if (periodo.IdPeriodo != periodoUltimo?.IdPeriodo)
            {
                pdiQuestoes = respostasPeriodo.Select(x => x.PDI_QUESTOES).Distinct().ToList();
            }

            var lastColuna = pdiQuestoes.Select(x => x.ColunaPosicao).DefaultIfEmpty(1).Max();
            for (int i = 1; i <= lastColuna; i++)
            {
                var colQuestoes = pdiQuestoes.Where(x => x.ColunaPosicao == i).ToList();
                var colunaModel = new PDIColunasModel { PDIRespostas = new List<PDIRespostasModel>() };
                foreach (var questao in colQuestoes)
                {
                    var resposta = respostasPeriodo.FirstOrDefault(x => x.idPDIQuestao == questao.idPDIQuestoes);
                    var addRespostas = PDIModelHelper.MapToPDIRespostasModel(questao, resposta, periodo, periodoUltimo, j == periodos.Count - 1);
                    colunaModel.PDIRespostas.Add(addRespostas);
                }
                periodoModel.PDIColunas.Add(colunaModel);
            }
            periodosModel.Add(periodoModel);
        }
        return periodosModel;
    }

    public async Task<List<PDIPillsModel>> GetPdiPillsAsync(int idAssociado)
    {
        var respostas = await _db.Set<PDI_RESPOSTAS>()
            .Include(r => r.PERIODOSAVALIACOES)
            .Where(r => r.idAssociado == idAssociado)
            .ToListAsync();
        var periodos = respostas.Select(x => x.PERIODOSAVALIACOES).Distinct().OrderBy(p => p.IdPeriodo).ToList();
        var pills = new List<PDIPillsModel>();
        for (int i = 0; i < periodos.Count; i++)
        {
            var periodo = periodos[i];
            pills.Add(new PDIPillsModel
            {
                id = $"tab-{i}-tab",
                href = $"#tab-{i}",
                ariacontrols = $"tab-{i}",
                ariaselected = i == periodos.Count - 1 ? "true" : "false",
                active = i == periodos.Count - 1 ? "active" : string.Empty,
                Periodo = periodo.Periodo
            });
        }
        return pills;
    }

    public async Task AtualizarRespostaAsync(int idPDIResposta, string resposta)
    {
        var pdiResposta = await _db.Set<PDI_RESPOSTAS>().FirstOrDefaultAsync(x => x.idPDIRespostas == idPDIResposta);
        if (pdiResposta != null)
        {
            pdiResposta.Resposta = resposta;
            _db.Update(pdiResposta);
            await _db.SaveChangesAsync();
            _telemetry.TrackEvent("PDIRespostaAtualizada", new Dictionary<string, string> { { "idPDIResposta", idPDIResposta.ToString() } });
        }
    }

    public async Task ValidaPDIRespostasAsync(int idAssociado, int idPeriodo)
    {
        var pdiQuestoes = await _db.Set<PDI_QUESTOES>().ToListAsync();
        var pdiRespostas = await _db.Set<PDI_RESPOSTAS>().Where(x => x.idAssociado == idAssociado && x.idPeriodo == idPeriodo).ToListAsync();
        foreach (var questao in pdiQuestoes)
        {
            var existe = pdiRespostas.Any(x => x.idPDIQuestao == questao.idPDIQuestoes);
            if (!existe)
            {
                var novaResposta = new PDI_RESPOSTAS
                {
                    idAssociado = idAssociado,
                    idPeriodo = idPeriodo,
                    idPDIQuestao = questao.idPDIQuestoes,
                    Resposta = "Preencha aqui",
                    DHC = DateTime.UtcNow
                };
                _db.Add(novaResposta);
            }
        }
        await _db.SaveChangesAsync();
    }
}