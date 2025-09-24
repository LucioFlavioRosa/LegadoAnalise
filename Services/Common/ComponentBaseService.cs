using Microsoft.AspNetCore.Components;

namespace Peers.Moderno.Services.Common;

public interface IComponentBaseService
{
    Task TrackPageViewAsync(string pageName, ComponentBase component);
    Task TrackComponentEventAsync(string eventName, ComponentBase component, Dictionary<string, string>? additionalProperties = null);
    Task HandleComponentErrorAsync(Exception ex, ComponentBase component, string operation = "Unknown");
    void ShowMessage(string message, MessageBoxType type);
}

public class ComponentBaseService : IComponentBaseService
{
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;

    public ComponentBaseService(
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService)
    {
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
    }

    public async Task TrackPageViewAsync(string pageName, ComponentBase component)
    {
        try
        {
            var properties = new Dictionary<string, string>
            {
                { "Component", component.GetType().Name },
                { "PageName", pageName },
                { "Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            _telemetryService.TrackPageView(pageName, properties);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "TrackPageViewAsync" },
                { "Component", "ComponentBaseService" },
                { "PageName", pageName }
            });
        }
    }

    public async Task TrackComponentEventAsync(string eventName, ComponentBase component, Dictionary<string, string>? additionalProperties = null)
    {
        try
        {
            var properties = new Dictionary<string, string>
            {
                { "Component", component.GetType().Name },
                { "EventName", eventName },
                { "Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            if (additionalProperties != null)
            {
                foreach (var prop in additionalProperties)
                {
                    properties[prop.Key] = prop.Value;
                }
            }

            _telemetryService.TrackEvent(eventName, properties);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "TrackComponentEventAsync" },
                { "Component", "ComponentBaseService" },
                { "EventName", eventName }
            });
        }
    }

    public async Task HandleComponentErrorAsync(Exception ex, ComponentBase component, string operation = "Unknown")
    {
        try
        {
            var properties = new Dictionary<string, string>
            {
                { "Component", component.GetType().Name },
                { "Operation", operation },
                { "ErrorMessage", ex.Message },
                { "StackTrace", ex.StackTrace ?? "No stack trace available" }
            };

            _telemetryService.TrackException(ex, properties);
            _messageBoxService.ShowError($"Erro na operação: {operation}");
            
            await Task.CompletedTask;
        }
        catch (Exception innerEx)
        {
            _telemetryService.TrackException(innerEx, new Dictionary<string, string>
            {
                { "Method", "HandleComponentErrorAsync" },
                { "Component", "ComponentBaseService" },
                { "OriginalError", ex.Message }
            });
        }
    }

    public void ShowMessage(string message, MessageBoxType type)
    {
        switch (type)
        {
            case MessageBoxType.Success:
                _messageBoxService.ShowSuccess(message);
                break;
            case MessageBoxType.Error:
                _messageBoxService.ShowError(message);
                break;
            case MessageBoxType.Info:
                _messageBoxService.ShowInfo(message);
                break;
            case MessageBoxType.Warning:
                _messageBoxService.ShowWarning(message);
                break;
            default:
                _messageBoxService.ShowInfo(message);
                break;
        }
    }
}