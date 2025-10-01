using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Services.Auth;
using Services.Associados;
using System.Threading.Tasks;
using System;
using System.Linq;

public partial class AuthCallback : ComponentBase
{
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private IAuthCallbackService AuthCallbackService { get; set; }
    [Inject] private IUserSessionService UserSessionService { get; set; }
    [Inject] private ILogger<AuthCallback> Logger { get; set; }
    [Inject] private AssociadosService AssociadosService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);
            var code = query.ContainsKey("code") ? query["code"].FirstOrDefault() : null;
            var state = query.ContainsKey("state") ? query["state"].FirstOrDefault() : null;
            var error = query.ContainsKey("error") ? query["error"].FirstOrDefault() : null;
            var errorDesc = query.ContainsKey("error_description") ? query["error_description"].FirstOrDefault() : null;

            if (!string.IsNullOrEmpty(error))
            {
                Navigation.NavigateTo($"/Login?error={Uri.EscapeDataString(errorDesc ?? error)}", true);
                return;
            }

            // Recupera state e codeVerifier do cache
            // Supondo que UserSessionService tem métodos para recuperar esses dados
            var sessionState = await UserSessionService.GetSessionStateAsync();
            var codeVerifier = await UserSessionService.GetCodeVerifierAsync();

            var authResult = await AuthCallbackService.ProcessCallbackAsync(code, state, sessionState, codeVerifier);

            if (!authResult.Success)
            {
                Navigation.NavigateTo($"/Login?error={Uri.EscapeDataString(authResult.ErrorMessage)}", true);
                return;
            }

            var usuario = await AssociadosService.ObterAssociadoAsync(authResult.UserInfo.Email);

            if (usuario == null)
            {
                Navigation.NavigateTo($"/Login?error={Uri.EscapeDataString("Usuário não autorizado")}", true);
                return;
            }

            await UserSessionService.CreateUserSessionAsync(authResult.UserInfo, usuario);
            Navigation.NavigateTo("/Index", true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro no callback de autenticação");
            Navigation.NavigateTo($"/Login?error={Uri.EscapeDataString("Erro ao processar autenticação")}", true);
        }
    }
}