using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Services.Common;

public interface IChartJsInteropService
{
    Task InitializeChartAsync(string canvasId, object chartConfig);
    Task UpdateChartAsync(string canvasId, object chartConfig);
    Task DestroyChartAsync(string canvasId);
}

public class ChartJsInteropService : IChartJsInteropService
{
    private readonly IJSRuntime _jsRuntime;

    public ChartJsInteropService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeChartAsync(string canvasId, object chartConfig)
    {
        await _jsRuntime.InvokeVoidAsync("chartJsInterop.initialize", canvasId, chartConfig);
    }

    public async Task UpdateChartAsync(string canvasId, object chartConfig)
    {
        await _jsRuntime.InvokeVoidAsync("chartJsInterop.update", canvasId, chartConfig);
    }

    public async Task DestroyChartAsync(string canvasId)
    {
        await _jsRuntime.InvokeVoidAsync("chartJsInterop.destroy", canvasId);
    }
}
