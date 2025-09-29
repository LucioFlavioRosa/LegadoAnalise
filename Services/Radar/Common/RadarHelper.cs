using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;
using Services.Radar;

namespace Services.Radar.Common;

public interface IRadarHelper
{
    List<RadarSeries> ConfiguraPontosRadar(List<PremissasRadar> premissas, List<AvaliacaoCompetencia> avaliacoes, bool showRotulo, bool backColor);
}

public class RadarHelper : IRadarHelper
{
    public List<RadarSeries> ConfiguraPontosRadar(List<PremissasRadar> premissas, List<AvaliacaoCompetencia> avaliacoes, bool showRotulo, bool backColor)
    {
        var alpha = backColor ? 0.2 : 0.0;
        var series = new List<RadarSeries>();

        var seriePeers = new RadarSeries
        {
            Name = "Radar Peers",
            Color = GetRgba(128, 128, 128, alpha),
            BorderColor = GetRgba(128, 128, 128, 1),
            BorderWidth = 3,
            ChartType = "radar",
            ShowLabel = showRotulo
        };
        var serieAuto = new RadarSeries
        {
            Name = "Auto Avaliação",
            Color = GetRgba(0, 100, 0, alpha),
            BorderColor = GetRgba(0, 100, 0, 1),
            BorderWidth = 3,
            ChartType = "radar",
            ShowLabel = showRotulo
        };
        var serieGestor = new RadarSeries
        {
            Name = "Avaliação Gestor",
            Color = GetRgba(0, 0, 128, alpha),
            BorderColor = GetRgba(0, 0, 128, 1),
            BorderWidth = 3,
            ChartType = "radar",
            ShowLabel = showRotulo
        };

        foreach (var premissa in premissas)
        {
            decimal somaAuto = premissa.ValorBaseAutoAvaliacao;
            decimal somaGestor = premissa.ValorBaseAvaliacaoGestor;

            var avaliacoesEixo = avaliacoes.Where(a => a.COMPETENCIAS.IdEixo == premissa.IdEixo).ToList();
            foreach (var aval in avaliacoesEixo)
            {
                somaAuto += aval.NotaCompetenciaAvaliado ?? 0;
                somaGestor += aval.NotaCompetenciaGestor ?? 0;
            }

            serieAuto.Points.Add(new RadarPoint { Eixo = premissa.IdEixo.ToString(), Valor = somaAuto });
            serieGestor.Points.Add(new RadarPoint { Eixo = premissa.IdEixo.ToString(), Valor = somaGestor });
            seriePeers.Points.Add(new RadarPoint { Eixo = premissa.IdEixo.ToString(), Valor = premissa.ValorRadarPeers });
        }

        series.Add(seriePeers);
        series.Add(serieAuto);
        series.Add(serieGestor);
        return series;
    }

    private string GetRgba(int r, int g, int b, double alpha)
    {
        return $"rgba({r},{g},{b},{alpha.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
    }
}
