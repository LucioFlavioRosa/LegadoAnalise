using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using Newtonsoft.Json;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.DataVisualization.Charting;
using TriaSoftware.Util.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class radar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboPeriodos();
                CarregaComboTipoEscopo();
            }
        }

        private void CarregaComboPeriodos()
        {
            var statusList = new PeriodoService().ListaPeriodos(WebStorage.GetUsuarioLogado().IdEmpresa);
            ddlPeriodos.DataValueField = "IdPeriodo";
            ddlPeriodos.DataTextField = "Periodo";
            ddlPeriodos.DataSource = statusList;
            ddlPeriodos.DataBind();
            ddlPeriodos.Items.Insert(0, "[Selecionar]");
        }

        protected void ddlPeriodos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPeriodos.SelectedIndex > 0)
            {
                CarregaComboProjetos();
            }
        }

        private void CarregaComboProjetos()
        {
            var periodo = new PeriodoService().ObterPeriodo(int.Parse(ddlPeriodos.SelectedValue.ToString()));
            var projetosList = new ProjetosService().ListaProjetosAtivos(periodo);
            ddlProjetos.DataValueField = "IdProjeto";
            ddlProjetos.DataTextField = "Projeto";
            ddlProjetos.DataSource = projetosList;
            ddlProjetos.DataBind();
            ddlProjetos.Items.Insert(0, "[Selecionar]");
        }
        private void CarregaComboTipoEscopo()
        {
            this.ddlTipo.Items.Clear();
            this.ddlTipo.Items.Insert(0, "[Selecionar]");
            this.ddlTipo.Items.Insert(1, "desempenho");
            this.ddlTipo.Items.Insert(2, "liderança");

            this.ddlEscopo.Items.Clear();
            this.ddlEscopo.Items.Insert(0, "[Selecionar]");
            this.ddlEscopo.Items.Insert(1, "projeto");
            this.ddlEscopo.Items.Insert(2, "lider");
        }

        protected void ddlProjetos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProjetos.SelectedIndex > 0)
            {
                CarregaComboAssociados();
            }
        }

        private void CarregaComboAssociados()
        {
            var associadosListId = new ProjetosService().ObterListaAssociados(int.Parse(ddlProjetos.SelectedValue.ToString())).Select(p => p.IdAssociado);
            var associadosList = new AssociadosService().ObterAssociados(true).Where(a => associadosListId.Contains(a.IdAssociado)).ToList();
            ddlAssociados.DataValueField = "IdAssociado";
            ddlAssociados.DataTextField = "Nome";
            ddlAssociados.DataSource = associadosList;
            ddlAssociados.DataBind();
            ddlAssociados.Items.Insert(0, "[Selecionar]");
        }

        protected void btnMostraGrafico_Click(object sender, EventArgs e)
        {
            if (ddlPeriodos.SelectedIndex <= 0 || ddlProjetos.SelectedIndex <= 0 || ddlAssociados.SelectedIndex <= 0 || ddlTipo.SelectedIndex <= 0 || ddlEscopo.SelectedIndex <= 0)
            {
                MessageBox.Show("Selecione Todos os Filtros para Gerar o Gráfico", "Filtros Inválidos", TIPO.Info, MessageBoxHandler);
                return;
            }

            var idPeriodo = int.Parse(ddlPeriodos.SelectedValue.ToString());
            var idProjeto = int.Parse(ddlProjetos.SelectedValue.ToString());
            var idAssociado = int.Parse(ddlAssociados.SelectedValue.ToString());
            var TipoAvaliacao = ddlTipo.SelectedValue.ToString();
            var Escopo = ddlEscopo.SelectedValue.ToString();

            var avaliacaoService = new AvaliacoesService();
            var listaAvaliacoes = avaliacaoService.ObterAvaliacoesCompetencias(idAssociado, idProjeto, idPeriodo, avaliacaoService.etapaAvaliacaoMentor, TipoAvaliacao, Escopo);

            if (listaAvaliacoes == null || listaAvaliacoes.Count == 0)
            {
                MessageBox.Show("Avaliação não Finalizada", "Impossível Gerar Gráfico", TIPO.Warning, MessageBoxHandler);
                return;
            }

            var premissas = new PremissaService().ObterLista(listaAvaliacoes[0].IdCargo, listaAvaliacoes[0].IdNivel);

            if (premissas == null || premissas.Count == 0)
            {
                MessageBox.Show("Premissa Não Cadastrada", "Impossível Gerar Gráfico", TIPO.Warning, MessageBoxHandler);
                return;
            }

            WebStorage.Set("BackColor", chkBackground.Checked.ToString());
            WebStorage.Set("ShowValue", chkRotulo.Checked.ToString());

            Series serieAAv; Series serieAGe; Series seriePee;
            configuraPontosRadar(out serieAAv, out serieAGe, out seriePee);
            
            var competenciaService = new CompetenciasService();
            var eixoService = new EixoService();
            var premissaService = new PremissaService();
            var listEixos = new List<string>();

            decimal somaAvaliado = 0;
            decimal somaGestor = 0;

            foreach (var premissa in premissas)
            {
                somaAvaliado = premissa.ValorBaseAutoAvaliacao;
                somaGestor = premissa.ValorBaseAvaliacaoGestor;

                var listaAvaliacaoDestaCompetencia = listaAvaliacoes.Where(a => a.COMPETENCIAS.IdEixo == premissa.IdEixo).ToList();

                foreach (var aval in listaAvaliacaoDestaCompetencia)
                {
                    somaAvaliado += aval.NotaCompetenciaAvaliado.Value;
                    somaGestor += aval.NotaCompetenciaGestor.Value;
                }

                serieAAv.Points.AddXY(premissa.IdEixo, somaAvaliado);
                serieAGe.Points.AddXY(premissa.IdEixo, somaGestor);
                seriePee.Points.AddXY(premissa.IdEixo, premissa.ValorRadarPeers);

                somaAvaliado = 0;
                somaGestor = 0;
            }
        }

        private void configuraPontosRadar(out Series serieAAv, out Series serieAGe, out Series seriePee)
        {
            var back = bool.Parse(WebStorage.Get("BackColor", "false"));
            var showRotulo = bool.Parse(WebStorage.Get("ShowValue", "true"));
            var alpha = back ? 20 : 0;

            // Configurações Padrão Radar Peers
            seriePee = myRadarChart.Series.Add("Radar Peers");
            seriePee.Color = Color.FromArgb(alpha, 128, 128, 128);
            seriePee.ChartType = SeriesChartType.Radar;
            seriePee.ChartArea = "MainChartArea";
            seriePee.YValueType = ChartValueType.Int32;
            seriePee.BorderColor = Color.FromArgb(255, 128, 128, 128);
            seriePee.BorderDashStyle = ChartDashStyle.Solid;
            seriePee.BorderWidth = 3;
            myRadarChart.Legends.Add("Radar Peers");
            seriePee.IsValueShownAsLabel = showRotulo;
            seriePee.MarkerBorderColor = Color.FromArgb(255, 128, 128, 128);
            seriePee.MarkerBorderWidth = 5;
            seriePee.MarkerStyle = MarkerStyle.Diamond;

            // Configurações Padrão Auto Avaliação 
            serieAAv = myRadarChart.Series.Add("Auto Avaliação");
            serieAAv.Color = Color.FromArgb(alpha, 0, 100, 0);
            serieAAv.ChartType = SeriesChartType.Radar;
            serieAAv.ChartArea = "MainChartArea";
            serieAAv.YValueType = ChartValueType.Int32;
            serieAAv.BorderColor = Color.FromArgb(255, 0, 100, 0);
            serieAAv.BorderDashStyle = ChartDashStyle.Solid;
            serieAAv.BorderWidth = 3;
            myRadarChart.Legends.Add("Auto Avaliação");
            serieAAv.IsValueShownAsLabel = showRotulo;
            serieAAv.MarkerBorderColor = Color.FromArgb(255, 0, 100, 0);
            serieAAv.MarkerBorderWidth = 5;
            serieAAv.MarkerStyle = MarkerStyle.Diamond;

            // Configurações Padrão Avaliação Gestor
            serieAGe = myRadarChart.Series.Add("Avaliação Gestor");
            serieAGe.Color = Color.FromArgb(alpha, 0, 0, 128);
            serieAGe.ChartType = SeriesChartType.Radar;
            serieAGe.ChartArea = "MainChartArea";
            serieAGe.YValueType = ChartValueType.Int32;
            serieAGe.BorderColor = Color.FromArgb(255, 0, 0, 128);
            serieAGe.BorderDashStyle = ChartDashStyle.Solid;
            serieAGe.BorderWidth = 3;
            myRadarChart.Legends.Add("Avaliação Gestor");
            serieAGe.IsValueShownAsLabel = showRotulo;
            serieAGe.MarkerBorderColor = Color.FromArgb(255, 0, 0, 128);
            serieAGe.MarkerBorderWidth = 5;
            serieAGe.MarkerStyle = MarkerStyle.Diamond;
        }

    }
}