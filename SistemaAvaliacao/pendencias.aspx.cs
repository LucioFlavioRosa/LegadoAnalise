using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using AjaxControlToolkit;
using SistemaAvaliacao.Scripts;
using System.Globalization;
using System.Linq;

namespace SistemaAvaliacao
{
    public partial class pendencias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnSearch_Click(sender, e);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var associadosService = new AssociadosService();
            var avaliacaoService = new AvaliacoesService();
            var periodosService = new PeriodoService();
            var fotoAssociadoService = new FotosAssociadosService();


            var associado = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var projetos = new AvaliacoesService().ObterAssociacoesRelacionadas(associado.IdAssociado);

            List<ProjetoModel> listaProjetosAvaliacoes = new List<ProjetoModel>();

            var ultimoPeriodo = periodosService.ObterPeriodoUltimo();

            foreach (var itemProjeto in projetos)
            {
                var umProjeto = new ProjetoModel();
                var periodo = periodosService.VerificaExistenciaPeriodo(itemProjeto.DataInicio, (DateTime)itemProjeto.DataFim, 1);
                if (periodo == null) { continue; }
                if (periodo.IdPeriodo != ultimoPeriodo.IdPeriodo) { continue; }

                var avaliacao = avaliacaoService.ObterAvaliacaoEmail(itemProjeto.IdProjeto, itemProjeto.IdAssociado, periodo.IdPeriodo, 1, itemProjeto.TipoAvaliacao, itemProjeto.Escopo, 
                    (int)itemProjeto.IdGestor);

                if (avaliacao == null) { continue; }
                if (avaliacao.PosicaoAtualFluxoAvaliacao == "AFI") { continue; }

                if (avaliacao.PROJETOS.ATV == 0) { continue; }

                umProjeto.FotoNome = avaliacao.ASSOCIADOS.FotoNome;
                var foto = fotoAssociadoService.ObterFotoPorAssociado(avaliacao.ASSOCIADOS.IdAssociado);
                if (foto == null)
                {
                    umProjeto.FotoNome = "assets/images/users/usernophoto.jpg";
                }
                else 
                {
                    umProjeto.FotoNome = foto.Imagem;
                }

                umProjeto.Nome = itemProjeto.ASSOCIADOS.Nome;
                umProjeto.Projeto = itemProjeto.PROJETOS.Projeto;
                umProjeto.Periodo = periodo.Periodo;

                PRAZOS prazo = avaliacao.PRAZOS;

                // CONFERE CADA CASO
                if (avaliacao.idAssociado == associado.IdAssociado)
                {
                    if (
                        avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaNaoIniciada ||
                        avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAutoAvaliacao ||
                        avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoAsCegas ||
                        avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaEmParalelo
                        )
                    {
                        umProjeto.Pendencia = "Auto Avaliação e Avaliação às cegas";
                        var dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");
                        umProjeto.DataLimite =
                            DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                                DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") :
                                dataFinal;

                        var avaliacoesPerformanceAtual = avaliacaoService.ObterAvaliacoesPerformances(avaliacao.idAssociado, avaliacao.idProjeto, avaliacao.idPeriodo, avaliacao.PosicaoAtualFluxoAvaliacao, true);
                        var avaliacoesCompetenciaAtual = avaliacaoService.ObterAvaliacoesCompetencias(avaliacao.idAssociado, avaliacao.idProjeto, avaliacao.idPeriodo, avaliacao.PosicaoAtualFluxoAvaliacao,
                                                    avaliacao.TipoAvaliacao, avaliacao.Escopo, avaliacao.idAvaliacao);

                        bool performanceAutoAvaliacaoFinalizada = avaliacoesPerformanceAtual.Any(p => p.DataHoraFimAutoAvaliacao != null);
                        bool competenciaAutoAvaliacaoFinalizada = avaliacoesCompetenciaAtual.Any(c => c.DataHoraFimAutoAvaliacao != null);

                        if (performanceAutoAvaliacaoFinalizada && competenciaAutoAvaliacaoFinalizada) { continue; }
                    }

                    else { continue; }
                }
                else if (itemProjeto.IdAvaliador == associado.IdAssociado || avaliacao.idGestor == associado.IdAssociado)
                {
                    if (itemProjeto.IdAvaliador == associado.IdAssociado && avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaEmParalelo)
                    {
                        umProjeto.Pendencia = "Auto Avaliação e Avaliação às cegas";
                        var dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");
                        umProjeto.DataLimite =
                            DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                                DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") :
                                dataFinal;

                        var avaliacoesPerformanceAtual = avaliacaoService.ObterAvaliacoesPerformances(avaliacao.idAssociado, avaliacao.idProjeto, avaliacao.idPeriodo, avaliacao.PosicaoAtualFluxoAvaliacao, true);
                        var avaliacoesCompetenciaAtual = avaliacaoService.ObterAvaliacoesCompetencias(avaliacao.idAssociado, avaliacao.idProjeto, avaliacao.idPeriodo, avaliacao.PosicaoAtualFluxoAvaliacao,
                                                    avaliacao.TipoAvaliacao, avaliacao.Escopo, avaliacao.idAvaliacao);

                        bool performanceAsCegasFinalizada = avaliacoesPerformanceAtual.Any(p => p.DataHoraFimAvaliacaoCegas != null);
                        bool competenciaAsCegasFinalizada = avaliacoesCompetenciaAtual.Any(c => c.DataHoraFimAvaliacaoCegas != null);

                        if (performanceAsCegasFinalizada && competenciaAsCegasFinalizada) { continue; }
                    }
                    else if (avaliacao.idGestor == associado.IdAssociado && avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoGestor)
                    {
                        if (avaliacao.TipoAvaliacao == "desempenho")
                        {
                            umProjeto.Pendencia = "Avaliação do Gestor";
                        }
                        else
                        {
                            umProjeto.Pendencia = "Avaliação de Liderança";
                        }
                        var dataFinal = avaliacao.DataLiberacao != null ? avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy") :
                            avaliacao.DHC.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");
                        umProjeto.DataLimite =
                            DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                                DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") :
                                dataFinal;
                    }
                    else if (avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaFeedback)
                    {
                        umProjeto.Pendencia = "Feedback";
                        var dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoFeedback).ToString("dd/MM/yyyy");
                        umProjeto.DataLimite =
                            DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                                DateTime.Today.AddDays(prazo.CompensadorFeedback).ToString("dd/MM/yyyy") :
                                dataFinal;
                    }
                    else { continue; }
                }
                else if (avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoMentor)
                {
                    umProjeto.Pendencia = "Mentoria";
                    var dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoMentor).ToString("dd/MM/yyyy");
                    umProjeto.DataLimite =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorMentor).ToString("dd/MM/yyyy") :
                            dataFinal;
                }
                else { continue; }

                listaProjetosAvaliacoes.Add(umProjeto);
            }

            rptProjetos.DataSource = listaProjetosAvaliacoes;
            rptProjetos.DataBind();
        }
    }
}