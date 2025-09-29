using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Radar.Common;

namespace Services.Radar;

public interface IRadarService
{
    Task<RadarResult> GetRadarDataAsync(int idPeriodo, int idProjeto, int idAssociado, string tipoAvaliacao, string escopo, bool showRotulo, bool backColor);
}

public class RadarService : IRadarService
{
    private readonly ApplicationDbContext _db;
    private readonly IRadarHelper _radarHelper;

    public RadarService(ApplicationDbContext db, IRadarHelper radarHelper)
    {
        _db = db;
        _radarHelper = radarHelper;
    }

    public async Task<RadarResult> GetRadarDataAsync(int idPeriodo, int idProjeto, int idAssociado, string tipoAvaliacao, string escopo, bool showRotulo, bool backColor)
    {
        var avaliacaoService = new AvaliacoesService(_db);
        var listaAvaliacoes = await avaliacaoService.ObterAvaliacoesCompetenciasAsync(idAssociado, idProjeto, idPeriodo, AvaliacoesService.EtapaAvaliacaoMentor, tipoAvaliacao, escopo);
        if (listaAvaliacoes == null || listaAvaliacoes.Count == 0)
        {
            return RadarResult.Empty("Avaliação não Finalizada");
        }

        var premissas = await _db.PremissasRadar
            .Where(p => p.IdCargo == listaAvaliacoes[0].IdCargo && p.IdNivel == listaAvaliacoes[0].IdNivel)
            .ToListAsync();

        if (premissas == null || premissas.Count == 0)
        {
            return RadarResult.Empty("Premissa Não Cadastrada");
        }

        var radarSeries = _radarHelper.ConfiguraPontosRadar(premissas, listaAvaliacoes, showRotulo, backColor);
        return new RadarResult
        {
            Success = true,
            Series = radarSeries,
            Message = null
        };
    }
}

public class RadarResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<RadarSeries> Series { get; set; } = new();

    public static RadarResult Empty(string message)
    {
        return new RadarResult { Success = false, Message = message, Series = new List<RadarSeries>() };
    }
}

public class RadarSeries
{
    public string Name { get; set; } = string.Empty;
    public List<RadarPoint> Points { get; set; } = new();
    public string Color { get; set; } = "#000000";
    public bool ShowLabel { get; set; } = true;
    public string BorderColor { get; set; } = "#000000";
    public int BorderWidth { get; set; } = 3;
    public string ChartType { get; set; } = "radar";
}

public class RadarPoint
{
    public string Eixo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
