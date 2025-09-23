using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Peers.Moderno.Services.Common.Auth;

namespace Peers.Moderno.Pages;

public partial class AuthCallback : ComponentBase
{
    [Inject] private IAuthCallbackService AuthCallbackService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private bool isProcessing = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await ProcessAuthCallback();
    }

    private async Task ProcessAuthCallback()
    {
        try
        {
            var uri = new Uri(Navigation.Uri);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            var code = query.TryGetValue("code", out var codeValues) ? codeValues.FirstOrDefault() : null;
            var state = query.TryGetValue("state", out var stateValues) ? stateValues.FirstOrDefault() : null;
            var error = query.TryGetValue("error", out var errorValues) ? errorValues.FirstOrDefault() : null;
            var errorDescription = query.TryGetValue("error_description", out var errorDescValues) ? errorDescValues.FirstOrDefault() : null;

            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                errorMessage = "Contexto HTTP não disponível";
                isProcessing = false;
                return;
            }

            var sessionState = httpContext.Session.GetString("AuthState");
            var codeVerifier = httpContext.Session.GetString("CodeVerifier");

            var result = await AuthCallbackService.ProcessCallbackAsync(
                code, state, error, errorDescription, sessionState, codeVerifier);

            if (result.IsSuccess && result.Usuario != null)
            {
                await ConfigurarAutenticacao(result.Usuario, httpContext);
                
                // Limpar dados temporários da sessão
                httpContext.Session.Remove("AuthState");
                httpContext.Session.Remove("CodeVerifier");
                
                // Redirecionar para página principal
                Navigation.NavigateTo("/", true);
            }
            else
            {
                errorMessage = result.ErrorMessage ?? "Erro desconhecido na autenticação";
                isProcessing = false;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao processar callback: {ex.Message}";
            isProcessing = false;
        }
    }

    private async Task ConfigurarAutenticacao(Models.Associado usuario, HttpContext httpContext)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new("IdEmpresa", usuario.IdEmpresa?.ToString() ?? "0"),
            new("IdPerfil", usuario.IdPerfil?.ToString() ?? "0"),
            new("IdCargo", usuario.IdCargo?.ToString() ?? "0"),
            new("IsAzureAD", "true")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Configurar sessão para compatibilidade com código legado
        httpContext.Session.SetString("EMAIL", usuario.Email);
        httpContext.Session.SetInt32("IDUSUARIO", usuario.Id);
        httpContext.Session.SetInt32("USR", usuario.Id);
        httpContext.Session.SetInt32("IDASSOCIADOLOGADO", usuario.Id);
        httpContext.Session.SetInt32("IDEMPRESA", usuario.IdEmpresa ?? 0);
        httpContext.Session.SetInt32("IDPERFIL", usuario.IdPerfil ?? 0);
        httpContext.Session.SetInt32("IDCARGO", usuario.IdCargo ?? 0);
        httpContext.Session.SetString("NOME", usuario.Nome);
        httpContext.Session.SetString("ISAZUREAD", "true");
    }
}