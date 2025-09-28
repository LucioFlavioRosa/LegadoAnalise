using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Services.Periodos.Common;

public class AvaliacoesSinalizadasService : IAvaliacoesSinalizadasService
{
    private readonly ApplicationDbContext _db;

    public AvaliacoesSinalizadasService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<AvaliacaoSinalizadaDto>> ObterAvaliacoesSinalizadasAsync(int? empresaId = null)
    {
        var query = _db.AssociadosProjetos
            .Include(ap => ap.Projeto)
            .Include(ap => ap.Associado)
            .Include(ap => ap.Avaliador)
            .Where(ap => ap.TipoAvaliacao == "desempenho" && ap.AvaliacaoSinalizadaProximoPeriodo == true);

        if (empresaId.HasValue)
        {
            query = query.Where(ap => ap.Projeto.IdEmpresa == empresaId.Value);
        }

        var result = await query
            .Select(ap => new AvaliacaoSinalizadaDto
            {
                Projeto = ap.Projeto != null ? ap.Projeto.Nome : string.Empty,
                Respondente = ap.Associado != null ? ap.Associado.Nome : string.Empty,
                Avaliador = ap.Avaliador != null ? ap.Avaliador.Nome : string.Empty,
                DataInicio = ap.DataInicio.HasValue ? ap.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataTermino = ap.DataTermino.HasValue ? ap.DataTermino.Value.ToString("dd/MM/yyyy") : string.Empty
            })
            .ToListAsync();

        return result;
    }

    public async Task AtualizarAvaliacoesParaUltimoPeriodoAsync(int idUltimoPeriodo)
    {
        var avaliacoes = await _db.AssociadosProjetos
            .Where(ap => ap.AvaliacaoSinalizadaProximoPeriodo == true)
            .ToListAsync();

        foreach (var item in avaliacoes)
        {
            item.IdPeriodoSinalizado = idUltimoPeriodo;
        }
        await _db.SaveChangesAsync();
    }
}
