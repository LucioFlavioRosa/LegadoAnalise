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
    public partial class avalizacao_gestorascegas_competencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var strProjeto = Request.QueryString["IdProjeto"];
                var strAssociado = Request.QueryString["IdAssociado"];
                var strPeriodo = Request.QueryString["IdPeriodo"];
                var strTipoAvaliacao = Request.QueryString["TipoAvaliacao"];
                var strEscopo = Request.QueryString["Escopo"];
                var strGestor = Request.QueryString["idGestor"];

                if (!int.TryParse(strProjeto, out int idProjeto))
                {
                    strProjeto = WebStorage.Get("IdProjeto", "");

                    if (!int.TryParse(strProjeto, out idProjeto))
                    {
                        MessageBox.Show("É obrigatório a seleção de um Projeto.", "Projeto Não Encontrado", TIPO.Info, MessageBoxHandler);
                        return;
                    }
                }

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

                if (!int.TryParse(strGestor, out int idGestor))
                {
                    strGestor = WebStorage.Get("idGestor", "");

                    if (!int.TryParse(strGestor, out idGestor))
                    {
                        MessageBox.Show("É obrigatório a seleção de um Gestor.", "Gestor Não Encontrado", TIPO.Info, MessageBoxHandler);
                        return;
                    }
                }

                if (strTipoAvaliacao == "" || strTipoAvaliacao == " " || strTipoAvaliacao == null)
                {
                    strTipoAvaliacao = WebStorage.Get("TipoAvaliacao", "");

                    if (strTipoAvaliacao == "" || strTipoAvaliacao == " " || strTipoAvaliacao == null)
                    {
                        MessageBox.Show("É obrigatório a seleção de um tipo de avaliação.", "Tipo de avaliação Não Encontrado", TIPO.Info, MessageBoxHandler);
                        return;
                    }
                }

                if (strEscopo == "" || strEscopo == " " || strEscopo == null)
                {
                    strEscopo = WebStorage.Get("Escopo", "");

                    if (strEscopo == "" || strEscopo == " " || strEscopo == null)
                    {
                        MessageBox.Show("É obrigatório a seleção de um escopo.", "Escopo Não Encontrado", TIPO.Info, MessageBoxHandler);
                        return;
                    }
                }

                var avaliacoesService = new AvaliacoesService();
                var competenciasService = new CompetenciasService();
                var clientesService = new ClientesService();
                var cargosService = new CargosService();
                var eixoService = new EixoService();
                var subCompetenciaService = new SubCompetenciasService();
                var dimensaoService = new DimensoesService();
                var associadosService = new AssociadosService();
                var util = new Util();
                var projetosService = new ProjetosService();
                var periodoService = new PeriodoService();

                var projeto = projetosService.ObterProjeto(idProjeto);
                var associado = associadosService.ObterAssociado(idAssociado);
                var periodo = periodoService.ObterPeriodo(idPeriodo);
                var gestor = associadosService.ObterAssociado(projeto.IdAssociadoGestor);

                lblProjeto.InnerText = projeto.Projeto;
                lblAssociado.InnerText = associado.Nome;
                lblPeriodo.InnerText = periodo.Periodo;
                lblCliente.InnerText = clientesService.ObterCliente(projeto.IdCliente).Cliente;
                lblGestor.InnerText = gestor.Nome;
                // TEMPO DE PEERS E TEMPO DE CARGO
                lblTempoPeers.InnerText = util.TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDePeers).ToString();
                lblTempoCargo.InnerText = util.TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDeCargo).ToString();

                // Calcula Tempo Restante
                var avaliacaoEmail = avaliacoesService.ObterAvaliacaoEmail(idProjeto, idAssociado, idPeriodo, projeto.IdEmpresa, strTipoAvaliacao, strEscopo, idGestor);

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

                WebStorage.Set("IdProjeto", idProjeto.ToString());
                WebStorage.Set("IdAssociado", idAssociado.ToString());
                WebStorage.Set("IdPeriodo", idPeriodo.ToString());
                WebStorage.Set("TipoAvaliacao", strTipoAvaliacao);
                WebStorage.Set("Escopo", strEscopo);
                WebStorage.Set("idAvaliacao", avaliacaoEmail.idAvaliacao.ToString());
                WebStorage.Set("idGestor", strGestor);

                AVALIACOESCOMPETENCIAS avaliacaoCompetencia = avaliacoesService.ObterAvaliacaoCompetencia(associado.IdAssociado, idProjeto, idPeriodo, strTipoAvaliacao, strEscopo,
                    avaliacaoEmail.idAvaliacao);

                var competencias = new List<COMPETENCIAS>();
                var listaCompetencias = new List<AVALIACOESCOMPETENCIAS>();

                if (avaliacaoCompetencia != null)
                {
                    listaCompetencias = avaliacoesService.ObterAvaliacoesCompetencias(associado.IdAssociado, idProjeto, idPeriodo, strTipoAvaliacao, strEscopo, avaliacaoEmail.idAvaliacao);
                    var listaIdCompetencias = listaCompetencias.Select(comp => comp.IdCompetencia).ToList();

                    competencias = competenciasService.ObterListaCompetencias(associado.IdEmpresa, avaliacaoCompetencia.IdCargo, associado.IdNivel, avaliacaoCompetencia.TipoAvaliacao,
                        avaliacaoCompetencia.Escopo, listaIdCompetencias);
                }
                else
                    competencias = competenciasService.ObterListaCompetencias(associado.IdEmpresa, associado.IdCargo, associado.IdNivel,
                        avaliacaoCompetencia.TipoAvaliacao, avaliacaoCompetencia.Escopo, null);

                // CABEÇALHO DE DESCRIÇÕES - LABELS FIXOS 
                CARGOS cargoAvaliacao = avaliacaoCompetencia != null ? avaliacaoCompetencia.CARGOS : associado.CARGOS;

                text_TabelaCabecalho_CargoAtual.Text = cargoAvaliacao.Cargo;
                text_TabelaCabecalho_CargoProximo.Text = cargoAvaliacao.idProximoCargo != null ? cargoAvaliacao.CARGOS2.Cargo : "";
                text_TabelaCabecalho_FuncaoAtual.Text = cargoAvaliacao.Funcao;
                text_TabelaCabecalho_FuncaoProximo.Text = cargoAvaliacao.idProximoCargo != null ? cargoAvaliacao.CARGOS2.Funcao : "";
                text_TabelaCabecalho_AutonomiaAtual.Text = cargoAvaliacao.Autonomia;
                text_TabelaCabecalho_AutonomiaProximo.Text = cargoAvaliacao.idProximoCargo != null ? cargoAvaliacao.CARGOS2.Autonomia : "";
                text_TabelaCabecalho_EscopoAtual.Text = cargoAvaliacao.EscopoDeAtuacao;
                text_TabelaCabecalho_EscopoProximo.Text = cargoAvaliacao.idProximoCargo != null ? cargoAvaliacao.CARGOS2.EscopoDeAtuacao : "";
                text_TabelaCabecalho_InterlocucaoAtual.Text = cargoAvaliacao.NivelInterlocucao;
                text_TabelaCabecalho_InterlocucaoProximo.Text = cargoAvaliacao.idProximoCargo != null ? cargoAvaliacao.CARGOS2.NivelInterlocucao : "";

                // CABEÇALHO DE DESCRIÇÕES - COLUNAS EXTENSÍVEIS DE COMPETÊNCIAS
                List<DescricoesCargoModel> listaDescricoesCompetencias = cargosService.ObterDescricoesCompetencias(cargoAvaliacao.IdCargo, competencias, associado);
                rpt_TabelaCabecalho_Titulos.DataSource = listaDescricoesCompetencias;
                rpt_TabelaCabecalho_Titulos.DataBind();
                rpt_TabelaCabecalho_CargoAtual.DataSource = listaDescricoesCompetencias;
                rpt_TabelaCabecalho_CargoAtual.DataBind();
                rpt_TabelaCabecalho_CargoProximo.DataSource = listaDescricoesCompetencias;
                rpt_TabelaCabecalho_CargoProximo.DataBind();

                List<CompetenciaModel> listaCompetenciasModel = new List<CompetenciaModel>();

                int lastEixo = -1;
                int lastSub = -1;
                int lastDimensao = -1;
                int cont = 0;
                int idEixo = -1;
                int IdSubCompetencia = -1;

                int lastEixoLideranca = -1;
                bool trocaTitulos = (associado.Vertical != null && associado.Vertical.ToLower() == "backoffice");
                trocaTitulos = false;

                foreach (var item in competencias)
                {
                    if (IdSubCompetencia != item.IdSubCompetencia)
                    {
                        lastEixo = -1;
                        lastSub = -1;
                        lastDimensao = -1;
                    }

                    idEixo = item.IdEixo;
                    IdSubCompetencia = item.IdSubCompetencia;

                    var linhaCompetencia = new CompetenciaModel();
                    linhaCompetencia.IdCompetencia = item.IdCompetencia;
                    linhaCompetencia.PalavrasChave = item.PalavrasChave != null ? item.PalavrasChave.Replace(Convert.ToChar(10).ToString(), "<br>") : "";
                    if (trocaTitulos)
                    {
                        linhaCompetencia.PalavrasChave = item.SUBCOMPETENCIAS.SubCompetencia;
                    }

                    // Verifica a Troca da linha do Eixo
                    if (lastEixo != item.IdEixo)
                    {
                        var eixo = eixoService.ObterEixo(item.IdEixo);
                        linhaCompetencia.Eixo = eixo.Eixo;
                    }
                    else
                        linhaCompetencia.Eixo = "";

                    // Verifica a Troca da linha da SubCompetencia
                    if (lastEixo != item.IdEixo && lastSub != item.IdSubCompetencia)
                    {
                        var subCompetencia = subCompetenciaService.ObterCompetencia(item.IdSubCompetencia);
                        linhaCompetencia.SubCompetencia = subCompetencia.SubCompetencia;
                        if (trocaTitulos)
                        {
                            if (lastEixoLideranca != item.IdEixo)
                            {
                                linhaCompetencia.SubCompetencia = item.EIXOS.Eixo;
                                lastEixoLideranca = item.IdEixo;
                            }
                            else
                            {
                                linhaCompetencia.Eixo = "";
                                linhaCompetencia.SubCompetencia = "";
                                linhaCompetencia.Dimensao = "";
                            }
                        }
                    }
                    else
                        linhaCompetencia.SubCompetencia = "";

                    // Verifica a Troca da linha da Dimensão
                    if (lastEixo != item.IdEixo && lastSub != item.IdSubCompetencia && lastDimensao != item.IdDimensao && !trocaTitulos)
                    {
                        var dimensao = dimensaoService.ObterDimensao(item.IdDimensao);
                        linhaCompetencia.Dimensao = dimensao.Dimensao;
                        lastDimensao = item.IdDimensao;
                        lastSub = item.IdSubCompetencia;
                        lastEixo = item.IdEixo;
                    }
                    else
                        linhaCompetencia.Dimensao = "";

                    // REFORMULAÇÃO 2022 - PRÓXIMO NÍVEL AGORA É DETALHAMENTO DA COMPETÊNCIA DO PRÓXIMO CARGO PRA AS MESMAS SUBCOMPETENCIAS
                    COMPETENCIAS competenciaProximoCargo = new COMPETENCIAS();

                    if (cargoAvaliacao.idProximoCargo != null)
                    {
                        competenciaProximoCargo = competenciasService.ObterCompetenciaCargoSub(cargoAvaliacao.CARGOS2.IdCargo, item.IdSubCompetencia, item.PalavrasChave);
                    }

                    switch (associado.IdNivel)
                    {
                        case 1: //Junior
                            linhaCompetencia.DetalheNivelAtual = item.CompetenciaJRDetalhe;
                            linhaCompetencia.CompetenciaAtual = item.CompetenciaJR;
                            linhaCompetencia.CompetenciaProximo = item.CompetenciaPL;
                            linhaCompetencia.DetalheProximoNivel = competenciaProximoCargo != null ? competenciaProximoCargo.CompetenciaJRDetalhe : ""; // item.CompetenciaPLDetalhe;
                            linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "hidden";
                            if (trocaTitulos)
                            {
                                lblDetalheNivel2.Visible = true;
                                tableDetalheNivel2.Visible = true;
                            }
                            break;
                    }

                    // NOVAS REGRAS DE AUTOPREENCHIMENTO
                    var getLinhaAvaliacao = listaCompetencias.Where(x => x.IdCompetencia == item.IdCompetencia).ToList()[0];
                    var inputEtapaAtual = item.InputAvaliacaoAsCegas;
                    var visivelEtapaAtual = item.VisivelAvaliacaoAsCegas;
                    var idNotaNivel1 = getLinhaAvaliacao.IdNotaNivel1AvaliacaoCegas;
                    var idNotaNivel2 = getLinhaAvaliacao.IdNotaNivel2AvaliacaoCegas;

                    linhaCompetencia.disableSelectNivel1 = inputEtapaAtual && item.InputNivel1 ? "" : "disabled";
                    linhaCompetencia.disableSelectNivel2 = inputEtapaAtual && item.InputNivel2 ? "" : "disabled";
                    linhaCompetencia.hiddenSelectNivel1 = inputEtapaAtual && item.InputNivel1 ? "" : "hidden";
                    linhaCompetencia.hiddenSelectNivel2 = inputEtapaAtual && item.InputNivel2 ? "" : "hidden";
                    linhaCompetencia.hiddenTextBoxNivel1 = !inputEtapaAtual || !item.InputNivel1 ? "" : "hidden";
                    linhaCompetencia.hiddenTextBoxNivel2 = !inputEtapaAtual || !item.InputNivel2 ? "" : "hidden";

                    if (item.IdModo == 1)
                    {
                        var getNotaNivel1 = avaliacoesService.ObterAvaliacaoCompetenciaNota(idNotaNivel1 ?? 0);
                        var getNotaNivel2 = avaliacoesService.ObterAvaliacaoCompetenciaNota(idNotaNivel2 ?? 0);
                        linhaCompetencia.textoNotaNivel1 = getNotaNivel1.DescricaoNota;
                        linhaCompetencia.textoNotaNivel2 = getNotaNivel2.DescricaoNota;
                    }
                    else if (item.IdModo == 2)
                    {
                        var getResultado = avaliacoesService.ObterResultadoLiderComoCompetencia(idAssociado, idPeriodo);
                        linhaCompetencia.textoNotaNivel1 = getResultado.textoMediaTotal;
                        linhaCompetencia.textoNotaNivel2 = getResultado.textoMediaTotal;
                    }

                    linhaCompetencia.textoNotaNivel1 = visivelEtapaAtual && item.VisivelNivel1 ? linhaCompetencia.textoNotaNivel1 : "Nota não disponível nesta etapa.";
                    linhaCompetencia.textoNotaNivel2 = visivelEtapaAtual && item.VisivelNivel2 ? linhaCompetencia.textoNotaNivel2 : "Nota não disponível nesta etapa.";

                    cont++;
                    listaCompetenciasModel.Add(linhaCompetencia);
                }

                rptCompetencias.DataSource = listaCompetenciasModel;
                rptCompetencias.DataBind();

                DesabilitaBotoes();
            }
        }

        private void CarregaCombosNota(ref HtmlSelect ddl, bool enableAll = false)
        {
            var statusList = new NotasAvaliacaoService().ListaNotasCompetencias(false, enableAll);

            ddl.DataValueField = "IdNota";
            ddl.DataTextField = "CodigoNotaAvaliador";
            ddl.DataSource = statusList;
            ddl.DataBind();
            //ddl.Items.Insert(0, new ListItem { Text="[Selecionar]", Value="0", Selected=false, Enabled=false });
        }

        protected void rptCompetencias_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                HtmlSelect ddl1 = (HtmlSelect)e.Item.FindControl("ddlNotaNivel1");
                HtmlSelect ddl2 = (HtmlSelect)e.Item.FindControl("ddlNotaNivel2");

                HiddenField hiddenCompetencia = (HiddenField)e.Item.FindControl("IdCompetenciaItem");
                int idCompetencia = Convert.ToInt32(hiddenCompetencia.Value);
                int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
                int idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
                int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
                string TipoAvaliacao = WebStorage.Get("TipoAvaliacao", "0");
                string Escopo = WebStorage.Get("Escopo", "0");
                int idAvaliacao = Convert.ToInt32(WebStorage.Get("idAvaliacao", "0"));

                var avaliacaoService = new AvaliacoesService();
                var competenciasService = new CompetenciasService();
                var avaliacao = avaliacaoService.ObterAvaliacaoCompetencia(idAssociado, idProjeto, idCompetencia, idPeriodo, TipoAvaliacao, Escopo, idAvaliacao);
                var competencia = competenciasService.ObterCompetencia(idCompetencia);

                

                // [Convivencia - Nova Régua]Verifica se todas as opções devem ser habilitadas para preenchimento do combobox
                // Caso o status de preenchimento da Avaliação esteja 3 - Concluído ou 4 - Congelado, inclui opções inativadas.
                bool enableAllOptions = avaliacao.DataHoraFimAvaliacaoCegas != null;

                if (ddl1 != null)
                    CarregaCombosNota(ref ddl1, enableAllOptions);

                if (ddl2 != null)
                    CarregaCombosNota(ref ddl2, enableAllOptions);

                CarregarAvaliacao(e.Item, avaliacao, competencia);
            }
        }

        private void CarregarAvaliacao(RepeaterItem repeaterItem, AVALIACOESCOMPETENCIAS avaliacao, COMPETENCIAS competencia)
        {
            HtmlSelect ddl1 = (HtmlSelect)repeaterItem.FindControl("ddlNotaNivel1");
            HtmlSelect ddl2 = (HtmlSelect)repeaterItem.FindControl("ddlNotaNivel2");
            HtmlTextArea txt1 = (HtmlTextArea)repeaterItem.FindControl("txtNivel1");

            var avaliacaoService = new AvaliacoesService();

            if (avaliacao != null)
            {
                txt1.InnerText = avaliacao.ComentariosAvaliacaoCegas;

                if (avaliacao.IdNotaNivel1AvaliacaoCegas == 0 || avaliacao.IdNotaNivel1AvaliacaoCegas == null)
                    ddl1.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });

                if (avaliacao.IdNotaNivel2AvaliacaoCegas == 0 || avaliacao.IdNotaNivel2AvaliacaoCegas == null)
                    ddl2.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });

                ddl1.Value = avaliacao.IdNotaNivel1AvaliacaoCegas.ToString();
                ddl2.Value = avaliacao.IdNotaNivel2AvaliacaoCegas.ToString();

                ddl1.Disabled = avaliacao.DataHoraFimAvaliacaoCegas != null;
                ddl2.Disabled = avaliacao.DataHoraFimAvaliacaoCegas != null;
                txt1.Disabled = avaliacao.DataHoraFimAvaliacaoCegas != null;
                // Verfica se está na etapa de Avaliação às Cegas
                var etapasPermitidas = new[] {
                    avaliacaoService.etapaAvaliacaoAsCegas,
                    avaliacaoService.etapaAutoAvaliacao,
                    avaliacaoService.etapaEmParalelo,
                    avaliacaoService.etapaNaoIniciada
                };

                if (!etapasPermitidas.Contains(avaliacao.PosicaoAtualFluxoAvaliacao))
                {
                    MessageBox.Show("A Avaliação Competência não está na fase de Avaliação às Cegas.", "Impossível Alterar", TIPO.Warning, MessageBoxHandler);
                    txt1.Disabled = true;
                    ddl1.Disabled = true;
                    ddl2.Disabled = true;
                }
            }
            else
            {
                MessageBox.Show("A Avaliação Competência ainda não foi finalizada pelo Associado.", "Impossível Prosseguir", TIPO.Warning, MessageBoxHandler);
                ddl1.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });
                ddl2.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });
                txt1.Disabled = true;
                ddl1.Disabled = true;
                ddl2.Disabled = true;
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarAvaliacao())
            {
                if (SalvarAvaliacao(false))
                    MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);
                else
                    MessageBox.Show("Falha ao Incluir Competência da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);
            }
        }

        private bool ValidarAvaliacao()
        {
            List<CompetenciaModel> listaRespostas = new List<CompetenciaModel>();
            foreach (RepeaterItem item in rptCompetencias.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    HtmlSelect ddl1 = (HtmlSelect)item.FindControl("ddlNotaNivel1");
                    HtmlSelect ddl2 = (HtmlSelect)item.FindControl("ddlNotaNivel2");
                    int notaDdl1 = Convert.ToInt32(ddl1.Value);
                    int notaDdl2 = Convert.ToInt32(ddl2.Value);
                    var idHiddenCompetencia = item.FindControl("IdCompetenciaItem") as HiddenField;
                    var getCompetencia = new CompetenciasService().ObterCompetencia(int.Parse(idHiddenCompetencia.Value));
                    var addCompetencia = new CompetenciaModel();

                    // NOVAS REGRAS DE PREENCHIMENTO
                    if (getCompetencia.IdModo == 1){ // VALIDAÇÃO PADRÃO

                        //Valida e preenche como "Não possuo" as notas do nível 2 para as notas de nível 1  igual  a Não se aplica
                        if (Convert.ToInt32(ddl1.Value) == 5)
                        {
                            if (Convert.ToInt32(ddl2.Value) != 5)
                            {
                                ddl2.Value = "5";
                                MessageBox.Show("ATENÇÃO ! Avaliação de Competência com inconsistência. (Detalhe: quando a nota de Competência do Nivel Atual for = Não Se Aplica, o Próximo Nivel deve ser Não Se Aplica). O Sistema ajustou sua avaliação. Favor revalidar sua avaliação !!", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);
                                ddl1.Style.Add("background-color", "#fffd91");
                                ddl2.Style.Add("background-color", "#fffd91");

                                return false;
                            }
                        }

                        //Valida se a nota do nível 2 é maior que o nível 1 e diferente da condição acima
                        var statusList = new NotasAvaliacaoService().ListaNotasCompetencias(false);

                        var ddl1Value = Convert.ToInt32(ddl1.Value);
                        var ddl2Value = Convert.ToInt32(ddl2.Value);

                        if ((ddl1Value == 0 && ddl2Value > 0) || (ddl1Value > 0 && ddl2Value == 0))
                        {
                            MessageBox.Show("É obrigatório selecionar uma nota para cada nível.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);
                            ddl1.Style.Add("background-color", "#fc7777");
                            ddl2.Style.Add("background-color", "#fc7777");
                            return false;
                        }

                        if ((ddl1Value > 0) && (ddl2Value > 0))
                        {
                            var pesoDdl1 = statusList.FirstOrDefault(x => x.IdNota == ddl1Value).Peso;
                            var pesoDdl2 = statusList.FirstOrDefault(x => x.IdNota == ddl2Value).Peso;

                            if (pesoDdl2 > pesoDdl1)
                            {
                                MessageBox.Show("Existem Avaliações de Competências inconsistentes. A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);
                                ddl1.Style.Add("background-color", "#fc7777");
                                ddl2.Style.Add("background-color", "#fc7777");

                                return false;
                            }
                        }
                    }
                    else if (getCompetencia.IdModo == 2) // MÉDIA TOTAL AV. LIDERANÇA
                    {
                        notaDdl1 = getCompetencia.IdNotaPadraoNivel1 ?? 5;
                        notaDdl2 = getCompetencia.IdNotaPadraoNivel2 ?? 5;
                    }

                    ddl1.Style.Remove("background-color");
                    ddl2.Style.Remove("background-color");

                    // CARREGA NOTAS AO PILAR
                    addCompetencia.notaValidaNivel1 = notaDdl1;
                    addCompetencia.notaValidaNivel2 = notaDdl2;
                    addCompetencia.ddl1 = ddl1;
                    addCompetencia.ddl2 = ddl2;
                    addCompetencia.IdModo = getCompetencia.IdModo;
                    addCompetencia.SubCompetencia = getCompetencia.SUBCOMPETENCIAS.SubCompetencia;
                    listaRespostas.Add(addCompetencia);
                }
            }

            // SEPARA PILARES (SUBCOMPETENCIAS)
            var listPilares = listaRespostas.Select(r => r.SubCompetencia).Distinct().ToList();
            var pilarVazio = false;
            foreach (var pilar in listPilares)
            {
                var countCompetenciasPilar = listaRespostas.Where(r => r.SubCompetencia == pilar).ToList();
                var countCompetenciasPadrao = countCompetenciasPilar.Where(r => r.IdModo != 2).ToList();
                var countCompetenciasAuto = countCompetenciasPilar.Where(r => r.IdModo == 2).ToList();
                var countRespostasVaziasNivel1 = countCompetenciasPadrao.Where(r => r.notaValidaNivel1 == 5).ToList();
                var countRespostasVaziasNivel2 = countCompetenciasPadrao.Where(r => r.notaValidaNivel2 == 5).ToList();

                if (countCompetenciasPilar != countCompetenciasAuto)
                {
                    if (countCompetenciasPadrao.Count == countRespostasVaziasNivel1.Count)
                    {
                        pilarVazio = true;
                        countRespostasVaziasNivel1.ForEach(r => r.ddl1.Style.Add("background-color", "#fc7777"));
                    }
                    if (countCompetenciasPadrao.Count == countRespostasVaziasNivel2.Count)
                    {
                        pilarVazio = true;
                        countRespostasVaziasNivel2.ForEach(r => r.ddl2.Style.Add("background-color", "#fc7777"));
                    }
                }
            }

            if (pilarVazio && WebStorage.Get("TipoAvaliacao", "") != "lideranca")
            {
                MessageBox.Show("Cada pilar precisa receber ao mínimo 1 nota mensurável em cada nível (diferente de não se aplica)", "ATENÇÃO", TIPO.Error, MessageBoxHandler);
                return false;
            }

            return true;
        }

        private bool SalvarAvaliacao(bool finalizarAvaliacao)
        {
            var idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
            var idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            var idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
            string TipoAvaliacao = WebStorage.Get("TipoAvaliacao", "0");
            string Escopo = WebStorage.Get("Escopo", "0");
            int idAvaliacao = Convert.ToInt32(WebStorage.Get("idAvaliacao", "0"));

            AvaliacoesService avaliacaoService = new AvaliacoesService();

            try
            {
                var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idAvaliacao);
                foreach (RepeaterItem item in rptCompetencias.Items)
                {
                    if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                    {
                        var idHiddenCompetencia = item.FindControl("IdCompetenciaItem") as HiddenField;

                        if (idHiddenCompetencia != null)
                        {
                            int idCompetencia = Convert.ToInt32(idHiddenCompetencia.Value);
                            var avaliacao = avaliacaoService.ObterAvaliacaoCompetencia(idAssociado, idProjeto, idCompetencia, idPeriodo, TipoAvaliacao, Escopo, idAvaliacao);

                           
                            // Busca Campos da LinhaItem atual
                            HtmlSelect ddl1 = (HtmlSelect)item.FindControl("ddlNotaNivel1");
                            HtmlSelect ddl2 = (HtmlSelect)item.FindControl("ddlNotaNivel2");
                            HtmlTextArea txt1 = (HtmlTextArea)item.FindControl("txtNivel1");

                            // Se ainda não existe, cria a Avaliação Competência
                            avaliacao.IdNotaNivel1AvaliacaoCegas = Convert.ToInt32(ddl1.Value);
                            avaliacao.IdNotaNivel2AvaliacaoCegas = Convert.ToInt32(ddl2.Value);
                            avaliacao.ComentariosAvaliacaoCegas = txt1.InnerText.Trim();
                            avaliacao.DHCAvaliacaoCegas = DateTime.Now;
                            avaliacao.DHCAvaliacaoCegas = DateTime.Now;
                            avaliacao.USRAvaliacaoCegas = WebStorage.GetUsuarioLogado().Id;
                            // INCLUIR FLUXO PARA PARELELO

                            if (avaliacao.DataHoraInicioAvaliacaoCegas == null || avaliacao?.DataHoraInicioAvaliacaoCegas == DateTime.MinValue)
                                avaliacao.DataHoraInicioAvaliacaoCegas = DateTime.Now;

                            if (finalizarAvaliacao)
                                avaliacao.DataHoraFimAvaliacaoCegas = DateTime.Now;

                            // Salva Alterações
                            if (avaliacaoService.AlterarAvaliacaoCompetencia(avaliacao.IdAvaliacaoCompetencia, avaliacao))
                            {
                                // Atualiza Status da Avaliação Email
                                avaliacaoEmail.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;
                                avaliacaoService.AlterarAvaliacaoEmail(avaliacaoEmail.idAvaliacao, avaliacaoEmail);

                                //Gera a informação para a próxima etapa da avaliação (Avaliação do Gestor) para reaproveitar esforços utilizados aqui para preenchimento da avaliação
                                //avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaAvaliacaoGestor;
                                // Mantem a mesma nota para a próxima etapa
                                avaliacao.IdNotaNivel1AvaliacaoGestor = Convert.ToInt32(ddl1.Value);
                                avaliacao.IdNotaNivel2AvaliacaoGestor = Convert.ToInt32(ddl2.Value);
                                avaliacao.ComentariosAvaliacaoGestor = txt1.InnerText.Trim();
                                avaliacao.DHCAvaliacaoGestor = DateTime.Now;
                                avaliacao.USRAvaliacaoGestor = WebStorage.GetUsuarioLogado().Id;
                                avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;


                                if (Convert.ToInt32(ddl1.Value) != 0)
                                    if (ddl1.Items[0].Text == "[Selecionar]")
                                        ddl1.Items.RemoveAt(0);

                                if (Convert.ToInt32(ddl2.Value) != 0)
                                    if (ddl2.Items[0].Text == "[Selecionar]")
                                        ddl2.Items.RemoveAt(0);

                                avaliacaoService.AlterarAvaliacaoCompetencia(avaliacao.IdAvaliacaoCompetencia, avaliacao);
                                
                                if (finalizarAvaliacao)
                                {
                                    // Avança para a Próxima Etapa
                                    avaliacaoService.AvancaProximaEtapaCompetencia(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
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

            foreach (RepeaterItem item in rptCompetencias.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    HtmlSelect ddl1 = (HtmlSelect)item.FindControl("ddlNotaNivel1");
                    HtmlSelect ddl2 = (HtmlSelect)item.FindControl("ddlNotaNivel2");
                    HtmlTextArea txt1 = (HtmlTextArea)item.FindControl("txtNivel1");

                    if (!ddl1.Disabled || !ddl2.Disabled || !txt1.Disabled)
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
            if (ValidarAvaliacao())
            {
                if (SalvarAvaliacao(false))
                    MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);
                else
                    MessageBox.Show("Falha ao Incluir Competência da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

                Response.Redirect($"~/avalizacao_gestorascegas.aspx?IdProjeto={WebStorage.Get("IdProjeto", "")}&IdAssociado={WebStorage.Get("IdAssociado", "")}&IdPeriodo={WebStorage.Get("IdPeriodo", "")}");

            }

        }

        protected void btnIrPerformance_Click(object sender, EventArgs e)
        {
            if (ValidarAvaliacao())
            {
                if (SalvarAvaliacao(false))
                    MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);

                else
                    MessageBox.Show("Falha ao Incluir Competência da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

                Response.Redirect("~/avalizacao_gestorascegas_performance.aspx");
            }




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

    }
}