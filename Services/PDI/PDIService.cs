using Data;
using Microsoft.EntityFrameworkCore;
using Services.PDI.Common;
using Services.PDI.Common.Models;

public class PDIService : IPDIService
{
    private readonly ApplicationDbContext _db;
    public PDIService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<PDIPeriodoModel>> GetPeriodosAsync(int idAssociado)
    {
        var pdiRespostas = await _db.Set<PDI_RESPOSTAS>()
            .Include(x => x.PERIODOSAVALIACOES)
            .Where(x => x.idAssociado == idAssociado)
            .ToListAsync();
        var pdiPeriodos = pdiRespostas.Select(x => x.PERIODOSAVALIACOES).Distinct().ToList();
        var periodoUltimo = pdiPeriodos.LastOrDefault();
        var result = new List<PDIPeriodoModel>();
        for (var j = 0; j < pdiPeriodos.Count; j++)
        {
            var getPeriodo = pdiPeriodos[j];
            var addPeriodo = new PDIPeriodoModel
            {
                Id = $"tab-{j}",
                Active = (j == pdiPeriodos.Count - 1 ? "active in show" : string.Empty),
                AriaLabelled = $"tab-{j}-tab",
                Periodo = getPeriodo.Periodo
            };
            result.Add(addPeriodo);
        }
        return result;
    }

    public async Task<List<PDIColunaModel>> GetColunasAsync(int idAssociado, int idPeriodo)
    {
        var pdiServiceQuestoes = await _db.Set<PDI_QUESTOES>().ToListAsync();
        var pdiRespostas = await _db.Set<PDI_RESPOSTAS>()
            .Where(x => x.idAssociado == idAssociado && x.idPeriodo == idPeriodo)
            .ToListAsync();
        var periodoUltimo = await _db.Set<PERIODOSAVALIACOES>().OrderByDescending(x => x.IdPeriodo).FirstOrDefaultAsync();
        var pdiQuestoes = pdiServiceQuestoes;
        if (idPeriodo != periodoUltimo?.IdPeriodo)
        {
            var questoesIds = pdiRespostas.Select(x => x.idPDIQuestao).Distinct().ToList();
            pdiQuestoes = pdiServiceQuestoes.Where(x => questoesIds.Contains(x.idPDIQuestoes)).ToList();
        }
        var lastColuna = pdiQuestoes.Select(x => x.ColunaPosicao).DefaultIfEmpty(0).Max();
        var colunas = new List<PDIColunaModel>();
        for (var i = 1; i <= lastColuna; i++)
        {
            var pdiColuna = pdiQuestoes.Where(x => x.ColunaPosicao == i).ToList();
            var addColuna = new PDIColunaModel();
            foreach (var getQuestao in pdiColuna)
            {
                var addRespostas = new PDIRespostaModel
                {
                    Titulo = !string.IsNullOrEmpty(getQuestao.Titulo) ? getQuestao.Titulo : "TITULO",
                    TituloStyle = string.IsNullOrEmpty(getQuestao.Titulo) ? "white" : "#021240",
                    Subtitulo = getQuestao.Subtitulo,
                    SubtituloStyle = !string.IsNullOrEmpty(getQuestao.Subtitulo) ? "style=\"align-self:center;background-color:#F2F2F2;font-weight:bolder;width:40%;min-width:80px;text-align:center;padding:2px\"" : string.Empty,
                    FlexGrow = getQuestao.ColunaTamanho.ToString().Replace(",", ".") + (getQuestao.FixTamanho > -1 ? $";min-height:{getQuestao.FixTamanho}%;max-height:{getQuestao.FixTamanho}%" : string.Empty),
                    Icone = !string.IsNullOrEmpty(getQuestao.Icone) ? getQuestao.Icone : "assets/images/icon/empty.png",
                    BorderStyle = (getQuestao.BordaEsquerda == false ? "border-left:none;" : string.Empty) +
                                  (getQuestao.BordaDireita == false ? "border-right:none;" : string.Empty) +
                                  (getQuestao.BordaCima == false ? "border-top:none;" : string.Empty) +
                                  (getQuestao.BordaBaixo == false ? "border-bottom:none;" : string.Empty)
                };
                var getResposta = pdiRespostas.FirstOrDefault(x => x.idPDIQuestao == getQuestao.idPDIQuestoes);
                if (getResposta != null)
                {
                    addRespostas.Resposta = getResposta.Resposta.Replace("\n", "fsdfsdfs").Replace("\r", "fsdfsdfs");
                    addRespostas.RespostaEnabled = idPeriodo == periodoUltimo?.IdPeriodo;
                    addRespostas.IdPDIResposta = getResposta.idPDIRespostas;
                    addRespostas.OnInput = addRespostas.RespostaEnabled ? $"AtualizarResposta({getResposta.idPDIRespostas}, this.value)" : string.Empty;
                }
                addColuna.PDIRespostas.Add(addRespostas);
            }
            colunas.Add(addColuna);
        }
        return colunas;
    }

    public async Task<List<PDIRespostaModel>> GetRespostasAsync(int idAssociado, int idPeriodo)
    {
        var colunas = await GetColunasAsync(idAssociado, idPeriodo);
        return colunas.SelectMany(c => c.PDIRespostas).ToList();
    }

    public async Task AtualizarRespostaAsync(int idPDIResposta, string resposta)
    {
        var pdiResposta = await _db.Set<PDI_RESPOSTAS>().FirstOrDefaultAsync(x => x.idPDIRespostas == idPDIResposta);
        if (pdiResposta != null)
        {
            pdiResposta.Resposta = resposta;
            await _db.SaveChangesAsync();
        }
    }

    public async Task ValidaPDIRespostasAsync(int idAssociado, int idPeriodo)
    {
        var pdiQuestoes = await _db.Set<PDI_QUESTOES>().ToListAsync();
        var pdiRespostas = await _db.Set<PDI_RESPOSTAS>().Where(x => x.idAssociado == idAssociado && x.idPeriodo == idPeriodo).ToListAsync();
        foreach (var getQuestao in pdiQuestoes)
        {
            var getResposta = pdiRespostas.Where(x => x.idPDIQuestao == getQuestao.idPDIQuestoes).ToList();
            if (getResposta == null || getResposta.Count == 0)
            {
                var addPDIResposta = new PDI_RESPOSTAS
                {
                    idAssociado = idAssociado,
                    idPeriodo = idPeriodo,
                    idPDIQuestao = getQuestao.idPDIQuestoes,
                    Resposta = "Preencha aqui",
                    DHC = DateTime.Now
                };
                _db.Set<PDI_RESPOSTAS>().Add(addPDIResposta);
            }
        }
        await _db.SaveChangesAsync();
    }
}