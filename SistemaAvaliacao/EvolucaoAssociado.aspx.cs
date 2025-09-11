using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SistemaAvaliacao
{
    public partial class EvolucaoAssociado : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                MontaCombos();
            //if (!IsPostBack)
            //{
            //    var associadoLogado = WebStorage.GetUsuarioLogado();
            //    var associado = new AssociadosService().ObterAssociadoMentorCargo(associadoLogado.Id);

            //    lblAssociado.InnerText = associado.Nome;
            //    lblMentor.InnerText = associado.Mentor;
            //    lblCargo.InnerText = associado.Cargo;
            //    lblProximoCargo.InnerText = associado.ProximoCargo;

            //    try
            //    {
            //        var resultadoAssociado = new EvolucaoAssociadoServices().Evolucao(associadoLogado.Id); 

            //        this.rptProjetos.ItemDataBound += RptProjetos_ItemDataBound;
            //        this.rptProjetos.DataSource = resultadoAssociado;
            //        this.rptProjetos.DataBind();

            //        var radar = JsonRadar(resultadoAssociado, associadoLogado.IdCargo);
            //        hfJsonRadar.Value = radar;
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, "Erro", TIPO.Error, MessageBoxHandler);
            //    }

            //}
        }
        protected void btnGerar_Click(object sender, EventArgs e)
        {
            var associadoLogado = WebStorage.GetUsuarioLogado();

            var strTipoAvaliacao = ddl_TipoAvaliacao.Items[ddl_TipoAvaliacao.SelectedIndex].Text;
            var strEscopo = ddl_Escopo.Items[ddl_Escopo.SelectedIndex].Text;
            if (strTipoAvaliacao == "[Selecionar]" || strEscopo == "[Selecionar]") { MessageBox.Show("Selecione o Tipo de Avaliações e Escopo", "Campos Pendentes", TIPO.Error, MessageBoxHandler); return; }

            var associado = new AssociadosService().ObterAssociadoMentorCargo(associadoLogado.Id);
            
            lblAssociado.InnerText = associado.Nome;
            lblMentor.InnerText = associado.Mentor;
            lblCargo.InnerText = associado.Cargo;
            lblProximoCargo.InnerText = associado.ProximoCargo;

            try
            {
                var resultadoAssociado = new EvolucaoAssociadoServices().Evolucao(associadoLogado.Id, strTipoAvaliacao, strEscopo);

                this.rptProjetos.ItemDataBound += RptProjetos_ItemDataBound;
                this.rptProjetos.DataSource = resultadoAssociado;
                this.rptProjetos.DataBind();

                var radar = JsonRadar(resultadoAssociado, associadoLogado.IdCargo);
                hfJsonRadar.Value = radar;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", TIPO.Error, MessageBoxHandler);
            }
        }
        public void MontaCombos()
        {
            ddl_TipoAvaliacao.Items.Clear();
            ddl_TipoAvaliacao.Items.Insert(0, "[Selecionar]");
            ddl_TipoAvaliacao.Items.Insert(1, "desempenho");

            ddl_Escopo.Items.Clear();
            ddl_Escopo.Items.Insert(0, "[Selecionar]");
            ddl_Escopo.Items.Insert(1, "projeto");
        }
        private void RptProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            EVOLUCAOASSOCIADO item = (EVOLUCAOASSOCIADO)e.Item.DataItem;

            Repeater repperf = (Repeater)e.Item.FindControl("rptNotaPerfomance");
            if (repperf != null)
            {
                repperf.DataSource = item.EVOLUCAOPERFORMANCE.OrderBy(x => x.IdPerformance);
                repperf.DataBind();

                if (item.TipoAvaliacao == "lideranca")
                {
                    HtmlControl tabela = (HtmlControl)e.Item.FindControl("divTabelaPerformance");
                    tabela.Visible = false;
                }
            }

            Repeater repSomaComp = (Repeater)e.Item.FindControl("rptSomaCompetencias");
            if (repSomaComp != null)
            {
                repSomaComp.DataSource = item.EVOLUCAOCOMPETENCIAS;
                repSomaComp.DataBind();
            }
        }
        public string FormatPercentagem(decimal nota)
        {

            if (nota > 0)
            {
                return nota.ToString("##0") + "%";
            }

            return "0%";

        }
        public string FormatDecimal(decimal nota)
        {
            return Math.Round(nota, 2).ToString();

        }
        private string JsonRadar(List<EVOLUCAOASSOCIADO> listprojetos, int idcargo)
        {
            
            dynamic obj = new JObject();
            List<string> labels = new List<string>();           
            List<JObject> datasets = new List<JObject>();

            int count = 1;
           
            foreach (var projeto in listprojetos.OrderByDescending(x => x.IdPeriodo))
            {
                
                dynamic itemprojeto = new JObject();
                itemprojeto.periodo = projeto.PERIODOSAVALIACOES.Periodo;
                List<decimal> dataset = new List<decimal>();

                foreach (var item in projeto.EVOLUCAOCOMPETENCIAS)
                {
                    if (count == 1)
                    {
                        labels.Add(item.EIXOS.Eixo);
                    }
                                       
                    dataset.Add(item.Nota.HasValue && item.Nota.Value > 0 ? item.Nota.Value : 0);
                }

                itemprojeto.dataset = new JArray(dataset);
                datasets.Add(itemprojeto);

                count++;
            }

            obj.labels = new JArray(labels);           
            obj.datasets = new JArray(datasets);

            var premissaradar = new PremissaService().ObterPremissaPorCardo(idcargo);
            obj.stepsize = 200;

            if (premissaradar != null)
            {
                if (premissaradar.ValorRadarPeers > 1)
                {
                    decimal step = (200m) / (premissaradar.ValorRadarPeers + 1);
                    obj.stepsize = Math.Round(step);
                }
            }

            return JsonConvert.SerializeObject(obj);

        }
    }
}