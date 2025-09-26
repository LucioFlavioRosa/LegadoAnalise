using Data;
using Microsoft.EntityFrameworkCore;
using Services.PDI.Common;
using Services.PDI.Common.Models;
using System.Linq;

namespace Services.PDI;

public class PDIService : IPDIService
{
    private readonly ApplicationDbContext _db;

    public PDIService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<PDIPeriodosModel>> ObterPeriodosAsync(int idAssociado)
    {
        var respostas = await _db.Set<PDI_RESPOSTAS>()
            .Include(x => x.PERIODOSAVALIACOES)
            .Where(x => x.idAssociado == idAssociado)
            .ToListAsync();

        var periodos = respostas
            .Select(x => x.PERIODOSAVALIACOES)
            .Distinct()
            .OrderBy(p => p.IdPeriodo)
            .ToList();

        var periodoUltimo = periodos.LastOrDefault();
        var modelos = new List<PDIPeriodosModel>();
        for (var j = 0; j < periodos.Count; j++)
        {
            var periodo = periodos[j];
            var model = new PDIPeriodosModel
            {
                Id = $"tab-{j}",
                Active = j == periodos.Count - 1 ? "active in show" : string.Empty,
                Arialabelled = $"tab-{j}-tab",
                Periodo = periodo.Periodo
            };
            modelos.Add(model);
        }
        return modelos;
    }

    public async Task<List<PDIQuestoesModel>> ObterQuestoesAsync(int idPeriodo)
    {
        var questoes = await _db.Set<PDI_QUESTOES>()
            .Where(q => q.PDI_RESPOSTAS.Any(r => r.idPeriodo == idPeriodo))
            .Select(q => new PDIQuestoesModel
            {
                IdPDIQuestoes = q.idPDIQuestoes,
                Titulo = q.Titulo,
                Subtitulo = q.Subtitulo,
                ColunaPosicao = q.ColunaPosicao,
                ColunaTamanho = q.ColunaTamanho,
                FixTamanho = q.FixTamanho,
                Icone = q.Icone,
                BordaEsquerda = q.BordaEsquerda,
                BordaDireita = q.BordaDireita,
                BordaCima = q.BordaCima,
                BordaBaixo = q.BordaBaixo
            })
            .ToListAsync();
        return questoes;
    }

    public async Task<List<PDIRespostasModel>> ObterRespostasAsync(int idAssociado, int idPeriodo)
    {
        var respostas = await _db.Set<PDI_RESPOSTAS>()
            .Include(r => r.PDI_QUESTOES)
            .Where(r => r.idAssociado == idAssociado && r.idPeriodo == idPeriodo)
            .ToListAsync();

        var periodoUltimo = await _db.Set<PERIODOSAVALIACOES>().OrderByDescending(p => p.IdPeriodo).FirstOrDefaultAsync();
        var corAzul = "#021240";
        var modelos = new List<PDIRespostasModel>();

        foreach (var resposta in respostas)
        {
            var questao = resposta.PDI_QUESTOES;
            var model = new PDIRespostasModel
            {
                Titulo = string.IsNullOrWhiteSpace(questao.Titulo) ? "TITULO" : questao.Titulo,
                TituloStyle = string.IsNullOrWhiteSpace(questao.Titulo) ? "white" : corAzul,
                Subtitulo = questao.Subtitulo,
                SubtituloStyle = !string.IsNullOrWhiteSpace(questao.Subtitulo) ? "style=\"align-self:center;background-color:#F2F2F2;font-weight:bolder;width:40%;min-width:80px;text-align:center;padding:2px\"" : string.Empty,
                FlexGrow = questao.ColunaTamanho.ToString().Replace(",", ".") + (questao.FixTamanho > -1 ? $";min-height:{questao.FixTamanho}%;max-height:{questao.FixTamanho}%" : string.Empty),
                Icone = string.IsNullOrWhiteSpace(questao.Icone) ? "assets/images/icon/empty.png" : questao.Icone,
                BorderStyle = (questao.BordaEsquerda ? string.Empty : "border-left:none;") +
                              (questao.BordaDireita ? string.Empty : "border-right:none;") +
                              (questao.BordaCima ? string.Empty : "border-top:none;") +
                              (questao.BordaBaixo ? string.Empty : "border-bottom:none;"),
                Resposta = (resposta.Resposta ?? string.Empty).Replace('\n', ' ').Replace('\r', ' '),
                RespostaEnabled = resposta.idPeriodo == periodoUltimo?.IdPeriodo,
                IdPDIResposta = resposta.idPDIRespostas,
                OnInput = resposta.idPeriodo == periodoUltimo?.IdPeriodo ? $"action_AtualizaResposta('{resposta.idPDIRespostas}', this); return false; this.focus();" : string.Empty
            };
            modelos.Add(model);
        }
        return modelos;
    }

    public async Task AtualizarRespostaAsync(int idPDIResposta, string resposta)
    {
        var respostaObj = await _db.Set<PDI_RESPOSTAS>().FirstOrDefaultAsync(r => r.idPDIRespostas == idPDIResposta);
        if (respostaObj != null)
        {
            respostaObj.Resposta = resposta;
            await _db.SaveChangesAsync();
        }
    }
}