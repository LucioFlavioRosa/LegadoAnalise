using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class TiposProjetos : System.Web.UI.Page
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
            montaComboListaTipos();
            txtTipoProjeto.Text = "";
            hdIdTipoProjeto.Value = "";
        }


        public void montaComboListaTipos()
        {
            List<PROJETOSTIPOS> tipos = new TipoProjetoService().ObterListaTipos();

            this.rptTiposProjetos.DataSource = tipos;
            this.rptTiposProjetos.DataBind();
        }


        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            TipoProjetoService tps = new TipoProjetoService();
            PROJETOSTIPOS tipoProjeto = new PROJETOSTIPOS();
            tipoProjeto.ProjetoTipo = txtTipoProjeto.Text;
            tipoProjeto.DHC = DateTime.Now;


            if (txtTipoProjeto.Text != "")
            {
                
                if (ddlStatus.SelectedItem.Text == "Ativo")
                    tipoProjeto.ATV = 1;
                else
                    tipoProjeto.ATV = 0;


                if (hdIdTipoProjeto.Value == "")
                {
                    tps.InserirTipoProjeto(tipoProjeto);
                    LimparCampos();
                    MessageBox.Show("Tipo de Projeto Inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                }
                else
                {
                    tipoProjeto.IdTipo = Convert.ToInt32(hdIdTipoProjeto.Value);

                    tps.AlterarTipoProjeto(tipoProjeto);
                    LimparCampos();
                    MessageBox.Show("Tipo de projeto alterado com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                }

            }
            else
            {
                MessageBox.Show("Preencha o campo Tipo de Projeto", "", TIPO.Warning, MessageBoxHandler);
            }
        }



        protected void rptTiposProjetos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AlterarTipoProjeto")
            {
                var idTipoProjeto = ((Label)e.Item.FindControl("lblIdTipoProjeto"));
                hdIdTipoProjeto.Value = Convert.ToString(idTipoProjeto.Text);

                MontaCamposTiposProjetos(Convert.ToInt32(idTipoProjeto.Text));
            }
            else
            {
                var idTipoProjeto = ((Label)e.Item.FindControl("lblIdTipoProjeto"));               
                
                var statusExclusao = new TipoProjetoService().ExcluirTipoProjeto(Convert.ToInt32(idTipoProjeto.Text));

                if (statusExclusao)
                {
                    LimparCampos();
                    MessageBox.Show("Tipo de Projeto Inativado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir um Tipo de Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }

        protected void rptTiposProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
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





        private void MontaCamposTiposProjetos(int idTipoProjeto)
        {
            var tipoProjeto = new TipoProjetoService().ObterTipoProjeto(Convert.ToInt32(idTipoProjeto));
            txtTipoProjeto.Text = tipoProjeto.ProjetoTipo;
            ddlStatus.SelectedValue = tipoProjeto.ATV.ToString();
        }
    }
}