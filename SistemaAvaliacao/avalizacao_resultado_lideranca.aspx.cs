using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Tria.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class avalizacao_resultado_lideranca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var user = WebStorage.GetUsuarioLogado();

                if (user == null || !user.IsLogged)
                {
                    Response.Redirect("~/Login");
                }
                else
                {
                    
                }

                if (!IsPostBack)
                {
                    var strAssociado = Request.QueryString["IdAssociado"];
                    var strPeriodo = Request.QueryString["IdPeriodo"];
                    var strTipoAvaliacao = Request.QueryString["TipoAvaliacao"];
                    var strEscopo = Request.QueryString["Escopo"];

                    // ANTIGO - CARREGA PAINEL
                    if (1 == 2)
                    {
                        if (!int.TryParse(strAssociado, out int idAssociado))
                        {
                            strAssociado = WebStorage.Get("IdAssociado", "");

                            if (!int.TryParse(strAssociado, out idAssociado))
                            {
                                MessageBox.Show("É obrigatório a seleção de um Associado.", "Associado Não Encontrado", TIPO.Info, MessageBoxHandler);
                                return;
                            }
                        }

                        if (!int.TryParse(strPeriodo, out int idPeriodo))
                        {
                            strPeriodo = WebStorage.Get("IdPeriodo", "");

                            if (!int.TryParse(strPeriodo, out idPeriodo))
                            {
                                MessageBox.Show("É obrigatório a seleção de um Período.", "Período Não Encontrado", TIPO.Info, MessageBoxHandler);
                                return;
                            }
                        }

                        var associado = new AssociadosService().ObterAssociadoMentorCargo(idAssociado);

                        if (associado == null)
                        {
                            MessageBox.Show("É obrigatório a seleção de um Associado cadastrado.", "Associado Não Encontrado", TIPO.Info, MessageBoxHandler);
                            return;
                        }

                        PeriodoService periodoService = new PeriodoService();

                        PERIODOSAVALIACOES periodo = periodoService.ObterPeriodo(idPeriodo);
                        lblPeriodo.InnerText = periodo.Periodo;

                        lblAssociado.InnerText = associado.Nome;
                        lblMentor.InnerText = associado.Mentor;
                        lblCargo.InnerText = associado.Cargo;
                        lblProximoCargo.InnerText = associado.ProximoCargo;
                        DataModel dtMod = new DataModel();
                        string foto = "";
                        foto = dtMod.ASSOCIADOS.FirstOrDefault(x => x.IdAssociado == idAssociado).FotoNome;
                        if (foto == null || foto == "") { foto = "assets/images/users/usernophoto.jpg"; }
                        foto = foto.Replace(" ", "%20");
                        imgUser1Comite.Src = foto;

                        if (!IsPostBack)
                            MontaProjetosLiderados(idAssociado, idPeriodo, strTipoAvaliacao, strEscopo);

                        AtualizarGraficos(idAssociado, idPeriodo, strTipoAvaliacao, strEscopo);

                        WebStorage.Set("idAssociado", idAssociado.ToString());
                        WebStorage.Set("idPeriodo", idPeriodo.ToString());
                        WebStorage.Set("strTipoAvaliacao", strTipoAvaliacao);
                        WebStorage.Set("strEscopo", strEscopo);

                    }
                    // NOVO - PAINEL ANONIMIZADO EQUIVALENTE AO PBI
                    else
                    {
                        var avaliacoes = new AvaliacoesService().ObterAvaliacoesTipo("AFI", "lideranca");
                        Session["RESPOSTAS"] = new List<AVALIACOESCOMPETENCIAS>();
                        Session["AVALIACOES"] = new List<AVALIACAO>();

                        var getPeriodos = new PeriodoService().ListaPeriodos(1).Where(x => x.fl_lib_res_lideranca).Select(x => x.IdPeriodo).ToList();
                       
                        Session["RESPOSTAS"] = new AvaliacoesService().ObterAvaliacoesCompetenciasPeriodo(getPeriodos, "lideranca");
                        Session["AVALIACOES"] = avaliacoes;

                        if (WebStorage.GetUsuarioLogado().Id != 389)
                        {
                            var idLider = WebStorage.GetUsuarioLogado().Id;
                            var nomeLider = WebStorage.GetUsuarioLogado().Nome;

                            var mentorados = new AssociadosService().ObterMentorados(idLider);
                            ddl_Lider.DataSource = mentorados;
                            ddl_Lider.DataValueField = "idAssociado";
                            ddl_Lider.DataTextField = "nome";
                            ddl_Lider.DataBind();

                            ddl_Lider.Items.Insert(0, nomeLider);
                            ddl_Lider.Items[0].Value = idLider.ToString();

                            ddl_Lider.SelectedValue = idLider.ToString();
                            //ddl_Lider.Enabled = false;
                            btn_NotificarLideres.Visible = false;
                        }
                        else
                        {
                            var respostasCompetencias = Session["RESPOSTAS"] as List<AVALIACOESCOMPETENCIAS>;
                            var associados = new AssociadosService().ObterAssociados();
                            var lideresIds = respostasCompetencias.Select(x => x.IdAssociado);
                            var lideres = associados.Where(x => lideresIds.Contains(x.IdAssociado)).Distinct();

                            ddl_Lider.DataTextField = "Nome";
                            ddl_Lider.DataValueField = "IdAssociado";
                            ddl_Lider.DataSource = lideres;
                            ddl_Lider.DataBind();
                            btn_NotificarLideres.Visible = true;
                        }

                        atualizaPainelLider();

                        ScriptManager.RegisterStartupScript(this, GetType(), "helperDDLCiclos", "helperDDLCiclos();", true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", TIPO.Error, MessageBoxHandler);
            }

        }

        private void MontaProjetosLiderados(int idAssociado, int idPeriodo, string strTipoAvaliacao, string strEscopo)
        {
            List<PROJETOS> projetos = new List<PROJETOS>();
            List<ASSOCIADOS> liderados = new List<ASSOCIADOS>();
            List<CARGOS> cargos = new List<CARGOS>();
            List<PERIODOSAVALIACOES> periodos = new List<PERIODOSAVALIACOES>();

            List<PERIODOSAVALIACOES> getPeriodos = new PeriodoService().ListaTodosPeriodos(1).Where(x => x.fl_lib_res_lideranca).ToList();

            foreach (var periodo in getPeriodos)
            {
                var avaliacoes = new AvaliacoesService().ObterAvaliacoesFinalizadasAssociadoPeriodo(idAssociado, periodo.IdPeriodo, strTipoAvaliacao, strEscopo);

                foreach (var aval in avaliacoes)
                {
                    if (projetos.Find(p => p.IdProjeto == aval.idProjeto) == null) { projetos.Add(new ProjetosService().ObterProjeto(aval.idProjeto)); }
                    var getGestor = new AssociadosService().ObterAssociado((int)aval.idGestor);
                    if (liderados.Find(l => l.IdAssociado == aval.idGestor) == null) { liderados.Add(getGestor); }
                    if (cargos.Find(c => c.IdCargo == getGestor.IdCargo) == null) { cargos.Add(getGestor.CARGOS); }
                    if (periodos.Find(p => p.IdPeriodo == aval.idPeriodo && p.ATV == 1) == null) { periodos.Add(aval.PERIODOSAVALIACOES); }
                }
            }

            cboxProjetos.DataValueField = "IdProjeto";
            cboxProjetos.DataTextField = "Projeto";
            cboxProjetos.DataSource = projetos;
            cboxProjetos.DataBind();

            cboxLiderados.DataValueField = "IdAssociado";
            cboxLiderados.DataTextField = "Nome";
            cboxLiderados.DataSource = liderados;
            cboxLiderados.DataBind();

            cboxCargos.DataValueField = "IdCargo";
            cboxCargos.DataTextField = "Cargo";
            cboxCargos.DataSource = cargos;
            cboxCargos.DataBind();

            cboxSemestres.DataValueField = "IdPeriodo";
            cboxSemestres.DataTextField = "Periodo";
            cboxSemestres.DataSource = periodos;
            cboxSemestres.DataBind();
        }
        private void AtualizarGraficos(int idAssociado, int idPeriodo, string strTipoAvaliacao, string strEscopo)
        {
            List<int> listProjetos = new List<int>();
            List<int> listLiderados = new List<int>();
            List<int> listCargos = new List<int>();
            List<int> listPeriodos = new List<int>();

            foreach (ListItem item in cboxProjetos.Items)
            {
                if (item.Selected) { listProjetos.Add(int.Parse(item.Value)); } 
            }
            foreach (ListItem item in cboxCargos.Items)
            {
                if (item.Selected) { listCargos.Add(int.Parse(item.Value)); }
            }
            foreach (ListItem item in cboxLiderados.Items)
            {
                if (listCargos.Count >= 1) {
                    var liderado = new AssociadosService().ObterAssociado(int.Parse(item.Value));
                    if (listCargos.Contains(liderado.CARGOS.IdCargo))
                        listLiderados.Add(int.Parse(item.Value));
                }
                else{
                    if (item.Selected)
                        listLiderados.Add(int.Parse(item.Value));
                }
            }
            foreach (ListItem item in cboxSemestres.Items)
            {
                if (item.Selected) { listPeriodos.Add(int.Parse(item.Value)); }
            }

            var resultadoAssociado = new ResultadoServices().ObterResultadoLider(idAssociado, listPeriodos, strTipoAvaliacao, strEscopo, listProjetos, listLiderados);

            graficoEixos.DataSource =
                from ra in resultadoAssociado
                select new RLM_Eixo
                {
                    eixo = ra.eixo,
                    resultado = ra.resultado * 100
                };
            graficoEixos.DataBind();

            List<RLM_Resultados> rlmResultados = new List<RLM_Resultados>();
            RLM_Resultados eixosResult = new RLM_Resultados();
            eixosResult.eixos = resultadoAssociado;
            rlmResultados.Add(eixosResult);
            tabelaNotasEixos.DataSource = rlmResultados;
            tabelaNotasEixos.DataBind();

            graficoSubs.DataSource = resultadoAssociado;
            graficoSubs.DataBind();

            rlmResultados[0].liderados = new List<RLM_Liderado>();
            foreach (var eixo in rlmResultados[0].eixos)
            {
                foreach (var liderado in eixo.liderados)
                {
                    if (rlmResultados[0].liderados.Find(rlm => rlm.idLiderado == liderado.idLiderado) == null)
                    {
                        rlmResultados[0].liderados.Add(liderado);
                    }
                }
            }
            tabelaRespostas.DataSource = rlmResultados;
            tabelaRespostas.DataBind();
        }
        protected void cboxProjetos_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarGraficos(int.Parse(WebStorage.Get("idAssociado", "0")),
                              int.Parse(WebStorage.Get("idPeriodo", "0")),
                              WebStorage.Get("strTipoAvaliacao", "lideranca"),
                              WebStorage.Get("strEscopo", "projeto"));
        }
        private string JsonSomaRadar(ResultadoSomaProjetosModel somaProjetos)
        {
            dynamic obj = new JObject();
            List<string> labels = new List<string>();
            List<decimal> datasetCompetencia = new List<decimal>();
            List<decimal> datasetproximonivel = new List<decimal>();

            var nivelAtual = Math.Ceiling(somaProjetos.SomaProjetosNotaCompetenciaRadar.Value / 100m) * 100m;

            foreach (var item in somaProjetos.ListProjetosSomaCompetenciasN1N2)
            {
                labels.Add(item.Eixo);
                datasetCompetencia.Add(item.NotaProjetoCompetenciaRadar.HasValue ? item.NotaProjetoCompetenciaRadar.Value : 0);
                datasetproximonivel.Add(nivelAtual);
            }

            obj.labels = new JArray(labels);
            obj.datasetcompetencias = new JArray(datasetCompetencia);
            obj.datasetproximonivel = new JArray(datasetproximonivel);

            return JsonConvert.SerializeObject(obj);
        }
        private string JsonRadar(List<ResultadoProjetosModel> listprojetos)
        {
            var listradar = new List<dynamic>();

            foreach (var projeto in listprojetos)
            {
                dynamic obj = new JObject();
                obj.idprojeto = projeto.IdProjeto;

                List<string> labels = new List<string>();
                List<decimal> datasetCompetencia = new List<decimal>();
                List<decimal> datasetNivelAtual = new List<decimal>();

                var nivelAtual = Math.Ceiling(projeto.SomaNotaCompetenciaRadar.Value / 100m) * 100m;

                foreach (var item in projeto.ListSomaCompetenciasN1N2)
                {
                    labels.Add(item.Eixo);
                    datasetCompetencia.Add(item.NotaCompetenciaRadar.HasValue ? item.NotaCompetenciaRadar.Value : 0);
                    datasetNivelAtual.Add(nivelAtual);
                }

                obj.labels = new JArray(labels);
                obj.datasetcompetencias = new JArray(datasetCompetencia);
                obj.datasetnivelatual = new JArray(datasetNivelAtual);

                listradar.Add(obj);
            }

            return JsonConvert.SerializeObject(listradar);

        }
        private void RptSomaProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ResultadoSomaProjetosModel item = (ResultadoSomaProjetosModel)e.Item.DataItem;

            Repeater repn1 = (Repeater)e.Item.FindControl("rptSomaProjetosItems");
            if (repn1 != null)
            {
                repn1.DataSource = item.ListProjetosSomaPerfomance;
                repn1.DataBind();
            }

            Repeater repsc = (Repeater)e.Item.FindControl("rptProjetosSomaCompetencias");
            if (repsc != null)
            {
                repsc.DataSource = item.ListProjetosSomaCompetenciasN1N2;
                repsc.DataBind();
            }

            Repeater repradar = (Repeater)e.Item.FindControl("rptSomaProjetosRadar");
            if (repradar != null)
            {
                repradar.DataSource = item.ListProjetosSomaCompetenciasN1N2;
                repradar.DataBind();
            }


        }
        private void RptProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ResultadoProjetosModel item = (ResultadoProjetosModel)e.Item.DataItem;

            Repeater repn1 = (Repeater)e.Item.FindControl("rptCompetenciasN1");
            if (repn1 != null)
            {
                repn1.DataSource = item.ListCompetenciasNivel1;
                repn1.DataBind();
            }

            Repeater repn2 = (Repeater)e.Item.FindControl("rptCompetenciasN2");
            if (repn2 != null)
            {
                repn2.DataSource = item.ListCompetenciasNivel2;
                repn2.DataBind();
            }

            Repeater repperf = (Repeater)e.Item.FindControl("rptNotaPerfomance");
            if (repperf != null)
            {
                repperf.DataSource = item.ListPerfomance;
                repperf.DataBind();
            }

            Repeater repSomaComp = (Repeater)e.Item.FindControl("rptSomaCompetencias");
            if (repSomaComp != null)
            {
                repSomaComp.DataSource = item.ListSomaCompetenciasN1N2;
                repSomaComp.DataBind();
            }

            Repeater repRadar = (Repeater)e.Item.FindControl("rptRadar");
            if (repRadar != null)
            {
                repRadar.DataSource = item.ListSomaCompetenciasN1N2;
                repRadar.DataBind();
            }


        }
        public string FormatPercentagem_Lideranca(decimal nota)
        {

            if (nota > 0)
            {
                return nota.ToString("##0") + "%";
            }

            return "0%";

        }
        public string FormatDecimal_Lideranca(decimal nota)
        {
            return Math.Round(nota, 2).ToString();

        }
        public string TruncarTexto_Lideranca(string texto, int qtdcaracteres)
        {

            if (!string.IsNullOrEmpty(texto))
            {
                if (texto.Length > qtdcaracteres)
                {
                    return string.Format("{0}...", texto.Substring(0, qtdcaracteres));
                }
            }

            return texto;
        }
        [WebMethod]
        public static string SalvarComplexidadeProjeto(int idprojeto, int idcomplexidade)
        {
            try
            {
                ProjetosService service = new ProjetosService();
                var projeto = service.ObterProjeto(idprojeto);

                bool atualizado = false;

                if (projeto.IdComplexidade != idcomplexidade)
                {
                    projeto.IdComplexidade = idcomplexidade;
                    bool ok = service.AlterarProjeto(projeto);

                    if (ok)
                    {
                        atualizado = true;
                    }
                }

                return JsonConvert.SerializeObject(new { atualizado = atualizado });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { erro = "Erro ao tentar salvar a complexidade do projeto: " + ex.Message });
            }
        }
        [WebMethod]
        public static string ComplexidadeProjeto(int id)
        {
            try
            {
                var listComplexidade = new ComplexidadesService().ObterProjetosComplexidade();
                var projeto = new ProjetosService().ObterProjeto(id);
                int idProjetoComplexidade = 0;

                if (projeto != null)
                {
                    idProjetoComplexidade = projeto.IdComplexidade;
                }

                return JsonConvert.SerializeObject(new { listcomplexidade = listComplexidade, idprojetocomplexidade = idProjetoComplexidade, nomeprojeto = projeto.Projeto });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { erro = "Erro ao tentar obter a lista de complexidade de projetos: " + ex.Message });
            }
        }

        protected void graficoSubs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RLM_Eixo rlmEixo = (RLM_Eixo)e.Item.DataItem;
            Chart chartSubComps = (Chart)e.Item.FindControl("chartSubCompetencias");

            foreach (var subComp in rlmEixo.subcompetencias)
            {
                if (subComp.notas_4_percent != 0)
                {
                    chartSubComps.Series[0].Points[rlmEixo.subcompetencias.IndexOf(subComp)].IsValueShownAsLabel = true;
                    chartSubComps.Series[0].Points[rlmEixo.subcompetencias.IndexOf(subComp)].Label = "#PERCENT{P0}";
                }
                if (subComp.notas_3_percent != 0)
                {
                    chartSubComps.Series[1].Points[rlmEixo.subcompetencias.IndexOf(subComp)].IsValueShownAsLabel = true;
                    chartSubComps.Series[1].Points[rlmEixo.subcompetencias.IndexOf(subComp)].Label = "#PERCENT{P0}";
                }
                if (subComp.notas_2_percent != 0)
                {
                    chartSubComps.Series[2].Points[rlmEixo.subcompetencias.IndexOf(subComp)].IsValueShownAsLabel = true;
                    chartSubComps.Series[2].Points[rlmEixo.subcompetencias.IndexOf(subComp)].Label = "#PERCENT{P0}";
                }
            }
        }

        protected void atualizaPainelLider()
        {
            // ESCONDE PAINEL PARA ATUALIZAÇÃO
            updResultados.Visible = false;

            // ALIMENTA REPEATER
            var periodosService = new PeriodoService();
            var notasService = new NotasAvaliacaoService();
            var lider = new AssociadosService().ObterAssociado(int.Parse(ddl_Lider.SelectedValue));
            var repeaterData = new List<ResultadoLiderModel>();
            var addResultadoCicloAtual = new ResultadoLiderModel();
            var addResultadoCicloAnterior = new ResultadoLiderModel();
            var cicloFiltrado = ddl_Ciclos.SelectedValue;
            var periodos = new PeriodoService().ListaPeriodos(1).Where(x => x.fl_lib_res_lideranca);
            var cicloAvaliado = cicloFiltrado == "" ? new PeriodoService().ObterPeriodoUltimo().IdPeriodo : int.Parse(cicloFiltrado);
            var cicloAnterior = periodos.OrderBy(x => x.IdPeriodo).Reverse().FirstOrDefault(x => x.IdPeriodo < cicloAvaliado && x.ATV == 1).IdPeriodo;
            var respostasCompetencias = Session["RESPOSTAS"] as List<AVALIACOESCOMPETENCIAS>;

            addResultadoCicloAtual.Lider = addResultadoCicloAnterior.Lider = lider.Nome;
            addResultadoCicloAtual.Ciclo = periodosService.ObterPeriodo(cicloAvaliado).Codigo;
            addResultadoCicloAnterior.Ciclo = periodosService.ObterPeriodo(cicloAnterior).Codigo;
            addResultadoCicloAtual.Ciclos = addResultadoCicloAnterior.Ciclos = (from periodo in respostasCompetencias.Select(x => x.PERIODOSAVALIACOES).Distinct()
                                   select new ResultadoLiderModel_Ciclos
                                   {
                                       idPeriodo = periodo.IdPeriodo,
                                       Ciclo = periodo.Codigo
                                   }).Reverse().ToList();

            var avaliacoes = Session["AVALIACOES"] as List<AVALIACAO>;
            var avaliacoesCicloAvaliado = avaliacoes.Where(x => x.idAssociado == lider.IdAssociado && x.TipoAvaliacao == "lideranca" && x.idPeriodo == cicloAvaliado).ToList();
            var avaliacoesCicloAnterior = avaliacoes.Where(x => x.idAssociado == lider.IdAssociado && x.TipoAvaliacao == "lideranca" && x.idPeriodo == cicloAnterior).ToList();
            var projetosAvaliadosCicloAvaliado = avaliacoesCicloAvaliado.Select(x => x.idProjeto).ToList();
            var projetosAvaliadosCicloAnterior = avaliacoesCicloAnterior.Select(x => x.idProjeto).ToList();
            var projetosCicloAvaliado = avaliacoesCicloAvaliado.Select(x => x.PROJETOS).Distinct();
            var projetosCicloAnterior = avaliacoesCicloAnterior.Select(x => x.PROJETOS).Distinct();
            projetosCicloAvaliado = projetosCicloAvaliado.Where(x => projetosAvaliadosCicloAvaliado.Contains(x.IdProjeto)).ToList();
            projetosCicloAnterior = projetosCicloAnterior.Where(x => projetosAvaliadosCicloAnterior.Contains(x.IdProjeto)).ToList();
            addResultadoCicloAtual.Projetos = (from projeto in projetosCicloAvaliado
                                     select new ResultadoLiderModel_Projetos
                                     {
                                         idProjeto = projeto.IdProjeto,
                                         Projeto = projeto.Projeto
                                     }).ToList();
            addResultadoCicloAnterior.Projetos = (from projeto in projetosCicloAnterior
                                                select new ResultadoLiderModel_Projetos
                                                {
                                                    idProjeto = projeto.IdProjeto,
                                                    Projeto = projeto.Projeto
                                                }).ToList();

            var resultadoTotalCicloAvaliado = getResultadoTotal(respostasCompetencias, cicloAvaliado, lider.IdAssociado);
            var resultadoTotalCicloAnterior = getResultadoTotal(respostasCompetencias, cicloAnterior, lider.IdAssociado);
            addResultadoCicloAtual.resultadoTotal = resultadoTotalCicloAvaliado;
            addResultadoCicloAnterior.resultadoTotal = resultadoTotalCicloAnterior;

            var jsonResultadoTotalCicloAvaliado = JsonConvert.SerializeObject(addResultadoCicloAtual.resultadoTotal);
            var jsonResultadoTotalCicloAnterior = JsonConvert.SerializeObject(addResultadoCicloAnterior.resultadoTotal);
            ScriptManager.RegisterStartupScript(this, GetType(), "geraGraficoTotal0", "geraGraficoNotaTotal('0'," + jsonResultadoTotalCicloAnterior + ");", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "geraGraficoTotal1", "geraGraficoNotaTotal('1'," + jsonResultadoTotalCicloAvaliado + ");", true);

            var resultadoPilaresCicloAvaliado = getResultadoPilares(respostasCompetencias, cicloAvaliado, lider.IdAssociado);
            var resultadoPilaresCicloAnterior = getResultadoPilares(respostasCompetencias, cicloAnterior, lider.IdAssociado);
            addResultadoCicloAtual.resultadoPilares = resultadoPilaresCicloAvaliado;
            addResultadoCicloAnterior.resultadoPilares = resultadoPilaresCicloAnterior;

            var resultadoSubcompetenciasCicloAvaliado = getResultadoSubcompetencias(respostasCompetencias, cicloAvaliado, lider.IdAssociado);
            var resultadoSubcompetenciaCicloAnterior = getResultadoSubcompetencias(respostasCompetencias, cicloAnterior, lider.IdAssociado);
            addResultadoCicloAtual.resultadoSubcompetencias = resultadoSubcompetenciasCicloAvaliado;
            addResultadoCicloAnterior.resultadoSubcompetencias = resultadoSubcompetenciaCicloAnterior;
            addResultadoCicloAtual.deltaMaior = getDeltasSubcompetencias_Maior(resultadoSubcompetenciasCicloAvaliado);
            addResultadoCicloAnterior.deltaMaior = getDeltasSubcompetencias_Maior(resultadoSubcompetenciaCicloAnterior);
            addResultadoCicloAtual.deltaMenor = getDeltasSubcompetencias_Menor(resultadoSubcompetenciasCicloAvaliado);
            addResultadoCicloAnterior.deltaMenor = getDeltasSubcompetencias_Menor(resultadoSubcompetenciaCicloAnterior);

            var palavrasLiderCicloAvaliado = getPalavras(respostasCompetencias, cicloAvaliado, lider.IdAssociado);
            var palavrasLiderCicloAnterior = getPalavras(respostasCompetencias, cicloAnterior, lider.IdAssociado);
            addResultadoCicloAtual.Palavras = palavrasLiderCicloAvaliado;
            addResultadoCicloAnterior.Palavras = palavrasLiderCicloAnterior;

            repeaterData.Add(addResultadoCicloAnterior);
            repeaterData.Add(addResultadoCicloAtual);

            rptResultadoLider.DataSource = repeaterData;
            rptResultadoLider.DataBind();

            // ATUALIZA CAMPOS FIXOS
            ddl_Ciclos.DataSource = addResultadoCicloAtual.Ciclos;
            ddl_Ciclos.DataValueField = "idPeriodo";
            ddl_Ciclos.DataTextField = "Ciclo";
            ddl_Ciclos.DataBind();
            ddl_Ciclos.Items.Insert(0, "Selecionar");
            ddl_Ciclos.Items[0].Value = "";
            ddl_Ciclos.Items[0].Enabled = false;

            var jsonPilaresCicloAnterior = JsonConvert.SerializeObject(addResultadoCicloAnterior.resultadoPilares.Pilares);
            var jsonResultadoPilaresLiderCicloAnterior = JsonConvert.SerializeObject(addResultadoCicloAnterior.resultadoPilares.resultadoPilaresLider);
            var jsonResultadoPilaresTodosCicloAnterior = JsonConvert.SerializeObject(addResultadoCicloAnterior.resultadoPilares.resultadoPilaresTodos);
            var jsonPilaresCicloAvaliado = JsonConvert.SerializeObject(addResultadoCicloAtual.resultadoPilares.Pilares);
            var jsonResultadoPilaresLiderCicloAvaliado = JsonConvert.SerializeObject(addResultadoCicloAtual.resultadoPilares.resultadoPilaresLider);
            var jsonResultadoPilaresTodosCicloAvaliado = JsonConvert.SerializeObject(addResultadoCicloAtual.resultadoPilares.resultadoPilaresTodos);
            ScriptManager.RegisterStartupScript(this, GetType(), "geraGraficoNotaPilares0",
                "geraGraficoNotaPilares('0','" + lider.Nome + "', " + jsonPilaresCicloAnterior + ", " +
                jsonResultadoPilaresLiderCicloAnterior + ", " +
                jsonResultadoPilaresTodosCicloAnterior + ");", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "geraGraficoNotaPilares1",
                "geraGraficoNotaPilares('1','" + lider.Nome + "', " + jsonPilaresCicloAvaliado + ", " +
                jsonResultadoPilaresLiderCicloAvaliado + ", " +
                jsonResultadoPilaresTodosCicloAvaliado + ");", true);

            var jsonSubcompetenciasCicloAnterior = JsonConvert.SerializeObject(addResultadoCicloAnterior.resultadoSubcompetencias.Subcompetencias);
            var jsonResultadoSubcompetenciasLiderCicloAnterior = JsonConvert.SerializeObject(addResultadoCicloAnterior.resultadoSubcompetencias.resultadoSubcompetenciasLider);
            var jsonResultadoSubcompetenciasTodosCicloAnterior = JsonConvert.SerializeObject(addResultadoCicloAnterior.resultadoSubcompetencias.resultadoSubcompetenciasTodos);
            var jsonSubcompetenciasCicloAvaliado = JsonConvert.SerializeObject(addResultadoCicloAtual.resultadoSubcompetencias.Subcompetencias);
            var jsonResultadoSubcompetenciasLiderCicloAvaliado = JsonConvert.SerializeObject(addResultadoCicloAtual.resultadoSubcompetencias.resultadoSubcompetenciasLider);
            var jsonResultadoSubcompetenciasTodosCicloAvaliado = JsonConvert.SerializeObject(addResultadoCicloAtual.resultadoSubcompetencias.resultadoSubcompetenciasTodos);
            ScriptManager.RegisterStartupScript(this, GetType(), "geraGraficoNotaSubcompetencias0",
                "geraGraficoNotaSubcompetencias('0','" + lider.Nome + "', " + jsonSubcompetenciasCicloAnterior + ", " +
                jsonResultadoSubcompetenciasLiderCicloAnterior + ", " +
                jsonResultadoSubcompetenciasTodosCicloAnterior + ");", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "geraGraficoNotaSubcompetencias1",
                "geraGraficoNotaSubcompetencias('1','" + lider.Nome + "', " + jsonSubcompetenciasCicloAvaliado + ", " +
                jsonResultadoSubcompetenciasLiderCicloAvaliado + ", " +
                jsonResultadoSubcompetenciasTodosCicloAvaliado + ");", true);

            updResultados.Visible = true;
        }
        protected void ddlCiclo_SelectedIndexChanged(object sender, EventArgs e)
        {
            atualizaPainelLider();
            updResultados.Update();
        }

        public List<Dictionary<object, object>> getResultadoTotal(List<AVALIACOESCOMPETENCIAS> respostas, int idPeriodo, int idLider)
        {
            var returnResultado = new List<Dictionary<object, object>>();
            var idNotasDesconsiderar = new List<int?>() { 0, 5, 10, null};
            var associadosService = new AssociadosService();
            var avaliacoesService = new AvaliacoesService();

            var respostasTodos = respostas.Where(x => x.IdPeriodo == idPeriodo);
            respostasTodos = respostasTodos.Where(x => !idNotasDesconsiderar.Contains(x.IdNotaNivel1AvaliacaoGestor)).ToList();
            respostasTodos = respostasTodos.Where(x => x.IdNotaNivel1AvaliacaoGestor != null).ToList();
            var respostasLider = respostasTodos.Where(x => x.IdAssociado == idLider).ToList();
            var listaNotas = avaliacoesService.ObterAvaliacaoCompetenciasNotas(-1);

            var notasTodos = new List<decimal?>();
            foreach (var item in respostasTodos)
            {
                notasTodos.Add(listaNotas.FirstOrDefault(x => x.IdNota == (int)item.IdNotaNivel1AvaliacaoGestor).Peso);
            }
            var notasLider = new List<decimal?>();
            foreach (var item in respostasLider)
            {
                notasLider.Add(listaNotas.FirstOrDefault(x => x.IdNota == (int)item.IdNotaNivel1AvaliacaoGestor).Peso);
            }

            var mediaTodos = notasTodos.Average();
            var mediaLider = notasLider.Average();

            returnResultado.Add(new Dictionary<object, object>());
            returnResultado[0].Add("year", associadosService.ObterAssociado(idLider).Nome.Split(' ')[0]);
            returnResultado[0].Add("count", mediaLider);
            returnResultado.Add(new Dictionary<object, object>());
            returnResultado[1].Add("year", "Média Líderes");
            returnResultado[1].Add("count", mediaTodos);

            return returnResultado;
        }

        public ResultadoLiderModel_Pilares getResultadoPilares(List<AVALIACOESCOMPETENCIAS> respostas, int idPeriodo, int idLider)
        {
            var returnResultado = new ResultadoLiderModel_Pilares();
            var idNotasDesconsiderar = new List<int?>() { 0, 5, 10, null };
            var associadosService = new AssociadosService();
            var avaliacoesService = new AvaliacoesService();

            var respostasTodos = respostas.Where(x => x.IdPeriodo == idPeriodo);
            respostasTodos = respostasTodos.Where(x => !idNotasDesconsiderar.Contains(x.IdNotaNivel1AvaliacaoGestor)).ToList();
            respostasTodos = respostasTodos.Where(x => x.IdNotaNivel1AvaliacaoGestor != null).ToList();
            var respostasLider = respostasTodos.Where(x => x.IdAssociado == idLider).ToList();

            var pilares = respostasLider.Select(x => x.COMPETENCIAS.EIXOS).Distinct();
            returnResultado.Pilares = new List<string>();
            returnResultado.resultadoPilaresLider = new List<decimal?>();
            returnResultado.resultadoPilaresTodos = new List<decimal?>();

            foreach (var pilar in pilares)
            {
                var respostasPilarTodos = respostasTodos.Where(x => x.COMPETENCIAS.EIXOS.IdEixo == pilar.IdEixo);
                var respostasPilarLider = respostasLider.Where(x => x.COMPETENCIAS.EIXOS.IdEixo == pilar.IdEixo);

                var notasTodos = new List<decimal?>();
                foreach (var item in respostasPilarTodos)
                {
                    notasTodos.Add(avaliacoesService.ObterAvaliacaoCompetenciaNota((int)item.IdNotaNivel1AvaliacaoGestor).Peso);
                }
                var notasLider = new List<decimal?>();
                foreach (var item in respostasPilarLider)
                {
                    notasLider.Add(avaliacoesService.ObterAvaliacaoCompetenciaNota((int)item.IdNotaNivel1AvaliacaoGestor).Peso);
                }

                returnResultado.Pilares.Add(pilar.Eixo);
                returnResultado.resultadoPilaresTodos.Add(notasTodos.Average());
                returnResultado.resultadoPilaresLider.Add(notasLider.Average());
            }

            return returnResultado;
        }

        public ResultadoLiderModel_Subcompetencias getResultadoSubcompetencias(List<AVALIACOESCOMPETENCIAS> respostas, int idPeriodo, int idLider)
        {
            var returnResultado = new ResultadoLiderModel_Subcompetencias();
            var idNotasDesconsiderar = new List<int?>() { 0, 5, 10, null };
            var associadosService = new AssociadosService();
            var avaliacoesService = new AvaliacoesService();

            var respostasTodos = respostas.Where(x => x.IdPeriodo == idPeriodo);
            respostasTodos = respostasTodos.Where(x => !idNotasDesconsiderar.Contains(x.IdNotaNivel1AvaliacaoGestor)).ToList();
            respostasTodos = respostasTodos.Where(x => x.IdNotaNivel1AvaliacaoGestor != null).ToList();
            var respostasLider = respostasTodos.Where(x => x.IdAssociado == idLider).ToList();

            var pilares = respostasLider.Select(x => x.COMPETENCIAS.SUBCOMPETENCIAS).Distinct();
            returnResultado.Subcompetencias = new List<string>();
            returnResultado.resultadoSubcompetenciasLider = new List<decimal?>();
            returnResultado.resultadoSubcompetenciasTodos = new List<decimal?>();

            foreach (var pilar in pilares)
            {
                var respostasPilarTodos = respostasTodos.Where(x => x.COMPETENCIAS.SUBCOMPETENCIAS.IdSubCompetencia == pilar.IdSubCompetencia);
                var respostasPilarLider = respostasLider.Where(x => x.COMPETENCIAS.SUBCOMPETENCIAS.IdSubCompetencia == pilar.IdSubCompetencia);

                var notasTodos = new List<decimal?>();
                foreach (var item in respostasPilarTodos)
                {
                    notasTodos.Add(avaliacoesService.ObterAvaliacaoCompetenciaNota((int)item.IdNotaNivel1AvaliacaoGestor).Peso);
                }
                var notasLider = new List<decimal?>();
                foreach (var item in respostasPilarLider)
                {
                    notasLider.Add(avaliacoesService.ObterAvaliacaoCompetenciaNota((int)item.IdNotaNivel1AvaliacaoGestor).Peso);
                }

                returnResultado.Subcompetencias.Add(pilar.SubCompetencia);
                returnResultado.resultadoSubcompetenciasTodos.Add(notasTodos.Average());
                returnResultado.resultadoSubcompetenciasLider.Add(notasLider.Average());
            }

            return returnResultado;
        }

        public List<ResultadoLiderModel_Palavras> getPalavras(List<AVALIACOESCOMPETENCIAS> respostas, int idPeriodo, int idLider)
        {
            var returnResultado = new List<ResultadoLiderModel_Palavras>();
            var idNotasDesconsiderar = new List<int?>() { 0, 5, 10, null };
            var associadosService = new AssociadosService();
            var getPalavrasIrrelevantes = new AvaliacoesService().ObterPalavrasIrrelevantes();
            var palavrasIrrelevantes = getPalavrasIrrelevantes.Select(x => x.Palavra.ToLower()).ToList();

            var respostasTodos = respostas.Where(x => x.IdPeriodo == idPeriodo);
            respostasTodos = respostasTodos.Where(x => !idNotasDesconsiderar.Contains(x.IdNotaNivel1AvaliacaoGestor)).ToList();
            respostasTodos = respostasTodos.Where(x => x.IdNotaNivel1AvaliacaoGestor != null).ToList();
            var respostasLider = respostasTodos.Where(x => x.IdAssociado == idLider).ToList();

            var comentariosLider = respostasLider.Select(x => x.ComentariosAvaliacaoGestor.Split(' ')).ToList();
            var palavrasLider = new List<string>();
            foreach (var item in comentariosLider)
            {
                var addItem = item.Select(x => x.Replace(",", " ").Replace(".", " ").Replace(";", " ").Replace("/", " ").Replace("(", " ").Replace(")", " ").Replace("-", " ")
                .Replace("\"", " ").Replace(":", " ").Replace("?", " ").Replace(Environment.NewLine, " ")).ToList();
                addItem = addItem.SelectMany(x => x.Split(' ')).ToList();
                addItem = addItem.Select(x => x.ToLower()).ToList();

                palavrasLider.AddRange(addItem);
            }
            palavrasLider = palavrasLider.Where(x => !palavrasIrrelevantes.Contains(x.ToLower())).ToList();

            var palavrasLiderDistintas = palavrasLider.Distinct();
            foreach (var item in palavrasLiderDistintas)
            {
                var palavraPeso = palavrasLider.Count(x => x == item);

                if (palavraPeso <= 1)
                {
                    continue;
                }

                var addPalavra = new ResultadoLiderModel_Palavras();
                addPalavra.Palavra = item;
                addPalavra.DataWeight = palavraPeso.ToString();
                returnResultado.Add(addPalavra);
            }

            Random rng = new Random();
            returnResultado = returnResultado.OrderBy(x => rng.Next()).ToList();

            return returnResultado;
        }

        protected void NotificarLideres()
        {
            var respostasCompetencias = Session["RESPOSTAS"] as List<AVALIACOESCOMPETENCIAS>;
            var associados = new AssociadosService().ObterAssociados();
            var lideresIds = respostasCompetencias.Select(x => x.IdAssociado);
            var lideres = associados.Where(x => lideresIds.Contains(x.IdAssociado)).Distinct();
            var ultimoPeriodo = new PeriodoService().ObterPeriodoUltimo();

            var paramEmail = new EmailParametroService().ObterParametro(1);
            var configEmail = new ConfigEmail();

            configEmail.From = paramEmail.RemetenteEmail;
            configEmail.SmtpServer = paramEmail.SMTPServer;
            configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
            configEmail.Dominio = paramEmail.Dominio;
            configEmail.Senha = paramEmail.Password;
            configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
            configEmail.Remetente = paramEmail.RemetenteNome;

            MailAddressCollection BCCs = new MailAddressCollection();

            foreach (var lider in lideres)
            {
                BCCs.Add(lider.Email);
            }

            var destinatario = new Destinatario();
            destinatario.Nome = "Avaliação";
            destinatario.Email = "avaliacao@peers.com.br";

            try
            {
                // MONTA O EMAIL
                var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
                _corpo = ConfiguraBody_ResultadoLideranca(_corpo, ultimoPeriodo.Periodo);

                var mensagem = new Mensagem
                {
                    Titulo = "[RH Peers] - Processo de Avaliação - Resultado Av. de Liderança - Ciclo: " + ultimoPeriodo.Periodo,
                    Corpo = _corpo
                };

                var emailService = new EmailService(configEmail, destinatario, mensagem);//, BCCs);

                var thread = new Thread(new ThreadStart(emailService.Enviar));
                thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
                thread.IsBackground = true;
                thread.Start();

            }
            catch (Exception)
            {
                MessageBox.Show("Falha ao enviar e-mail de notificação!", "Notificação", TIPO.Default, MessageBoxHandler);
            }
        }

        private string ConfiguraBody_ResultadoLideranca(string corpo, string periodo)
        {
            // Dados Pessoais
            var newCorpo = corpo.Replace("[NOME]", "Líder");
            newCorpo = newCorpo.Replace("[DESCRICAO]", "Seu resultado da última avaliação de liderança está disponível.");

            // ALTERAR MENSAGENS PADRÃO
            newCorpo = newCorpo.Replace("http://avaliacao.peers.com.br/login", "http://avaliacao.peers.com.br/avalizacao_resultado_lideranca.aspx");
            newCorpo = newCorpo.Replace("Link para acesso ao Sistema de Avaliação", "Link para acesso à página (necessário estar logado)");

            // REMOVER CAMPOS NÃO UTILIZADOS
            newCorpo = newCorpo.Replace("<p>Seguem informações do processo de avaliação.</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Etapa atual:</strong> [ETAPA_AVALIACAO]</p>", "");
            newCorpo = newCorpo.Replace("<p style=\"color:#ff0000;\"><strong>Prazo para finalização da [ETAPA_AVALIACAO]:</strong> [PRAZO_FINAL]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Projeto:</strong> [PROJETO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Associado:</strong> [NOME_AVALIADO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Cargo:</strong> [CARGO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Mentor:</strong> [MENTOR]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Avaliador:</strong> [AVALIADOR]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Gestor do Projeto:</strong> [GESTOR]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Data de Início:</strong> [DATA_INICIO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Data de Término:</strong> [DATA_FINAL]</p>", "");

            return newCorpo;
        }

        protected void btn_NotificarLideres_Click1(object sender, EventArgs e)
        {
            NotificarLideres();
            MessageBox.Show("Emails disparados aos líderes com sucesso.", "Sucesso", TIPO.Info, MessageBoxHandler);
        }

        public List<ResultadoLiderModel_Subcompetencias_Delta> getDeltasSubcompetencias_Maior(ResultadoLiderModel_Subcompetencias ciclo)
        {
            var returnModel = new List<ResultadoLiderModel_Subcompetencias_Delta>();

            decimal? maiorAtual = -1;
            foreach (var item in ciclo.resultadoSubcompetenciasLider)
            {
                if (item > maiorAtual)
                {
                    maiorAtual = item;
                }
            }
            var index = 0;
            foreach (var item in ciclo.resultadoSubcompetenciasLider)
            {
                var addDelta = new ResultadoLiderModel_Subcompetencias_Delta();
                if (item >= maiorAtual)
                {
                    addDelta.Subcompetencia = ciclo.Subcompetencias[index];
                    addDelta.Resultado = (decimal?)Math.Round((double)ciclo.resultadoSubcompetenciasLider[index], 2);
                    returnModel.Add(addDelta);
                }

                index += 1;
            }
            return returnModel;
        }
        public List<ResultadoLiderModel_Subcompetencias_Delta> getDeltasSubcompetencias_Menor(ResultadoLiderModel_Subcompetencias ciclo)
        {
            var returnModel = new List<ResultadoLiderModel_Subcompetencias_Delta>();

            decimal? menorAtual = -1;
            foreach (var item in ciclo.resultadoSubcompetenciasLider)
            {
                if (item < menorAtual || menorAtual == -1)
                {
                    menorAtual = item;
                }
            }
            var index = 0;
            foreach (var item in ciclo.resultadoSubcompetenciasLider)
            {
                var addDelta = new ResultadoLiderModel_Subcompetencias_Delta();
                if (item <= menorAtual)
                {
                    addDelta.Subcompetencia = ciclo.Subcompetencias[index];
                    addDelta.Resultado = (decimal?)Math.Round((double)ciclo.resultadoSubcompetenciasLider[index], 2);
                    returnModel.Add(addDelta);
                }

                index += 1;
            }
            return returnModel;
        }
    }
}