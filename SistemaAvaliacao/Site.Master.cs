using Business.DataAccess;
using Business.Services;
using Business.Util;
using System;
using System.Linq;
using System.Web.UI;

namespace SistemaAvaliacao
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = WebStorage.GetUsuarioLogado();

            FotosAssociadosService fotosService = new FotosAssociadosService();

            if (user == null || !user.IsLogged)
            {
                Response.Redirect("~/Login");
            }
            else
            {
                lblNome1.Text = lblNome2.Text = user.Nome;
                lblEmail.Text = user.Email;
                DataModel dtMod = new DataModel();
                string fotoBase64 = "";
                var foto = fotosService.ObterFotoPorAssociado(WebStorage.GetUsuarioLogado().Id);
                if (foto != null)
                {
                    fotoBase64 = foto.Imagem;
                }
                if (fotoBase64 == null || fotoBase64 == "") { fotoBase64 = "assets/images/users/usernophoto.jpg"; }
                fotoBase64 = fotoBase64.Replace(" ", "%20");
                imgUser1.Src = fotoBase64;
                imgUser2.Src = fotoBase64;

                var alocacoes = new ProjetosService().ObterAlocacoesAvaliador(user.Id);

                panelCadastros.Visible = user.IdPerfil > 2;
                panelAvaliacoes.Visible = user.IdPerfil > 1;
                itemAvAsCegas.Visible = alocacoes.Count > 0;
                panelFinal.Visible = user.IdPerfil > 2 || user.Email == "alexandra.nunes@peers.com.br";

                var frenteService = new FrenteInternaService();
                var frentes = frenteService.ObterLiderFrenteInterna(idAssociado: user.Id);
                if (frentes == null || frentes.Count <= 0)
                {
                    linkFrenteInterna.Visible = false;
                }

                // HIDE
                //linkFrenteInterna.Visible = false;
            }
        }

        protected void btnAlterar_Click(object sender, EventArgs e)
        {
            lblMensagem.Text = "";

            if (txtSenha.Value.Trim() == "")
            {
                lblMensagem.Text = "Digite a Senha Atual";
                return;
            }

            var userLogado = WebStorage.GetUsuarioLogado();
            if (txtSenha.Value.Trim().GetHashCode().ToString() != userLogado.Senha)
            {
                lblMensagem.Text = "Senha Atual Inválida";
                return;
            }

            if (txtNovaSenha.Value.Trim() == "")
            {
                lblMensagem.Text = "Digite a Nova Senha";
                return;
            }

            if (txtConfirmaSenha.Value.Trim() == "")
            {
                lblMensagem.Text = "Confirme a Nova Senha";
                return;
            }

            if (txtNovaSenha.Value.Trim() != txtConfirmaSenha.Value.Trim())
            {
                lblMensagem.Text = "As Senhas não são iguais";
                return;
            }

            var service = new AssociadosService();
            var associado = service.ObterAssociado(userLogado.Id);
            associado.Senha = txtNovaSenha.Value.Trim();
            if (service.AlteraAssociadoSenha(userLogado.Id, associado))
            {
                lblMensagem.Text = "Senha Alterada com Sucesso";
                lblMensagem.ForeColor = System.Drawing.Color.Blue;
                userLogado.Senha = userLogado.Senha.GetHashCode().ToString();
                WebStorage.Set(userLogado);
            }
            else
                lblMensagem.Text = "Falha ao Alterar a Senha";
        }
    }
}