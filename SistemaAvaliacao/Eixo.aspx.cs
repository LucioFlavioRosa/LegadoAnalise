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
    public partial class Eixo : System.Web.UI.Page
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
            montaComboListaEixo();
            txtEixo.Text = "";
            this.ddlStatus.SelectedIndex = 0;
            hdId.Value = "";
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            EixoService eixos = new EixoService();
            EIXOS eixo = new EIXOS();

            try
            {
                if (txtEixo.Text != "")
                {
                    if (ddlStatus.SelectedValue == "1")
                        eixo.ATV = 1;
                    else
                        eixo.ATV = 0;

                    if (hdId.Value == "")
                    {
                        eixo.Eixo = txtEixo.Text;
                        eixo.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()); ;
                        eixo.DHC = DateTime.Now;
                        eixo.TipoAvaliacao = "desempenho";
                        eixos.InserirEixo(eixo);
                        LimparCampos();
                    }
                    else
                    {
                        eixo.IdEixo = Convert.ToInt32(hdId.Value);
                        eixo.Eixo = txtEixo.Text;
                        eixo.DHC = DateTime.Now;
                        eixos.AlterarEixo(eixo);
                        LimparCampos();

                    }
                }
                else
                {
                    MessageBox.Show("Preencha o Campo Eixo", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Preencha todos os campos", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        public void montaComboListaEixo()
        {
            List<EIXOS> eixo = new EixoService().ListaEixos();

            this.rptEixo.DataSource = eixo;
            this.rptEixo.DataBind();
        }


        protected void rptEixo_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AlterarEixo")
            {
                var id = ((Label)e.Item.FindControl("lblIdEixo"));
                hdId.Value = Convert.ToString(id.Text);
                MontaCampos(Convert.ToInt32(id.Text));
            }
            else
            {
                var idEixo = ((Label)e.Item.FindControl("lblIdEixo"));

                var statusExclusao = new EixoService().ExcluirEixo(Convert.ToInt32(idEixo.Text));

                if (statusExclusao)
                {
                    MessageBox.Show("Eixo Inativado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                    montaComboListaEixo();
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir um Eixo !!", "", TIPO.Warning, MessageBoxHandler);
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
            var eixo = new EixoService().ObterEixo(Convert.ToInt32(idEixo));
            txtEixo.Text = eixo.Eixo;
            ddlStatus.SelectedValue = eixo.ATV.ToString();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<EixosModelExport> listItems = new List<EixosModelExport>();
            var eixosService = new EixoService();

            var getItems = eixosService.ListaEixos();

            foreach (var item in getItems)
            {
                EixosModelExport addItem = new EixosModelExport();
                addItem.IdEixo = item.IdEixo;
                addItem.Eixo = item.Eixo;
                addItem.TipoAvaliacao = item.TipoAvaliacao;
                addItem.ATV = (int)item.ATV;
                listItems.Add(addItem);
            }

            string fileName = "Eixos_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            //Gera arquivo
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listItems);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
    }
}