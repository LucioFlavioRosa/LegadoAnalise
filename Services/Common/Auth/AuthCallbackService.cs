using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Common.Auth;
using System.Text;

namespace Peers.Moderno.Services.Common.Auth;

public interface IAuthCallbackService
{
    Task<AuthCallbackResult> ProcessCallbackAsync(string? code, string? state, string? error, string? errorDescription, string? sessionState, string? codeVerifier);
    Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string? codeVerifier);
    bool ValidateState(string? receivedState, string? sessionState);
}

public class AuthCallbackService : IAuthCallbackService
{
    private readonly IConfiguration _configuration;
    private readonly ITokenDecoder _tokenDecoder;
    private readonly IAssociadosService _associadosService;
    private readonly HttpClient _httpClient;

    public AuthCallbackService(
        IConfiguration configuration,
        ITokenDecoder tokenDecoder,
        IAssociadosService associadosService,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _tokenDecoder = tokenDecoder;
        _associadosService = associadosService;
        _httpClient = httpClient;
    }

    public async Task<AuthCallbackResult> ProcessCallbackAsync(
        string? code, 
        string? state, 
        string? error, 
        string? errorDescription, 
        string? sessionState, 
        string? codeVerifier)
    {
        try
        {
            // Verificar erro
            if (!string.IsNullOrEmpty(error))
            {
                return AuthCallbackResult.Error($"Erro de autenticação: {errorDescription ?? error}");
            }

            // Verificar state
            if (!ValidateState(state, sessionState))
            {
                return AuthCallbackResult.Error("Estado de autenticação inválido");
            }

            // Verificar code
            if (string.IsNullOrEmpty(code))
            {
                return AuthCallbackResult.Error("Código de autorização não fornecido");
            }

            // Trocar code por token
            var tokenResponse = await ExchangeCodeForTokenAsync(code, codeVerifier);

            // Decodificar o ID Token
            var userInfo = _tokenDecoder.DecodeIdToken(tokenResponse.IdToken);

            // Verificar se usuário existe no sistema
            var usuario = await _associadosService.ObterAssociadoPorEmailAsync(userInfo.Email);

            if (usuario == null)
            {
                return AuthCallbackResult.Error("Usuário não encontrado no sistema");
            }

            if (!usuario.Ativo)
            {
                return AuthCallbackResult.Error("Usuário inativo");
            }

            return AuthCallbackResult.Success(usuario, userInfo, tokenResponse);
        }
        catch (Exception ex)
        {
            return AuthCallbackResult.Error($"Erro ao processar autenticação: {ex.Message}");
        }
    }

    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string? codeVerifier)
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
            {"scope", scopes ?? "openid profile email"}
        };

        if (!string.IsNullOrEmpty(clientSecret))
        {
            parameters.Add("client_secret", clientSecret);
        }
        if (!string.IsNullOrEmpty(codeVerifier))
        {
            parameters.Add("code_verifier", codeVerifier);
        }

        var content = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(tokenEndpoint, content);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao obter token: {json}");
        }

        return JsonConvert.DeserializeObject<TokenResponse>(json) ?? throw new InvalidOperationException("Resposta de token inválida");
    }

    public bool ValidateState(string? receivedState, string? sessionState)
    {
        return !string.IsNullOrEmpty(receivedState) && 
               !string.IsNullOrEmpty(sessionState) && 
               receivedState == sessionState;
    }
}

public class AuthCallbackResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Models.Associado? Usuario { get; private set; }
    public UserInfo? UserInfo { get; private set; }
    public TokenResponse? TokenResponse { get; private set; }

    private AuthCallbackResult() { }

    public static AuthCallbackResult Success(Models.Associado usuario, UserInfo userInfo, TokenResponse tokenResponse)
    {
        return new AuthCallbackResult
        {
            IsSuccess = true,
            Usuario = usuario,
            UserInfo = userInfo,
            TokenResponse = tokenResponse
        };
    }

    public static AuthCallbackResult Error(string errorMessage)
    {
        return new AuthCallbackResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public class TokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty("id_token")]
    public string IdToken { get; set; } = string.Empty;

    [JsonProperty("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }
}