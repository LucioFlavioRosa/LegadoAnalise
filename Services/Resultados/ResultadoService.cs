using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Resultados;

public class ResultadoService : IResultadoService
{
    private readonly ApplicationDbContext _db;

    public ResultadoService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ResultadoProjetosModel>> ObterResultadosProjetosAsync(int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo)
    {
        // Busca resultados de projetos para o associado, período, tipo de avaliação e escopo
        return await _db.ResultadoProjetos
            .Where(r => r.IdAssociado == idAssociado && r.IdPeriodo == idPeriodo && r.TipoAvaliacao == tipoAvaliacao && r.Escopo == escopo)
            .Include(r => r.ListCompetenciasNivel1)
            .Include(r => r.ListCompetenciasNivel2)
            .Include(r => r.ListPerfomance)
            .Include(r => r.ListSomaCompetenciasN1N2)
            .Include(r => r.ListSomaCompetenciasN1N2)
            .ToListAsync();
    }

    public async Task<ResultadoSomaProjetosModel?> ObterSomaResultadosProjetosAsync(List<ResultadoProjetosModel> resultadosProjetos)
    {
        if (resultadosProjetos == null || resultadosProjetos.Count == 0)
            return null;
        var ids = resultadosProjetos.Select(r => r.Id).ToList();
        // Busca o somatório consolidado dos projetos
        return await _db.ResultadoSomaProjetos
            .Where(s => ids.Contains(s.IdAssociadoProjeto))
            .Include(s => s.ListProjetosSomaPerfomance)
            .Include(s => s.ListProjetosSomaCompetenciasN1N2)
            .FirstOrDefaultAsync();
    }

    public async Task<Associado?> ObterAssociadoMentorCargoAsync(int idAssociado)
    {
        return await _db.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Mentor)
            .FirstOrDefaultAsync(a => a.Id == idAssociado);
    }

    public async Task<PERIODOSAVALIACOES?> ObterPeriodoAsync(int idPeriodo)
    {
        return await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
    }
}
