using Microsoft.AspNetCore.Components;

namespace Peers.Moderno.Services.Common;

public interface IBlazorNavigationService
{
    void NavigateTo(string uri, bool forceLoad = false);
    void NavigateToWithParameters(string baseUri, Dictionary<string, object> parameters, bool forceLoad = false);
    void NavigateBack();
    void Refresh(bool forceLoad = true);
    string GetCurrentUri();
    Dictionary<string, string> GetQueryParameters();
    string? GetQueryParameter(string parameterName);
}

public class BlazorNavigationService : IBlazorNavigationService
{
    private readonly NavigationManager _navigationManager;
    private readonly ITelemetryService _telemetryService;

    public BlazorNavigationService(
        NavigationManager navigationManager,
        ITelemetryService telemetryService)
    {
        _navigationManager = navigationManager;
        _telemetryService = telemetryService;
    }

    public void NavigateTo(string uri, bool forceLoad = false)
    {
        try
        {
            _telemetryService.TrackEvent("Navigation", new Dictionary<string, string>
            {
                { "TargetUri", uri },
                { "ForceLoad", forceLoad.ToString() },
                { "SourceUri", GetCurrentUri() }
            });

            _navigationManager.NavigateTo(uri, forceLoad);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "NavigateTo" },
                { "Component", "BlazorNavigationService" },
                { "TargetUri", uri }
            });
            throw;
        }
    }

    public void NavigateToWithParameters(string baseUri, Dictionary<string, object> parameters, bool forceLoad = false)
    {
        try
        {
            var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value?.ToString() ?? "")}"));
            var fullUri = string.IsNullOrEmpty(queryString) ? baseUri : $"{baseUri}?{queryString}";

            NavigateTo(fullUri, forceLoad);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "NavigateToWithParameters" },
                { "Component", "BlazorNavigationService" },
                { "BaseUri", baseUri }
            });
            throw;
        }
    }

    public void NavigateBack()
    {
        try
        {
            _telemetryService.TrackEvent("NavigationBack", new Dictionary<string, string>
            {
                { "SourceUri", GetCurrentUri() }
            });

            // Em Blazor, não há um método nativo para voltar, então navegamos para a página anterior conhecida
            // ou para uma página padrão
            NavigateTo("/");
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "NavigateBack" },
                { "Component", "BlazorNavigationService" }
            });
            throw;
        }
    }

    public void Refresh(bool forceLoad = true)
    {
        try
        {
            var currentUri = GetCurrentUri();
            NavigateTo(currentUri, forceLoad);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "Refresh" },
                { "Component", "BlazorNavigationService" }
            });
            throw;
        }
    }

    public string GetCurrentUri()
    {
        return _navigationManager.Uri;
    }

    public Dictionary<string, string> GetQueryParameters()
    {
        try
        {
            var uri = new Uri(_navigationManager.Uri);
            var queryParameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(uri.Query))
            {
                var query = uri.Query.TrimStart('?');
                var pairs = query.Split('&');

                foreach (var pair in pairs)
                {
                    var keyValue = pair.Split('=');
                    if (keyValue.Length == 2)
                    {
                        var key = Uri.UnescapeDataString(keyValue[0]);
                        var value = Uri.UnescapeDataString(keyValue[1]);
                        queryParameters[key] = value;
                    }
                }
            }

            return queryParameters;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetQueryParameters" },
                { "Component", "BlazorNavigationService" }
            });
            return new Dictionary<string, string>();
        }
    }

    public string? GetQueryParameter(string parameterName)
    {
        try
        {
            var parameters = GetQueryParameters();
            return parameters.TryGetValue(parameterName, out var value) ? value : null;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetQueryParameter" },
                { "Component", "BlazorNavigationService" },
                { "ParameterName", parameterName }
            });
            return null;
        }
    }
}