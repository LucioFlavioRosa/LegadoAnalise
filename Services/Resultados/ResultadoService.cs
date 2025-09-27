using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Services.Resultados.Common;
using Microsoft.EntityFrameworkCore;

namespace Services.Resultados;

public class ResultadoService : IResultadoService
{
    private readonly ApplicationDbContext _db;
    private readonly ResultadoHelper _helper;

    public ResultadoService(ApplicationDbContext db, ResultadoHelper helper)
    {
        _db = db;
        _helper = helper;
    }

    public async Task<List<ResultadoProjetosModel>> ObterResultadoAssociadoAsync(int idAssociado, int idPeriodo, int idCargo, string tipoAvaliacao, string escopo)
    {
        var projetos = await _db.ResultadoProjetos
            .Where(x => x.IdAssociado == idAssociado && x.IdPeriodo == idPeriodo && x.TipoAvaliacao == tipoAvaliacao && x.Escopo == escopo)
            .ToListAsync();
        return projetos;
    }

    public async Task<List<ResultadoSomaProjetosModel>> ObterSomaProjetosAsync(List<ResultadoProjetosModel> resultadosProjetos)
    {
        var ids = resultadosProjetos.Select(r => r.Id).ToList();
        var somaProjetos = await _db.ResultadoSomaProjetos
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
        return somaProjetos;
    }

    public string FormatPercentagem(object nota)
    {
        return _helper.FormatPercentagem(nota);
    }

    public string FormatDecimal(object nota)
    {
        return _helper.FormatDecimal(nota);
    }

    public string TruncarTexto(string texto, int qtdCaracteres)
    {
        return _helper.TruncarTexto(texto, qtdCaracteres);
    }
}
