using Business.Services;
using Business.Util;
using Business.DataAccess;
using SistemaAvaliacao.UserControls;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;

namespace SistemaAvaliacao
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly LoginService _loginService;

        public Login()
        {
            _loginService = new LoginService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar se já está logado
            if (WebStorage.GetUsuarioLogado().IsLogged)
            {
                Response.Redirect("~/Index.aspx", false);
                return;
            }

            if (!IsPostBack && string.IsNullOrEmpty(Request.QueryString["error"]))
            {
                VerificarLoginAzureAD();
                

                // Verificar se veio de logout
                if (Request.QueryString["logout"] == "true")
                {
                    //lblInfo.Text = "Você foi desconectado com sucesso.";
                    //lblInfo.CssClass = "alert alert-info";
                }
            }
            lblVersion.Text = $"Versão: {ConfigurationManager.AppSettings["AppVersion"]}";

            // Links do rodapé
            var links = new[]
            {
                    new { Name = "TikTok", Url = "https://www.tiktok.com/@peersbr", Icon = "https://img.icons8.com/?size=48&id=118640&format=png" },
                    new { Name = "Instagram", Url = "https://www.instagram.com/peers_br", Icon = "https://img.icons8.com/?size=48&id=Xy10Jcu1L2Su&format=png" },
                    new { Name = "LinkedIn", Url = "https://pt.linkedin.com/company/peersbr", Icon = "https://cdn-icons-png.flaticon.com/128/145/145807.png" },
                    new { Name = "Ouvidoria", Url = "https://www.peers4you.com.br/ouvidoria", Icon = "https://cdn-icons-png.flaticon.com/128/9314/9314332.png" },
                    new { Name = "Peers4you", Url = "https://www.peers4you.com.br/", Icon = "https://static.wixstatic.com/media/9b7b3f_37cdc40265e04f81816fb55efb8e3a2c~mv2.png/v1/fill/w_51,h_40,al_c,q_85,usm_0.66_1.00_0.01,enc_avif,quality_auto/Favicon-01.png" },
                    new { Name = "Capacita", Url = "https://peerscapacita.learning.rocks/login", Icon = "https://cdn-icons-png.flaticon.com/128/8991/8991706.png" },
                    new { Name = "Reembolso/CRM", Url = "https://star1crm.starsoft.com.br/peers/1crm_login.asp", Icon = "https://cdn-icons-png.flaticon.com/128/3808/3808310.png" },
                    new { Name = "Holerite", Url = "https://portal.starsoft.com.br/login", Icon = "https://cdn-icons-png.flaticon.com/128/1127/1127283.png" },
                    new { Name = "Peers Brain", Url = "https://brain.peers.com.br", Icon = "https://brain.peers.com.br/avatar/peers.webp" },
                    new { Name = "Onfly", Url = "https://app.onfly.com/login", Icon = "https://cdn-icons-png.flaticon.com/128/10256/10256883.png" }
                };

            rptLinks.DataSource = links;
            rptLinks.DataBind();
        }

        #region Login convencional
        //protected void btnLogin_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Validação
        //        if (string.IsNullOrWhiteSpace(txtLogin.Value) || string.IsNullOrWhiteSpace(txtSenha.Value))
        //        {
        //            MostrarErro("Por favor, preencha email e senha.");
        //            return;
        //        }

        //        // Efetuar login
        //        var usuario = _loginService.EfetuarLogin(txtLogin.Value.Trim(), txtSenha.Value);

        //        if (usuario != null)
        //        {
        //            // IMPORTANTE: Implementar hash seguro de senha
        //            var senhaHash = GerarHashSeguro(txtSenha.Value);

        //            // Criar objeto de usuário logado
        //            var usuarioLogado = new UsuarioLogado
        //            {
        //                Email = usuario.Email,
        //                Id = usuario.IdAssociado,
        //                IdEmpresa = usuario.IdEmpresa,
        //                Nome = usuario.Nome,
        //                Senha = senhaHash, // Usar hash seguro
        //                IsLogged = true,
        //                IdCargo = usuario.IdCargo,
        //                IdPerfil = usuario.IdPerfil
        //            };

        //            // Salvar em WebStorage e Session
        //            WebStorage.Set(usuarioLogado);
        //            ConfigurarSessao(usuario);

        //            // Criar cookie de autenticação
        //            FormsAuthentication.SetAuthCookie(usuario.Email, false);

        //            // Redirecionar
        //            Response.Redirect("~/Index.aspx", false);
        //        }
        //        else
        //        {
        //            MostrarErro("Email ou senha inválidos.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogarErro(ex);
        //        MostrarErro("Ocorreu um erro ao efetuar login. Tente novamente.");
        //    }
        //}

        #endregion
        protected void btnLoginSSO_Click(object sender, EventArgs e)
        {

            try
            {
                var tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
                var clientId = ConfigurationManager.AppSettings["ida:ClientId"];

                var redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
                var addInstance = ConfigurationManager.AppSettings["ida:ADInstance"];

                var state = Guid.NewGuid().ToString();
                Session["AuthState"] = state;

                var codeVerifier = GenerateCodeVerifier();
                var codeChallenge = GenerateCodeChallenge(codeVerifier);
                Session["CodeVerifier"] = codeVerifier;

                var authUrl = $"{addInstance}{tenantId}/oauth2/v2.0/authorize?" +
                             $"client_id={clientId}" +
                             $"&response_type=code" +
                             $"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}" +
                             $"&response_mode=query" +
                             $"&scope=openid%20profile%20email" +
                             $"&state={state}" +
                             $"&code_challenge={codeChallenge}" +
                             $"&code_challenge_method=S256";

                Response.Redirect(authUrl, false);
            }
            catch (Exception ex)
            {
                LogarErro(ex);
                MostrarErro("Erro ao iniciar login com Microsoft. Tente novamente.");
            }
        }

        #region Métodos Auxiliares

        private void ConfigurarSessao(ASSOCIADOS usuario)
        {
            Session["EMAIL"] = usuario.Email;
            Session["IDUSUARIO"] = usuario.IdAssociado;
            Session["USR"] = usuario.IdAssociado;
            Session["IDASSOCIADOLOGADO"] = usuario.IdAssociado;
            Session["IDEMPRESA"] = usuario.IdEmpresa;
            Session["IDPERFIL"] = usuario.IdPerfil;
            Session["IDCARGO"] = usuario.IdCargo;
            Session["NOME"] = usuario.Nome;
        }

        private string GerarHashSeguro(string senha)
        {
            // Usar BCrypt ou similar em produção
            // Por enquanto, usar SHA256 como exemplo
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(senha + ConfigurationManager.AppSettings["SaltKey"]);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(codeVerifier);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }

        private void MostrarErro(string mensagem)
        {
            Response.Redirect($"~/Login.aspx?error={HttpUtility.UrlEncode(mensagem)}", false);
        }

        private void LogarErro(Exception ex)
        {
            // Implementar logging apropriado
            System.Diagnostics.Trace.TraceError($"Erro no login: {ex.Message}");
        }

        private void VerificarLoginAzureAD()
        {
            var tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
            var clientId = ConfigurationManager.AppSettings["ida:ClientId"];
            var redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
            var addInstance = ConfigurationManager.AppSettings["ida:ADInstance"];

            var state = Guid.NewGuid().ToString();
            Session["AuthState"] = state;

            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            Session["CodeVerifier"] = codeVerifier;

            var authUrl = $"{addInstance}{tenantId}/oauth2/v2.0/authorize?" +
                          $"client_id={clientId}" +
                          $"&response_type=code" +
                          $"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}" +
                          $"&response_mode=query" +
                          $"&scope=openid%20profile%20email" +
                          $"&state={state}" +
                          $"&code_challenge={codeChallenge}" +
                          $"&code_challenge_method=S256" +
                          $"&prompt=none"; // <- ESSENCIAL

            Response.Redirect(authUrl, false);
        }


        #endregion
    }
}