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
    public partial class reworkavaliacao_competencia : System.Web.UI.Page
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

                var projeto = new ProjetosService().ObterProjeto(idProjeto);
                var associado = new AssociadosService().ObterAssociado(idAssociado);
                var periodo = new PeriodoService().ObterPeriodo(idPeriodo);

                lblProjeto.InnerText = projeto.Projeto;
                lblAssociado.InnerText = associado.Nome;
                lblPeriodo.InnerText = periodo.Periodo;
                lblCliente.InnerText = new ClientesService().ObterCliente(projeto.IdCliente).Cliente;
                // TEMPO DE PEERS E TEMPO DE CARGO
                lblTempoPeers.InnerText = new Util().TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDePeers).ToString();
                lblTempoCargo.InnerText = new Util().TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDeCargo).ToString();

                // Calcula Tempo Restante
                var avaliacaoService = new AvaliacoesService();
                var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idProjeto, idAssociado, idPeriodo, projeto.IdEmpresa, strTipoAvaliacao, strEscopo, idGestor);

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

                WebStorage.Set("IdProjeto", idProjeto.ToString());
                WebStorage.Set("IdAssociado", idAssociado.ToString());
                WebStorage.Set("IdPeriodo", idPeriodo.ToString());
                WebStorage.Set("TipoAvaliacao", strTipoAvaliacao);
                WebStorage.Set("Escopo", strEscopo);
                WebStorage.Set("idAvaliacao", avaliacaoEmail.idAvaliacao.ToString());
                WebStorage.Set("idGestor", strGestor);

                AVALIACOESCOMPETENCIAS avaliacaoCompetencia = new AvaliacoesService().ObterAvaliacaoCompetencia(associado.IdAssociado, idProjeto, idPeriodo, strTipoAvaliacao, strEscopo,
                    avaliacaoEmail.idAvaliacao);

                var competencias = new List<COMPETENCIAS>();

                if (avaliacaoCompetencia != null)
                { 
                    var listaCompetencias = new AvaliacoesService().ObterAvaliacoesCompetencias(associado.IdAssociado, idProjeto, idPeriodo, strTipoAvaliacao, strEscopo, avaliacaoEmail.idAvaliacao);
                    var listaIdCompetencias = listaCompetencias.Select(comp => comp.IdCompetencia).ToList();

                    competencias = new CompetenciasService().ObterListaCompetencias(associado.IdEmpresa, avaliacaoCompetencia.IdCargo, associado.IdNivel, avaliacaoCompetencia.TipoAvaliacao, 
                        avaliacaoCompetencia.Escopo, listaIdCompetencias);
                }
                else
                competencias = new CompetenciasService().ObterListaCompetencias(associado.IdEmpresa, associado.IdCargo, associado.IdNivel,
                    avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, null);

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
                List<DescricoesCargoModel> listaDescricoesCompetencias = new CargosService().ObterDescricoesCompetencias(cargoAvaliacao.IdCargo, competencias);
                rpt_TabelaCabecalho_Titulos.DataSource = listaDescricoesCompetencias;
                rpt_TabelaCabecalho_Titulos.DataBind();
                rpt_TabelaCabecalho_CargoAtual.DataSource = listaDescricoesCompetencias;
                rpt_TabelaCabecalho_CargoAtual.DataBind();
                rpt_TabelaCabecalho_CargoProximo.DataSource = listaDescricoesCompetencias;
                rpt_TabelaCabecalho_CargoProximo.DataBind();

                List<CompetenciaModel> listaCompetenciasModel = new List<CompetenciaModel>();

                var eixoService = new EixoService();
                var subCompetenciaService = new SubCompetenciasService();
                var dimensaoService = new DimensoesService();
                int lastEixo = -1;
                int lastSub = -1;
                int lastDimensao = -1;
                int cont = 0;
                int idEixo = -1;
                int IdSubCompetencia = -1;

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
                    }
                    else
                        linhaCompetencia.SubCompetencia = "";

                    // Verifica a Troca da linha da Dimensão
                    if (lastEixo != item.IdEixo && lastSub != item.IdSubCompetencia && lastDimensao != item.IdDimensao)
                    {
                        var dimensao = dimensaoService.ObterDimensao(item.IdDimensao);
                        linhaCompetencia.Dimensao = dimensao.Dimensao;
                        lastDimensao = item.IdDimensao;
                        lastSub = item.IdSubCompetencia;
                        lastEixo = item.IdEixo;
                    }
                    else
                        linhaCompetencia.Dimensao = "";

                    // COMENTÁRIO
                    // REFORMULAÇÃO 2022 - PRÓXIMO NÍVEL AGORA É DETALHAMENTO DA COMPETÊNCIA DO PRÓXIMO CARGO PRA MESMA SUBCOMPETENCIA
                    COMPETENCIAS competenciaProximoCargo = new COMPETENCIAS();
                    CompetenciasService competenciasService = new CompetenciasService();

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
                            lblDetalheNivel2.Visible = true;
                            tableDetalheNivel2.Visible = true;
                            break;

                        case 2: //Pleno
                            linhaCompetencia.DetalheNivelAtual = item.CompetenciaPLDetalhe;
                            linhaCompetencia.CompetenciaAtual = item.CompetenciaPL;
                            linhaCompetencia.CompetenciaProximo = item.CompetenciaSR;
                            linhaCompetencia.DetalheProximoNivel = competenciaProximoCargo != null ? competenciaProximoCargo.CompetenciaPLDetalhe : ""; // item.CompetenciaSRDetalhe;
                            linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "hidden";
                            lblDetalheNivel2.Visible = true;
                            tableDetalheNivel2.Visible = true;
                            break;

                        case 3: //Senior
                            linhaCompetencia.DetalheNivelAtual = item.CompetenciaSRDetalhe;
                            linhaCompetencia.CompetenciaAtual = item.CompetenciaSR;
                            linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "";
                            lblDetalheNivel2.Visible = true;
                            tableDetalheNivel2.Visible = true;
                            //em caso de  Sr, pega a lista de competências do próximo nível e faz um de para, porém considerando a ordem d e cadastro do banco para DE PARA  das duas funções Sr/ próximo nivel
                            List<COMPETENCIAS> nextCompetencia = new CompetenciasService().ObterCompetenciasDeCargoENivel(item.IdEmpresa, item.IdCargo + 1, 1);

                            if (nextCompetencia != null && nextCompetencia.Count >0)
                            {
                                linhaCompetencia.CompetenciaProximo = nextCompetencia[cont].CompetenciaJR;
                                linhaCompetencia.DetalheProximoNivel = nextCompetencia[cont].CompetenciaJRDetalhe;
                            }
                            else
                            {
                                linhaCompetencia.CompetenciaProximo = "Não existe parametrização para o próximo nível";
                                linhaCompetencia.DetalheProximoNivel = "Não existe parametrização para o próximo nível";
                            }
                            break;
                    }

                    cont++;
                    listaCompetenciasModel.Add(linhaCompetencia);
                }

                rptCompetencias.DataSource = listaCompetenciasModel;
                rptCompetencias.DataBind();

                DesabilitaBotoes();
            }
        }

        private void CarregaCombosNota(ref HtmlSelect ddl)
        {
            var statusList = new NotasAvaliacaoService().ListaNotasCompetencias(false);

            ddl.DataValueField = "IdNota";
            ddl.DataTextField = "CodigoNota";
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

                if (ddl1 != null)
                    CarregaCombosNota(ref ddl1);

                if (ddl2 != null)
                    CarregaCombosNota(ref ddl2);

                CarregarAvaliacao(e.Item);
            }
        }

        private static void CarregarAvaliacao(RepeaterItem repeaterItem)
        {
            HtmlSelect ddl1 = (HtmlSelect)repeaterItem.FindControl("ddlNotaNivel1");
            HtmlSelect ddl2 = (HtmlSelect)repeaterItem.FindControl("ddlNotaNivel2");
            HtmlTextArea txt1 = (HtmlTextArea)repeaterItem.FindControl("txtNivel1");

            HiddenField hiddenCompetencia = (HiddenField)repeaterItem.FindControl("IdCompetenciaItem");
            int idCompetencia = Convert.ToInt32(hiddenCompetencia.Value);
            int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
            string TipoAvaliacao = WebStorage.Get("TipoAvaliacao", "0");
            string Escopo = WebStorage.Get("Escopo", "0");
            int idAvaliacao = Convert.ToInt32(WebStorage.Get("idAvaliacao", "0"));

            var avaliacaoService = new AvaliacoesService();
            var avaliacao = avaliacaoService.ObterAvaliacaoCompetencia(idAssociado, idProjeto, idCompetencia, idPeriodo, TipoAvaliacao, Escopo, idAvaliacao);

            if (avaliacao != null)
            {
                txt1.InnerText = avaliacao.ComentariosAutoAvaliacao;

                if (avaliacao.IdNotaNivel1AutoAvaliacao == 0)
                     ddl1.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });

                if (avaliacao.IdNotaNivel2AutoAvaliacao == 0)
                    ddl2.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });


                ddl1.Value = avaliacao.IdNotaNivel1AutoAvaliacao.ToString();
                ddl2.Value = avaliacao.IdNotaNivel2AutoAvaliacao.ToString();

                if (avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaNaoIniciada &&
                    avaliacao.PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAutoAvaliacao)
                {
                    txt1.Disabled = true;
                    ddl1.Disabled = true;
                    ddl2.Disabled = true;
                }
            }
            else
            {
                txt1.InnerText = "";
                ddl1.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });
                ddl2.Items.Insert(0, new ListItem { Text = "[Selecionar]", Value = "0", Selected = false, Enabled = false });
                ddl1.SelectedIndex = 0;
                ddl2.SelectedIndex = 0;
                txt1.Disabled = false;
                ddl1.Disabled = false;
                ddl2.Disabled = false;
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

            foreach (RepeaterItem item in rptCompetencias.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    HtmlSelect ddl1 = (HtmlSelect)item.FindControl("ddlNotaNivel1");
                    HtmlSelect ddl2 = (HtmlSelect)item.FindControl("ddlNotaNivel2");

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
                    if (Convert.ToInt32(ddl1.Value) != 5 &&
                        Convert.ToInt32(ddl1.Value) != 0 &&
                        Convert.ToInt32(ddl2.Value) != 0 &&
                        Convert.ToInt32(ddl2.Value) < Convert.ToInt32(ddl1.Value))
                    {
                        MessageBox.Show("Existem Avaliações de Competências inconsistentes. A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);
                        ddl1.Style.Add("background-color", "#fc7777");
                        ddl2.Style.Add("background-color", "#fc7777");

                        return false;
                    }

                    ddl1.Style.Remove("background-color");
                    ddl2.Style.Remove("background-color");
                }
            }


            return true;
        }

        private bool SalvarAvaliacao(bool finalizarAvaliacao)
        {
            var idProjeto = Convert.ToInt32(WebStorage.Get("IdProjeto", "0"));
            var idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            var idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
            var associado = new AssociadosService().ObterAssociado(idAssociado);
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
                            if (avaliacao == null)
                            {
                                avaliacao = new AVALIACOESCOMPETENCIAS();
                                avaliacao.IdEmpresa = associado.IdEmpresa;
                                avaliacao.IdAssociado = idAssociado;
                                avaliacao.USRAutoAvaliacao = idAssociado;
                                avaliacao.IdCargo = associado.IdCargo;
                                avaliacao.IdNivel = associado.IdNivel;
                                avaliacao.IdProjeto = idProjeto;
                                avaliacao.IdPeriodo = idPeriodo;
                                avaliacao.IdCompetencia = idCompetencia;
                                avaliacao.IdAvaliacaoStatus = 2; // Adiciona com Status "Em Andamento"
                                avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaAutoAvaliacao;
                                avaliacao.DataHoraInicio = DateTime.Now;
                                avaliacao.DHCAutoAvaliacao = DateTime.Now;
                                avaliacao.USR = WebStorage.GetUsuarioLogado().Id;
                                avaliacao.DHC = DateTime.Now;
                                avaliacao.ATV = 1;
                                avaliacao.IdNotaNivel1AutoAvaliacao = Convert.ToInt32(ddl1.Value);
                                avaliacao.IdNotaNivel2AutoAvaliacao = Convert.ToInt32(ddl2.Value);
                                avaliacao.ComentariosAutoAvaliacao = txt1.InnerText.Trim(); ;
                                avaliacao.DataHoraInicioAutoAvaliacao = DateTime.Now;
                                avaliacao.TipoAvaliacao = "desempenho";
                                avaliacao.Escopo = "projeto";
                                avaliacao.idAvaliacao = idAvaliacao;

                                if (!avaliacaoService.SalvarAvaliacaoCompetencia(avaliacao))
                                    return false;
                            }
                            else
                            {
                                avaliacao.IdNotaNivel1AutoAvaliacao = Convert.ToInt32(ddl1.Value);
                                avaliacao.IdNotaNivel2AutoAvaliacao = Convert.ToInt32(ddl2.Value);
                                avaliacao.ComentariosAutoAvaliacao = txt1.InnerText.Trim();

                                // Se estiver como "Não Iniciada", muda o Status para "Em Andamento"
                                if (avaliacao.IdAvaliacaoStatus == 1)
                                {
                                    avaliacao.IdAvaliacaoStatus = 2;
                                    avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaAutoAvaliacao;

                                    if (avaliacao.DataHoraInicio == DateTime.MinValue)
                                        avaliacao.DataHoraInicio = DateTime.Now;
                                    avaliacao.DataHoraInicioAutoAvaliacao = DateTime.Now;
                                }
                            }

                            if (Convert.ToInt32(ddl1.Value) != 0)
                                if (ddl1.Items[0].Text == "[Selecionar]")
                                    ddl1.Items.RemoveAt(0);

                            if (Convert.ToInt32(ddl2.Value) != 0)
                                if (ddl2.Items[0].Text == "[Selecionar]")
                                    ddl2.Items.RemoveAt(0);

                            if (finalizarAvaliacao)
                                avaliacao.DataHoraFimAutoAvaliacao = DateTime.Now;

                            // Salva Alterações
                            if (avaliacaoService.AlterarAvaliacaoCompetencia(avaliacao.IdAvaliacaoCompetencia, avaliacao))
                            {
                                // Atualiza Status da Avaliação Email
                                avaliacaoEmail.idStatus = avaliacao.IdAvaliacaoStatus;
                                avaliacaoEmail.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;
                                avaliacaoService.AlterarAvaliacaoEmail(avaliacaoEmail.idAvaliacao, avaliacaoEmail);

                                if (finalizarAvaliacao)
                                {
                                    // Avança para a Próxima Etapa
                                    avaliacaoService.AvancaProximaEtapaCompetencia(avaliacao);
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

            btnSalvar.Visible = btnSalvar2.Visible = btnFinalizar.Visible = !tudoFinalizado;
            //btnSalvar.Visible = btnSalvar2.Visible = btnFinalizar.Visible = btnFinalizar2.Visible = !tudoFinalizado;
        }

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (ValidarAvaliacao())
            {
                if (SalvarAvaliacao(false))
                    MessageBox.Show("Avaliação Salva com Sucesso.", "Salvar Avaliação", TIPO.Info, MessageBoxHandler);

                else
                    MessageBox.Show("Falha ao Incluir Competência da Avaliação.", "Erro ao Incluir", TIPO.Error, MessageBoxHandler);

                Response.Redirect($"~/autoavalizacao.aspx?IdProjeto={WebStorage.Get("IdProjeto", "")}&IdAssociado={WebStorage.Get("IdAssociado", "")}&IdPeriodo={WebStorage.Get("IdPeriodo", "")}");

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

                Response.Redirect("~/autoavalizacao_performance.aspx");
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
