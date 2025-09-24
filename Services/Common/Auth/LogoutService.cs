using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Common.Auth;

public class LogoutService : ILogoutService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContextService _userContextService;
    private readonly IConfiguration _configuration;
    private readonly ITelemetryService _telemetryService;

    public LogoutService(
        IHttpContextAccessor httpContextAccessor,
        IUserContextService userContextService,
        IConfiguration configuration,
        ITelemetryService telemetryService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userContextService = userContextService;
        _configuration = configuration;
        _telemetryService = telemetryService;
    }

    public async Task<string> LogoutAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                await _userContextService.ClearUserContextAsync();
            }

            var tenantId = _configuration["Authentication:AzureAd:TenantId"];
            var adInstance = _configuration["Authentication:AzureAd:ADInstance"];
            var postLogoutRedirectUri = Uri.EscapeDataString(
                _configuration["Authentication:AzureAd:RedirectUri"] ?? "/login"
            );

            var azureLogoutUrl = $"{adInstance}{tenantId}/oauth2/v2.0/logout?post_logout_redirect_uri={postLogoutRedirectUri}";

            _telemetryService.TrackEvent("UserLogout", new Dictionary<string, string>
            {
                { "LogoutUrl", azureLogoutUrl },
                { "Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
            });

            return azureLogoutUrl;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "LogoutAsync" },
                { "Component", "LogoutService" }
            });
            
            var fallbackUrl = _configuration["Authentication:AzureAd:RedirectUri"] ?? "/login";
            return fallbackUrl;
        }
    }
}