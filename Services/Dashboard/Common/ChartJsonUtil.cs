using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Dashboard.Common;

public class ChartJsonUtil : IChartJsonUtil
{
    public string SerializePerformanceChart(List<EvolucaoPerformanceModel> model)
    {
        if (model == null || !model.Any())
            return string.Empty;

        dynamic obj = new JObject();
        List<string> labels = new List<string>();
        List<decimal> dataset = new List<decimal>();

        foreach (var item in model)
        {
            labels.Add(item.Performance);
            dataset.Add(item.Nota.HasValue ? Math.Round(item.Nota.Value) : 0);
        }

        obj.labels = new JArray(labels);
        obj.dataset = new JArray(dataset);

        return JsonConvert.SerializeObject(obj);
    }

    public string SerializeDashboardChart(DashboardModel model)
    {
        if (model?.ListAndamento == null || !model.ListAndamento.Any())
            return string.Empty;

        dynamic obj = new JObject();
        List<string> labels = new List<string>();
        List<int> dataset = new List<int>();

        foreach (var item in model.ListAndamento)
        {
            labels.Add(item.Status);
            dataset.Add(item.QtdStatus);
        }

        obj.labels = new JArray(labels);
        obj.dataset = new JArray(dataset);

        return JsonConvert.SerializeObject(obj);
    }

    public string SerializeRadarChart(ResultadoProjetosModel model)
    {
        if (model?.ListSomaCompetenciasN1N2 == null || !model.ListSomaCompetenciasN1N2.Any())
            return string.Empty;

        dynamic obj = new JObject();
        List<string> labels = new List<string>();
        List<decimal> datasetconsolidado = new List<decimal>();
        List<decimal> datasetnivel = new List<decimal>();

        foreach (var item in model.ListSomaCompetenciasN1N2)
        {
            labels.Add(item.Eixo);
            var menN1 = item.PercentualSomaNotaFinalN1N2 ?? 0;
            datasetconsolidado.Add(menN1);
            datasetnivel.Add(200);
        }

        obj.labels = new JArray(labels);
        obj.datasetconsolidado = new JArray(datasetconsolidado);
        obj.datasetnivel = new JArray(datasetnivel);

        return JsonConvert.SerializeObject(obj);
    }

    public string SerializeBarChart<T>(List<T> data, Func<T, string> labelSelector, Func<T, decimal> valueSelector)
    {
        if (data == null || !data.Any())
            return string.Empty;

        dynamic obj = new JObject();
        List<string> labels = new List<string>();
        List<decimal> dataset = new List<decimal>();

        foreach (var item in data)
        {
            labels.Add(labelSelector(item));
            dataset.Add(valueSelector(item));
        }

        obj.labels = new JArray(labels);
        obj.dataset = new JArray(dataset);

        return JsonConvert.SerializeObject(obj);
    }
}