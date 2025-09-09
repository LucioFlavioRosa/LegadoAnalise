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
    public partial class avalizacao_gestorascegas_performance : System.Web.UI.Page
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
                    var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");
                    lblTempo.InnerText =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") :
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
                            linhaPerformance.Abrangencia = item.Abrangencia != null ? item.Abrangencia.ToUpper() : "";
                            linhaPerformance.SeparadorAbrangencia = headerSeparador ? "" : "hidden";
                            linhaPerformance.Input = item.InputAvaliacaoAsCegas ? "" : "";// "hidden";
                            linhaPerformance.DisclaimerInput = item.InputAvaliacaoAsCegas ? "" : "Esta nota não requer preenchimento do avaliador";

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
            HtmlSelect ddl1 = (HtmlSelect)repeaterItem.FindControl("ddlNota");
            HtmlTextArea txt1 = (HtmlTextArea)repeaterItem.FindControl("txtObservacao");

            HiddenField hiddenPerformance = (HiddenField)repeaterItem.FindControl("IdPerformanceItem");
            int idPerformance = Convert.ToInt32(hiddenPerformance.Value);
            int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));

            var avaliacaoService = new AvaliacoesService();
            var associadosService = new AssociadosService();
            var avaliacao = avaliacaoService.ObterAvaliacaoPerformance(idAssociado, idProjeto, idPerformance, idPeriodo);
            var associado = associadosService.ObterAssociado(idAssociado);

            PERFORMANCES performance = new PERFORMANCES();

            if (avaliacao != null)
            {
                performance = new PerformancesService().ObterPerformance(avaliacao.IdPerformance);

                txt1.InnerText = avaliacao.ComentariosAvaliacaoCegas;
                ddl1.Value = avaliacao.IdNotaNivel1AvaliacaoCegas.ToString();

                if (avaliacao.IdNotaNivel1AvaliacaoCegas != 0 && avaliacao.IdNotaNivel1AvaliacaoCegas != null)
                    if (ddl1.Items[0].Text == "[Selecionar]")
                        ddl1.Items.RemoveAt(0);

                ddl1.Disabled = avaliacao.DataHoraFimAvaliacaoCegas != null;
                txt1.Disabled = avaliacao.DataHoraFimAvaliacaoCegas != null;

                if (avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAvaliacaoAsCegas &&
                                avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaNaoIniciada &&
                                avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAutoAvaliacao &&
                                avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaEmParalelo)
                {
                    MessageBox.Show("A Avaliação Performance não está na Etapa de Avaliação às Cegas.", "Alteração não Permitida", TIPO.Warning, MessageBoxHandler);
                    txt1.Disabled = true;
                    ddl1.Disabled = true;
                }
            }
            else
            {
                avaliacao = new AVALIACOESPERFORMANCES();
                avaliacao.IdEmpresa = associado.IdEmpresa;
                avaliacao.IdAssociado = idAssociado;
                avaliacao.USRAutoAvaliacao = idAssociado;
                avaliacao.IdCargo = associado.IdCargo;
                avaliacao.IdNivel = associado.IdNivel;
                avaliacao.IdProjeto = idProjeto;
                avaliacao.IdPeriodo = idPeriodo;
                avaliacao.IdPerformance = idPerformance;
                avaliacao.IdAvaliacaoStatus = 2; // Adiciona com Status "Em Andamento"
                avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaEmParalelo;
                avaliacao.DataHoraInicio = DateTime.Now;
                avaliacao.DHCAutoAvaliacao = DateTime.Now;
                avaliacao.USR = WebStorage.GetUsuarioLogado().Id;
                avaliacao.DHC = DateTime.Now;
                avaliacao.ATV = 1;
                avaliacao.IdNotaNivel1AutoAvaliacao = Convert.ToInt32(ddl1.Value);
                avaliacao.ComentariosAutoAvaliacao = txt1.InnerText.Trim();
                avaliacao.DataHoraInicioAvaliacaoCegas = DateTime.Now;

                // Não está incluindo, qual propriedade falta??

                if (!avaliacaoService.SalvarAvaliacaoPerformance(avaliacao))
                {
                    MessageBox.Show("Falha ao Incluir Performance da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);
                    txt1.Disabled = true;
                    ddl1.Disabled = true;
                }
            }
            // INPUTS
            performance = new PerformancesService().ObterPerformance(idPerformance);
            if (performance != null)
            {
                ddl1.Disabled = ddl1.Disabled ? true : !performance.InputAvaliacaoAsCegas;
                if (!performance.InputAvaliacaoAsCegas)
                {
                    ddl1.Value = performance.NotaPadraoAvaliacaoAsCegas.ToString();
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
            ddl.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });
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

                            if (avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAvaliacaoAsCegas &&
                                avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaNaoIniciada &&
                                avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAutoAvaliacao &&
                                avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaEmParalelo)
                            {
                                MessageBox.Show($"A Performance {idPerformance} ainda não foi finalizada pelo Associado.", "Impossível Prosseguir", TIPO.Info, MessageBoxHandler);
                                return false;
                            }

                            // Busca Campos da LinhaItem atual
                            HtmlSelect ddl1 = (HtmlSelect)item.FindControl("ddlNota");
                            HtmlTextArea txt1 = (HtmlTextArea)item.FindControl("txtObservacao");

                            avaliacao.IdNotaNivel1AvaliacaoCegas = Convert.ToInt32(ddl1.Value);
                            avaliacao.ComentariosAvaliacaoCegas = txt1.InnerText.Trim();
                            avaliacao.DHCAvaliacaoCegas = DateTime.Now;
                            avaliacao.DHCAvaliacaoCegas = DateTime.Now;
                            avaliacao.USRAvaliacaoCegas = WebStorage.GetUsuarioLogado().Id;
                            avaliacao.DataHoraInicioAvaliacaoCegas = DateTime.Now;

                            // Mantem a mesma nota para a próxima etapa
                            avaliacao.IdNotaNivel1AvaliacaoGestor = Convert.ToInt32(ddl1.Value);

                            if (avaliacao.DataHoraInicioAvaliacaoCegas == null || avaliacao?.DataHoraInicioAvaliacaoCegas == DateTime.MinValue)
                                avaliacao.DataHoraInicioAvaliacaoCegas = DateTime.Now;

                            if (finalizarAvaliacao)
                                avaliacao.DataHoraFimAvaliacaoCegas = DateTime.Now;

                            // Salva Alterações
                            if (avaliacaoService.AlterarAvaliacaoPerformance(avaliacao.IdAvaliacaoPerformance, avaliacao))
                            {
                                // Atualiza Status da Avaliação Email
                                var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idAvaliacao);
                                avaliacaoEmail.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;
                                avaliacaoService.AlterarAvaliacaoEmail(avaliacaoEmail.idAvaliacao, avaliacaoEmail);

                                //Salva as informações para o próximo fluxo para gerar reutilização das informações aqui inseridas
                                avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaAvaliacaoGestor;
                                avaliacao.IdNotaNivel1AvaliacaoGestor = Convert.ToInt32(ddl1.Value);
                                avaliacao.ComentariosAvaliacaoGestor = txt1.InnerText.Trim();
                                avaliacao.DHCAvaliacaoGestor = DateTime.Now;
                                avaliacao.USRAvaliacaoGestor = WebStorage.GetUsuarioLogado().Id;
                                avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;

                                avaliacaoService.AlterarAvaliacaoPerformance(avaliacao.IdAvaliacaoPerformance, avaliacao);


                                if (Convert.ToInt32(ddl1.Value) != 0)
                                    if (ddl1.Items[0].Text == "[Selecionar]")
                                        ddl1.Items.RemoveAt(0);


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

            btnSalvar.Visible = btnSalvar2.Visible = btnFinalizar.Visible = btn_Finalizar_2.Visible = !tudoFinalizado;
        }

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {

            if (SalvarAvaliacao(false))
                MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);
            else
                MessageBox.Show("Falha ao Incluir Performance da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

            Response.Redirect($"~/avalizacao_gestorascegas.aspx?IdProjeto={WebStorage.Get("IdProjeto", "")}&IdAssociado={WebStorage.Get("IdAssociado", "")}&IdPeriodo={WebStorage.Get("IdPeriodo", "")}");
        }

        protected void btnIrCompetencia_Click(object sender, EventArgs e)
        {
            if (SalvarAvaliacao(false))
                MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);

            else
                MessageBox.Show("Falha ao Incluir Competência da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

            Response.Redirect("~/avalizacao_gestorascegas_competencia.aspx");

        }


    }
}