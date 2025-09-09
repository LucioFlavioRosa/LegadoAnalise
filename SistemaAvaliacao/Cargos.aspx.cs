using AjaxControlToolkit;
using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Business.Model;
using System.IO;
using TriaSoftware.Util.Framework.Domain.Service;
using System.Web;
using System.Configuration;
using System.Data.OleDb;

namespace SistemaAvaliacao
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                montaComboListaCargos();
                montaComboProximoCargo();
            }
        }

        #region MONTAR COMBOS
        public void montaComboProximoCargo()
        {
            List<CARGOS> cargos = new CargosService().ObterListaCargos(true);

            this.ddlProximocargo.DataSource = cargos;
            this.ddlProximocargo.DataValueField = "IdCargo";
            this.ddlProximocargo.DataTextField = "Cargo";
            this.ddlProximocargo.DataBind();
            this.ddlProximocargo.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboListaCargos()
        {
            List<CARGOS> cargos = new CargosService().ObterListaCargos();

            this.rptCargos.DataSource = cargos;
            this.rptCargos.DataBind();
        }
        #endregion

        #region LER CARGO
        private void MontaCamposCargos(int idCargo)
        {
            var cargo = new CargosService().ObterCargo(Convert.ToInt32(idCargo));
            txtCargo.Text = cargo.Cargo;
            ddlStatus.SelectedValue = cargo.ATV.ToString();

            if (cargo.idProximoCargo.HasValue)
            {
                ddlProximocargo.SelectedValue = cargo.idProximoCargo.ToString();
            }
            else
            {
                ddlProximocargo.SelectedIndex = 0;
            }

            // TEMPO PROMOÇÃO
            txtTempoMinimo.Text = cargo.TempoMinimoPromocao.ToString();

            // CAMPOS DE DESCRIÇÃO
            txtFuncao.Text = cargo.Funcao;
            txtAutonomia.Text = cargo.Autonomia;
            txtEscopoAtuacao.Text = cargo.EscopoDeAtuacao;
            txtNivelInterlocucao.Text = cargo.NivelInterlocucao;
        }
        #endregion

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            CargosService cs = new CargosService();
            CARGOS cargo = new CARGOS();
            cargo.Cargo = txtCargo.Text;

            cargo.DHC = DateTime.Now;

            if (txtCargo.Text != "")
            {

                if (ddlStatus.SelectedItem.Text == "Ativo")
                    cargo.ATV = 1;
                else
                    cargo.ATV = 0;


                if (ddlProximocargo.SelectedIndex > 0)
                {
                    cargo.idProximoCargo = Convert.ToInt32(ddlProximocargo.SelectedValue.ToString());
                }

                int tempoMinimo = 12;
                if (txtTempoMinimo.Text != "" && txtTempoMinimo.Text != null)
                {
                    if (int.TryParse(txtTempoMinimo.Text, out int parsedTempoMinimo))
                    {
                        tempoMinimo = parsedTempoMinimo;
                    }
                }
                cargo.TempoMinimoPromocao = tempoMinimo;

                // CAMPOS DESCRIÇÕES
                cargo.Funcao = txtFuncao.Text;
                cargo.Autonomia = txtAutonomia.Text;
                cargo.EscopoDeAtuacao = txtEscopoAtuacao.Text;
                cargo.NivelInterlocucao = txtNivelInterlocucao.Text;

                if (hdIdCargo.Value == "")
                {
                    cs.InserirCargo(cargo);
                    montaComboListaCargos();
                    montaComboProximoCargo();
                    txtCargo.Text = "";
                    hdIdCargo.Value = "";
                    ddlProximocargo.SelectedIndex = 0;
                    MessageBox.Show("Cargo Inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                }
                else
                {
                    cargo.IdCargo = Convert.ToInt32(hdIdCargo.Value);
                    cargo.Cargo = txtCargo.Text;

                    cs.AlterarCargo(cargo);
                    montaComboListaCargos();
                    montaComboProximoCargo();
                    txtCargo.Text = "";
                    hdIdCargo.Value = "";
                    ddlProximocargo.SelectedIndex = 0;
                    MessageBox.Show("Cargo alterado com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                }
            }
            else
            {
                MessageBox.Show("Favor Preencher o Cargo", "", TIPO.Info, MessageBoxHandler);
            }

        }

        protected void rptCargos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AlterarCargo")
            {
                var idCargo = ((Label)e.Item.FindControl("lblIdCargo"));
                hdIdCargo.Value = Convert.ToString(idCargo.Text);
                MontaCamposCargos(Convert.ToInt32(idCargo.Text));

            }
            else
            {
                var idCargo = ((Label)e.Item.FindControl("lblIdCargo"));

                var associado = new CargosService().ObterCargo(Convert.ToInt32(idCargo.Text));

                CargosService cargoservice = new CargosService();
                var statusExclusao = cargoservice.ExcluirCargo(Convert.ToInt32(idCargo.Text));

                if (statusExclusao)
                {
                    MessageBox.Show("Cargo Inativado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                    montaComboListaCargos();
                    montaComboProximoCargo();
                    hdIdCargo.Value = "";
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir um Cargo !!", "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }

        protected void rptCargos_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<CargosModelExport> listItems = new List<CargosModelExport>();
            var cargoService = new CargosService();

            var getItems = cargoService.ObterListaCargos();

            foreach (var item in getItems)
            {
                CargosModelExport addItem = new CargosModelExport();
                addItem.IdCargo = item.IdCargo;
                addItem.Cargo = item.Cargo;
                addItem.IdProximoCargo = item.idProximoCargo != null ? (int)item.idProximoCargo : 0;
                addItem.ProximoCargo = item.idProximoCargo != null ? cargoService.ObterCargo((int)item.idProximoCargo).Cargo : "";
                addItem.TempoMinimoPromocao = item.TempoMinimoPromocao;
                addItem.Funcao = item.Funcao;
                addItem.Autonomia = item.Autonomia;
                addItem.EscopoDeAtuacao = item.EscopoDeAtuacao;
                addItem.NivelInterlocucao = item.NivelInterlocucao;
                addItem.ATV = (int)item.ATV;
                listItems.Add(addItem);
            }

            string fileName = "Cargos_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

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