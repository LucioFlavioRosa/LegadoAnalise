using Microsoft.Extensions.Configuration;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Associados;
using System.Web;
using Newtonsoft.Json;

namespace Peers.Moderno.Services.Common.Auth;

public interface IAuthService
{
    Task<AuthUrlResult> GerarUrlAutorizacaoAsync();
    Task<AuthResult> ProcessarCallbackAsync(string? code, string? state, string? error, string? errorDescription);
    Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier);
    void SetSessionState(string state, string codeVerifier);
    string? GetSessionState();
    string? GetSessionCodeVerifier();
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IAssociadosService _associadosService;
    private readonly ITokenDecoder _tokenDecoder;
    private readonly IMessageBoxService _messageBoxService;
    private readonly ITelemetryService _telemetryService;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IConfiguration configuration,
        IAssociadosService associadosService,
        ITokenDecoder tokenDecoder,
        IMessageBoxService messageBoxService,
        ITelemetryService telemetryService,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _associadosService = associadosService;
        _tokenDecoder = tokenDecoder;
        _messageBoxService = messageBoxService;
        _telemetryService = telemetryService;
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthUrlResult> GerarUrlAutorizacaoAsync()
    {
        try
        {
            var tenantId = _configuration["Authentication:AzureAd:TenantId"];
            var clientId = _configuration["Authentication:AzureAd:ClientId"];
            var redirectUri = _configuration["Authentication:AzureAd:RedirectUri"];
            var adInstance = _configuration["Authentication:AzureAd:ADInstance"];

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) ||
                string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(adInstance))
            {
                return AuthUrlResult.Error("Configuração de autenticação incompleta");
            }

            var state = AuthPkceHelper.GenerateState();
            var codeVerifier = AuthPkceHelper.GenerateCodeVerifier();
            var codeChallenge = AuthPkceHelper.GenerateCodeChallenge(codeVerifier);

            SetSessionState(state, codeVerifier);

            var authUrl = $"{adInstance}{tenantId}/oauth2/v2.0/authorize?" +
                         $"client_id={clientId}" +
                         $"&response_type=code" +
                         $"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}" +
                         $"&response_mode=query" +
                         $"&scope=openid%20profile%20email" +
                         $"&state={state}" +
                         $"&code_challenge={codeChallenge}" +
                         $"&code_challenge_method=S256";

            _telemetryService.TrackEvent("AuthUrlGenerated", new Dictionary<string, string>
            {
                { "TenantId", tenantId },
                { "State", state }
            });

            return AuthUrlResult.Success(authUrl);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GerarUrlAutorizacaoAsync" },
                { "Component", "AuthService" }
            });
            return AuthUrlResult.Error($"Erro ao gerar URL de autorização: {ex.Message}");
        }
    }

    public async Task<AuthResult> ProcessarCallbackAsync(string? code, string? state, string? error, string? errorDescription)
    {
        try
        {
            if (!string.IsNullOrEmpty(error))
            {
                var errorMsg = $"Erro de autenticação: {errorDescription ?? error}";
                _messageBoxService.ShowError(errorMsg);
                return AuthResult.Error(errorMsg);
            }

            if (string.IsNullOrEmpty(code))
            {
                var errorMsg = "Código de autorização não fornecido";
                _messageBoxService.ShowError(errorMsg);
                return AuthResult.Error(errorMsg);
            }

            var sessionState = GetSessionState();
            if (!AuthPkceHelper.ValidateState(state, sessionState))
            {
                var errorMsg = "Estado de autenticação inválido";
                _messageBoxService.ShowError(errorMsg);
                return AuthResult.Error(errorMsg);
            }

            var codeVerifier = GetSessionCodeVerifier();
            if (string.IsNullOrEmpty(codeVerifier))
            {
                var errorMsg = "Verificador de código não encontrado";
                _messageBoxService.ShowError(errorMsg);
                return AuthResult.Error(errorMsg);
            }

            var tokenResponse = await ExchangeCodeForTokenAsync(code, codeVerifier);
            var userInfo = _tokenDecoder.DecodeIdToken(tokenResponse.IdToken);

            var usuario = await _associadosService.ObterAssociadoPorEmailAsync(userInfo.Email);
            if (usuario == null)
            {
                var errorMsg = "Usuário não encontrado no sistema";
                _messageBoxService.ShowError(errorMsg);
                return AuthResult.Error(errorMsg);
            }

            if (!usuario.Ativo)
            {
                var errorMsg = "Usuário inativo";
                _messageBoxService.ShowError(errorMsg);
                return AuthResult.Error(errorMsg);
            }

            _messageBoxService.ShowSuccess("Login realizado com sucesso");
            _telemetryService.TrackEvent("LoginSuccess", new Dictionary<string, string>
            {
                { "UserId", usuario.Id.ToString() },
                { "UserEmail", usuario.Email }
            });

            return AuthResult.Success(usuario, userInfo, tokenResponse);
        }
        catch (Exception ex)
        {
            var errorMsg = $"Erro ao processar autenticação: {ex.Message}";
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ProcessarCallbackAsync" },
                { "Component", "AuthService" }
            });
            _messageBoxService.ShowError(errorMsg);
            return AuthResult.Error(errorMsg);
        }
    }

    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier)
    {
        var tenantId = _configuration["Authentication:AzureAd:TenantId"];
        var clientId = _configuration["Authentication:AzureAd:ClientId"];
        var clientSecret = _configuration["Authentication:AzureAd:ClientSecret"];
        var adInstance = _configuration["Authentication:AzureAd:ADInstance"];
        var redirectUri = _configuration["Authentication:AzureAd:RedirectUri"];
        var scopes = _configuration["Authentication:AzureAd:Scopes"];

        var tokenEndpoint = $"{adInstance}{tenantId}/oauth2/v2.0/token";

        var parameters = new Dictionary<string, string>
        {
            {"grant_type", "authorization_code"},
            {"client_id", clientId ?? throw new InvalidOperationException("ClientId não configurado")},
            {"code", code},
            {"redirect_uri", redirectUri ?? throw new InvalidOperationException("RedirectUri não configurado")},
            {"scope", scopes ?? "openid profile email"},
            {"code_verifier", codeVerifier}
        };

        if (!string.IsNullOrEmpty(clientSecret))
        {
            parameters.Add("client_secret", clientSecret);
        }

        var content = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(tokenEndpoint, content);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao obter token: {json}");
        }

        return JsonConvert.DeserializeObject<TokenResponse>(json) ?? 
               throw new InvalidOperationException("Resposta de token inválida");
    }

    public void SetSessionState(string state, string codeVerifier)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session != null)
        {
            session.SetString("AuthState", state);
            session.SetString("CodeVerifier", codeVerifier);
        }
    }

    public string? GetSessionState()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        return session?.GetString("AuthState");
    }

    public string? GetSessionCodeVerifier()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        return session?.GetString("CodeVerifier");
    }
}

public class AuthUrlResult
{
    public bool IsSuccess { get; private set; }
    public string? Url { get; private set; }
    public string? ErrorMessage { get; private set; }

    private AuthUrlResult() { }

    public static AuthUrlResult Success(string url)
    {
        return new AuthUrlResult { IsSuccess = true, Url = url };
    }

    public static AuthUrlResult Error(string errorMessage)
    {
        return new AuthUrlResult { IsSuccess = false, ErrorMessage = errorMessage };
    }
}

public class AuthResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Models.Associado? Usuario { get; private set; }
    public UserInfo? UserInfo { get; private set; }
    public TokenResponse? TokenResponse { get; private set; }

    private AuthResult() { }

    public static AuthResult Success(Models.Associado usuario, UserInfo userInfo, TokenResponse tokenResponse)
    {
        return new AuthResult
        {
            IsSuccess = true,
            Usuario = usuario,
            UserInfo = userInfo,
            TokenResponse = tokenResponse
        };
    }

    public static AuthResult Error(string errorMessage)
    {
        return new AuthResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}