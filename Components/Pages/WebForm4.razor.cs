using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Pages;

public partial class WebForm4Base : ComponentBase
{
    [Inject] protected ITelemetryService TelemetryService { get; set; } = default!;
    [Inject] protected IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            TelemetryService.TrackPageView("WebForm4", new Dictionary<string, string>
            {
                { "Component", "WebForm4" },
                { "RenderMode", "InteractiveAuto" }
            });

            await base.OnInitializedAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "OnInitializedAsync" },
                { "Component", "WebForm4" }
            });
            MessageBoxService.ShowError("Erro ao carregar a página");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            TelemetryService.TrackEvent("WebForm4Rendered", new Dictionary<string, string>
            {
                { "FirstRender", "true" },
                { "Component", "WebForm4" }
            });
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected virtual void HandleError(Exception ex, string operation = "Unknown")
    {
        TelemetryService.TrackException(ex, new Dictionary<string, string>
        {
            { "Operation", operation },
            { "Component", "WebForm4" }
        });
        MessageBoxService.ShowError($"Erro na operação: {operation}");
    }

    protected virtual void ShowSuccess(string message)
    {
        MessageBoxService.ShowSuccess(message);
    }

    protected virtual void ShowError(string message)
    {
        MessageBoxService.ShowError(message);
    }

    protected virtual void ShowInfo(string message)
    {
        MessageBoxService.ShowInfo(message);
    }

    protected virtual void ShowWarning(string message)
    {
        MessageBoxService.ShowWarning(message);
    }
}