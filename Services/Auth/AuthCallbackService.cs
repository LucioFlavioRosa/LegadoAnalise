using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using Services.Auth;

namespace Services.Auth
{
    public class AuthCallbackService : IAuthCallbackService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthCallbackService> _logger;
        private readonly IDistributedCache _cache;

        public AuthCallbackService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<AuthCallbackService> logger,
            IDistributedCache cache)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = cache;
        }

        public async Task<AuthResult> ProcessCallbackAsync(string code, string state, string sessionState, string codeVerifier)
        {
            if (state != sessionState)
            {
                return new AuthResult { Success = false, ErrorMessage = "Estado inválido" };
            }

            var client = _httpClientFactory.CreateClient();
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var redirectUri = _configuration["AzureAd:RedirectUri"];
            var aadInstance = _configuration["AzureAd:Instance"];
            var scopes = string.Join(" ", _configuration.GetSection("AzureAd:Scopes").Get<string[]>() ?? new string[] { });
            var tokenEndpoint = $"{aadInstance}{tenantId}/oauth2/v2.0/token";

            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", clientId },
                { "code", code },
                { "redirect_uri", redirectUri },
                { "scope", scopes }
            };

            var clientSecret = _configuration["AzureAd:ClientSecret"];
            if (!string.IsNullOrEmpty(clientSecret))
            {
                parameters.Add("client_secret", clientSecret);
            }
            if (!string.IsNullOrEmpty(codeVerifier))
            {
                parameters.Add("code_verifier", codeVerifier);
            }

            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(tokenEndpoint, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter token: {json}", json);
                return new AuthResult { Success = false, ErrorMessage = "Erro ao obter token" };
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var idToken = root.GetProperty("id_token").GetString();

            var userInfo = await GetUserInfoFromTokenAsync(idToken);
            if (userInfo == null)
            {
                return new AuthResult { Success = false, ErrorMessage = "Token inválido" };
            }

            return new AuthResult { Success = true, UserInfo = userInfo };
        }

        public Task<UserInfo> GetUserInfoFromTokenAsync(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(idToken);
            var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);

            var email = claims.ContainsKey("preferred_username") ? claims["preferred_username"] :
                        claims.ContainsKey("email") ? claims["email"] :
                        claims.ContainsKey("upn") ? claims["upn"] : null;
            var name = claims.ContainsKey("name") ? claims["name"] :
                       (claims.ContainsKey("given_name") && claims.ContainsKey("family_name") ? claims["given_name"] + " " + claims["family_name"] : null);
            var id = claims.ContainsKey("oid") ? claims["oid"] :
                     claims.ContainsKey("sub") ? claims["sub"] : null;

            var userInfo = new UserInfo
            {
                Email = email,
                Name = name,
                Id = id,
                Claims = claims
            };
            return Task.FromResult(userInfo);
        }
    }
}
