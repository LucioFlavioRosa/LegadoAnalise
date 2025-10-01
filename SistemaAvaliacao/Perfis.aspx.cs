using AjaxControlToolkit;
using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class Perfis : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { 
                montaComboListaPerfis();
            }
        }

        public void montaComboListaPerfis()
        {
            List<PERFIS> perfis = new PerfisService().ObterListaPerfis();

            this.rptPerfis.DataSource = perfis;
            this.rptPerfis.DataBind();
        }

        public void btnCadastrar_Click(object sender, EventArgs e)
        {
            PerfisService cs = new PerfisService();
            PERFIS perfil = new PERFIS();
            perfil.Perfil = txtPerfil.Text;
            perfil.DHC = DateTime.Now;

            if (txtPerfil.Text != "")
            {
                if (ddlStatus.SelectedItem.Text == "Ativo")
                    perfil.ATV = 1;
                else
                    perfil.ATV = 0;

                if (hdIdPerfil.Value == "")
                {
                    cs.InserirPerfil(perfil);
                    montaComboListaPerfis();
                    txtPerfil.Text = "";
                    hdIdPerfil.Value = "";
                    MessageBox.Show("Perfil Inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }
                else
                {
                    perfil.IdPerfil= Convert.ToInt32(hdIdPerfil.Value);
                    perfil.Perfil = txtPerfil.Text;

                    cs.AlterarPerfil(perfil);
                    montaComboListaPerfis();
                    txtPerfil.Text = "";
                    hdIdPerfil.Value = "";
                    MessageBox.Show("Perfil alterado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }



            }
            else
            {
                MessageBox.Show("Preencha o campo Perfil", "", TIPO.Warning, MessageBoxHandler);
            }



            
        }




        protected void rptPerfis_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AlterarPerfil")
            {
                var idPerfil = ((Label)e.Item.FindControl("lblIdPerfil"));
                hdIdPerfil.Value = Convert.ToString(idPerfil.Text);

                MontaCamposPerfis(Convert.ToInt32(idPerfil.Text));
            }
            else
            {
                var idPerfil = ((Label)e.Item.FindControl("lblIdPerfil"));

                
                PerfisService perfisservice = new PerfisService();
                var statusExclusao = perfisservice.ExcluirPerfil(Convert.ToInt32(idPerfil.Text));

                if (statusExclusao)
                {
                    montaComboListaPerfis();
                    hdIdPerfil.Value = "";
                    MessageBox.Show("Perfil Inativado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir um Perfil !!", "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }

        protected void rptPerfis_ItemDataBound(object sender, RepeaterItemEventArgs e)
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




        private void MontaCamposPerfis(int idPerfil)
        {
            PERFIS perfilInfo = new PERFIS()
            {
                IdPerfil = idPerfil
            };

            var perfil = new PerfisService().ObterPerfil(perfilInfo);
            txtPerfil.Text = perfil.Perfil;
            ddlStatus.SelectedValue = perfil.ATV.ToString();
        }




    }
}