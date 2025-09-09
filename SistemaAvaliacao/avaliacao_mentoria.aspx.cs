using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Business.Model;
using System.Linq;
using Business.Util;

namespace SistemaAvaliacao
{
    public partial class avaliacao_mentoria : System.Web.UI.Page
    {

        private  PeriodoService periodoService = new PeriodoService();
        private  AssociadosService associadoService = new AssociadosService();
        private  MentoriaService mentoriaService = new MentoriaService();

        
        private ASSOCIADOS mentor;
        private ASSOCIADOS mentorado;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
              

                LimparCampos();
                MessageBox.Show("As respostas desta página são salvas automáticamente.", "Salvamento", TIPO.Info, MessageBoxHandler1);
            }
        }

        private void LimparCampos()
        {
            mentorado = associadoService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            mentor = associadoService.ObterAssociado(mentorado.IdAssociadoMentor);
            carregaPeriodos();
            carregaAvaliacoes();
        }

        public void carregaPeriodos()
        {

        List<PERIODOSAVALIACOES> periodos = periodoService.ListaTodosPeriodos(1);
        PERIODOSAVALIACOES ultimoPeriodo = periodoService.ObterPeriodoUltimo();


        var pdiPillsModel = new List<PDIPillsModel>();
            foreach (var periodo in periodos)
            {
                if (periodo.IdPeriodo == ultimoPeriodo.IdPeriodo)
                {
                    validaMentoradosRespostas(periodo.IdPeriodo);
                }

                var getRespostasMentorado = mentoriaService.ObterMentoradoRespostas(idMentorado: mentorado.IdAssociado, idPeriodo: periodo.IdPeriodo, idMentor: mentor.IdAssociado);
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
        public void validaMentoradosRespostas(int idPeriodo)
        {
            var getPerguntas = mentoriaService.ObterMentorPerguntas();
            foreach (var pergunta in getPerguntas)
            {
                var addResposta = new MENTORADORESPOSTAS();
                addResposta.idPergunta = pergunta.idMentorPergunta;
                addResposta.idMentorado = mentorado.IdAssociado;
                addResposta.idMentor = mentor.IdAssociado;
                addResposta.idPeriodo = idPeriodo;
                addResposta.DHC = DateTime.Now;

                var getRespostasMentorado = mentoriaService.ObterMentoradoRespostas(idMentorado: mentorado.IdAssociado, idPeriodo: idPeriodo, idMentor: mentor.IdAssociado, idPergunta: pergunta.idMentorPergunta);

                if (getRespostasMentorado == null || getRespostasMentorado.Count > 0)
                {
                    addResposta.idResposta = getRespostasMentorado.FirstOrDefault().idResposta;
                }
                mentoriaService.GerirMentoradoRespostas(addResposta);
            }
        }
        public void carregaAvaliacoes()
        {

            PERIODOSAVALIACOES ultimoPeriodo = periodoService.ObterPeriodoUltimo();

            mentorado = associadoService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            mentor = associadoService.ObterAssociado(mentorado.IdAssociadoMentor);
            var fotoMentor = new FotosAssociadosService().ObterFotoPorAssociado(mentorado.IdAssociadoMentor);

            var pillModel = new List<MentoradoRespostaPill>();

            var getRespostasTodas = mentoriaService.ObterMentoradoRespostas(idMentorado: mentorado.IdAssociado);
            var periodosTodos = getRespostasTodas.Select(x => x.idPeriodo).Distinct().OrderBy(x => x).ToList();
            var getMentoriaNotas = mentoriaService.ObterMentorNotas();

            foreach (var periodo in periodosTodos)
            {
                var addPeriodo = new MentoradoRespostaPill();
                addPeriodo.idPeriodo = periodo;
                addPeriodo.Periodo = periodoService.ObterPeriodo(periodo).Periodo;
                addPeriodo.id = "tab-" + periodo.ToString();
                addPeriodo.active = periodo == ultimoPeriodo.IdPeriodo ? "active in show" : "";
                addPeriodo.arialabelled = "tab-" + periodo.ToString() + "-tab";
                addPeriodo.respostas = new List<MentoradoRespostasModel>();

                var periodoEnabled = periodo == ultimoPeriodo.IdPeriodo;
                var unselectColor = periodoEnabled ? "#FFFFFF" : "#C0C4CF";
                
                
                var getPerguntas = mentoriaService.ObterMentorPerguntas();
                foreach (var pergunta in getPerguntas)
                {
                    var item = getRespostasTodas.Where(x => x.idPeriodo == periodo  && x.idPergunta == pergunta.idMentorPergunta).FirstOrDefault();

                    var addResposta = new MentoradoRespostasModel();

                    addResposta.idPergunta = pergunta.idMentorPergunta;
                    addResposta.Pergunta = pergunta.Descricao;
                    addResposta.idModo = pergunta.idModo;

                    if (item != null)
                    {
                        addResposta.idResposta = item.idResposta;
                        addResposta.Comentarios = item.Comentario;
                    }
                    addResposta.ComentarioColor = unselectColor;
                    addResposta.Enabled = periodoEnabled;
                    addResposta.notas = new List<MentoradoRespostasNotasModel>();

                    foreach (var nota in getMentoriaNotas)
                    {
                        var addNota = new MentoradoRespostasNotasModel();

                        addNota.BackgroundColor = unselectColor;

                        if (item != null)
                        {
                            addNota.idResposta = item.idResposta;
                            addNota.BackgroundColor = item.idNota == nota.idNota ? "#E5F419": addNota.BackgroundColor;
                        }
                        addNota.idNota = nota.idNota;
                        addNota.Escala = -1;
                        addNota.NotaTexto = nota.Descricao.ToString();
                        addNota.Icone = nota.Icone;
                        addNota.HideIcone = "hidden"; // ESCONDER ICONES
                        addNota.Enabled = periodoEnabled;

                        addResposta.notas.Add(addNota);
                    }

                    addPeriodo.respostas.Add(addResposta);
                }
         
                addPeriodo.MentorNome = mentor.Nome;
                addPeriodo.MentorFoto = fotoMentor.Imagem;

                pillModel.Add(addPeriodo);
            }

            rptPeriodos.DataSource = pillModel;
            rptPeriodos.DataBind();
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

            var getResposta = mentoriaService.ObterMentoradoRespostas(idResposta: idResposta).FirstOrDefault();

            getResposta.idNota = idNota;
            
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
            PERIODOSAVALIACOES ultimoPeriodo = periodoService.ObterPeriodoUltimo();

            var comentarios = txtComentarios.Text;
            var idResposta = int.Parse(txtIdResposta.Text);

            var getResposta = mentoriaService.ObterMentoradoRespostas(idResposta: idResposta)[0];
            getResposta.Comentario = comentarios;
            getResposta.DHC = DateTime.Now;

            if (ultimoPeriodo.IdPeriodo == getResposta.PERIODOSAVALIACOES.IdPeriodo)
            {
                mentoriaService.GerirMentoradoRespostas(getResposta);
                carregaAvaliacoes();
            }
        }

    }
}