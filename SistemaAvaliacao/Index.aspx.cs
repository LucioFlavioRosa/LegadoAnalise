using Business.Services;
using Business.Util;
using System;
using System.Net.Mail;
using System.Web.UI;
using Business.DataAccess;
using Business.Model;
using Microsoft.Ajax.Utilities;
using Microsoft.Graph;
using Microsoft.PowerBI.Api.Models;
using SistemaAvaliacao.UserControls;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;


namespace SistemaAvaliacao
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var user = WebStorage.GetUsuarioLogado();

                var projetosService = new ProjetosService();
                var projetosUsuarioGestor = projetosService.ObterListaProjetosAtributos(user.Id);
                btnGerenciarProjetos.Visible = projetosUsuarioGestor.Count > 0;

                if (user != null && user.Id != 0)
                {
                    var associado = new AssociadosService().ObterAssociado(user.Id);
                    if (associado.Senha == "avaliacao")
                    {
                        divAlterarSenha.Visible = true;
                        divAcessoRapido.Visible = false;
                        divAcessoAvaliacoes.Visible = false;
                        panelMenu.Visible = false;
                    }
                    else
                    {
                        divAlterarSenha.Visible = false;
                        divAcessoRapido.Visible = true;
                        divAcessoAvaliacoes.Visible = true;
                        panelMenu.Visible = true;

                        panelMenu.Visible = user.IdPerfil > 2;
                        divAvaliacao.Visible = user.IdPerfil > 1;

                        var frenteService = new FrenteInternaService();
                        var frentesLideresInternas = frenteService.ObterLideresFrentesInternas();
                        var obterFrenteLider = frentesLideresInternas.Where(x => x.idAssociado == user.Id).ToList();
                        if (obterFrenteLider != null && obterFrenteLider.Count > 0)
                        {
                            btnFrenteInterna.Visible = true;
                        }

                        var associadosService = new AssociadosService();
                        var getMentorados = associadosService.ObterMentorados(user.Id).ToList();
                        if (getMentorados == null || getMentorados.Count == 0)
                        {
                            divMentor.Visible = false;
                        }
                    }
                }

                // REMOVER APÓS OK DO CLTV
                //btnGerenciarProjetos.Visible = false;
                //btnFrenteInterna.Visible = false;
            }
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            Response.Redirect("TiposProjetos.aspx");
        }

        protected void btnClientes_Click(object sender, EventArgs e)
        {
            Response.Redirect("Clientes.aspx");
        }

        protected void btnPEncerramento_Click(object sender, EventArgs e)
        {
            Response.Redirect("PerguntasEncerramento.aspx");
        }

        protected void btnSubCompetencia_Click(object sender, EventArgs e)
        {
            Response.Redirect("SubCompetencias.aspx");
        }

        protected void btnEixo_Click(object sender, EventArgs e)
        {
            Response.Redirect("Eixo.aspx");
        }

        protected void btnCompetencias_Click(object sender, EventArgs e)
        {
            Response.Redirect("Competencias.aspx");
        }

        protected void btnDimensoes_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dimensoes.aspx");
        }

        protected void btnPerformance_Click(object sender, EventArgs e)
        {
            Response.Redirect("Performance.aspx");
        }

        protected void btnComplexidade_Click(object sender, EventArgs e)
        {
            Response.Redirect("Complexidade.aspx");
        }

        protected void btnAssociado_Click(object sender, EventArgs e)
        {
            Response.Redirect("Associados.aspx");
        }

        protected void btnCargos_Click(object sender, EventArgs e)
        {
            Response.Redirect("Cargos.aspx");
        }

        protected void btnPAcesso_Click(object sender, EventArgs e)
        {
            Response.Redirect("Perfis.aspx");
        }

        protected void btnProjetos_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadastroProjetos.aspx");
        }

        protected void btnEnvioAvaliacoes_Click(object sender, EventArgs e)
        {
            Response.Redirect("EnvioAvaliacoes.aspx");
        }

        protected void btnEvolucaoAssociado_Click(object sender, EventArgs e)
        {
            Response.Redirect("EvolucaoAssociado.aspx");
        }

        protected void btnAutoAvaliacao_Click(object sender, EventArgs e)
        {
            Response.Redirect("autoavalizacao.aspx");
        }

        protected void btnCegas_Click(object sender, EventArgs e)
        {
            Response.Redirect("avalizacao_gestorascegas.aspx");
        }

        protected void btnGestor_Click(object sender, EventArgs e)
        {
            Response.Redirect("avalizacao_gestor.aspx");
        }

        protected void btnFeedback_Click(object sender, EventArgs e)
        {
            Response.Redirect("avalizacao_feedback.aspx");
        }

        protected void btnMentor_Click(object sender, EventArgs e)
        {
            Response.Redirect("avalizacao_mentor.aspx");
        }

        protected void btnPendencias_Click(object sender, EventArgs e)
        {
            Response.Redirect("pendencias.aspx");
        }

        protected void btnGerenciarProjetos_Click(object sender, EventArgs e)
        {
            Response.Redirect("GerenciarProjetos");
        }

        protected void btnResultadoLideranca_Click(object sender, EventArgs e)
        {
            Response.Redirect("avalizacao_resultado_lideranca");
        }

        protected void btnPDI_Click(object sender, EventArgs e)
        {
            Response.Redirect("avaliacao_PDI");
        }

        protected void btnAlterar_Click(object sender, EventArgs e)
        {
            lblMensagem.Text = "";
            var userLogado = WebStorage.GetUsuarioLogado();
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

                Response.Redirect("index");
            }
            else
                lblMensagem.Text = "Falha ao Alterar a Senha";
        }

        protected void btnFrenteInterna_Click(object sender, EventArgs e)
        {
            Response.Redirect("frentesinternas");
        }

        protected void btnAvMentor_Click(object sender, EventArgs e)
        {
            Response.Redirect("avaliacao_mentoria");
        }

        protected void btnResultadoMentor_Click(object sender, EventArgs e)
        {
            Response.Redirect("resultado_mentoria");
        }
    }
}