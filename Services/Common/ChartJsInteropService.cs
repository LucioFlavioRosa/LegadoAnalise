using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Services.Common;

public interface IChartJsInteropService
{
    Task RenderBarChartAsync(string canvasId, IEnumerable<string> labels, IEnumerable<decimal> data, string label = "", string backgroundColor = "#355AA5");
    Task RenderRadarChartAsync(string canvasId, IEnumerable<string> labels, IEnumerable<decimal> data, string label = "", string backgroundColor = "#355AA5");
    Task RenderMultiDatasetBarChartAsync(string canvasId, IEnumerable<string> labels, IEnumerable<(string Label, IEnumerable<decimal> Data, string Color)> datasets);
    Task RenderMultiDatasetRadarChartAsync(string canvasId, IEnumerable<string> labels, IEnumerable<(string Label, IEnumerable<decimal> Data, string Color)> datasets);
}

public class ChartJsInteropService : IChartJsInteropService
{
    private readonly IJSRuntime _jsRuntime;

    public ChartJsInteropService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task RenderBarChartAsync(string canvasId, IEnumerable<string> labels, IEnumerable<decimal> data, string label = "", string backgroundColor = "#355AA5")
    {
        await _jsRuntime.InvokeVoidAsync("ChartJsInterop.renderBarChart", canvasId, labels, data, label, backgroundColor);
    }

    public async Task RenderRadarChartAsync(string canvasId, IEnumerable<string> labels, IEnumerable<decimal> data, string label = "", string backgroundColor = "#355AA5")
    {
        await _jsRuntime.InvokeVoidAsync("ChartJsInterop.renderRadarChart", canvasId, labels, data, label, backgroundColor);
    }

    public async Task RenderMultiDatasetBarChartAsync(string canvasId, IEnumerable<string> labels, IEnumerable<(string Label, IEnumerable<decimal> Data, string Color)> datasets)
    {
        var datasetsList = new List<object>();
        foreach (var ds in datasets)
        {
            datasetsList.Add(new { label = ds.Label, data = ds.Data, backgroundColor = ds.Color });
        }
        await _jsRuntime.InvokeVoidAsync("ChartJsInterop.renderMultiBarChart", canvasId, labels, datasetsList);
    }

    public async Task RenderMultiDatasetRadarChartAsync(string canvasId, IEnumerable<string> labels, IEnumerable<(string Label, IEnumerable<decimal> Data, string Color)> datasets)
    {
        var datasetsList = new List<object>();
        foreach (var ds in datasets)
        {
            datasetsList.Add(new { label = ds.Label, data = ds.Data, backgroundColor = ds.Color });
        }
        await _jsRuntime.InvokeVoidAsync("ChartJsInterop.renderMultiRadarChart", canvasId, labels, datasetsList);
    }
}