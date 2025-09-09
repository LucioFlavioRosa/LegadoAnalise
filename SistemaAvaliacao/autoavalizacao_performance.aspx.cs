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
    public partial class autoavalizacao_performance : System.Web.UI.Page
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

                lblAssociado.InnerText = associado.Nome;
                lblPeriodo.InnerText = periodo.Periodo;
                lblProjeto.InnerText = projeto.Projeto;
                lblGestor.InnerText = gestor.Nome;
                lblCliente.InnerText = new ClientesService().ObterCliente(projeto.IdCliente).Cliente;
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
                    var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");
                    lblTempo.InnerText =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") :
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
                            linhaPerformance.Input = item.InputAutoavaliacao ? "" : "";// "hidden";
                            linhaPerformance.DisclaimerInput = item.InputAutoavaliacao ? "" : "Esta nota não requer preenchimento do avaliado";

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
            HtmlSelect ddl1 = (HtmlSelect)repeaterItem.FindControl("ddlNota");
            HtmlTextArea txt1 = (HtmlTextArea)repeaterItem.FindControl("txtObservacao");

            HiddenField hiddenPerformance = (HiddenField)repeaterItem.FindControl("IdPerformanceItem");
            int idPerformance = Convert.ToInt32(hiddenPerformance.Value);
            int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));

            var avaliacaoService = new AvaliacoesService();
            var avaliacao = avaliacaoService.ObterAvaliacaoPerformance(idAssociado, idProjeto, idPerformance, idPeriodo);

            PERFORMANCES performance = new PERFORMANCES();
            
            if (avaliacao != null)
            {
                txt1.InnerText = avaliacao.ComentariosAutoAvaliacao;
                ddl1.Value = avaliacao.IdNotaNivel1AutoAvaliacao.ToString();

                if (avaliacao.IdNotaNivel1AutoAvaliacao != 0)
                    if (ddl1.Items[0].Text == "[Selecionar]")
                        ddl1.Items.RemoveAt(0);


                if (avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaNaoIniciada &&
                    avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaEmParalelo &&
                    avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAvaliacaoAsCegas &&
                    avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAutoAvaliacao)
                {
                    txt1.Disabled = true;
                    ddl1.Disabled = true;
                }
                txt1.Disabled = avaliacao.DataHoraFimAutoAvaliacao != null;
                ddl1.Disabled = avaliacao.DataHoraFimAutoAvaliacao != null;

            }
            else
            {
                txt1.InnerText = "";
                ddl1.SelectedIndex = 0;
                txt1.Disabled = false;
                //ddl1.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });
                ddl1.Disabled = false;
            }

            // INPUTS
            performance = new PerformancesService().ObterPerformance(idPerformance);
            if (performance != null)
            {
                ddl1.Disabled = ddl1.Disabled ? true : !performance.InputAutoavaliacao;
                if (!performance.InputAutoavaliacao)
                {
                    ddl1.Value = performance.NotaPadraoAutoAvaliacao.ToString();
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
            var associado = new AssociadosService().ObterAssociado(idAssociado);
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

                            // Busca Campos da LinhaItem atual
                            HtmlSelect ddl1 = (HtmlSelect)item.FindControl("ddlNota");
                            HtmlTextArea txt1 = (HtmlTextArea)item.FindControl("txtObservacao");

                            // Se ainda não existe, cria a Avaliação Performance
                            if (avaliacao == null)
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
                                avaliacao.DataHoraInicioAutoAvaliacao = DateTime.Now;


                                if (!avaliacaoService.SalvarAvaliacaoPerformance(avaliacao))
                                    return false;
                            }
                            else
                            {
                                avaliacao.IdNotaNivel1AutoAvaliacao = Convert.ToInt32(ddl1.Value);
                                avaliacao.ComentariosAutoAvaliacao = txt1.InnerText.Trim();


                                // Se estiver como "Não Iniciada", muda o Status para "Em Andamento"
                                if (avaliacao.IdAvaliacaoStatus == 1)
                                {
                                    avaliacao.IdAvaliacaoStatus = 2;
                                    avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaEmParalelo;

                                    if (avaliacao.DataHoraInicio == DateTime.MinValue)
                                        avaliacao.DataHoraInicio = DateTime.Now;
                                    avaliacao.DataHoraInicioAutoAvaliacao = DateTime.Now;
                                }
                            }

                            if (Convert.ToInt32(ddl1.Value) != 0)
                                if (ddl1.Items[0].Text == "[Selecionar]")
                                    ddl1.Items.RemoveAt(0);

                            if (finalizarAvaliacao)
                                avaliacao.DataHoraFimAutoAvaliacao = DateTime.Now;

                            // Salva Alterações
                            if (avaliacaoService.AlterarAvaliacaoPerformance(avaliacao.IdAvaliacaoPerformance, avaliacao))
                            {
                                // Atualiza Status da Avaliação Email
                                var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idAvaliacao);
                                avaliacaoEmail.idStatus = avaliacao.IdAvaliacaoStatus;
                                avaliacaoEmail.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;
                                avaliacaoService.AlterarAvaliacaoEmail(avaliacaoEmail.idAvaliacao, avaliacaoEmail);

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

            btnSalvar.Visible = btnSalvar2.Visible = btnFinalizar.Visible = btn_Finalizar_2.Visible = !tudoFinalizado;
        }

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (SalvarAvaliacao(false))
                MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);
            else
                MessageBox.Show("Falha ao Incluir Performance da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

            Response.Redirect($"~/autoavalizacao.aspx?IdProjeto={WebStorage.Get("IdProjeto", "")}&IdAssociado={WebStorage.Get("IdAssociado", "")}&IdPeriodo={WebStorage.Get("IdPeriodo", "")}");
        }

        protected void btnIrCompetencia_Click(object sender, EventArgs e)
        {
            if (SalvarAvaliacao(false))
                MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);

            else
                MessageBox.Show("Falha ao Incluir Competência da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

            Response.Redirect("~/autoavalizacao_competencia.aspx");

        }
    }
}