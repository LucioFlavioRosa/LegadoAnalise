using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class avalizacao_mentor_performance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var strProjeto = WebStorage.Get("IdProjeto", "");
                var strAssociado = WebStorage.Get("IdAssociado", "");
                var strPeriodo = WebStorage.Get("IdPeriodo", "");
                int idProjeto = 0;
                int idAssociado = 0;
                int idPeriodo = 0;

                if (!int.TryParse(strProjeto, out idProjeto))
                {
                    MessageBox.Show("É obrigatório a seleção de um Projeto.", "Projeto Não Encontrado", TIPO.Info, MessageBoxHandler);
                    return;
                }

                if (!int.TryParse(strAssociado, out idAssociado))
                {
                    MessageBox.Show("É obrigatório a seleção de um Associado.", "Associado Não Encontrado", TIPO.Info, MessageBoxHandler);
                    return;
                }

                if (!int.TryParse(strPeriodo, out idPeriodo))
                {
                    MessageBox.Show("É obrigatório a seleção de um Período.", "Período Não Encontrado", TIPO.Info, MessageBoxHandler);
                    return;
                }

                var projeto = new ProjetosService().ObterProjeto(idProjeto);
                var periodo = new PeriodoService().ObterPeriodo(idPeriodo);
                var pessoaService = new AssociadosService();
                var associado = pessoaService.ObterAssociado(idAssociado);
                var gestor = pessoaService.ObterAssociado(projeto.IdAssociadoGestor);

                lblCliente.InnerText = new ClientesService().ObterCliente(projeto.IdCliente).Cliente;
                lblAssociado.InnerText = associado.Nome;
                lblPeriodo.InnerText = periodo.Periodo;
                lblProjeto.InnerText = projeto.Projeto;
                lblCargo.InnerText = associado.CARGOS.Cargo;
                lblGestor.InnerText = gestor.Nome;
                // TEMPO DE PEERS E TEMPO DE CARGO
                lblTempoPeers.InnerText = new Util().TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDePeers).ToString();
                lblTempoCargo.InnerText = new Util().TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDeCargo).ToString();

                //Obtém o cargo do colaborador no momento daquela avaliação
                int idCargoNaAvaliacao = new AvaliacoesService().ObterAvaliacaoPerformance(associado.IdAssociado, projeto.IdProjeto, periodo.IdPeriodo, true).IdCargo;
                int idNivelNaAvaliacao = new AvaliacoesService().ObterAvaliacaoPerformance(associado.IdAssociado, projeto.IdProjeto, periodo.IdPeriodo, true).IdNivel;

                associado.IdCargo = idCargoNaAvaliacao;
                associado.IdNivel = idNivelNaAvaliacao;

                var listaPerformances = new AvaliacoesService().ObterAvaliacoesPerformances(associado.IdAssociado, idProjeto, idPeriodo, true);
                var listaIdPerformances = listaPerformances.Select(perf => perf.IdPerformance).ToList();
                var performances = new PerformancesService().ObterListaPerformances(associado.IdEmpresa, idCargoNaAvaliacao, idNivelNaAvaliacao, listaIdPerformances);
                List<PerformanceModel> listaPerformancesModel = new List<PerformanceModel>();

                // ORGANIZADOR DE ABRANGENCIAS
                bool novaAbrangencia = true, addItem = false, headerSeparador = false;
                List<string> abrangenciasContadas = new List<string>();
                string setAbrangencia = "";

                while (novaAbrangencia)
                {
                    novaAbrangencia = false;
                    setAbrangencia = "";

                    foreach (var item in performances)
                    {
                        addItem = false;

                        if (setAbrangencia != "")
                        {
                            if (item.Abrangencia == setAbrangencia)
                            {
                                addItem = true;
                                headerSeparador = false;
                            }
                        }
                        else if (!abrangenciasContadas.Contains(item.Abrangencia))
                        {
                            abrangenciasContadas.Add(item.Abrangencia);
                            setAbrangencia = item.Abrangencia;
                            novaAbrangencia = true;
                            headerSeparador = true;
                            addItem = true;
                        }

                        if (addItem)
                        {
                            var linhaPerformance = new PerformanceModel();
                            linhaPerformance.IdPerformance = item.IdPerformance;
                            linhaPerformance.Descricao = item.Performance;
                            linhaPerformance.Abaixo = item.PerformanceAbaixo;
                            linhaPerformance.Esperado = item.PerformanceEsperado;
                            linhaPerformance.Acima = item.PerformanceAcima;
                            linhaPerformance.Abrangencia = item.Abrangencia != null ? item.Abrangencia.ToUpper() : "";
                            linhaPerformance.SeparadorAbrangencia = headerSeparador ? "" : "hidden";
                            //linhaPerformance.Input = item.InputAutoavaliacao ? "" : "";// "hidden";
                            //linhaPerformance.DisclaimerInput = item.InputAutoavaliacao ? "" : "Esta nota não requer preenchimento do mentor";

                            listaPerformancesModel.Add(linhaPerformance);
                        }
                    }
                }

                rptPerformances.DataSource = listaPerformancesModel;
                rptPerformances.DataBind();
            }
        }

        protected void rptPerformances_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                CarregarAvaliacao(e.Item);
            }
        }

        public string truncaTexto(string texto, int qtdCaracter)
        {
            var dados = string.Empty;

            if (!string.IsNullOrEmpty(texto))
            {
                if (texto.Length > qtdCaracter)
                    dados = texto.Substring(0, qtdCaracter) + "...";
                else
                    dados = texto;
            }

            return dados;
        }

        private void CarregarAvaliacao(RepeaterItem repeaterItem)
        {
            //Estáticos
            HiddenField hiddenPerformance = (HiddenField)repeaterItem.FindControl("IdPerformanceItem");
            HtmlTableCell lblNotaAvaliado = (HtmlTableCell)repeaterItem.FindControl("lblNotaAvaliado");
            HtmlTableCell lblNotaCegas = (HtmlTableCell)repeaterItem.FindControl("lblNotaCegas");
            HtmlTableCell lblNotaGestor = (HtmlTableCell)repeaterItem.FindControl("lblNotaGestor");
            HtmlTableCell lblNotaFeedback = (HtmlTableCell)repeaterItem.FindControl("lblNotaFeedback");
            Label lblObservacaoAvaliado = (Label)repeaterItem.FindControl("lblObservacaoAvaliado");
            Label lblObservacaoGestor = (Label)repeaterItem.FindControl("lblObservacaoGestor");            

            int idPerformance = Convert.ToInt32(hiddenPerformance.Value);
            int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));

            var avaliacaoService = new AvaliacoesService();
            var avaliacao = avaliacaoService.ObterAvaliacaoPerformance(idAssociado, idProjeto, idPerformance, idPeriodo);

            if (avaliacao != null)
            {
                var notasService = new NotasAvaliacaoService();

                //Estáticos
                var nota = notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1AutoAvaliacao);
                lblNotaAvaliado.InnerText = nota.CodigoNota;
                lblObservacaoAvaliado.Text = avaliacao.ComentariosAutoAvaliacao;

                nota = notasService.ObterNotaPerformance(Convert.ToInt32(avaliacao.IdNotaNivel1AvaliacaoCegas));
                lblNotaCegas.InnerText = nota.CodigoNota;
                lblObservacaoGestor.Text = avaliacao.ComentariosAvaliacaoCegas;

                nota = notasService.ObterNotaPerformance(Convert.ToInt32(avaliacao.IdNotaNivel1AvaliacaoGestor));
                lblNotaGestor.InnerText = nota.CodigoNota;
                lblObservacaoGestor.Text += " / " + avaliacao.ComentariosAvaliacaoGestor;

                nota = notasService.ObterNotaPerformance(Convert.ToInt32(avaliacao.IdNotaNivel1Feedback));
                lblNotaFeedback.InnerText = nota.CodigoNota;
                lblObservacaoGestor.Text += " / " + avaliacao.ComentariosFeedback;
            }
            else
            {
                MessageBox.Show("O Feedback da Avaliação Performance ainda não foi finalizado.", "Impossível Prosseguir", TIPO.Warning, MessageBoxHandler);
            }
        }
    }
}