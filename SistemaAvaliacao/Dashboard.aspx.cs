using Business.Model;
using Business.Services;
using Business.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services.Description;

namespace SistemaAvaliacao
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private DashboardService Service { get; set; }
        private int IdPeriodo { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = WebStorage.GetUsuarioLogado();

            if (user == null || !user.IsLogged)
            {
                Response.Redirect("~/Login");
            }
            else
            {
                if (user.IdPerfil < 3)
                {
                    Response.Redirect("~/Index");
                }
            }

            if (!IsPostBack)
            {
                try
                {
                    CarregaComboPeriodos();
                    CarregaDashboard();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao tentar carregar o Dashboard: " + ex.Message, "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }

        protected void BtnPeriodo_Click(object sender, EventArgs e)
        {
            CarregaDashboard();
        }

        private void CarregaDashboard()
        {
            var id = ddlPeriodos.SelectedValue.ToString();

            if (int.TryParse(id, out int idperiodo))
            {
                this.Service = new DashboardService();
                this.IdPeriodo = idperiodo;
                InfoPeriodo();
                CarregaNumeros();
                CarregaAndamentoAvaliacoes();
                CarregaAvaliadosPorPeriodo();
                CarregaAvaliadosPorProjeto();
                CarregaRadarCompetencia();
                CarregaLinePerformance();
            }
            else
            {
                MessageBox.Show("Período não selecionado!", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        private void CarregaLinePerformance()
        {
            var performance = Service.ChartConsolidadoPerformance(IdPeriodo);

            string json = JsonPerformance(performance);

            if (!string.IsNullOrEmpty(json))
            {
                hfPerformance.Value = json;
            }
        }

        private void CarregaRadarCompetencia()
        {
            var competencias = Service.RadarConsolidadoCompetencia(IdPeriodo);
            
            string json = JsonRadar(competencias);

            if (!string.IsNullOrEmpty(json))
            {
                hfCompetencia.Value = json;
            }
        }

        private void CarregaAvaliadosPorProjeto()
        {
            List<DashboardModel> model = Service.AvaliadosPorProjeto(IdPeriodo);

            if (model != null)
            {
                rptProjetos.DataSource = model;
                rptProjetos.DataBind();
            }
        }

        private void CarregaAvaliadosPorPeriodo()
        {
            var dash = Service.AvaliadosPorPeriodo();
            string json = JsonChart(dash);

            if (!string.IsNullOrEmpty(json))
            {
                hfAvaliadosPeriodo.Value = json;
            }
        }

        private void CarregaAndamentoAvaliacoes()
        {
            var dash = Service.AndamentoAvaliacao(IdPeriodo);
            
            if (dash.ListAndamento != null && dash.ListAndamento.Count > 0)
            {
                rptAndamento.DataSource = dash.ListAndamento;
                rptAndamento.DataBind();

                string json = JsonChart(dash);

                if (!string.IsNullOrEmpty(json))
                {
                    hfAndamentoAvaliacoes.Value = json;
                }
            }
        }

        private void CarregaNumeros()
        {
            DashboardModel dash = Service.Numeros(this.IdPeriodo);
            lblAssociados.Text = dash.QtdAssociados.ToString();
            lblMentores.Text = dash.QtdMentores.ToString();
            lblGestores.Text = dash.QtdGestores.ToString();
            lblAvaliadores.Text = dash.QtdAvaliadores.ToString();
            lblAvaliacoes.Text = dash.QtdAvaliacoes.ToString();
        }

        private void InfoPeriodo()
        {
            var periodo = new PeriodoService().ObterPeriodo(IdPeriodo);
            lblPeriodo.Text = string.Format("{0} - {1}", periodo.DataInicio.Value.ToShortDateString(), periodo.DataFim.Value.ToShortDateString());
        }

        private void CarregaComboPeriodos()
        {
            PeriodoService service = new PeriodoService();
            var statusList = service.ListaPeriodos(WebStorage.GetUsuarioLogado().IdEmpresa);
            var atualPeriodo = service.ObterPeriodoAtual();
            ddlPeriodos.DataValueField = "IdPeriodo";
            ddlPeriodos.DataTextField = "Periodo";
            ddlPeriodos.DataSource = statusList.OrderByDescending(x => x.DataInicio);
            ddlPeriodos.DataBind();
            ddlPeriodos.SelectedValue = atualPeriodo.IdPeriodo.ToString();

        }

        private string JsonPerformance(List<EvolucaoPerformanceModel> model)
        {
            dynamic obj = new JObject();

            List<string> labels = new List<string>();
            List<decimal> dataset = new List<decimal>();

            foreach (var item in model)
            {
                labels.Add(item.Performance);

                if (item.Nota.HasValue)
                {
                    dataset.Add(Math.Round(item.Nota.Value));
                }
                else
                {
                    dataset.Add(0);
                }

            }

            obj.labels = new JArray(labels);
            obj.dataset = new JArray(dataset);

            return JsonConvert.SerializeObject(obj);

        }

        private string JsonChart(DashboardModel model)
        {
            dynamic obj = new JObject();

            List<string> labels = new List<string>();
            List<int> dataset = new List<int>();

            foreach (var item in model.ListAndamento)
            {
                labels.Add(item.Status);
                dataset.Add(item.QtdStatus); 
            }

            obj.labels = new JArray(labels);
            obj.dataset = new JArray(dataset);

            return JsonConvert.SerializeObject(obj);

        }

        private string JsonRadar(ResultadoProjetosModel model)
        {
            dynamic obj = new JObject();

            List<string> labels = new List<string>();
            List<decimal> datasetconsolidado = new List<decimal>();
            List<decimal> datasetnivel = new List<decimal>();


            foreach (var item in model.ListSomaCompetenciasN1N2)
            {
                labels.Add(item.Eixo);
                var menN1 = item.PercentualSomaNotaFinalN1N2 ?? 0;
                datasetconsolidado.Add(menN1);        
                datasetnivel.Add(200);
            }

            obj.labels = new JArray(labels);
            obj.datasetconsolidado = new JArray(datasetconsolidado);
            obj.datasetnivel = new JArray(datasetnivel);

            return JsonConvert.SerializeObject(obj);

        }
    }
}