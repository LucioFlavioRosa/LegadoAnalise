using Peers.Moderno.Models;
using Services.PDI.Common;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;

namespace Services.PDI;

public interface IPDIService
{
    Task<List<PDIPeriodo>> GetPeriodosAsync(int idAssociado);
    Task<List<PDIQuestao>> GetQuestoesAsync(int idPeriodo);
    Task<List<PDIResposta>> GetRespostasAsync(int idAssociado, int idPeriodo);
    Task AtualizarRespostaAsync(int idResposta, string resposta);
}

public class PDIService : IPDIService
{
    private readonly ApplicationDbContext _db;
    private readonly IPDIHelper _helper;

    public PDIService(ApplicationDbContext db, IPDIHelper helper)
    {
        _db = db;
        _helper = helper;
    }

    public async Task<List<PDIPeriodo>> GetPeriodosAsync(int idAssociado)
    {
        var respostas = await _db.PDIRespostas
            .Include(r => r.Periodo)
            .Where(r => r.IdAssociado == idAssociado)
            .ToListAsync();
        return respostas.Select(r => r.Periodo)
            .Distinct()
            .OrderBy(p => p.IdPeriodo)
            .ToList();
    }

    public async Task<List<PDIQuestao>> GetQuestoesAsync(int idPeriodo)
    {
        var questoes = await _db.PDIQuestoes
            .Where(q => q.IdPeriodo == idPeriodo)
            .OrderBy(q => q.ColunaPosicao)
            .ThenBy(q => q.Ordem)
            .ToListAsync();
        return questoes;
    }

    public async Task<List<PDIResposta>> GetRespostasAsync(int idAssociado, int idPeriodo)
    {
        var respostas = await _db.PDIRespostas
            .Include(r => r.Questao)
            .Where(r => r.IdAssociado == idAssociado && r.IdPeriodo == idPeriodo)
            .ToListAsync();
        return respostas;
    }

    public async Task AtualizarRespostaAsync(int idResposta, string resposta)
    {
        var entity = await _db.PDIRespostas.FindAsync(idResposta);
        if (entity != null)
        {
            entity.Resposta = _helper.NormalizeResposta(resposta);
            entity.DataAtualizacao = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
