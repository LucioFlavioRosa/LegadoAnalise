using AjaxControlToolkit;
using Business.DataAccess;
using Business.Model;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TriaSoftware.Util.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class Performance : System.Web.UI.Page
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
            montaComboCargos();
            montaComboNivel();
            montaComboListaPerformance();
            montaComboNotasPadrao();

            txtPerformance.Text = "";
            txtAbaixo.Text = "";
            txtAcima.Text = "";
            txtEsperado.Text = "";
            hdId.Value = "";
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            PerformancesService ps = new PerformancesService();
            PERFORMANCES performance = new PERFORMANCES();

            if (ddlCargo.SelectedIndex != 0)
            {
                if (txtPerformance.Text != "")
                {
                    if (txtAbaixo.Text != "")
                    {
                        if (txtEsperado.Text != "")
                        {
                            if (txtAcima.Text != "")
                            {
                                if (ddlStatus.SelectedItem.Text == "Ativo")
                                    performance.ATV = 1;
                                else
                                    performance.ATV = 0;

                                performance.Performance = txtPerformance.Text;
                                performance.PerformanceAbaixo = txtAbaixo.Text;
                                performance.PerformanceEsperado = txtEsperado.Text;
                                performance.PerformanceAcima = txtAcima.Text;
                                performance.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
                                performance.IdCargo = Convert.ToInt32(this.ddlCargo.SelectedValue);
                                performance.IdNivel = 1;
                                performance.ATV = 1;
                                performance.DHC = DateTime.Now;
                                performance.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                performance.Abrangencia = ddlAbrangencia.SelectedItem.Text;

                                // INPUTS
                                string checkCboxAutoAvaliacao = Request.Form["chboxInputAutoAvaliacao"];
                                string checkCboxAvaliacaoAsCegas = Request.Form["chboxInputAvaliacaoAsCegas"];
                                string checkCboxAvaliacaoGestor = Request.Form["chboxInputAvaliacaoGestor"];

                                performance.InputAutoavaliacao = checkCboxAutoAvaliacao == "on" ? true : false;
                                if (!performance.InputAutoavaliacao)
                                {
                                    if (ddlNotaPadraoAutoAvaliacao.SelectedIndex != 0)
                                    {
                                        performance.NotaPadraoAutoAvaliacao = int.Parse(ddlNotaPadraoAutoAvaliacao.SelectedValue);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Selecione a nota padrão para auto avaliação", "", TIPO.Warning, MessageBoxHandler);
                                        return;
                                    }
                                }
                                performance.InputAvaliacaoAsCegas = checkCboxAvaliacaoAsCegas == "on" ? true : false;
                                if (!performance.InputAvaliacaoAsCegas)
                                {
                                    if (ddlNotaPadraoAvaliacaoAsCegas.SelectedIndex != 0)
                                    {
                                        performance.NotaPadraoAvaliacaoAsCegas = int.Parse(ddlNotaPadraoAvaliacaoAsCegas.SelectedValue);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Selecione a nota padrão para avaliação as cegas", "", TIPO.Warning, MessageBoxHandler);
                                        return;
                                    }
                                }
                                performance.InputAvaliacaoGestor = checkCboxAvaliacaoGestor == "on" ? true : false;
                                if (!performance.InputAvaliacaoGestor)
                                {
                                    if (ddlNotaPadraoAvaliacaoAsGestor.SelectedIndex != 0)
                                    {
                                        performance.NotaPadraoAvaliacaoGestor = int.Parse(ddlNotaPadraoAvaliacaoAsGestor.SelectedValue);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Selecione a nota padrão para avaliação do gestor", "", TIPO.Warning, MessageBoxHandler);
                                        return;
                                    }
                                }


                                if (hdId.Value == "")
                                {
                                    ps.InserirPerformance(performance);
                                    LimparCampos();
                                    MessageBox.Show("Performance Inserida com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                                }
                                else
                                {
                                    performance.IdPerformance = Convert.ToInt32(hdId.Value);

                                    ps.AlterarPerformance(performance);
                                    LimparCampos();
                                    MessageBox.Show("Performance Alterada com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                                }
                            }
                            else
                            {
                                MessageBox.Show("Preencha o campo Performance Acma", "", TIPO.Warning, MessageBoxHandler);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Preencha o campo Performance Esperado", "", TIPO.Warning, MessageBoxHandler);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Preencha o campo Performance Abaixo", "", TIPO.Warning, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Preencha o campo Performance", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            else
            {
                MessageBox.Show("Selecione o campo Cargo", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        public void montaComboCargos()
        {
            List<CARGOS> cargos = new CargosService().ObterListaCargos(true);
            this.ddlCargo.DataValueField = "IdCargo";
            this.ddlCargo.DataTextField = "Cargo";
            this.ddlCargo.DataSource = cargos;
            this.ddlCargo.DataBind();
            this.ddlCargo.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboNivel()
        {
            //List<CARGOSNIVEIS> cargosniveis = new CargosNiveisService().ObterListaNiveis();
            //this.ddlNivel.DataValueField = "IdNivel";
            //this.ddlNivel.DataTextField = "Nivel";
            //this.ddlNivel.DataSource = cargosniveis;
            //this.ddlNivel.DataBind();
            //this.ddlNivel.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboListaPerformance()
        {
            List<PERFORMANCES> performances = new PerformancesService().ObterListaPerformances();

            this.rptPerformance.DataSource = performances;
            this.rptPerformance.DataBind();
        }

        public void montaComboNotasPadrao()
        {
            List<AVALIACOESPERFORMANCESNOTAS> notasPerformance = new NotasAvaliacaoService().ListaNotasPerformances();

            this.ddlNotaPadraoAutoAvaliacao.DataValueField = "IdNota";
            this.ddlNotaPadraoAutoAvaliacao.DataTextField = "CodigoNota";
            this.ddlNotaPadraoAutoAvaliacao.DataSource = notasPerformance;
            this.ddlNotaPadraoAutoAvaliacao.DataBind();
            this.ddlNotaPadraoAutoAvaliacao.Items.Insert(0, "[Selecionar]");

            this.ddlNotaPadraoAvaliacaoAsCegas.DataValueField = "IdNota";
            this.ddlNotaPadraoAvaliacaoAsCegas.DataTextField = "CodigoNota";
            this.ddlNotaPadraoAvaliacaoAsCegas.DataSource = notasPerformance;
            this.ddlNotaPadraoAvaliacaoAsCegas.DataBind();
            this.ddlNotaPadraoAvaliacaoAsCegas.Items.Insert(0, "[Selecionar]");

            this.ddlNotaPadraoAvaliacaoAsGestor.DataValueField = "IdNota";
            this.ddlNotaPadraoAvaliacaoAsGestor.DataTextField = "CodigoNota";
            this.ddlNotaPadraoAvaliacaoAsGestor.DataSource = notasPerformance;
            this.ddlNotaPadraoAvaliacaoAsGestor.DataBind();
            this.ddlNotaPadraoAvaliacaoAsGestor.Items.Insert(0, "[Selecionar]");
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

                PerformancesService ps = new PerformancesService();
                var statusExclusao = ps.ExcuirPerformance(Convert.ToInt32(id.Text));

                if (statusExclusao)
                {
                    LimparCampos();
                    MessageBox.Show("Performance Inativada com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir uma Performance !!", "", TIPO.Warning, MessageBoxHandler);
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


        private void MontaCampos(int idPerformance)
        {
            var performance = new PerformancesService().ObterPerformance(Convert.ToInt32(idPerformance));
            ddlCargo.SelectedValue = performance.IdCargo.ToString();
            //ddlNivel.SelectedValue = performance.IdNivel.ToString();
            ddlStatus.SelectedValue = performance.ATV.ToString();
            txtPerformance.Text = performance.Performance;
            txtAbaixo.Text = performance.PerformanceAbaixo;
            txtEsperado.Text = performance.PerformanceEsperado;
            txtAcima.Text = performance.PerformanceAcima;
            ddlAbrangencia.SelectedIndex = ddlAbrangencia.Items.IndexOf(ddlAbrangencia.Items.FindByText(performance.Abrangencia));

            // INPUT
            var valorInputAutoAvaliacao = performance.InputAutoavaliacao ? "1" : "0";
            var valorInputAvaliacaoAsCegas = performance.InputAvaliacaoAsCegas ? "1" : "0";
            var valorInputAvaliacaoGestor = performance.InputAvaliacaoGestor ? "1" : "0";
            ClientScript.RegisterStartupScript(this.GetType(), "CallValorCheckbox", "javascript:ValorCheckbox(" + valorInputAutoAvaliacao + ", " +
                valorInputAvaliacaoAsCegas + ", " + valorInputAvaliacaoGestor + ");", true);
            if (!performance.InputAutoavaliacao)
            {
                ddlNotaPadraoAutoAvaliacao.SelectedIndex = ddlNotaPadraoAutoAvaliacao.Items.IndexOf(ddlNotaPadraoAutoAvaliacao.Items.FindByValue(performance.NotaPadraoAutoAvaliacao.ToString()));
            }
            if (!performance.InputAvaliacaoAsCegas)
            {
                ddlNotaPadraoAvaliacaoAsCegas.SelectedIndex = ddlNotaPadraoAvaliacaoAsCegas.Items.IndexOf(ddlNotaPadraoAvaliacaoAsCegas.Items.FindByValue(performance.NotaPadraoAvaliacaoAsCegas.ToString()));
            }
            if (!performance.InputAvaliacaoGestor)
            {
                ddlNotaPadraoAvaliacaoAsGestor.SelectedIndex = ddlNotaPadraoAvaliacaoAsGestor.Items.IndexOf(ddlNotaPadraoAvaliacaoAsGestor.Items.FindByValue(performance.NotaPadraoAvaliacaoGestor.ToString()));
            }
        }

        protected void ddlNotaPadraoAutoAvaliacao_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<PerformanceModelExport> listItems = new List<PerformanceModelExport>();
            var performancesService = new PerformancesService();
            var cargoService = new CargosService();
            var notasService = new NotasAvaliacaoService();

            var getItems = performancesService.ObterListaPerformances();

            foreach (var item in getItems)
            {
                PerformanceModelExport addItem = new PerformanceModelExport();
                addItem.IdPerformance = item.IdPerformance;
                addItem.IdCargo = item.IdCargo;
                addItem.Cargo = cargoService.ObterCargo(item.IdCargo).Cargo;
                addItem.Performance = item.Performance;
                addItem.DescricaoAbaixo = item.PerformanceAbaixo;
                addItem.DescricaoEsperado = item.PerformanceEsperado;
                addItem.DescricaoAcima = item.PerformanceAcima;
                addItem.ATV = (int)item.ATV;
                addItem.Abrangencia = item.Abrangencia;
                addItem.InputAutoAvaliacao = item.InputAutoavaliacao ? 1 : 0;
                addItem.IdNotaPadraoAutoAvaliacao = item.NotaPadraoAutoAvaliacao;
                addItem.NotaPadraoAutoAvaliacao = item.NotaPadraoAutoAvaliacao == null ? "" : notasService.ObterNotaPerformance((int)item.NotaPadraoAutoAvaliacao).CodigoNota;
                addItem.InputAvaliacaoAsCegas = item.InputAvaliacaoAsCegas ? 1 : 0;
                addItem.IdNotaPadraoAvaliacaoAsCegas = item.NotaPadraoAvaliacaoAsCegas;
                addItem.NotaPadraoAvaliacaoAsCegas = item.NotaPadraoAvaliacaoAsCegas == null ? "" : notasService.ObterNotaPerformance((int)item.NotaPadraoAvaliacaoAsCegas).CodigoNota;
                addItem.InputAvaliacaoGestor = item.InputAvaliacaoGestor ? 1 : 0;
                addItem.IdNotaPadraoAvaliacaoGestor = item.NotaPadraoAvaliacaoGestor;
                addItem.NotaPadraoAvaliacaoGestor = item.NotaPadraoAvaliacaoGestor == null ? "" : notasService.ObterNotaPerformance((int)item.NotaPadraoAvaliacaoGestor).CodigoNota;
                listItems.Add(addItem);
            }

            string fileName = "Performance_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

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

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUpload.HasFile)
                {
                    // LER ARQUIVO DIRETO DA MEMÓRIA USANDO EPPlus
                    using (var package = new OfficeOpenXml.ExcelPackage(fileUpload.FileContent))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        var dt = new DataTable();
                        bool hasHeader = true; // Assume que a primeira linha é o cabeçalho
                        foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                        {
                            dt.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
                        }
                        var startRow = hasHeader ? 2 : 1;
                        for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                        {
                            var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                            DataRow row = dt.NewRow();
                            foreach (var cell in wsRow)
                            {
                                row[cell.Start.Column - 1] = cell.Text;
                            }
                            dt.Rows.Add(row);
                        }

                        // LER E ATUALIZAR TABELA
                        var performanceService = new PerformancesService();
                        int somaLinhasDesconsideradas = 0, somaLinhasInseridas = 0, somaLinhasAlteradas = 0;
                        foreach (DataRow item in dt.Rows)
                        {
                            int IdPerformance = !string.IsNullOrWhiteSpace(item[0]?.ToString()) ? Convert.ToInt32(item[0]) : 0;
                            int IdCargo = !string.IsNullOrWhiteSpace(item[1]?.ToString()) ? Convert.ToInt32(item[1]) : 0;
                            string Performance = !string.IsNullOrWhiteSpace(item[3]?.ToString()) ? item[3].ToString() : "-";
                            string DescricaoAbaixo = !string.IsNullOrWhiteSpace(item[4]?.ToString()) ? item[4].ToString() : "-";
                            string DescricaoEsperado = !string.IsNullOrWhiteSpace(item[5]?.ToString()) ? item[5].ToString() : "-";
                            string DescricaoAcima = !string.IsNullOrWhiteSpace(item[6]?.ToString()) ? item[6].ToString() : "-";
                            int ATV = !string.IsNullOrWhiteSpace(item[7]?.ToString()) ? Convert.ToInt32(item[7]) : 1;
                            string Abrangencia = !string.IsNullOrWhiteSpace(item[8]?.ToString()) ? item[8].ToString() : "-";
                            int InputAutoAvaliacao = !string.IsNullOrWhiteSpace(item[9]?.ToString()) ? Convert.ToInt32(item[9]) : 1;
                            int? IdNotaPadraoAutoAvaliacao = !string.IsNullOrWhiteSpace(item[10]?.ToString()) ? (int?)Convert.ToInt32(item[10]) : null;
                            int InputAvaliacaoAsCegas = !string.IsNullOrWhiteSpace(item[12]?.ToString()) ? Convert.ToInt32(item[12]) : 1;
                            int? IdNotaAvaliacaoAsCegas = !string.IsNullOrWhiteSpace(item[13]?.ToString()) ? (int?)Convert.ToInt32(item[13]) : null;
                            int InputAvaliacaoGestor = !string.IsNullOrWhiteSpace(item[15]?.ToString()) ? Convert.ToInt32(item[15]) : 1;
                            int? IdNotaPadraoAvaliacaoGestor = !string.IsNullOrWhiteSpace(item[16]?.ToString()) ? (int?)Convert.ToInt32(item[16]) : null;
                            int IdEmpresa = 1;
                            int IdNivel = 1;

                            if (Performance != "-" && IdCargo > 0 && DescricaoAbaixo != "-" && DescricaoEsperado != "-" && DescricaoAcima != "-" && Abrangencia != "-")
                            {
                                PERFORMANCES importItem = new PERFORMANCES();
                                importItem.IdEmpresa = IdEmpresa;
                                importItem.IdCargo = IdCargo;
                                importItem.IdNivel = IdNivel;
                                importItem.Performance = Performance;
                                importItem.PerformanceAbaixo = DescricaoAbaixo;
                                importItem.PerformanceEsperado = DescricaoEsperado;
                                importItem.PerformanceAcima = DescricaoAcima;
                                importItem.Abrangencia = Abrangencia;
                                importItem.InputAutoavaliacao = InputAutoAvaliacao == 1 ? true : false;
                                importItem.NotaPadraoAutoAvaliacao = IdNotaPadraoAutoAvaliacao;
                                importItem.InputAvaliacaoAsCegas = InputAvaliacaoAsCegas == 1 ? true : false;
                                importItem.NotaPadraoAvaliacaoAsCegas = IdNotaAvaliacaoAsCegas;
                                importItem.InputAvaliacaoGestor = InputAvaliacaoGestor == 1 ? true : false;
                                importItem.NotaPadraoAvaliacaoGestor = IdNotaPadraoAvaliacaoGestor;
                                importItem.ATV = ATV;
                                importItem.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                importItem.DHC = DateTime.Now;

                                if (IdPerformance == 0)
                                {
                                    performanceService.InserirPerformance(importItem);
                                    somaLinhasInseridas += 1;
                                }
                                else
                                {
                                    importItem.IdPerformance = IdPerformance;
                                    performanceService.AlterarPerformance(importItem);
                                    somaLinhasAlteradas += 1;
                                }
                            }
                            else
                            {
                                somaLinhasDesconsideradas += 1;
                            }
                        }

                        MessageBox.Show("Performances importadas com sucesso<br>Inseridas: " + somaLinhasInseridas.ToString() + "<br>Alteradas: " + somaLinhasAlteradas.ToString() +
                            "<br>Desconsideradas: " + somaLinhasDesconsideradas.ToString(), "", TIPO.Info, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo selecionado", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }
    }
}