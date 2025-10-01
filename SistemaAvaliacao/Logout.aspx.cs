using Business.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();

            // Limpa a sessão
            Session.Clear();
            Session.Abandon();

            WebStorage.Delete("UsuarioLogado");

            // Parâmetros do Azure AD
            var tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
            var addInstance = ConfigurationManager.AppSettings["ida:ADInstance"];
            var postLogoutRedirectUri = HttpUtility.UrlEncode(
                ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"] ?? ResolveUrl("~/Login.aspx")
            );

            var azureLogoutUrl = $"{addInstance}{tenantId}/oauth2/v2.0/logout?post_logout_redirect_uri={postLogoutRedirectUri}";

            Response.Redirect(azureLogoutUrl, false);
        }
    }
}