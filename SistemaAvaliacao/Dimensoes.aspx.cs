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
    public partial class Dimensoes : System.Web.UI.Page
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
            montaComboListaDimensoes();
            ddlStatus.SelectedIndex = 0;
            txtDimensao.Text = "";
            hdId.Value = "";
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            DimensoesService ds = new DimensoesService();
            DIMENSOES dimensao = new DIMENSOES();

            if (txtDimensao.Text != "")
            {
                if (ddlStatus.SelectedItem.Text == "Ativo")
                    dimensao.ATV = 1;
                else
                    dimensao.ATV = 0;

                if (hdId.Value == "")
                {
                    dimensao.Dimensao = txtDimensao.Text;
                    dimensao.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()); ;
                    dimensao.DHC = DateTime.Now;

                    ds.InserirDimensao(dimensao);
                    LimparCampos();
                    MessageBox.Show("Dimensão Inserida com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }
                else
                {
                    dimensao.IdDimensao = Convert.ToInt32(hdId.Value);
                    dimensao.Dimensao = txtDimensao.Text;
                    dimensao.DHC = DateTime.Now;

                    ds.AlterarDimensao(dimensao);
                    LimparCampos();
                    MessageBox.Show("Dimensão alterada  com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }
            }
            else
            {
                MessageBox.Show("Preencha o Campo Dimensão", "", TIPO.Warning, MessageBoxHandler);
            }


        }

        public void montaComboListaDimensoes()
        {
            List<DIMENSOES> dimensao = new DimensoesService().ListaDimensoes();

            this.rptDimensoes.DataSource = dimensao;
            this.rptDimensoes.DataBind();
        }


        protected void rpt_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Alterar")
            {
                var id = ((Label)e.Item.FindControl("lblId"));
                hdId.Value = Convert.ToString(id.Text);
                MontaCampos(Convert.ToInt32(id.Text));
            }
            else
            {
                var id = ((Label)e.Item.FindControl("lblId"));

                var statusExclusao = new DimensoesService().ExcluirDimensao(Convert.ToInt32(id.Text));

                if (statusExclusao)
                {
                    MessageBox.Show("Dimensão Inativada com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                    montaComboListaDimensoes();
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir uma Dimensão  !!", "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }

        protected void rpt_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

        private void MontaCampos(int idDimensao)
        {
            var dimensao = new DimensoesService().ObterDimensao(Convert.ToInt32(idDimensao));
            txtDimensao.Text = dimensao.Dimensao;
            ddlStatus.SelectedValue = dimensao.ATV.ToString();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<DimensoesModelExport> listItems = new List<DimensoesModelExport>();
            var dimensoesService = new DimensoesService();

            var getItems = dimensoesService.ListaDimensoes();

            foreach (var item in getItems)
            {
                DimensoesModelExport addItem = new DimensoesModelExport();
                addItem.IdDimensao = item.IdDimensao;
                addItem.Dimensao = item.Dimensao;
                addItem.TipoAvaliacao = item.TipoAvaliacao;
                addItem.ATV = (int)item.ATV;
                listItems.Add(addItem);
            }

            string fileName = "Dimensões_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

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