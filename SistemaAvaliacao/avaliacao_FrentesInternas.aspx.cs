using AjaxControlToolkit;
using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Business.Model;
using System.IO;
using TriaSoftware.Util.Framework.Domain.Service;
using System.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using Business.Util;
using System.Web.UI.HtmlControls;

namespace SistemaAvaliacao
{
    public partial class avaliacao_FrentesInternas : System.Web.UI.Page
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
            carregaPeriodos();
            carregaAvaliacoes();
        }

        public void carregaPeriodos()
        {
            var periodoService = new PeriodoService();
            var frenteService = new FrenteInternaService();
            var associadoService = new AssociadosService();

            var todosPeriodos = periodoService.ListaTodosPeriodos(1);
            var ultimoPeriodo = periodoService.ObterPeriodoUltimo();
            var liderLogado = associadoService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);

            var pdiPillsModel = new List<PDIPillsModel>();
            foreach (var periodo in todosPeriodos)
            {
                if (periodo.IdPeriodo == ultimoPeriodo.IdPeriodo)
                {
                    validaAlocacoesInternas(periodo.IdPeriodo);
                }

                var avaliacoesAlocacaoInterna = frenteService.ObterAvaliacoesAlocacoesInternas(idAvaliador: liderLogado.IdAssociado, idPeriodo: periodo.IdPeriodo).ToList();
                if (avaliacoesAlocacaoInterna != null && avaliacoesAlocacaoInterna.Count > 0)
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
        public void validaAlocacoesInternas(int idPeriodo)
        {
            var periodoService = new PeriodoService();
            var frenteService = new FrenteInternaService();
            var associadoService = new AssociadosService();
            var liderLogado = associadoService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var frentesLideradas = frenteService.ObterLiderFrenteInterna(idAssociado: liderLogado.IdAssociado).Select(x => x.idFrenteInterna).Distinct().ToList();
            var frentes = frenteService.ObterFrenteInterna().Where(x => frentesLideradas.Contains(x.idFrenteInterna));

            foreach (var frente in frentes)
            {
                var lideresFrente = frenteService.ObterLiderFrenteInterna(idFrenteInterna: frente.idFrenteInterna);
                var participantesFrente = frenteService.ObterParticipantesFrentesInternas(idFrenteInterna: frente.idFrenteInterna);
                foreach (var lider in lideresFrente)
                {
                    foreach (var participante in participantesFrente)
                    {
                        var avaliacao = frenteService.ObterAvaliacoesAlocacoesInternas(idAlocacaoInterna: frente.idFrenteInterna, idAvaliador: lider.idAssociado, idAssociado: participante.idAssociado,
                            idPeriodo: idPeriodo).ToList();

                        if (avaliacao == null || avaliacao.Count <= 0)
                        {
                            var newAvaliacao = new AVALIACOESALOCACOESINTERNAS();
                            newAvaliacao.idAlocacaoInterna = frente.idFrenteInterna;
                            newAvaliacao.idAvaliador = lider.idAssociado;
                            newAvaliacao.idAssociado = participante.idAssociado;
                            newAvaliacao.idPeriodo = idPeriodo;
                            newAvaliacao.idNota = 4; // Selecionar
                            newAvaliacao.Comentarios = "-";
                            newAvaliacao.DHC = DateTime.Now;

                            frenteService.GerirAvaliacaoAlocacaoInterna(newAvaliacao);
                        }
                    }
                }
            }
        }
        public void carregaAvaliacoes()
        {
            var periodoService = new PeriodoService();
            var frenteService = new FrenteInternaService();
            var associadoService = new AssociadosService();
            var ultimoPeriodo = periodoService.ObterPeriodoUltimo();
            var liderLogado = associadoService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var pillModel = new List<FrentePill>();
            var fotosAssociadosService = new FotosAssociadosService();

            var avaliacoesLiderTodas = frenteService.ObterAvaliacoesAlocacoesInternas(idAvaliador: liderLogado.IdAssociado);
            var periodosTodos = avaliacoesLiderTodas.Select(x => x.idPeriodo).Distinct().OrderBy(x => x).ToList();
            foreach (var periodo in periodosTodos)
            {
                var addPeriodo = new FrentePill();
                addPeriodo.idPeriodo = periodo;
                addPeriodo.Periodo = periodoService.ObterPeriodo(periodo).Periodo;
                addPeriodo.id = "tab-" + periodo.ToString();
                addPeriodo.active = periodo == ultimoPeriodo.IdPeriodo ? "active in show" : "";
                addPeriodo.arialabelled = "tab-" + periodo.ToString() + "-tab";
                addPeriodo.alocacoes = new List<AlocacaoInternaPill>();

                var avaliacoesPeriodoLider = avaliacoesLiderTodas.Where(x => x.idPeriodo == periodo).ToList();
                var frentesAvaliadas = avaliacoesPeriodoLider.Select(x => x.idAlocacaoInterna).Distinct().ToList();

                var avaliacoesPeriodo = frenteService.ObterAvaliacoesAlocacoesInternas(idPeriodo: periodo);
                var avaliacoesFrentes = avaliacoesPeriodo.Where(x => frentesAvaliadas.Contains(x.idAlocacaoInterna)).ToList();

                var frentes = avaliacoesFrentes.Select(x => x.FRENTEINTERNA).Distinct().ToList();
                foreach (var frente in frentes)
                {
                    var addAlocacao = new AlocacaoInternaPill();
                    addAlocacao.idAlocacao = frente.idFrenteInterna;
                    addAlocacao.Alocacao = frente.FrenteInterna1;
                    addAlocacao.Avaliados = new List<AvaliacaoAlocacaoPill>();

                    var getAvaliacoes = avaliacoesFrentes.Where(x => x.idAlocacaoInterna == frente.idFrenteInterna).ToList();
                    foreach (var avaliacao in getAvaliacoes)
                    {
                        if (avaliacao.idAvaliador == liderLogado.IdAssociado)
                        {
                            var addAvaliacao = new AvaliacaoAlocacaoPill();
                            addAvaliacao.idAvaliacao = avaliacao.idAvaliacaoAlocacaoInterna;
                            addAvaliacao.idAvaliado = avaliacao.idAssociado;
                            addAvaliacao.Avaliado = associadoService.ObterAssociado(avaliacao.idAssociado).Nome;
                            addAvaliacao.FotoNome = fotosAssociadosService.ObterFotoPorAssociado(avaliacao.idAssociado).Imagem;
                            addAvaliacao.idNota = avaliacao.idNota;
                            addAvaliacao.Nota = frenteService.ObterNotasAlocacoesInternas(idNotaAlocacaoInterna: avaliacao.idNota)[0].Descricao;
                            addAvaliacao.Comentarios = avaliacao.Comentarios;
                            addAvaliacao.Enabled = periodo == ultimoPeriodo.IdPeriodo ? true : false;
                            addAvaliacao.Notas = new FrenteInternaService().ObterNotasAlocacoesInternas();
                            addAvaliacao.ValidadoMD = avaliacao.ValidadoMD == true ? "checked" : "";

                            addAlocacao.Avaliados.Add(addAvaliacao);
                        }
                    }

                    addPeriodo.alocacoes.Add(addAlocacao);
                }

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
            var frenteService = new FrenteInternaService();

            var avaliacaoText = txtIdAvaliacao.Text;
            var idAvaliacao = int.Parse(avaliacaoText);
            var notaText = txtIdNota.Text;
            var idNota = int.Parse(notaText);

            var getAvaliacao = frenteService.ObterAvaliacoesAlocacoesInternas(idAvaliacaoAlocacaoInterna: idAvaliacao)[0];
            getAvaliacao.idNota = idNota;
            getAvaliacao.DHC = DateTime.Now;

            frenteService.GerirAvaliacaoAlocacaoInterna(getAvaliacao);
        }

        protected void btnAtualizaComentario_Click(object sender, EventArgs e)
        {
            var frenteService = new FrenteInternaService();

            var avaliacaoText = txtIdAvaliacao.Text;
            var idAvaliacao = int.Parse(avaliacaoText);
            var comentarioText = txtComentarios.Text;

            var getAvaliacao = frenteService.ObterAvaliacoesAlocacoesInternas(idAvaliacaoAlocacaoInterna: idAvaliacao)[0];
            getAvaliacao.Comentarios = comentarioText;
            getAvaliacao.DHC = DateTime.Now;

            frenteService.GerirAvaliacaoAlocacaoInterna(getAvaliacao);
        }

        protected void btnAtualizaValidado_Click(object sender, EventArgs e)
        {
            var frenteService = new FrenteInternaService();

            var avaliacaoText = txtIdAvaliacao.Text;
            var idAvaliacao = int.Parse(avaliacaoText);
            var validadoText = txtValidado.Text;
            var boolValidado = bool.Parse(validadoText);

            var getAvaliacao = frenteService.ObterAvaliacoesAlocacoesInternas(idAvaliacaoAlocacaoInterna: idAvaliacao)[0];
            getAvaliacao.ValidadoMD = boolValidado;
            getAvaliacao.DHCValidadoMD = DateTime.Now;

            frenteService.GerirAvaliacaoAlocacaoInterna(getAvaliacao);
        }
    }
}