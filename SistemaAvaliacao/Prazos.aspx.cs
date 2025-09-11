using AjaxControlToolkit;
using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Business.Model;
using System.IO;
using TriaSoftware.Util.Framework.Domain.Service;
using System.Web;
using System.Configuration;
using System.Data.OleDb;

namespace SistemaAvaliacao
{
    public partial class Prazo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LimparCampos();
            }
        }

        private void LimparCampos()
        {
            montaListaPrazos();
            txt_NomeDisparo.Text = "";
            txt_Duracao_AUT.Text = "";
            txt_Duracao_CEG.Text = "";
            txt_Duracao_GES.Text = "";
            txt_Duracao_FEE.Text = "";
            txt_Duracao_MEN.Text = "";
            ddlGatilho_AUT.SelectedIndex = 0;
            ddlGatilho_CEG.SelectedIndex = 0;
            ddlGatilho_GES.SelectedIndex = 0;
            ddlGatilho_FEE.SelectedIndex = 0;
            ddlGatilho_MEN.SelectedIndex = 0;
            txt_Compensacao_AUT.Text = "";
            txt_Compensacao_CEG.Text = "";
            txt_Compensacao_GES.Text = "";
            txt_Compensacao_FEE.Text = "";
            txt_Compensacao_MEN.Text = "";
            this.ddlStatus.SelectedIndex = 0;
            hdId.Value = ""; 
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                // SETS
                var prazosService = new WorkflowService();
                PRAZOS prazo = new PRAZOS();

                string NomeDisparo = "";
                int IdPrazo = -1;
                int duracao_AUT = -1, duracao_CEG = -1, duracao_GES = -1, duracao_FEE = -1, duracao_MEN = -1;
                int gatilho_AUT = -1, gatilho_CEG = -1, gatilho_GES = -1, gatilho_FEE = -1, gatilho_MEN = -1;
                int compensacao_AUT = -1, compensacao_CEG = -1, compensacao_GES = -1, compensacao_FEE = -1, compensacao_MEN = -1;
                int ATV = -1;
                DateTime DHC = DateTime.Today;
                int USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()); ;

                // VERIFICAR SE OS CAMPOS FORAM PREENCHIDOS
                bool returnCampoVazio = false;
                if (txt_NomeDisparo.Text != "") { NomeDisparo = txt_NomeDisparo.Text; } else { returnCampoVazio = true; }
                if (txt_Duracao_AUT.Text != "") { duracao_AUT = Convert.ToInt32(txt_Duracao_AUT.Text); } else { returnCampoVazio = true; }
                gatilho_AUT = Convert.ToInt32(ddlGatilho_AUT.Value);
                if (txt_Compensacao_AUT.Text != "") { compensacao_AUT = Convert.ToInt32(txt_Compensacao_AUT.Text); } else { returnCampoVazio = true; }
                if (txt_Duracao_CEG.Text != "") { duracao_CEG = Convert.ToInt32(txt_Duracao_CEG.Text); } else { returnCampoVazio = true; }
                gatilho_CEG = Convert.ToInt32(ddlGatilho_CEG.Value);
                if (txt_Compensacao_CEG.Text != "") { compensacao_CEG = Convert.ToInt32(txt_Compensacao_CEG.Text); } else { returnCampoVazio = true; }
                if (txt_Duracao_GES.Text != "") { duracao_GES = Convert.ToInt32(txt_Duracao_GES.Text); } else { returnCampoVazio = true; }
                gatilho_GES = Convert.ToInt32(ddlGatilho_GES.Value);
                if (txt_Compensacao_GES.Text != "") { compensacao_GES = Convert.ToInt32(txt_Compensacao_GES.Text); } else { returnCampoVazio = true; }
                if (txt_Duracao_FEE.Text != "") { duracao_FEE = Convert.ToInt32(txt_Duracao_FEE.Text); } else { returnCampoVazio = true; }
                gatilho_FEE = Convert.ToInt32(ddlGatilho_FEE.Value);
                if (txt_Compensacao_FEE.Text != "") { compensacao_FEE = Convert.ToInt32(txt_Compensacao_FEE.Text); } else { returnCampoVazio = true; }
                if (txt_Duracao_MEN.Text != "") { duracao_MEN = Convert.ToInt32(txt_Duracao_MEN .Text); } else { returnCampoVazio = true; }
                gatilho_MEN = Convert.ToInt32(ddlGatilho_MEN.Value);
                if (txt_Compensacao_MEN.Text != "") { compensacao_MEN = Convert.ToInt32(txt_Compensacao_MEN.Text); } else { returnCampoVazio = true; }
                ATV = Convert.ToInt32(ddlStatus.SelectedValue);

                if (hdId.Value != "") { IdPrazo = Convert.ToInt32(hdId.Value); }

                if (returnCampoVazio) { MessageBox.Show("Preencha todos os campos", "Erro", TIPO.Warning, MessageBoxHandler); }

                // INSERIR/ATUALIZAR
                prazo.IdPrazo = IdPrazo;
                prazo.NomeDisparo = NomeDisparo;
                prazo.DuracaoAutoAvaliacao = duracao_AUT;
                prazo.GatilhoAutoAvaliacao = gatilho_AUT;
                prazo.CompensadorAutoAvaliacao = compensacao_AUT;
                prazo.DuracaoAvaliacaoAsCegas = duracao_CEG;
                prazo.GatilhoAvaliacaoAsCegas = gatilho_CEG;
                prazo.CompensadorAvaliacaoAsCegas = compensacao_CEG;
                prazo.DuracaoAvaliacaoGestor = duracao_GES;
                prazo.GatilhoAvaliacaoGestor = gatilho_GES;
                prazo.CompensadorAvaliacaoGestor = compensacao_GES;
                prazo.DuracaoFeedback = duracao_FEE;
                prazo.GatilhoFeedback = gatilho_FEE;
                prazo.CompensadorFeedback = compensacao_FEE;
                prazo.DuracaoMentor = duracao_MEN;
                prazo.GatilhoMentor = gatilho_MEN;
                prazo.CompensadorMentor = compensacao_MEN;
                prazo.DHC = DHC;
                prazo.ATV = ATV;
                prazo.USR = USR;

                int resultado = prazosService.CadastrarPrazo(prazo);

                if (resultado == 1)
                {
                    MessageBox.Show("Prazo inserido com sucesso", "Erro", TIPO.Info, MessageBoxHandler);
                    montaListaPrazos();
                }
                else if(resultado == 2)
                {
                    MessageBox.Show("Prazo atualizado com sucesso", "Erro", TIPO.Info, MessageBoxHandler);
                    montaListaPrazos();
                }
                else
                {
                    MessageBox.Show("Erro ao cadastrar o prazo", "Erro", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Erro", TIPO.Warning, MessageBoxHandler);
            }
        }

        public void montaListaPrazos()
        {
            List<PRAZOS> listaPrazos = new WorkflowService().ObterTodosPrazos();

            this.rptPrazos.DataSource = listaPrazos;
            this.rptPrazos.DataBind();
        }

        protected void rptEixo_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AlterarPrazo")
            {
                var id = ((Label)e.Item.FindControl("lblIdPrazo"));
                hdId.Value = Convert.ToString(id.Text);
                MontaCampos(Convert.ToInt32(id.Text));
            }
            else
            {
                var idEixo = ((Label)e.Item.FindControl("lblIdPrazo"));

                var statusExclusao = new EixoService().ExcluirEixo(Convert.ToInt32(idEixo.Text));

                if (statusExclusao)
                {
                    MessageBox.Show("Prazo inativado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                    montaListaPrazos();
                }
                else
                {
                    MessageBox.Show("Erro ao inativar o Prazo !!", "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }

        protected void rptEixo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (((Label)e.Item.FindControl("lblATV")).Text == "1")
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Ativo";
                }
                else
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Inativo";
                    ((Button)e.Item.FindControl("btnDeletar")).Visible = false;
                }
            }
        }
        private void MontaCampos(int idEixo)
        {
            var prazo = new WorkflowService().ObterPrazo(Convert.ToInt32(idEixo));
            // TITULO
            txt_NomeDisparo.Text = prazo.NomeDisparo;
            ddlStatus.SelectedValue = prazo.ATV.ToString();
            // AUTO AVALIAÇÃO
            txt_Duracao_AUT.Text = prazo.DuracaoAutoAvaliacao.ToString();
            ddlGatilho_AUT.SelectedIndex = prazo.GatilhoAutoAvaliacao;
            txt_Compensacao_AUT.Text = prazo.CompensadorAutoAvaliacao.ToString();
            // AVALIAÇÃO AS CEGAS
            txt_Duracao_CEG.Text = prazo.DuracaoAvaliacaoAsCegas.ToString();
            ddlGatilho_CEG.SelectedIndex = prazo.GatilhoAvaliacaoAsCegas;
            txt_Compensacao_CEG.Text = prazo.CompensadorAvaliacaoAsCegas.ToString();
            // AVALIAÇÃO DO GESTOR
            txt_Duracao_GES.Text = prazo.DuracaoAvaliacaoGestor.ToString();
            ddlGatilho_GES.SelectedIndex = prazo.GatilhoAvaliacaoGestor;
            txt_Compensacao_GES.Text = prazo.CompensadorAvaliacaoGestor.ToString();
            // FEEDBACK
            txt_Duracao_FEE.Text = prazo.DuracaoFeedback.ToString();
            ddlGatilho_FEE.SelectedIndex = prazo.GatilhoFeedback;
            txt_Compensacao_FEE.Text = prazo.CompensadorFeedback.ToString();
            // MENTOR
            txt_Duracao_MEN.Text = prazo.DuracaoMentor.ToString();
            ddlGatilho_MEN.SelectedIndex = prazo.GatilhoMentor;
            txt_Compensacao_MEN.Text = prazo.CompensadorMentor.ToString();
        }
    }
}