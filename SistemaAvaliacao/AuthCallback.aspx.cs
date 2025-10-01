using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Business.Services;
using Business.Util;
using Business.DataAccess;

namespace SistemaAvaliacao
{
    public partial class AuthCallback : System.Web.UI.Page
    {
        private readonly AssociadosService _associadosService;

        public AuthCallback()
        {
            _associadosService = new AssociadosService();
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var code = Request.QueryString["code"];
                var state = Request.QueryString["state"];
                var error = Request.QueryString["error"];

                // Verificar erro
                if (!string.IsNullOrEmpty(error))
                {
                    var errorDesc = Request.QueryString["error_description"];
                    Response.Redirect($"~/Login.aspx?error={HttpUtility.UrlEncode(errorDesc)}", false);
                    return;
                }

                // Verificar state
                var sessionState = Session["AuthState"]?.ToString();
                if (state != sessionState)
                {
                    Response.Redirect("~/Login.aspx?error=Estado inválido", false);
                    return;
                }

                // Trocar code por token
                if (!string.IsNullOrEmpty(code))
                {
                    var tokenResponse = await TrocarCodePorToken(code);

                    // Decodificar o ID Token
                    var userInfo = DecodeIdToken(tokenResponse.IdToken);

                    // Verificar se usuário existe no sistema
                    var usuario = _associadosService.ObterAssociado(userInfo.Email);

                    if (usuario == null)
                    {
                        Response.Redirect("~/Login.aspx?error=Usuário não encontrado!", false);
                        // Criar usuário se não existir (opcional)
                        // usuario = await CriarUsuarioAzureAD(userInfo);
                    }

                    if (usuario != null)
                    {
                        // Configurar sessão
                        ConfigurarSessaoUsuario(usuario);

                        // Criar cookie de autenticação
                        FormsAuthentication.SetAuthCookie(usuario.Email, false);

                        // Redirecionar para index
                        Response.Redirect("~/Index.aspx", false);
                    }
                    else
                    {
                        Response.Redirect("~/Login.aspx?error=Usuário não autorizado", false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Erro no callback: {ex.Message}");
                Response.Redirect($"~/Login.aspx?error={HttpUtility.UrlEncode("Erro ao processar autenticação")}", false);
            }
        }

        private async Task<TokenResponse> TrocarCodePorToken(string code)
        {
            using (var client = new HttpClient())
            {
                var tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
                var clientId = ConfigurationManager.AppSettings["ida:ClientId"];
                var clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
                var codeVerifier = Session["CodeVerifier"]?.ToString();
                var aadInstance = ConfigurationManager.AppSettings["ida:ADInstance"];

                var tokenEndpoint = $"{aadInstance}{tenantId}/oauth2/v2.0/token";

                var redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];

                var parameters = new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"client_id", clientId},
                    {"code", code},
                    {"redirect_uri", redirectUri},
                    {"scope", "openid profile email"}
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
                var response = await client.PostAsync(tokenEndpoint, content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro ao obter token: {json}");
                }

                return JsonConvert.DeserializeObject<TokenResponse>(json);
            }
        }

        private UserInfo DecodeIdToken(string idToken)
        {
            var parts = idToken.Split('.');
            var payload = parts[1];

            // Adicionar padding se necessário
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(payload);
            var jsonString = Encoding.UTF8.GetString(jsonBytes);

            dynamic data = JsonConvert.DeserializeObject(jsonString);

            return new UserInfo
            {
                Email = data.preferred_username ?? data.email ?? data.upn,
                Name = data.name ?? data.given_name + " " + data.family_name,
                Id = data.oid ?? data.sub
            };
        }

        private async Task<ASSOCIADOS> CriarUsuarioAzureAD(UserInfo userInfo)
        {
            // Implementar lógica para criar usuário baseado no Azure AD
            // Por enquanto, retornar null para não criar automaticamente
            return null;
        }

        private void ConfigurarSessaoUsuario(ASSOCIADOS usuario)
        {
            var usuarioLogado = new UsuarioLogado
            {
                Email = usuario.Email,
                Id = usuario.IdAssociado,
                IdEmpresa = usuario.IdEmpresa,
                Nome = usuario.Nome,
                IsLogged = true,
                IdCargo = usuario.IdCargo,
                IdPerfil = usuario.IdPerfil,
                //IsAzureAD = true
            };

            WebStorage.Set(usuarioLogado);

            Session["EMAIL"] = usuario.Email;
            Session["IDUSUARIO"] = usuario.IdAssociado;
            Session["USR"] = usuario.IdAssociado;
            Session["IDASSOCIADOLOGADO"] = usuario.IdAssociado;
            Session["IDEMPRESA"] = usuario.IdEmpresa;
            Session["IDPERFIL"] = usuario.IdPerfil;
            Session["IDCARGO"] = usuario.IdCargo;
            Session["NOME"] = usuario.Nome;
            Session["ISAZUREAD"] = true;
        }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class UserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}