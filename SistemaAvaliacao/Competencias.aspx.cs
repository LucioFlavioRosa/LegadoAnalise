using AjaxControlToolkit;
using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Business.Model;
using System.IO;
using TriaSoftware.Util.Framework.Domain.Service;
using System.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
/**/
namespace SistemaAvaliacao
{
    public partial class Competencias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LimparCampos();

                divDesempenho.Visible = false;
                divLideranca.Visible = false;
            }
        }

        private void LimparCampos()
        {
            montaComboListaCompetencias();
            montaComboCargos();
            montaComboNiveis();
            montaComboEixos();
            montaComboSubCompetencias();
            montaComboDimensoes();
            montaComboTipoAvaliacoes();
            montaComboEscopo();
            montaComboNotasPadrao();
            montaComboModosCalculos();

            txtJunior.Text = "";
            txtDetalhamentoJr.Value = "";
            txtPleno.Text = "";
            txtDetalhamentoPleno.Value = "";
            textDetalheLideranca.Text = "";
            txtPalavrasChave.Text = "";
            txtRelacaoSubcompetencia.Text = "";
            hdId.Value = "";
            cboxInputAutoAv.Checked = true;
            cboxInputAvCegas.Checked = true;
            cboxInputAvGestor.Checked = true;
            cboxInputFeedback.Checked = true;
            cboxInputNivel1.Checked = true;
            cboxInputNivel2.Checked = true;
            cboxVisivelAutoAv.Checked = true;
            cboxVisivelAvCegas.Checked = true;
            cboxVisivelAvGestor.Checked = true;
            cboxVisivelFeedback.Checked = true;
            cboxVisivelNivel1.Checked = true;
            cboxVisivelNivel2.Checked = true;
            btnAgregar.Visible = false;
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            CompetenciasService cs = new CompetenciasService();
            COMPETENCIAS competencia = new COMPETENCIAS();

            try
            {

                if (ddlTipoAvaliacao.SelectedIndex == 1)
                {
                    if (ddlCargo.SelectedIndex != 0)
                    {
                        if (ddlEixo.SelectedIndex != 0)
                        {
                            if (ddlSubCompetencia.SelectedIndex != 0)
                            {
                                if (ddlDimensao.SelectedIndex != 0)
                                {
                                    if (txtDetalhamentoJr.Value != "")
                                    {
                                        if (txtJunior.Text != "")
                                        {
                                            if (txtDetalhamentoPleno.Value != "" || 1 == 1) // DETALHAMENTO DE PRÓXIMO NÍVEL DESCONTINUADO
                                            {
                                                if (txtPleno.Text != "" || 1 == 1) // NIVEL PLENO OBSOLETO
                                                {
                                                    competencia.CompetenciaJR = txtJunior.Text;
                                                    competencia.CompetenciaPL = "Não aplicável";
                                                    competencia.CompetenciaSR = "Não aplicável";
                                                    competencia.CompetenciaJRDetalhe = txtDetalhamentoJr.Value;
                                                    competencia.CompetenciaPLDetalhe = txtDetalhamentoPleno.Value;
                                                    competencia.CompetenciaSRDetalhe = "Não informado - Nova matriz de cargos";
                                                    competencia.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
                                                    competencia.IdCargo = Convert.ToInt16(ddlCargo.SelectedItem.Value);
                                                    competencia.IdNivel = 1;
                                                    competencia.IdEixo = Convert.ToInt16(ddlEixo.SelectedItem.Value);
                                                    competencia.IdSubCompetencia = Convert.ToInt16(ddlSubCompetencia.SelectedItem.Value);
                                                    competencia.IdDimensao = Convert.ToInt16(ddlDimensao.SelectedItem.Value);
                                                    competencia.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()); ;
                                                    competencia.DHC = DateTime.Now;
                                                    competencia.ATV = 1;
                                                    competencia.TipoAvaliacao = "desempenho";
                                                    competencia.Escopo = "projeto";
                                                    competencia.PalavrasChave = txtPalavrasChave.Text;

                                                    // REGRAS DE AUTO PREENCHIMENTO
                                                    competencia.InputAutoAvaliacao = cboxInputAutoAv.Checked;
                                                    competencia.InputAvaliacaoAsCegas = cboxInputAvCegas.Checked;
                                                    competencia.InputAvaliacaoGestor = cboxInputAvGestor.Checked;
                                                    competencia.InputFeedback = cboxInputFeedback.Checked;
                                                    competencia.InputNivel1 = cboxInputNivel1.Checked;
                                                    competencia.InputNivel2 = cboxInputNivel2.Checked;
                                                    competencia.VisivelAutoAvaliacao = cboxVisivelAutoAv.Checked;
                                                    competencia.VisivelAvaliacaoAsCegas = cboxVisivelAvCegas.Checked;
                                                    competencia.VisivelAvaliacaoGestor = cboxVisivelAvGestor.Checked;
                                                    competencia.VisivelFeedback = cboxVisivelFeedback.Checked;
                                                    competencia.VisivelNivel1 = cboxVisivelNivel1.Checked;
                                                    competencia.VisivelNivel2 = cboxVisivelNivel2.Checked;
                                                    competencia.IdNotaPadraoNivel1 = int.Parse(ddlNotaPadraoNivel1.SelectedItem.Value);
                                                    competencia.IdNotaPadraoNivel2 = int.Parse(ddlNotaPadraoNivel2.SelectedItem.Value);
                                                    competencia.IdModo = int.Parse(ddlModoCalculo.SelectedItem.Value);

                                                    // RELAÇÃO CARGOxSUBCOMPETENCIA
                                                    RELACAO_CARGO_SUBCOMPETENCIA relacaoCargoSub = new RELACAO_CARGO_SUBCOMPETENCIA();
                                                    relacaoCargoSub.idCargo = competencia.IdCargo;
                                                    relacaoCargoSub.idSubcompetencia = competencia.IdSubCompetencia;
                                                    relacaoCargoSub.Descricao = txtRelacaoSubcompetencia.Text;

                                                    CargosService cargosService = new CargosService();
                                                    cargosService.AtualizarRelacaoCargoSubcompetencia(relacaoCargoSub);

                                                    if (hdId.Value == "")
                                                    {
                                                        if (cs.InserirCompetencia(competencia))
                                                        {
                                                            LimparCampos();
                                                            MessageBox.Show("Competência Inserida com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Falha na Inserção da Competência  !!", "", TIPO.Error, MessageBoxHandler);

                                                        }
                                                    }
                                                    else
                                                    {
                                                        competencia.IdCompetencia = Convert.ToInt32(hdId.Value);

                                                        if (cs.AlterarCompetencia(competencia))
                                                        {
                                                            LimparCampos();
                                                            MessageBox.Show("Competência Alterada com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Falha na Alteração da Competência  !!", "", TIPO.Error, MessageBoxHandler);

                                                        }
                                                    }


                                                }
                                                else
                                                {
                                                    MessageBox.Show("Favor preencher o Próximo Nível !!", "", TIPO.Info, MessageBoxHandler);
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Favor preencher o Detalhamento do Próximo Nível !!", "", TIPO.Info, MessageBoxHandler);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Favor preencher o Nivel Atual !!", "", TIPO.Info, MessageBoxHandler);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Favor preencher o Detalhamento do Nível Atual !!", "", TIPO.Info, MessageBoxHandler);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Favor selecionar a Dimensão !!", "", TIPO.Info, MessageBoxHandler);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Favor selecionar a Sub Competência !!", "", TIPO.Info, MessageBoxHandler);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Favor selecionar o Eixo !!", "", TIPO.Info, MessageBoxHandler);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Favor selecionar o Cargo !!", "", TIPO.Info, MessageBoxHandler);
                    }
                }
                else if (ddlTipoAvaliacao.SelectedIndex == 2)
                {
                    if (ddlEixoLideranca.SelectedIndex != 0 && ddlEscopoLideranca.SelectedIndex != 0 && ddlTituloLideranca.SelectedIndex != 0 && textDetalheLideranca.Text != "")
                    {
                        competencia.CompetenciaJR = textDetalheLideranca.Text;
                        competencia.CompetenciaPL = textDetalheLideranca.Text;
                        competencia.CompetenciaSR = textDetalheLideranca.Text;
                        competencia.CompetenciaJRDetalhe = textDetalheLideranca.Text;
                        competencia.CompetenciaPLDetalhe = textDetalheLideranca.Text;
                        competencia.CompetenciaSRDetalhe = textDetalheLideranca.Text;
                        competencia.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
                        competencia.IdCargo = 96;
                        competencia.IdNivel = 1;
                        competencia.IdEixo = Convert.ToInt16(ddlEixoLideranca.SelectedItem.Value);
                        competencia.IdSubCompetencia = Convert.ToInt16(ddlTituloLideranca.SelectedItem.Value);
                        competencia.IdDimensao = 10;
                        competencia.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()); ;
                        competencia.DHC = DateTime.Now;
                        competencia.ATV = 1;
                        competencia.TipoAvaliacao = "lideranca";
                        competencia.Escopo = ddlEscopoLideranca.SelectedItem.Text.Replace("Por ", "");
                        competencia.PalavrasChave = "";

                        if (hdId.Value == "")
                        {
                            if (cs.InserirCompetencia(competencia))
                            {
                                LimparCampos();
                                MessageBox.Show("Competência Inserida com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                            }
                            else
                            {
                                MessageBox.Show("Falha na Inserção da Competência  !!", "", TIPO.Error, MessageBoxHandler);

                            }
                        }
                        else
                        {
                            competencia.IdCompetencia = Convert.ToInt32(hdId.Value);

                            if (cs.AlterarCompetencia(competencia))
                            {
                                LimparCampos();
                                MessageBox.Show("Competência Alterada com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                            }
                            else
                            {
                                MessageBox.Show("Falha na Alteração da Competência  !!", "", TIPO.Error, MessageBoxHandler);

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Preencha todos os campos", "", TIPO.Warning, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Selecione o tipo de avaliação", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Preencha todos os campos", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        #region SHOW/HIDE DIVS DE DESEMPENHO/LIDERANÇA
        protected void comboTrocaAvaliacao(object sender, EventArgs e)
        {
            if (ddlTipoAvaliacao.SelectedIndex == 1)
            {
                divDesempenho.Visible = true;
                divLideranca.Visible = false;
                hdId.Value = "";
                textDetalheLideranca.Text = "";
            }
            else if (ddlTipoAvaliacao.SelectedIndex == 2)
            {
                divDesempenho.Visible = false;
                divLideranca.Visible = true;
                hdId.Value = "";
                textDetalheLideranca.Text = "";
            }
            else
            {
                divDesempenho.Visible = false;
                divLideranca.Visible = false;
            }
        }
        #endregion

        #region LER RELACAO CARGOxSUBCOMPETENCIA
        protected void ddlSubCompetencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            LerRelacaoCargoSubcompetencia();
        }
        public void LerRelacaoCargoSubcompetencia()
        {
            if (ddlCargo.SelectedIndex != 0 && ddlSubCompetencia.SelectedIndex != 0)
            {
                int IdCargo = Convert.ToInt16(ddlCargo.SelectedItem.Value);
                int IdSubCompetencia = Convert.ToInt16(ddlSubCompetencia.SelectedItem.Value);

                RELACAO_CARGO_SUBCOMPETENCIA relacaoCargoSub = new CargosService().ObterRelacaoCargoSubcompetencia(IdCargo, IdSubCompetencia);

                if (relacaoCargoSub != null)
                {
                    txtRelacaoSubcompetencia.Text = relacaoCargoSub.Descricao;
                }
                else
                {
                    txtRelacaoSubcompetencia.Text = "";
                }
            }
        }
        #endregion

        #region MONTAR COMBOS
        public void montaComboEscopo()
        {
            this.ddlEscopoLideranca.Items.Clear();
            this.ddlEscopoLideranca.Items.Insert(0, "[Selecionar]");
            this.ddlEscopoLideranca.Items.Insert(1, "Por projeto");
            this.ddlEscopoLideranca.Items.Insert(2, "Por líder");
            this.ddlEscopoLideranca.Items.Insert(2, "backoffice");
        }
        public void montaComboTipoAvaliacoes()
        {
            this.ddlTipoAvaliacao.Items.Clear();
            this.ddlTipoAvaliacao.Items.Insert(0, "[Selecionar]");
            this.ddlTipoAvaliacao.Items.Insert(1, "Avaliação de Desempenho");
            this.ddlTipoAvaliacao.Items.Insert(2, "Avaliação de Liderança");
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
        public void montaComboNiveis()
        {
            //List<CARGOSNIVEIS> cargosniveis = new CargosNiveisService().ObterListaNiveis(true);
            //this.ddlNivel.DataValueField = "IdNivel";
            //this.ddlNivel.DataTextField = "Nivel";
            //this.ddlNivel.DataSource = cargosniveis;
            //this.ddlNivel.DataBind();
            //this.ddlNivel.Items.Insert(0, "[Selecionar]");
        }
        public void montaComboEixos()
        {
            // DESEMPENHO
            List<EIXOS> eixos = new EixoService().ListaEixos(true, "desempenho");
            this.ddlEixo.DataValueField = "IdEixo";
            this.ddlEixo.DataTextField = "Eixo";
            this.ddlEixo.DataSource = eixos;
            this.ddlEixo.DataBind();
            this.ddlEixo.Items.Insert(0, "[Selecionar]");
            // LIDERANÇA
            eixos = new EixoService().ListaEixos(true, "lideranca");
            this.ddlEixoLideranca.DataValueField = "IdEixo";
            this.ddlEixoLideranca.DataTextField = "Eixo";
            this.ddlEixoLideranca.DataSource = eixos;
            this.ddlEixoLideranca.DataBind();
            this.ddlEixoLideranca.Items.Insert(0, "[Selecionar]");
        }
        public void montaComboSubCompetencias()
        {
            // DESEMPENHO
            List<SUBCOMPETENCIAS> subcompetencias = new SubCompetenciasService().ListaSubCompetencias(true);
            this.ddlSubCompetencia.DataValueField = "IdSubCompetencia";
            this.ddlSubCompetencia.DataTextField = "Subcompetencia";
            this.ddlSubCompetencia.DataSource = subcompetencias;
            this.ddlSubCompetencia.DataBind();
            this.ddlSubCompetencia.Items.Insert(0, "[Selecionar]");
            // LIDERANCA
            this.ddlTituloLideranca.DataValueField = "IdSubCompetencia";
            this.ddlTituloLideranca.DataTextField = "Subcompetencia";
            this.ddlTituloLideranca.DataSource = subcompetencias;
            this.ddlTituloLideranca.DataBind();
            this.ddlTituloLideranca.Items.Insert(0, "[Selecionar]");
        }
        public void montaComboDimensoes()
        {
            List<DIMENSOES> dimensoes = new DimensoesService().ListaDimensoes(true, "desempenho");
            this.ddlDimensao.DataValueField = "IdDimensao";
            this.ddlDimensao.DataTextField = "Dimensao";
            this.ddlDimensao.DataSource = dimensoes;
            this.ddlDimensao.DataBind();
            this.ddlDimensao.Items.Insert(0, "[Selecionar]");
        }
        public void montaComboListaCompetencias()
        {
            List<COMPETENCIAS> competencias = new CompetenciasService().ObterListaCompetencias(true);

            this.rptCompetencias.DataSource = competencias;
            this.rptCompetencias.DataBind();
        }
        public void montaComboNotasPadrao()
        {
            List<AVALIACOESCOMPETENCIASNOTAS> items = new AvaliacoesService().ObterAvaliacaoCompetenciasNotas().Where(x => !x.IndFeedback == true && x.ATV == 1).ToList();

            ddlNotaPadraoNivel1.DataValueField = "IdNota";
            ddlNotaPadraoNivel1.DataTextField = "CodigoNota";
            ddlNotaPadraoNivel1.DataSource = items;
            ddlNotaPadraoNivel1.DataBind();
            ddlNotaPadraoNivel1.Items.Insert(0, "[Selecionar]");
            ddlNotaPadraoNivel1.Items[0].Value = "0";


            ddlNotaPadraoNivel2.DataValueField = "IdNota";
            ddlNotaPadraoNivel2.DataTextField = "CodigoNota";
            ddlNotaPadraoNivel2.DataSource = items;
            ddlNotaPadraoNivel2.DataBind();
            ddlNotaPadraoNivel2.Items.Insert(0, "[Selecionar]");
            ddlNotaPadraoNivel2.Items[0].Value = "0";
        }
        public void montaComboModosCalculos()
        {
            var items = new AvaliacoesService().ObterModosCalculosCompetencias();

            ddlModoCalculo.DataValueField = "idModo";
            ddlModoCalculo.DataTextField = "ModoDesc";
            ddlModoCalculo.DataSource = items;
            ddlModoCalculo.DataBind();
        }
        #endregion

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

                var statusExclusao = new CompetenciasService().ExcluirCompetencia(Convert.ToInt32(id.Text));

                if (statusExclusao)
                {
                    MessageBox.Show("Competência Inativada com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                    LimparCampos();
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir uma Competência  !!", "", TIPO.Warning, MessageBoxHandler);
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

        private void MontaCampos(int idCompetencia)
        {
            var competencia = new CompetenciasService().ObterCompetencia(Convert.ToInt32(idCompetencia));
            if (competencia.TipoAvaliacao == "desempenho")
            {
                //ddlNivel.SelectedValue = competencia.IdNivel.ToString();
                ddlEixo.SelectedValue = competencia.IdEixo.ToString();
                ddlDimensao.SelectedValue = competencia.IdDimensao.ToString();
                txtJunior.Text = competencia.CompetenciaJR;
                txtDetalhamentoJr.Value = competencia.CompetenciaJRDetalhe;
                txtPleno.Text = competencia.CompetenciaPL;
                txtDetalhamentoPleno.Value = competencia.CompetenciaPLDetalhe;
                //txtSenior.Text = competencia.CompetenciaSR;
                //txtDetalhamentoSenior.Value = competencia.CompetenciaSRDetalhe;
                txtPalavrasChave.Text = competencia.PalavrasChave;

                ddlSubCompetencia.SelectedIndex = ddlSubCompetencia.Items.IndexOf(ddlSubCompetencia.Items.FindByValue(competencia.IdSubCompetencia.ToString()));
                ddlCargo.SelectedIndex = ddlCargo.Items.IndexOf(ddlCargo.Items.FindByValue(competencia.IdCargo.ToString())); // competencia.IdCargo.ToString();

                cboxInputAutoAv.Checked = competencia.InputAutoAvaliacao;
                cboxInputAvCegas.Checked = competencia.InputAvaliacaoAsCegas;
                cboxInputAvGestor.Checked = competencia.InputAvaliacaoGestor;
                cboxInputFeedback.Checked = competencia.InputFeedback;
                cboxInputNivel1.Checked = competencia.InputNivel1;
                cboxInputNivel2.Checked = competencia.InputNivel2;
                cboxVisivelAutoAv.Checked = competencia.VisivelAutoAvaliacao;
                cboxVisivelAvCegas.Checked = competencia.VisivelAvaliacaoAsCegas;
                cboxVisivelAvGestor.Checked = competencia.VisivelAvaliacaoGestor;
                cboxVisivelFeedback.Checked = competencia.VisivelFeedback;
                cboxVisivelNivel1.Checked = competencia.VisivelNivel1;
                cboxVisivelNivel2.Checked = competencia.VisivelNivel2;
                ddlNotaPadraoNivel1.SelectedIndex = ddlNotaPadraoNivel1.Items.IndexOf(ddlNotaPadraoNivel1.Items.FindByValue(competencia.IdNotaPadraoNivel1.ToString() ?? "0"));
                ddlNotaPadraoNivel2.SelectedIndex = ddlNotaPadraoNivel2.Items.IndexOf(ddlNotaPadraoNivel2.Items.FindByValue(competencia.IdNotaPadraoNivel2.ToString() ?? "0"));
                ddlModoCalculo.SelectedIndex = ddlModoCalculo.Items.IndexOf(ddlModoCalculo.Items.FindByValue(competencia.IdModo.ToString()));

                LerRelacaoCargoSubcompetencia();

                ddlTipoAvaliacao.SelectedIndex = 1;
                divDesempenho.Visible = true;
                divLideranca.Visible = false;
                btnAgregar.Visible = true;
            }
            else if (competencia.TipoAvaliacao == "lideranca")
            {
                ddlEscopoLideranca.SelectedValue = ddlEscopoLideranca.Items.FindByText("Por " + competencia.Escopo).Value;
                ddlEixoLideranca.SelectedValue = competencia.IdEixo.ToString();
                ddlTituloLideranca.SelectedValue = competencia.IdSubCompetencia.ToString();
                textDetalheLideranca.Text = competencia.CompetenciaJRDetalhe;

                ddlTipoAvaliacao.SelectedIndex = 2;
                divDesempenho.Visible = false;
                divLideranca.Visible = true;
                btnAgregar.Visible = false;
            }

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<CompetenciaModelExport> listCompetenciaModel = new List<CompetenciaModelExport>();
            var competenciasService = new CompetenciasService();
            var subcompetenciaService = new SubCompetenciasService();
            var eixoService = new EixoService();
            var dimensaoService = new DimensoesService();
            var cargoService = new CargosService();

            var getCompetencias = competenciasService.ObterListaCompetencias();

            foreach (var item in getCompetencias)
            {
                CompetenciaModelExport addCompetencia = new CompetenciaModelExport();
                addCompetencia.IdCompetencia = item.IdCompetencia;
                addCompetencia.IdCargo = item.IdCargo;
                addCompetencia.Cargo = cargoService.ObterCargo(item.IdCargo).Cargo;
                addCompetencia.IdEixo = item.IdEixo;
                addCompetencia.Eixo = eixoService.ObterEixo(item.IdEixo).Eixo;
                addCompetencia.IdSubCompetencia = item.IdSubCompetencia;
                addCompetencia.SubCompetencia = subcompetenciaService.ObterCompetencia(item.IdSubCompetencia).SubCompetencia;
                addCompetencia.IdDimensao = item.IdDimensao;
                addCompetencia.Dimensao = dimensaoService.ObterDimensao(item.IdDimensao).Dimensao;
                addCompetencia.DetalheNivelAtual = item.CompetenciaJRDetalhe;
                addCompetencia.CompetenciaAtual = item.CompetenciaJR;
                addCompetencia.ATV = (int)item.ATV;
                addCompetencia.TipoAvaliacao = item.TipoAvaliacao;
                addCompetencia.Escopo = item.Escopo;
                addCompetencia.PalavrasChave = item.PalavrasChave;
                listCompetenciaModel.Add(addCompetencia);
            }

            string fileName = "Competências_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            //Gera arquivo
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listCompetenciaModel);

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
                    using (var package = new OfficeOpenXml.ExcelPackage(fileUpload.FileContent))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            MessageBox.Show("O arquivo Excel não possui nenhuma planilha.", "", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        var competenciasService = new CompetenciasService();
                        int rowCount = worksheet.Dimension.End.Row;
                        int somaLinhasDesconsideradas = 0, somaLinhasInseridas = 0, somaLinhasAlteradas = 0, somaLinhasComErro = 0;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            int IdCompetencia = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                            int IdCargo = worksheet.Cells[row, 2].GetValue<int?>() ?? 0;
                            int IdEixo = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                            int IdSubCompetencia = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                            int IdDimensao = worksheet.Cells[row, 8].GetValue<int?>() ?? 0;
                            string DetalheNivelAtual = worksheet.Cells[row, 10].GetValue<string>() ?? "-";
                            string CompetenciaAtual = worksheet.Cells[row, 11].GetValue<string>() ?? "-";
                            string PalavrasChave = worksheet.Cells[row, 12].GetValue<string>() ?? "-";
                            string TipoAvaliacao = worksheet.Cells[row, 13].GetValue<string>() ?? "-";
                            string Escopo = worksheet.Cells[row, 14].GetValue<string>() ?? "-";
                            int ATV = worksheet.Cells[row, 15].GetValue<int?>() ?? 0;
                            DetalheNivelAtual = DetalheNivelAtual != "" ? DetalheNivelAtual : "-";
                            CompetenciaAtual = CompetenciaAtual != "" ? CompetenciaAtual : "-";
                            if (IdCargo > 0 && IdEixo > 0 && IdSubCompetencia > 0 && IdDimensao > 0 && TipoAvaliacao != "" && Escopo != "")
                            {
                                COMPETENCIAS competencia = new COMPETENCIAS();
                                competencia.IdCargo = IdCargo;
                                competencia.IdEixo = IdEixo;
                                competencia.IdSubCompetencia = IdSubCompetencia;
                                competencia.IdDimensao = IdDimensao;
                                competencia.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
                                competencia.IdNivel = 1;
                                competencia.CompetenciaJRDetalhe = DetalheNivelAtual;
                                competencia.CompetenciaJR = CompetenciaAtual;
                                competencia.CompetenciaPLDetalhe = DetalheNivelAtual;
                                competencia.CompetenciaPL = CompetenciaAtual;
                                competencia.CompetenciaSRDetalhe = DetalheNivelAtual;
                                competencia.CompetenciaSR = CompetenciaAtual;
                                competencia.PalavrasChave = PalavrasChave;
                                competencia.TipoAvaliacao = TipoAvaliacao;
                                competencia.Escopo = Escopo;
                                competencia.ATV = ATV;
                                competencia.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                competencia.DHC = DateTime.Now;
                                competencia.IdModo = int.Parse(ddlModoCalculo.SelectedItem.Value);
                                competencia.InputAutoAvaliacao = true;
                                competencia.InputAvaliacaoAsCegas = true;
                                competencia.InputAvaliacaoGestor = true;
                                competencia.InputFeedback = true;
                                competencia.InputNivel1 = true;
                                competencia.InputNivel2 = true;
                                competencia.VisivelAutoAvaliacao = true;
                                competencia.VisivelAvaliacaoAsCegas = true;
                                competencia.VisivelAvaliacaoGestor = true;
                                competencia.VisivelFeedback = true;
                                competencia.VisivelNivel1 = true;
                                competencia.VisivelNivel2 = true;
                                RELACAO_CARGO_SUBCOMPETENCIA relacaoCargoSub = new RELACAO_CARGO_SUBCOMPETENCIA();
                                relacaoCargoSub.idCargo = IdCargo;
                                relacaoCargoSub.idSubcompetencia = IdSubCompetencia;
                                relacaoCargoSub.Descricao = CompetenciaAtual;
                                CargosService cargosService = new CargosService();
                                cargosService.AtualizarRelacaoCargoSubcompetencia(relacaoCargoSub);
                                var existeCompetencia = competenciasService.ObterCompetencia(1, IdCargo, 1, IdEixo, IdSubCompetencia, IdDimensao, DetalheNivelAtual);
                                if (IdCompetencia == 0 && existeCompetencia == null)
                                {
                                    var inseriuCompetencia = competenciasService.InserirCompetencia(competencia);
                                    if (!inseriuCompetencia)
                                    {
                                        somaLinhasComErro += 1;
                                    }
                                    else
                                    {
                                        somaLinhasInseridas += 1;
                                    }
                                }
                                else if (existeCompetencia != null && existeCompetencia.ATV == 0)
                                {
                                    somaLinhasDesconsideradas += 1;
                                }
                                else
                                {
                                    competencia.IdCompetencia = IdCompetencia > 0 ? IdCompetencia : (existeCompetencia != null ? existeCompetencia.IdCompetencia : 0);
                                    var alterouCompetencia = competenciasService.AlterarCompetencia(competencia);
                                    if (!alterouCompetencia)
                                    {
                                        somaLinhasComErro += 1;
                                    }
                                    else
                                    {
                                        somaLinhasAlteradas += 1;
                                    }
                                }
                            }
                            else
                            {
                                somaLinhasDesconsideradas += 1;
                            }
                        }
                        MessageBox.Show("Competências importadas com sucesso<br>Inseridas: " + somaLinhasInseridas.ToString() + "<br>Alteradas: " + somaLinhasAlteradas.ToString() +
                            "<br>Desconsideradas: " + somaLinhasDesconsideradas.ToString() + "<br>Com erro: " + somaLinhasComErro.ToString(), "", TIPO.Info, MessageBoxHandler);
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

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            var avaliacoesService = new AvaliacoesService();
            var competenciasService = new CompetenciasService();
            var periodosService = new PeriodoService();
            int competenciasAdd = 0, associadosAdd = 0;

            var IdCompetencia = Convert.ToInt32(hdId.Value);
            var competencia = competenciasService.ObterCompetencia(IdCompetencia);
            var ultimoPeriodo = periodosService.ObterPeriodoUltimo();

            var avaliacoes = avaliacoesService.ObterAvaliacoesCompetencias(-1, -1, ultimoPeriodo.IdPeriodo, competencia.IdCargo);
            avaliacoes = avaliacoes.Where(x => x.TipoAvaliacao == "desempenho").ToList();
            var associados = avaliacoes.Select(x => x.IdAssociado).Distinct().ToList();
            foreach (var associado in associados)
            {
                var avaliacoesAssociado = avaliacoes.Where(x => x.IdAssociado == associado);
                var projetos = avaliacoesAssociado.Select(x => x.IdProjeto).Distinct().ToList();
                var associadoAdded = false;

                foreach (var projeto in projetos)
                {
                    var avaliacoesProjeto = avaliacoesAssociado.Where(x => x.IdProjeto == projeto).ToList();
                    var competenciasAvaliadas = avaliacoesProjeto.Select(x => x.IdCompetencia).Distinct().ToList();
                    var competenciaExemplo = avaliacoesProjeto[0];
                    var possuiCompetencia = competenciasAvaliadas.Contains(IdCompetencia);

                    if (!possuiCompetencia)
                    {
                        var addCompetencia = new AVALIACOESCOMPETENCIAS();
                        addCompetencia.IdEmpresa = competenciaExemplo.IdEmpresa;
                        addCompetencia.IdAssociado = associado;
                        addCompetencia.IdCargo = competencia.IdCargo;
                        addCompetencia.IdNivel = competenciaExemplo.IdNivel;
                        addCompetencia.IdProjeto = projeto;
                        addCompetencia.IdPeriodo = competenciaExemplo.IdPeriodo;
                        addCompetencia.IdCompetencia = IdCompetencia;
                        addCompetencia.IdAvaliacaoStatus = competenciaExemplo.IdAvaliacaoStatus;
                        addCompetencia.DataHoraInicio = DateTime.Now;
                        addCompetencia.DataHoraTermino = competenciaExemplo.DataHoraTermino;
                        addCompetencia.PosicaoAtualFluxoAvaliacao = competenciaExemplo.PosicaoAtualFluxoAvaliacao;
                        addCompetencia.DHC = competenciaExemplo.DHC;
                        addCompetencia.USR = competenciaExemplo.USR;
                        addCompetencia.ATV = competenciaExemplo.ATV;
                        addCompetencia.TipoAvaliacao = competenciaExemplo.TipoAvaliacao;
                        addCompetencia.Escopo = competenciaExemplo.Escopo;
                        addCompetencia.idAvaliacao = competenciaExemplo.idAvaliacao;
                        // AUTO AV
                        addCompetencia.IdNotaNivel1AutoAvaliacao = competencia.IdNotaPadraoNivel1 ?? competenciaExemplo.IdNotaNivel1AutoAvaliacao;
                        addCompetencia.IdNotaNivel2AutoAvaliacao = competencia.IdNotaPadraoNivel2 ?? competenciaExemplo.IdNotaNivel2AutoAvaliacao;
                        addCompetencia.ComentariosAutoAvaliacao = "";
                        addCompetencia.DHCAutoAvaliacao = competenciaExemplo.DHCAutoAvaliacao;
                        addCompetencia.USRAutoAvaliacao = competenciaExemplo.USRAutoAvaliacao;
                        addCompetencia.DataHoraInicioAutoAvaliacao = competenciaExemplo.DataHoraInicioAutoAvaliacao;
                        addCompetencia.DataHoraFimAutoAvaliacao = competenciaExemplo.DataHoraFimAutoAvaliacao;
                        // AV CEGAS
                        addCompetencia.IdNotaNivel1AvaliacaoCegas = competencia.IdNotaPadraoNivel1 ?? competenciaExemplo.IdNotaNivel1AvaliacaoCegas;
                        addCompetencia.IdNotaNivel2AvaliacaoCegas = competencia.IdNotaPadraoNivel2 ?? competenciaExemplo.IdNotaNivel2AvaliacaoCegas;
                        addCompetencia.ComentariosAvaliacaoCegas = "";
                        addCompetencia.DHCAvaliacaoCegas = competenciaExemplo.DHCAutoAvaliacao;
                        addCompetencia.USRAvaliacaoCegas = competenciaExemplo.USRAvaliacaoCegas;
                        addCompetencia.DataHoraInicioAvaliacaoCegas = competenciaExemplo.DataHoraInicioAvaliacaoCegas;
                        addCompetencia.DataHoraFimAvaliacaoCegas = competenciaExemplo.DataHoraFimAvaliacaoCegas;
                        // AV GESTOR
                        addCompetencia.IdNotaNivel1AvaliacaoGestor = competencia.IdNotaPadraoNivel1 ?? competenciaExemplo.IdNotaNivel1AvaliacaoGestor;
                        addCompetencia.IdNotaNivel2AvaliacaoGestor = competencia.IdNotaPadraoNivel2 ?? competenciaExemplo.IdNotaNivel2AvaliacaoGestor;
                        addCompetencia.ComentariosAvaliacaoGestor = "";
                        addCompetencia.DHCAvaliacaoGestor = competenciaExemplo.DHCAvaliacaoGestor;
                        addCompetencia.USRAvaliacaoGestor = competenciaExemplo.USRAvaliacaoGestor;
                        addCompetencia.DataHoraInicioAvaliacaoGestor = competenciaExemplo.DataHoraInicioAvaliacaoGestor;
                        addCompetencia.DataHoraFimAvaliacaoGestor = competenciaExemplo.DataHoraFimAvaliacaoGestor;
                        // FEEDBACK
                        addCompetencia.IdNotaNivel1Feedback = competencia.IdNotaPadraoNivel1 ?? competenciaExemplo.IdNotaNivel1Feedback;
                        addCompetencia.IdNotaNivel2Feedback = competencia.IdNotaPadraoNivel2 ?? competenciaExemplo.IdNotaNivel2Feedback;
                        addCompetencia.ComentariosFeedback = "";
                        addCompetencia.DHCFeedback = competenciaExemplo.DHCFeedback;
                        addCompetencia.USRFeedback = competenciaExemplo.USRFeedback;
                        addCompetencia.DataHoraInicioFeedback = competenciaExemplo.DataHoraInicioFeedback;
                        addCompetencia.DataHoraFimFeedback = competenciaExemplo.DataHoraFimFeedback;

                        avaliacoesService.SalvarAvaliacaoCompetencia(addCompetencia);
                        competenciasAdd += 1;
                        associadoAdded = true;
                    }
                }

                associadosAdd += associadoAdded ? 1 : 0;
            }

            MessageBox.Show("Agregadas " + competenciasAdd.ToString() + " competências as avaliações existentes de " + associadosAdd.ToString() + " associados.", "", TIPO.Info, MessageBoxHandler);
        }
    }
}