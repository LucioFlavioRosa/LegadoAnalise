using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class avalizacao_resultado : System.Web.UI.Page
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
                    if (user.IdPerfil < 3)
                    {
                        Response.Redirect("~/Index");
                    }
                }

                if (!IsPostBack)
                {
                    var strAssociado = Request.QueryString["IdAssociado"];
                    var strPeriodo = Request.QueryString["IdPeriodo"];
                    var strTipoAvaliacao = Request.QueryString["TipoAvaliacao"];
                    var strEscopo = Request.QueryString["Escopo"];

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

                    if (strTipoAvaliacao == "lideranca")
                    {
                        Response.Redirect($"~/avalizacao_resultado_lideranca.aspx?&IdAssociado=" + idAssociado.ToString() + "&IdPeriodo=" + idPeriodo.ToString() + "&TipoAvaliacao=" + strTipoAvaliacao +
                            "&Escopo=" + strEscopo);
                    }

                    PeriodoService periodoService = new PeriodoService();

                    PERIODOSAVALIACOES periodo = periodoService.ObterPeriodo(idPeriodo);
                    var util = new Util();
                    var resultadoService = new ResultadoServices();

                    lblPeriodo.InnerText = periodo.Periodo;
                    lblAssociado.InnerText = associado.Nome;
                    lblMentor.InnerText = associado.Mentor;
                    lblCargo.InnerText = associado.Cargo;
                    lblProximoCargo.InnerText = associado.ProximoCargo;
                    // TEMPO DE PEERS E TEMPO DE CARGO
                    lblTempoPeers.InnerText = util.TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDePeers).ToString();
                    lblTempoCargo.InnerText = util.TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDeCargo).ToString();

                    DataModel dtMod = new DataModel();
                    string foto = "";

                    var fotoAssociado = dtMod.FOTOSASSOCIADOS.FirstOrDefault(x => x.IdAssociado == idAssociado);

                    if (fotoAssociado == null) { foto = "assets/images/users/usernophoto.jpg"; }
                    else { foto = fotoAssociado.Imagem; }

                    imgUser1Comite.Src = foto;

                    WebStorage.Set("IdAssociado", idAssociado.ToString());
                    WebStorage.Set("IdPeriodo", idPeriodo.ToString());
                    WebStorage.Set("TipoAvaliacao", strTipoAvaliacao);
                    WebStorage.Set("Escopo", strEscopo);

                    var avaliacoesService = new AvaliacoesService();
                    var cargosService = new CargosService();
                    var associadosService = new AssociadosService();
                    var competenciasService = new CompetenciasService();
                    var performancesService = new PerformancesService();

                    var getAvaliacoes = avaliacoesService.ObterListaAvaliacao();
                    var getCompetencias = competenciasService.ObterListaCompetencias();
                    var getPerformances = performancesService.ObterListaPerformances();
                    var getAvPerformances = avaliacoesService.ObterAvaliacaoPerformanceTodos(-1, -1, idPeriodo);

                    var getAvCompetencias = avaliacoesService.ListaAvCompetencias();
                    getAvCompetencias = getAvCompetencias.Where(x => x.ATV == 1).ToList();
                    getAvCompetencias = getAvCompetencias.Where(x => x.IdAvaliacaoStatus == 3).ToList();
                    getAvCompetencias = getAvCompetencias.Where(x => x.IdPeriodo == idPeriodo).ToList();

                    var resultadoAssociado = resultadoService.ObterResultadoAssociado_Otimizado(getAvaliacoes, getAvCompetencias, getCompetencias, getAvPerformances, getPerformances,
                        idAssociado, idPeriodo, associado.IdCargo, strTipoAvaliacao, strEscopo);
                   
                    this.rptProjetos.ItemDataBound += RptProjetos_ItemDataBound;
                    this.rptProjetos.DataSource = resultadoAssociado;
                    this.rptProjetos.DataBind();

                    var somaProjetos = resultadoService.SomaResultadoProjetos(resultadoAssociado);

                    this.rptSomaProjetos.ItemDataBound += RptSomaProjetos_ItemDataBound;
                    this.rptSomaProjetos.DataSource = somaProjetos;
                    this.rptSomaProjetos.DataBind();


                    //var radar = JsonRadar(resultadoAssociado);
                    //hfJsonRadar.Value = radar;

                    //var somaRadar = JsonSomaRadar(somaProjetos.FirstOrDefault());
                    //hfJsonSomaRadar.Value = somaRadar;

                    // CONSIDERAÇÕES MENTOR
                    CarregarCombosConsideracoes();
                    CarregarConsideracoesMentor();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", TIPO.Error, MessageBoxHandler);
            }

        }

        //private string JsonSomaRadar(ResultadoSomaProjetosModel somaProjetos)
        //{
        //    dynamic obj = new JObject();
        //    List<string> labels = new List<string>();
        //    List<decimal> datasetCompetencia = new List<decimal>();
        //    List<decimal> datasetproximonivel = new List<decimal>();

        //    var nivelAtual = Math.Ceiling(somaProjetos.SomaProjetosNotaCompetenciaRadar.Value / 100m) * 100m;

        //    foreach (var item in somaProjetos.ListProjetosSomaCompetenciasN1N2)
        //    {
        //        labels.Add(item.Eixo);
        //        datasetCompetencia.Add(item.NotaProjetoCompetenciaRadar.HasValue ? item.NotaProjetoCompetenciaRadar.Value : 0);
        //        datasetproximonivel.Add(nivelAtual);
        //    }

        //    obj.labels = new JArray(labels);
        //    obj.datasetcompetencias = new JArray(datasetCompetencia);
        //    obj.datasetproximonivel = new JArray(datasetproximonivel);

        //    return JsonConvert.SerializeObject(obj);
        //}

        //private string JsonRadar(List<ResultadoProjetosModel> listprojetos)
        //{
        //    var listradar = new List<dynamic>();

        //    foreach (var projeto in listprojetos)
        //    {
        //        dynamic obj = new JObject();
        //        obj.idprojeto = projeto.IdProjeto;

        //        List<string> labels = new List<string>();
        //        List<decimal> datasetCompetencia = new List<decimal>();
        //        List<decimal> datasetNivelAtual = new List<decimal>();

        //        var nivelAtual = Math.Ceiling(projeto.SomaNotaCompetenciaRadar.Value / 100m) * 100m;

        //        foreach (var item in projeto.ListSomaCompetenciasN1N2)
        //        {
        //            labels.Add(item.Eixo);
        //            datasetCompetencia.Add(item.NotaCompetenciaRadar.HasValue ? item.NotaCompetenciaRadar.Value : 0);
        //            datasetNivelAtual.Add(nivelAtual);
        //        }

        //        obj.labels = new JArray(labels);
        //        obj.datasetcompetencias = new JArray(datasetCompetencia);
        //        obj.datasetnivelatual = new JArray(datasetNivelAtual);

        //        listradar.Add(obj);
        //    }

        //    return JsonConvert.SerializeObject(listradar);

        //}

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

            int IdAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
            //ClientScript.RegisterStartupScript(this.GetType(), "CarregarPainelTotal", "javascript:CarregarPainelTotal(" + IdAssociado + ", " + idPeriodo + ");", true);
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

            int IdAssociado = item.IdAssociado;
            int IdPeriodo = item.IdPeriodo;
            int IdProjeto = item.IdProjeto;
            //ClientScript.RegisterStartupScript(this.GetType(), "CarregarPainelProjeto", "javascript:CarregarPainelProjeto('CompEixo', " + IdProjeto + ", " + IdAssociado + ", " + IdPeriodo + ", 'ReportSection');", true);
        }

        public string FormatPercentagem(object nota)
        {

            if (nota != null)
            {
                var notaFormatted = Convert.ToDecimal(nota);
                return notaFormatted.ToString("##0") + "%";
            }

            return "0%";

        }

        public string FormatDecimal(object nota)
        {
            if (nota != null)
            {
                var notaFormatted = Convert.ToDecimal(nota);
                return Math.Round(notaFormatted, 2).ToString();
            }
            return "0";

        }

        public string TruncarTexto(string texto, int qtdcaracteres)
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

        //[WebMethod]
        //public static string SalvarComplexidadeProjeto(int idprojeto, int idcomplexidade)
        //{
        //    try
        //    {
        //        ProjetosService service = new ProjetosService();
        //        var projeto = service.ObterProjeto(idprojeto);

        //        bool atualizado = false;

        //        if (projeto.IdComplexidade != idcomplexidade)
        //        {
        //            projeto.IdComplexidade = idcomplexidade;
        //            bool ok = service.AlterarProjeto(projeto);

        //            if (ok)
        //            {
        //                atualizado = true;
        //            }
        //        }

        //        return JsonConvert.SerializeObject(new { atualizado = atualizado });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonConvert.SerializeObject(new { erro = "Erro ao tentar salvar a complexidade do projeto: " + ex.Message });
        //    }
        //}

        //[WebMethod]
        //public static string ComplexidadeProjeto(int id)
        //{
        //    try
        //    {
        //        var listComplexidade = new ComplexidadesService().ObterProjetosComplexidade();
        //        var projeto = new ProjetosService().ObterProjeto(id);
        //        int idProjetoComplexidade = 0;

        //        if (projeto != null)
        //        {
        //            idProjetoComplexidade = projeto.IdComplexidade;
        //        }

        //        return JsonConvert.SerializeObject(new { listcomplexidade = listComplexidade, idprojetocomplexidade = idProjetoComplexidade, nomeprojeto = projeto.Projeto });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonConvert.SerializeObject(new { erro = "Erro ao tentar obter a lista de complexidade de projetos: " + ex.Message });
        //    }
        //}

        public void CarregarCombosConsideracoes()
        {
            ddlElegivelPromocao.Items.Clear();
            ddlElegivelPromocao.Items.Insert(0, "Não");
            ddlElegivelPromocao.Items.Insert(1, "Sim");
            ddlInputPromocao.Items.Clear();
            ddlInputPromocao.Items.Insert(0, "Não");
            ddlInputPromocao.Items.Insert(1, "Sim");
        }
        public void CarregarConsideracoesMentor()
        {
            int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
            string TipoAvaliacao = WebStorage.Get("TipoAvaliacao", "0");
            string Escopo = WebStorage.Get("Escopo", "0");

            var associadosService = new AssociadosService();
            var cargosService = new CargosService();
            var consideracoesMentorService = new ConsideracoesMentorService();
            var avaliacoesService = new AvaliacoesService();
            var projetosService = new ProjetosService();
            var complexidadeService = new ComplexidadesService();

            var mentor = associadosService.ObterAssociado(associadosService.ObterAssociado(idAssociado).IdAssociadoMentor);

            var consideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorPorAtributos(mentor.IdAssociado, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
            if (consideracoesMentor == null)
            {
                CONSIDERACOESMENTOR addConsideracoesMentor = new CONSIDERACOESMENTOR();
                addConsideracoesMentor.idMentor = mentor.IdAssociado;
                addConsideracoesMentor.idAssociado = idAssociado;
                addConsideracoesMentor.idPeriodo = idPeriodo;
                addConsideracoesMentor.TipoAvaliacao = TipoAvaliacao;
                addConsideracoesMentor.Escopo = Escopo;
                addConsideracoesMentor.LiberadoRH = false;
                addConsideracoesMentor.AcaoComite = "-";
                addConsideracoesMentor.PontosFortesRH = "-";
                addConsideracoesMentor.PontosFracosRH = "-";
                addConsideracoesMentor.SalarioAtual = 1;
                addConsideracoesMentor.SalarioNovo = 1;
                addConsideracoesMentor.RegimeContratacaoAtual = "-";
                addConsideracoesMentor.RegimeContratacaoNovo = "-";
                addConsideracoesMentor.MentoriaRealizada = false;

                CONSIDERACOESMENTOR ultimaConsideracoesMentor = new CONSIDERACOESMENTOR();
                ultimaConsideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorUltimaDoAvaliado(idAssociado, idPeriodo, TipoAvaliacao, Escopo);
                if (ultimaConsideracoesMentor != null && ultimaConsideracoesMentor.idConsideracoesMentor > 0)
                {
                    addConsideracoesMentor.SalarioAtual = ultimaConsideracoesMentor.SalarioAtual;
                    addConsideracoesMentor.RegimeContratacaoAtual = ultimaConsideracoesMentor.RegimeContratacaoAtual;
                }

                consideracoesMentorService.AdicionarConsideracoesMentor(addConsideracoesMentor);
                consideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorPorAtributos(mentor.IdAssociado, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
            }

            WebStorage.Set("idConsideracoesMentor", consideracoesMentor.idConsideracoesMentor.ToString());

            var avaliacoes = avaliacoesService.ObterAvaliacoesAssociadoSemestre(idAssociado, idPeriodo, TipoAvaliacao);
            string addTextProjetosEnvolvidos = "";
            foreach (var avaliacao in avaliacoes)
            {
                addTextProjetosEnvolvidos += projetosService.ObterProjeto(avaliacao.idProjeto).Projeto + " (" +
                    complexidadeService.ObterComplexidade(projetosService.ObterProjeto(avaliacao.idProjeto).IdComplexidade).Complexidade + ")";
                if (avaliacoes.IndexOf(avaliacao) < avaliacoes.Count - 1) { addTextProjetosEnvolvidos += "<br />"; }
            }

            var cargoAtual = cargosService.ObterCargo(associadosService.ObterAssociado(consideracoesMentor.idAssociado).IdCargo);
            var associado = associadosService.ObterAssociado(consideracoesMentor.idAssociado);
            var ultimaPromocao = cargosService.ObterUltimaPromocaoAssociado(associado.IdAssociado);

            // ACCORDION CONSIDERAÇÕES MENTOR
            ddlElegivelPromocao.SelectedIndex = Convert.ToInt32(consideracoesMentor.ElegivelPromocao);
            ddlInputPromocao.SelectedIndex = Convert.ToInt32(consideracoesMentor.InputPromocao);
            textTrajetoria.Text = consideracoesMentor.TrajetoriaAssociado;
            textPontosFortes.Text = consideracoesMentor.PontosFortes;
            textPontosFracos.Text = consideracoesMentor.PontosFracos;
            labelAssociado.Text = associado.Nome;
            labelCargo.Text = cargoAtual.Cargo;
            labelVertical.Text = associado.Vertical;
            labelProjetosEnvolvidos.Text = addTextProjetosEnvolvidos;
            cboxMentoriaRealizada.Checked = consideracoesMentor.MentoriaRealizada;
            labelTempoDePeers.Text = "-";
            labelTempoDeCargo.Text = "-";
            if (ultimaPromocao != null)
            {
                int promocaoAnos = DateTime.Now.Year - ((DateTime)ultimaPromocao.DataPromocao).Year;
                int promocaoMesesTotal = (promocaoAnos * 12) + DateTime.Now.Month - ((DateTime)ultimaPromocao.DataPromocao).Month;
                int promocaoMesesRestantes = promocaoMesesTotal - (promocaoAnos * 12);
                labelTempoDeCargo.Text =
                    (promocaoAnos > 0 ? promocaoAnos.ToString() + " ano" + (promocaoAnos > 1 ? "s" : "") : "") +
                    (promocaoAnos > 0 && promocaoMesesRestantes > 0 ? " e " : "") +
                    (promocaoMesesRestantes > 0 ? promocaoMesesRestantes.ToString() +
                        (promocaoMesesRestantes > 1 ? " meses" : " mês") : "");
            }
            if (associado.DataAdmissao != null)
            {
                int admissaoAnos = DateTime.Now.Year - ((DateTime)associado.DataAdmissao).Year;
                int admissaoMesesTotal = (admissaoAnos * 12) + DateTime.Now.Month - ((DateTime)associado.DataAdmissao).Month;
                int admissaoMesesRestantes = admissaoMesesTotal - (admissaoAnos * 12);

                labelTempoDePeers.Text =
                    (admissaoAnos > 0 ? admissaoAnos.ToString() + " ano" + (admissaoAnos > 1 ? "s" : "") : "") +
                    (admissaoAnos > 0 && admissaoMesesRestantes > 0 ? " e " : "") +
                    (admissaoMesesRestantes > 0 ? admissaoMesesRestantes.ToString() +
                        (admissaoMesesRestantes > 1 ? " meses" : " mês") : "");

                if (admissaoMesesTotal <= 12)
                {
                    labelElegivelPromocao.Text = "Readequação";
                }
                else
                {
                    if (ultimaPromocao != null)
                    {
                        int promocaoAnos = DateTime.Now.Year - ((DateTime)ultimaPromocao.DataPromocao).Year;
                        int promocaoMesesTotal = (promocaoAnos * 12) + DateTime.Now.Month - ((DateTime)ultimaPromocao.DataPromocao).Month;
                        if (promocaoMesesTotal >= cargoAtual.TempoMinimoPromocao)
                        {
                            labelElegivelPromocao.Text = "Sim";
                        }
                        else { labelElegivelPromocao.Text = "Não"; }
                    }
                    else { labelElegivelPromocao.Text = "Não há última promoção!"; }
                }
            }
            else { labelElegivelPromocao.Text = "Não há data de admissão!"; }

            // TEMPORÁRIO ENQUANTO O RH REFAZ O RACIONAL DE ELEBIGILIDADE
            labelElegivelPromocao.Text = consideracoesMentor.ElegivelPromocao ? "Sim" : "Não";
        }

    }
}