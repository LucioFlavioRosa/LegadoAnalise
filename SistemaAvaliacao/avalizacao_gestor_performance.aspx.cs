using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class avalizacao_gestor_performance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var strProjeto = WebStorage.Get("IdProjeto", "");
                var strAssociado = WebStorage.Get("IdAssociado", "");
                var strPeriodo = WebStorage.Get("IdPeriodo", "");
                int idAvaliacao = Convert.ToInt32(WebStorage.Get("idAvaliacao", "0"));

                if (!int.TryParse(strProjeto, out int idProjeto))
                {
                    MessageBox.Show("É obrigatório a seleção de um Projeto.", "Projeto Não Encontrado", TIPO.Info, MessageBoxHandler);
                    return;
                }

                if (!int.TryParse(strAssociado, out int idAssociado))
                {
                    MessageBox.Show("É obrigatório a seleção de um Associado.", "Associado Não Encontrado", TIPO.Info, MessageBoxHandler);
                    return;
                }

                if (!int.TryParse(strPeriodo, out int idPeriodo))
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
                lblGestor.InnerText = gestor.Nome;
                lblCargo.InnerText = associado.CARGOS.Cargo;
                // TEMPO DE PEERS E TEMPO DE CARGO
                lblTempoPeers.InnerText = new Util().TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDePeers).ToString();
                lblTempoCargo.InnerText = new Util().TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDeCargo).ToString();

                // Calcula Tempo Restante
                var avaliacaoService = new AvaliacoesService();
                var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idAvaliacao);

                // REVISÃO DO TEMPO DURAÇÃO DA AVALIAÇÃO RESTANTE COM BASE NOS NOVOS PRAZOS
                // lblTempo.InnerText = avaliacaoService.CalculaTempoRestante(avaliacaoEmail, avaliacaoService.etapaAutoAvaliacao);
                if (avaliacaoEmail != null)
                {
                    var prazo = avaliacaoEmail.PRAZOS;
                    var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");
                    lblTempo.InnerText =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") :
                            dataFinal;
                }

                var performances = new List<PERFORMANCES>();

                AVALIACOESPERFORMANCES avaliacaoPerformance = new AvaliacoesService().ObterAvaliacaoPerformance(associado.IdAssociado, idProjeto, idPeriodo, true);

                if (avaliacaoPerformance != null)
                {
                    var listaPerformances = new AvaliacoesService().ObterAvaliacoesPerformances(associado.IdAssociado, idProjeto, idPeriodo, true);
                    var listaIdPerformances = listaPerformances.Select(perf => perf.IdPerformance).ToList();

                    performances = new PerformancesService().ObterListaPerformances(associado.IdEmpresa, avaliacaoPerformance.IdCargo, associado.IdNivel, listaIdPerformances);
                }
                else
                    performances = new PerformancesService().ObterListaPerformances(associado.IdEmpresa, associado.IdCargo, associado.IdNivel, null);


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
                            linhaPerformance.Abrangencia = item.Abrangencia;
                            linhaPerformance.SeparadorAbrangencia = headerSeparador ? "" : "hidden";
                            linhaPerformance.Input = item.InputAvaliacaoGestor ? "" : "";// "hidden";
                            linhaPerformance.DisclaimerInput = item.InputAvaliacaoGestor ? "" : "Esta nota não requer preenchimento do gestor";

                            listaPerformancesModel.Add(linhaPerformance);
                        }
                    }
                }

                rptPerformances.DataSource = listaPerformancesModel;
                rptPerformances.DataBind();

                DesabilitaBotoes();
            }
        }

        protected void rptPerformances_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                HtmlSelect ddl1 = (HtmlSelect)e.Item.FindControl("ddlNota");

                if (ddl1 != null)
                    CarregaCombosNota(ref ddl1);

                CarregarAvaliacao(e.Item);
            }
        }

        private void CarregarAvaliacao(RepeaterItem repeaterItem)
        {
            //Editáveis
            HtmlSelect ddl1 = (HtmlSelect)repeaterItem.FindControl("ddlNota");
            HtmlTextArea txt1 = (HtmlTextArea)repeaterItem.FindControl("txtObservacao");

            //Estáticos
            HiddenField hiddenPerformance = (HiddenField)repeaterItem.FindControl("IdPerformanceItem");
            HtmlTableCell lblNotaAvaliado = (HtmlTableCell)repeaterItem.FindControl("lblNotaAvaliado");
            Label lblObservacaoAvaliado = (Label)repeaterItem.FindControl("lblObservacaoAvaliado");
            HtmlTableCell lblNotaCegas = (HtmlTableCell)repeaterItem.FindControl("lblNotaCegas");
            Label lblObservacaoCegas = (Label)repeaterItem.FindControl("lblObservacaoCegas");

            int idPerformance = Convert.ToInt32(hiddenPerformance.Value);
            int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));

            var avaliacaoService = new AvaliacoesService();
            var avaliacao = avaliacaoService.ObterAvaliacaoPerformance(idAssociado, idProjeto, idPerformance, idPeriodo);

            PERFORMANCES performance = new PERFORMANCES();

            if (avaliacao != null)
            {
                performance = new PerformancesService().ObterPerformance(avaliacao.IdPerformance);

                var notasService = new NotasAvaliacaoService();

                //Estáticos
                var nota = notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1AutoAvaliacao);
                lblNotaAvaliado.InnerText = nota.CodigoNota;
                lblObservacaoAvaliado.Text = avaliacao.ComentariosAutoAvaliacao;

                nota = notasService.ObterNotaPerformance(Convert.ToInt32(avaliacao.IdNotaNivel1AvaliacaoCegas));
                lblNotaCegas.InnerText = nota.CodigoNota;
                lblObservacaoCegas.Text = avaliacao.ComentariosAvaliacaoCegas;

                //Editáveis
                txt1.InnerText = avaliacao.ComentariosAvaliacaoGestor;
                ddl1.Value = avaliacao.IdNotaNivel1AvaliacaoGestor.ToString();

                if (avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAvaliacaoGestor)
                {
                    MessageBox.Show("A Avaliação Performance não está na Etapa de Avaliação do Gestor.", "Alteração não Permitida", TIPO.Warning, MessageBoxHandler);
                    txt1.Disabled = true;
                    ddl1.Disabled = true;
                }
            }
            else
            {
                MessageBox.Show("A Avaliação Performance ainda não foi finalizada pelo Associado ou Gestor às Cegas.", "Impossível Prosseguir", TIPO.Warning, MessageBoxHandler);
                txt1.Disabled = true;
                ddl1.Disabled = true;
            }

            // INPUTS
            performance = new PerformancesService().ObterPerformance(idPerformance);
            if (performance != null)
            {
                ddl1.Disabled = ddl1.Disabled ? true : !performance.InputAvaliacaoGestor;
                if (!performance.InputAvaliacaoGestor)
                {
                    ddl1.Value = performance.NotaPadraoAvaliacaoGestor.ToString();
                }
            }
        }

        private void CarregaCombosNota(ref HtmlSelect ddl)
        {
            var statusList = new NotasAvaliacaoService().ListaNotasPerformances(false);

            ddl.DataValueField = "IdNota";
            ddl.DataTextField = "CodigoNota";
            ddl.DataSource = statusList;
            ddl.DataBind();
            //ddl.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (SalvarAvaliacao(false))
                MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);
            else
                MessageBox.Show("Falha ao Incluir Performance da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);
        }

        private bool SalvarAvaliacao(bool finalizarAvaliacao)
        {
            var idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
            var idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            var idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
            int idAvaliacao = Convert.ToInt32(WebStorage.Get("idAvaliacao", "0"));

            AvaliacoesService avaliacaoService = new AvaliacoesService();

            try
            {
                foreach (RepeaterItem item in rptPerformances.Items)
                {
                    if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                    {
                        var idHiddenPerformance = item.FindControl("IdPerformanceItem") as HiddenField;

                        if (idHiddenPerformance != null)
                        {
                            int idPerformance = Convert.ToInt32(idHiddenPerformance.Value);
                            var avaliacao = avaliacaoService.ObterAvaliacaoPerformance(idAssociado, idProjeto, idPerformance, idPeriodo);

                            if (avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAvaliacaoGestor)
                            {
                                MessageBox.Show($"A Performance {idPerformance} ainda não foi finalizada pelo Gestor às Cegas.", "Impossível Prosseguir", TIPO.Info, MessageBoxHandler);
                                return false;
                            }

                            // Busca Campos da LinhaItem atual
                            HtmlSelect ddl1 = (HtmlSelect)item.FindControl("ddlNota");
                            HtmlTextArea txt1 = (HtmlTextArea)item.FindControl("txtObservacao");

                            avaliacao.IdNotaNivel1AvaliacaoGestor = Convert.ToInt32(ddl1.Value);
                            avaliacao.ComentariosAvaliacaoGestor = txt1.InnerText.Trim();
                            avaliacao.DHCAvaliacaoGestor = DateTime.Now;
                            avaliacao.USRAvaliacaoGestor = WebStorage.GetUsuarioLogado().Id;
                            avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;


                            // Mantem a mesma nota para a próxima etapa
                            avaliacao.IdNotaNivel1Feedback = Convert.ToInt32(ddl1.Value);

                            if (avaliacao.DataHoraInicioAvaliacaoGestor == null || avaliacao?.DataHoraInicioAvaliacaoGestor == DateTime.MinValue)
                                avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;

                            if (finalizarAvaliacao)
                                avaliacao.DataHoraFimAvaliacaoGestor = DateTime.Now;

                            // Salva Alterações
                            if (avaliacaoService.AlterarAvaliacaoPerformance(avaliacao.IdAvaliacaoPerformance, avaliacao))
                            {
                                // Atualiza Status da Avaliação Email
                                var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idAvaliacao);
                                avaliacaoEmail.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;
                                avaliacaoService.AlterarAvaliacaoEmail(avaliacaoEmail.idAvaliacao, avaliacaoEmail);


                                //Salva as informações para o próximo fluxo para gerar reutilização das informações aqui inseridas
                                avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaFeedback;
                                avaliacao.IdNotaNivel1Feedback = Convert.ToInt32(ddl1.Value);
                                avaliacao.ComentariosFeedback = txt1.InnerText.Trim();
                                avaliacao.DHCFeedback = DateTime.Now;
                                avaliacao.USRFeedback = WebStorage.GetUsuarioLogado().Id;
                                avaliacao.DataHoraInicioFeedback = DateTime.Now;
                                avaliacaoService.AlterarAvaliacaoPerformance(avaliacao.IdAvaliacaoPerformance, avaliacao);

                                if (finalizarAvaliacao)
                                {
                                    // Avança para a Próxima Etapa
                                    avaliacaoService.AvancaProximaEtapaPerformance(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                                }
                            }
                            else
                                return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void DesabilitaBotoes()
        {
            bool tudoFinalizado = true;

            foreach (RepeaterItem item in rptPerformances.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    HtmlSelect ddl1 = (HtmlSelect)item.FindControl("ddlNota");
                    HtmlTextArea txt1 = (HtmlTextArea)item.FindControl("txtObservacao");

                    if (!ddl1.Disabled || !txt1.Disabled)
                    {
                        tudoFinalizado = false;
                        break;
                    }
                }
            }

            btnSalvar.Visible = btnSalvar2.Visible = btnFinalizar.Visible = !tudoFinalizado;
        }

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (SalvarAvaliacao(false))
                MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);
            else
                MessageBox.Show("Falha ao Incluir Performance da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

            Response.Redirect($"~/avalizacao_gestor.aspx?IdProjeto={WebStorage.Get("IdProjeto", "")}&IdAssociado={WebStorage.Get("IdAssociado", "")}&IdPeriodo={WebStorage.Get("IdPeriodo", "")}");
        }


        protected void btnIrCompetencia_Click(object sender, EventArgs e)
        {
            if (SalvarAvaliacao(false))
                MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);

            else
                MessageBox.Show("Falha ao Incluir Competência da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

            Response.Redirect("~/avalizacao_gestor_competencia.aspx");

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
    }
}