using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Dashboard.Common;

public interface IChartJsonUtil
{
    string SerializePerformanceChart(List<EvolucaoPerformanceModel> model);
    string SerializeDashboardChart(DashboardModel model);
    string SerializeRadarChart(ResultadoProjetosModel model);
    string SerializeBarChart<T>(List<T> data, Func<T, string> labelSelector, Func<T, decimal> valueSelector);
}