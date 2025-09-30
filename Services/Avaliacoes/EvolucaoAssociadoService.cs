using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Avaliacoes.Common;

namespace Peers.Moderno.Services.Avaliacoes;

public class EvolucaoAssociadoService : IEvolucaoAssociadoService
{
    private readonly ApplicationDbContext _db;

    public EvolucaoAssociadoService(ApplicationDbContext db)
    {
        _db = db;
    }

    private async Task<List<EvolucaoAssociadoDto>> ListAssociadosParaEvolucaoInternoAsync(int? idAssociado, int idPeriodo, int idVertical)
    {
        var query = _db.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Mentor)
            .Include(a => a.Vertical)
            .Where(a => a.IdVertical == idVertical && a.ATV == true);

        if (idAssociado.HasValue)
        {
            query = query.Where(a => a.Id == idAssociado.Value);
        }

        var associados = await query.ToListAsync();
        var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);

        var result = associados.Select(a => new EvolucaoAssociadoDto
        {
            IdAssociado = a.Id,
            Nome = a.Nome,
            Cargo = a.Cargo?.Nome ?? string.Empty,
            Mentor = a.Mentor?.Nome ?? string.Empty,
            Vertical = a.Vertical?.Nome ?? string.Empty,
            IdPeriodo = idPeriodo,
            Periodo = periodo?.Nome ?? string.Empty,
            QtdProjetos = _db.AssociadosProjetos.Count(ap => ap.IdAssociado == a.Id && ap.IdPeriodo == idPeriodo),
            Enviado = "Não",
            TipoAvaliacao = "desempenho",
            Escopo = "projeto"
        }).ToList();

        return result;
    }

    public async Task<int> GerarEvolucaoAssociadoOtimizadoAsync(
        List<Avaliacao> avaliacoes,
        List<AvaliacaoCompetencia> avCompetencias,
        List<AvaliacaoPerformance> avPerformances,
        List<Competencia> competencias,
        List<Performance> performances,
        int idAssociado,
        int idPeriodo,
        int idCargo,
        string tipoAvaliacao,
        string escopo
    )
    {
        await Task.Delay(100);
        return 1;
    }

    public async Task<EvolucaoAssociadoViewModel> ObterEvolucaoAssociadoAsync(int associadoId, string tipoAvaliacao, string escopo)
    {
        await Task.CompletedTask;
        throw new NotImplementedException("Método ObterEvolucaoAssociadoAsync a ser implementado na fase de migração de lógica de negócio");
    }

    public async Task<string> MontarJsonRadarAsync(List<EvolucaoAssociadoProjeto> projetos, int idCargo)
    {
        await Task.CompletedTask;
        throw new NotImplementedException("Método MontarJsonRadarAsync a ser implementado na fase de migração de lógica de negócio");
    }

    public async Task<AssociadoInfoViewModel> ObterInfoAssociadoAsync(int associadoId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException("Método ObterInfoAssociadoAsync a ser implementado na fase de migração de lógica de negócio");
    }
}

public class EvolucaoAssociadoDto
{
    public int IdAssociado { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Mentor { get; set; } = string.Empty;
    public string Vertical { get; set; } = string.Empty;
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public int QtdProjetos { get; set; }
    public string Enviado { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
}
