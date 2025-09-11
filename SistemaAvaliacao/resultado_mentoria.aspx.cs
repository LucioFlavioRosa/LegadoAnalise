using Business.DataAccess;
using Business.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Business.Model;
using System.Linq;
using Business.Util;

using System.Web.UI;


namespace SistemaAvaliacao
{
    public partial class resultado_mentoria : System.Web.UI.Page
    {

        private static PeriodoService periodoService = new PeriodoService();
        private static AssociadosService associadoService = new AssociadosService();
        private static MentoriaService mentoriaService = new MentoriaService();

        private const double NOTA_MAXIMA = 5;

        private static List<PERIODOSAVALIACOES> periodos = periodoService.ListaTodosPeriodosLiberadosMentoria(1);
        private static PERIODOSAVALIACOES ultimoPeriodo = periodoService.ObterPeriodoUltimoLiberadoMentoria();
        private ASSOCIADOS mentor;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                mentor = associadoService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
                LimparCampos();
            }
        }

        private void LimparCampos()
        {
            carregaRespostas();
            carregaAvaliacoes();
        }

        public void carregaRespostas()
        {

            var pdiPillsModel = new List<PDIPillsModel>();
            foreach (var periodo in periodos)
            {

                var getRespostasMentorado = mentoriaService.ObterMentoradoRespostas(idPeriodo: periodo.IdPeriodo, idMentor: mentor.IdAssociado);


                if (getRespostasMentorado != null && getRespostasMentorado.Count > 0)
                {
                    var addPill = new PDIPillsModel();
                    addPill.id = "tab-" + periodo.IdPeriodo.ToString() + "-tab";
                    addPill.href = "#tab-" + periodo.IdPeriodo.ToString();
                    addPill.ariacontrols = "tab-" + periodo.IdPeriodo.ToString();
                    addPill.ariaselected = periodo.IdPeriodo == ultimoPeriodo.IdPeriodo ? "true" : "false";
                    addPill.active = periodo.IdPeriodo == ultimoPeriodo.IdPeriodo ? "active" : "";
                    addPill.classe = periodo.IdPeriodo == ultimoPeriodo.IdPeriodo ? "btn btn-danger" : "btn btn-facebook";
                    addPill.Periodo = periodo.Periodo;

                    pdiPillsModel.Add(addPill);
                }
            }

            rptPills.DataSource = pdiPillsModel;
            rptPills.DataBind();
        }
        public void carregaAvaliacoes()
        {
            var pillModel = new List<MentoradoRespostaPill>();

            var getPeriodo = periodos
                    .OrderByDescending(x => x.IdPeriodo)
                    .Select(x => x.IdPeriodo);

            var getRespostasPeers = mentoriaService.ObterMentoradoRespostas();
            var getRespostasTodas = getRespostasPeers.Where(x => x.idMentor == mentor.IdAssociado);
            var periodosTodos = getRespostasTodas.Select(x => x.idPeriodo).Distinct().OrderBy(x => x).ToList();
            periodosTodos = periodosTodos.Where(x => getPeriodo.Contains(x)).Distinct().OrderBy(x => x).ToList();

            foreach (var periodo in periodosTodos)
            {
                var addPeriodo = new MentoradoRespostaPill();
                addPeriodo.idPeriodo = periodo;
                addPeriodo.Periodo = periodos.Where(x => x.IdPeriodo == periodo).FirstOrDefault().Periodo;
                addPeriodo.id = "tab-" + periodo.ToString();
                addPeriodo.active = periodo == ultimoPeriodo.IdPeriodo ? "active in show" : "";
                addPeriodo.arialabelled = "tab-" + periodo.ToString() + "-tab";
                addPeriodo.notaMaxima = NOTA_MAXIMA;
                addPeriodo.MentorNome = mentor.Nome;
                addPeriodo.MentorFoto = mentor.FotoNome;
                addPeriodo.resultados = new List<MentoradoRespostasModel>();

                var getRespostasPeersPeriodo = getRespostasPeers.Where(x => x.idPeriodo == periodo).ToList();
                var getRespostasPeriodo = getRespostasTodas.Where(x => x.idPeriodo == periodo).ToList();

                addPeriodo.CountMentores = getRespostasPeersPeriodo.Select(x => x.idMentor).Distinct().Count();
                addPeriodo.CountMentorados = getRespostasPeriodo.Select(x => x.idMentorado).Distinct().Count();

                var getPerguntas = mentoriaService.ObterMentorPerguntas();
                foreach (var pergunta in getPerguntas)
                {
                    var respostaPerguntaPeers = getRespostasPeersPeriodo.Where(x => x.idPergunta == pergunta.idMentorPergunta).ToList();
                    var respostaPerguntaMentor = getRespostasPeriodo.Where(x => x.idPergunta == pergunta.idMentorPergunta).ToList();

                    if (respostaPerguntaMentor.Count > 0)
                    {

                        var notaPeers = calculaNota(respostaPerguntaPeers);

                         var notaMentor = calculaNota(respostaPerguntaMentor);

                        var item = respostaPerguntaMentor.FirstOrDefault();

                        var addResposta = getRespostaModel(item, notaPeers, notaMentor);
                        addResposta.showNotaMentor = addPeriodo.CountMentorados > 1;
                        addResposta.HiddenVelocimetro = addPeriodo.CountMentorados <= 1 ? "hidden" : "";
                        addPeriodo.resultados.Add(addResposta);
                    }
                }

                var getAvgResultadoMentor = addPeriodo.resultados.Select(x => x.ResultadoMentor).Average();
                var getAvgResultadoPeers = addPeriodo.resultados.Select(x => x.ResultadoPeers).Average();
                addPeriodo.mediaMentor = getAvgResultadoMentor != null ? Math.Round((double)getAvgResultadoMentor, 2) : getAvgResultadoMentor;
                addPeriodo.mediaPeers = getAvgResultadoPeers != null ? Math.Round((double)getAvgResultadoPeers, 2) : getAvgResultadoPeers;

                pillModel.Add(addPeriodo);
            }

            rptPeriodos.DataSource = pillModel;
            rptPeriodos.DataBind();
        }

        private MentoradoRespostasModel getRespostaModel(MENTORADORESPOSTAS item, double notaPeers, double notaMentor)
        {
            var mentoradoResposta = new MentoradoRespostasModel();
            mentoradoResposta.idResposta = item.idResposta;
            mentoradoResposta.idPergunta = item.idPergunta;
            mentoradoResposta.Pergunta = item.MENTORPERGUNTAS.Descricao;
            mentoradoResposta.idModo = item.MENTORPERGUNTAS.idModo;
            mentoradoResposta.idPeriodo = item.idPeriodo;
            mentoradoResposta.ResultadoPeers = notaPeers;
            mentoradoResposta.ResultadoMentor = notaMentor;
            mentoradoResposta.NotaMaxima = NOTA_MAXIMA;
            mentoradoResposta.VelocValorMentor = notaMentor;
            mentoradoResposta.VelocValorPeers = notaPeers;

            return mentoradoResposta;
        }



        private static Double calculaNota(List<MENTORADORESPOSTAS> getRespostasPeriodo)
        {
            var getRespostasValidasPeers = getRespostasPeriodo.Where(x => x.idNota != null && x.MENTORPERGUNTASNOTAS.Valor > 0).ToList();

            if (getRespostasValidasPeers.Count > 0)
            {
                return Math.Round((double)getRespostasValidasPeers.Select(x => x.MENTORPERGUNTASNOTAS.Valor).Average(), 2);
            }

            return Double.NaN;
        }


        protected void rptAvaliacoes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var item = (AvaliacaoAlocacaoPill)e.Item.DataItem;
                DropDownList ddlNota = (DropDownList)e.Item.FindControl("ddlNota");
                if (ddlNota != null)
                {
                    var notas = new FrenteInternaService().ObterNotasAlocacoesInternas();
                    ddlNota.SelectedIndex = notas.FindIndex(x => x.idNotaAlocacaoInterna == item.idNota);
                    ddlNota.Enabled = item.Enabled;
                }
            }
        }

        protected void btnAtualizaNota_Click(object sender, EventArgs e)
        {
            var mentoriaService = new MentoriaService();

            var idNota = int.Parse(txtIdNota.Text);
            var Escala = int.Parse(txtEscala.Text);
            var idResposta = int.Parse(txtIdResposta.Text);

            var getResposta = mentoriaService.ObterMentoradoRespostas(idResposta: idResposta)[0];

            if (idNota != -1)
            {
                getResposta.idNota = idNota;
            }
            else if (Escala != -1)
            {
                getResposta.Escala = Escala;
            }
            getResposta.DHC = DateTime.Now;

            var periodoService = new PeriodoService();
            var ultimoPeriodo = periodoService.ObterPeriodoUltimo();
            if (ultimoPeriodo.IdPeriodo == getResposta.PERIODOSAVALIACOES.IdPeriodo)
            {
                mentoriaService.GerirMentoradoRespostas(getResposta);
                carregaAvaliacoes();
            }
        }

        protected void btnAtualizaComentario_Click(object sender, EventArgs e)
        {
            var mentoriaService = new MentoriaService();

            var comentarios = txtComentarios.Text;
            var idResposta = int.Parse(txtIdResposta.Text);

            var getResposta = mentoriaService.ObterMentoradoRespostas(idResposta: idResposta)[0];
            getResposta.Comentario = comentarios;
            getResposta.DHC = DateTime.Now;

            var periodoService = new PeriodoService();
            var ultimoPeriodo = periodoService.ObterPeriodoUltimo();
            if (ultimoPeriodo.IdPeriodo == getResposta.PERIODOSAVALIACOES.IdPeriodo)
            {
                mentoriaService.GerirMentoradoRespostas(getResposta);
                carregaAvaliacoes();
            }
        }

        protected void rptRespostasPill_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var getItem = (MentoradoRespostasModel)e.Item.DataItem;
            var chartMentor = "chartMentor" + getItem.idPeriodo.ToString() + "_" + getItem.idResposta.ToString();
            var chartPeers = "chartPeers" + getItem.idPeriodo.ToString() + "_" + getItem.idResposta.ToString();

            if (getItem.idModo == 2)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "carregaChartMentor" + getItem.idPeriodo.ToString(),
                            "carregaChart('" + chartMentor + "'," + getItem.VelocValorMentor + ");", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "carregaChartPeers" + getItem.idPeriodo.ToString(),
                           "carregaChart('" + chartPeers + "'," + getItem.VelocValorPeers + ");", true);

                updRespostasMentoria.Update();
            }

        }

        protected void rptPeriodos_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var getItem = (MentoradoRespostaPill)e.Item.DataItem;
            var chartMentor = "chartTotalMentor" + getItem.idPeriodo.ToString();
            var chartPeers = "chartTotalPeers" + getItem.idPeriodo.ToString();

            var getMediaMentor = getItem.mediaMentor / NOTA_MAXIMA * 100;
            var getMediaPeers = getItem.mediaPeers / NOTA_MAXIMA * 100;

            ScriptManager.RegisterStartupScript(this, GetType(), "carregaChartTotalMentor" + getItem.idPeriodo.ToString(),
                        "carregaChart('" + chartMentor + "'," + getMediaMentor + ");", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "carregaChartTotalPeers" + getItem.idPeriodo.ToString(),
                        "carregaChart('" + chartPeers + "'," + getMediaPeers + ");", true);

            updRespostasMentoria.Update();
        }
    }
}