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
    public partial class SubCompetencias : System.Web.UI.Page
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
            montaComboListaSubCompetencias();
            txtSubCompetencia.Text = "";
            hdId.Value = "";
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            SubCompetenciasService scs = new SubCompetenciasService();
            SUBCOMPETENCIAS subcompetencia = new SUBCOMPETENCIAS();

            if (txtSubCompetencia.Text != "")
            {

                if (ddlStatus.SelectedItem.Text == "Ativo")
                    subcompetencia.ATV = 1;
                else
                    subcompetencia.ATV = 0;

                if (hdId.Value == "")
                {
                    subcompetencia.SubCompetencia = txtSubCompetencia.Text;
                    subcompetencia.DHC = DateTime.Now;
                    subcompetencia.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()); ;
                    scs.InserirSubCompetencia(subcompetencia);

                    MessageBox.Show("Sub Competência Inserida com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                }
                else
                {
                    subcompetencia.IdSubCompetencia = Convert.ToInt32(hdId.Value);
                    subcompetencia.SubCompetencia = txtSubCompetencia.Text;
                    subcompetencia.DHC = DateTime.Now;
                    subcompetencia.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()); ;

                    scs.AlterarSubCompetencia(subcompetencia);
                    LimparCampos();
                    MessageBox.Show("Sub Competência alterada com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                }

                LimparCampos();
            }
            else
            {
                MessageBox.Show("Preencha o campo SubCompetência", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        public void montaComboListaSubCompetencias()
        {
            List<SUBCOMPETENCIAS> subcompetencias = new SubCompetenciasService().ListaSubCompetencias();

            this.rptSubCompetencias.DataSource = subcompetencias;
            this.rptSubCompetencias.DataBind();
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

                var statusExclusao = new SubCompetenciasService().ExcluirSubCompetencia(Convert.ToInt32(id.Text));

                if (statusExclusao)
                {
                    MessageBox.Show("Sub Competência Inativada com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                    LimparCampos();
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir uma Sub Competência !!", "", TIPO.Warning, MessageBoxHandler);
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

        private void MontaCampos(int idSubCompetencia)
        {
            var subCompetencia = new SubCompetenciasService().ObterCompetencia(idSubCompetencia);
            txtSubCompetencia.Text = subCompetencia.SubCompetencia;
            ddlStatus.SelectedValue = subCompetencia.ATV.ToString();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<SubcompetenciaModelExport> listItems = new List<SubcompetenciaModelExport>();
            var subcompetenciaService = new SubCompetenciasService();

            var getItems = subcompetenciaService.ListaSubCompetencias();

            foreach (var item in getItems)
            {
                SubcompetenciaModelExport addItem = new SubcompetenciaModelExport();
                addItem.IdSubcompetencia = item.IdSubCompetencia;
                addItem.Subcompetencia = item.SubCompetencia;
                addItem.TipoAvaliacao = item.TipoAvaliacao;
                addItem.ATV = (int)item.ATV;
                listItems.Add(addItem);
            }

            string fileName = "Subcompetencias_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

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