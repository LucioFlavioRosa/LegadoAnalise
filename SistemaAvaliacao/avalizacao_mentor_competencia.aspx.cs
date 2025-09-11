using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace SistemaAvaliacao
{
    public partial class avalizacao_mentor_competencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    var strProjeto = Request.QueryString["IdProjeto"];
                    var strAssociado = Request.QueryString["IdAssociado"];
                    var strPeriodo = Request.QueryString["IdPeriodo"];
                    int idProjeto = 0;
                    int idAssociado = 0;
                    int idPeriodo = 0;
                    var strTipoAvaliacao = Request.QueryString["TipoAvaliacao"];
                    var strEscopo = Request.QueryString["Escopo"];
                    var strGestor = Request.QueryString["idGestor"];
                    var strExibir = Request.QueryString["Exibir"];

                    if (!int.TryParse(strProjeto, out idProjeto))
                    {
                        strProjeto = WebStorage.Get("IdProjeto", "");

                        if (!int.TryParse(strProjeto, out idProjeto))
                        {
                            MessageBox.Show("É obrigatório a seleção de um Projeto.", "Projeto Não Encontrado", TIPO.Info, MessageBoxHandler);
                            return;
                        }
                    }

                    if (!int.TryParse(strAssociado, out idAssociado))
                    {
                        strAssociado = WebStorage.Get("IdAssociado", "");

                        if (!int.TryParse(strAssociado, out idAssociado))
                        {
                            MessageBox.Show("É obrigatório a seleção de um Associado.", "Associado Não Encontrado", TIPO.Info, MessageBoxHandler);
                            return;
                        }
                    }

                    if (!int.TryParse(strPeriodo, out idPeriodo))
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

                    if (strExibir == "" || strExibir == null)
                    {
                        strExibir = WebStorage.Get("Exibir", "Tudo");
                    }

                    var associado = new AssociadosService().ObterAssociado(idAssociado);
                    var periodo = new PeriodoService().ObterPeriodo(idPeriodo);
                    var pessoaService = new AssociadosService();
                    AVALIACAO avaliacaoEmail = null;
                    List<COMPETENCIAS> competencias = null;
                    CARGOS cargoAvaliacao = null;
                    AVALIACOESCOMPETENCIAS avaliacaoCompetencia = null;

                    lblAssociado.InnerText = associado.Nome;
                    lblPeriodo.InnerText = periodo.Periodo;
                    lblCargo.InnerText = associado.CARGOS.Cargo;
                    // TEMPO DE PEERS E TEMPO DE CARGO
                    lblTempoPeers.InnerText = new Util().TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDePeers).ToString();
                    lblTempoCargo.InnerText = new Util().TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDeCargo).ToString();

                    WebStorage.Set("IdProjeto", idProjeto.ToString());
                    WebStorage.Set("IdAssociado", idAssociado.ToString());
                    WebStorage.Set("IdPeriodo", idPeriodo.ToString());
                    WebStorage.Set("TipoAvaliacao", strTipoAvaliacao);
                    WebStorage.Set("Escopo", strEscopo);
                    WebStorage.Set("idGestor", idGestor.ToString());
                    WebStorage.Set("Exibir", strExibir);

                    if (idProjeto > -1 && idGestor > -1)
                    {
                        var projeto = new ProjetosService().ObterProjeto(idProjeto);
                        var gestor = pessoaService.ObterAssociado(projeto.IdAssociadoGestor);

                        lblProjeto.InnerText = projeto.Projeto;
                        lblCliente.InnerText = new ClientesService().ObterCliente(projeto.IdCliente).Cliente;
                        lblGestor.InnerText = gestor.Nome;

                        var avaliacaoService = new AvaliacoesService();
                        avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idProjeto, idAssociado, idPeriodo, projeto.IdEmpresa, strTipoAvaliacao, strEscopo, idGestor);

                        WebStorage.Set("idAvaliacao", avaliacaoEmail.idAvaliacao.ToString());

                        //Obtém o cargo do colaborador no momento daquela avaliação
                        int idCargoNaAvaliacao = new AvaliacoesService().ObterAvaliacaoCompetencia(associado.IdAssociado, projeto.IdProjeto, periodo.IdPeriodo, strTipoAvaliacao, strEscopo,
                            avaliacaoEmail.idAvaliacao).IdCargo;
                        int idNivelNaAvaliacao = new AvaliacoesService().ObterAvaliacaoCompetencia(associado.IdAssociado, projeto.IdProjeto, periodo.IdPeriodo, strTipoAvaliacao, strEscopo,
                            avaliacaoEmail.idAvaliacao).IdNivel;

                        associado.IdCargo = idCargoNaAvaliacao;
                        associado.IdNivel = idNivelNaAvaliacao;

                        var listaCompetencias = new AvaliacoesService().ObterAvaliacoesCompetencias(associado.IdAssociado, idProjeto, idPeriodo, strTipoAvaliacao, strEscopo, avaliacaoEmail.idAvaliacao);
                        var listaIdCompetencias = listaCompetencias.Select(comp => comp.IdCompetencia).ToList();
                        competencias = new CompetenciasService().ObterListaCompetencias(associado.IdEmpresa, idCargoNaAvaliacao, idNivelNaAvaliacao,
                            avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, listaIdCompetencias);

                        List<CompetenciaModel> listaCompetenciasModel = new List<CompetenciaModel>();
                        avaliacaoCompetencia = avaliacaoEmail != null ? new AvaliacoesService().ObterAvaliacaoCompetencia(associado.IdAssociado, idProjeto, idPeriodo,
                            avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, avaliacaoEmail.idAvaliacao) : null;
                        cargoAvaliacao = avaliacaoCompetencia != null ? avaliacaoCompetencia.CARGOS : associado.CARGOS;

                        var eixoService = new EixoService();
                        var subCompetenciaService = new SubCompetenciasService();
                        var dimensaoService = new DimensoesService();
                        int lastEixo = -1;
                        int lastSub = -1;
                        int lastDimensao = -1;
                        int cont = 0;
                        int idEixo = -1;
                        int IdSubCompetencia = -1;

                        #region OLD - ORGANIZADOR DE COMPETENCIAS
                        //foreach (var item in competencias)
                        //{
                        //    if (IdSubCompetencia != item.IdSubCompetencia)
                        //    {
                        //        lastEixo = -1;
                        //        lastSub = -1;
                        //        lastDimensao = -1;
                        //    }

                        //    idEixo = item.IdEixo;
                        //    IdSubCompetencia = item.IdSubCompetencia;

                        //    var linhaCompetencia = new CompetenciaModel();
                        //    linhaCompetencia.IdCompetencia = item.IdCompetencia;
                        //    linhaCompetencia.PalavrasChave = item.PalavrasChave != null ? item.PalavrasChave.Replace(Convert.ToChar(10).ToString(), "<br>") : "";

                        //    // Verifica a Troca da linha do Eixo
                        //    if (lastEixo != item.IdEixo)
                        //    {
                        //        var eixo = eixoService.ObterEixo(item.IdEixo);
                        //        linhaCompetencia.Eixo = eixo.Eixo;
                        //    }
                        //    else
                        //        linhaCompetencia.Eixo = "";

                        //    // Verifica a Troca da linha da SubCompetencia
                        //    if (lastEixo != item.IdEixo && lastSub != item.IdSubCompetencia)
                        //    {
                        //        var subCompetencia = subCompetenciaService.ObterCompetencia(item.IdSubCompetencia);
                        //        linhaCompetencia.SubCompetencia = subCompetencia.SubCompetencia;
                        //    }
                        //    else
                        //        linhaCompetencia.SubCompetencia = "";

                        //    // Verifica a Troca da linha da Dimensão
                        //    if (lastEixo != item.IdEixo && lastSub != item.IdSubCompetencia && lastDimensao != item.IdDimensao)
                        //    {
                        //        var dimensao = dimensaoService.ObterDimensao(item.IdDimensao);
                        //        linhaCompetencia.Dimensao = dimensao.Dimensao;
                        //        lastDimensao = item.IdDimensao;
                        //        lastSub = item.IdSubCompetencia;
                        //        lastEixo = item.IdEixo;
                        //    }
                        //    else
                        //        linhaCompetencia.Dimensao = "";

                        //    // REFORMULAÇÃO 2022 - PRÓXIMO NÍVEL AGORA É DETALHAMENTO DA COMPETÊNCIA DO PRÓXIMO CARGO PRA MESMA SUBCOMPETENCIA
                        //    COMPETENCIAS competenciaProximoCargo = new COMPETENCIAS();
                        //    CompetenciasService competenciasService = new CompetenciasService();

                        //    if (cargoAvaliacao.idProximoCargo != null)
                        //    {
                        //        competenciaProximoCargo = competenciasService.ObterCompetenciaCargoSub(cargoAvaliacao.CARGOS2.IdCargo, item.IdSubCompetencia, item.PalavrasChave);
                        //    }

                        //    switch (associado.IdNivel)
                        //    {
                        //        case 1: //Junior
                        //            linhaCompetencia.DetalheNivelAtual = item.CompetenciaJRDetalhe;
                        //            linhaCompetencia.CompetenciaAtual = item.CompetenciaJR;
                        //            linhaCompetencia.CompetenciaProximo = item.CompetenciaPL;
                        //            linhaCompetencia.DetalheProximoNivel = competenciaProximoCargo != null ? competenciaProximoCargo.CompetenciaJRDetalhe : ""; // item.CompetenciaPLDetalhe;
                        //            linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "hidden";
                        //            lblDetalheNivel2.Visible = true;
                        //            tableDetalheNivel2.Visible = true;


                        //            break;

                        //        case 2: //Pleno
                        //            linhaCompetencia.DetalheNivelAtual = item.CompetenciaPLDetalhe;
                        //            linhaCompetencia.CompetenciaAtual = item.CompetenciaPL;
                        //            linhaCompetencia.CompetenciaProximo = item.CompetenciaSR;
                        //            linhaCompetencia.DetalheProximoNivel = competenciaProximoCargo != null ? competenciaProximoCargo.CompetenciaPLDetalhe : ""; // item.CompetenciaSRDetalhe;
                        //            linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "hidden";
                        //            lblDetalheNivel2.Visible = true;
                        //            tableDetalheNivel2.Visible = true;
                        //            break;

                        //        case 3: //Senior
                        //            linhaCompetencia.DetalheNivelAtual = item.CompetenciaSRDetalhe;
                        //            linhaCompetencia.CompetenciaAtual = item.CompetenciaSR;
                        //            linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "";
                        //            lblDetalheNivel2.Visible = true;
                        //            tableDetalheNivel2.Visible = true;
                        //            //em caso de  Sr, pega a lista de competências do próximo nível e faz um de para, porém considerando a ordem d e cadastro do banco para DE PARA  das duas funções Sr/ próximo nivel
                        //            List<COMPETENCIAS> nextCompetencia = new CompetenciasService().ObterCompetenciasDeCargoENivel(item.IdEmpresa, item.IdCargo + 1, 1);

                        //            if (nextCompetencia != null && nextCompetencia.Count > 0)
                        //            {
                        //                linhaCompetencia.CompetenciaProximo = nextCompetencia[cont].CompetenciaJR;
                        //                linhaCompetencia.DetalheProximoNivel = nextCompetencia[cont].CompetenciaJRDetalhe;
                        //            }
                        //            else
                        //            {
                        //                linhaCompetencia.CompetenciaProximo = "Não existe parametrização para o próximo nível";
                        //                linhaCompetencia.DetalheProximoNivel = "Não existe parametrização para o próximo nível";
                        //            }
                        //            break;
                        //    }

                        //    cont++;
                        //    listaCompetenciasModel.Add(linhaCompetencia);
                        //}
                        #endregion

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
                                    if (trocaTitulos)
                                    {
                                        lblDetalheNivel2.Visible = true;
                                        tableDetalheNivel2.Visible = true;
                                    }
                                    break;
                            }

                            cont++;

                            listaCompetenciasModel.Add(linhaCompetencia);
                        }

                        rptCompetencias.DataSource = listaCompetenciasModel;
                        rptCompetencias.DataBind();

                        //Radar
                        var radar = new ConsolidacaoService().RadarFeedback(idAssociado, idProjeto, idPeriodo, associado.IdCargo);
                        if (radar != null)
                        {
                            hfJsonRadar.Value = JsonRadar(radar);
                        }

                        CarregarResultados(idAssociado, idPeriodo, associado, strTipoAvaliacao, strEscopo);
                    }
                    else
                    {
                        competencias = new CompetenciasService().ObterListaCompetencias(associado.IdEmpresa, associado.IdCargo, associado.IdNivel, strTipoAvaliacao, strEscopo, null);

                        botaoPerformance.Visible = false;
                        btn_Performance_2.Visible = false;
                        accordionCompetencia.Visible = false;
                        divCompetencias.Visible = false;
                        accordionResultados.Visible = false;
                    }

                    // CABEÇALHO DE DESCRIÇÕES - LABELS FIXOS
                    avaliacaoCompetencia = avaliacaoEmail != null ? new AvaliacoesService().ObterAvaliacaoCompetencia(associado.IdAssociado, idProjeto, idPeriodo, 
                        avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, avaliacaoEmail.idAvaliacao) : null;
                    cargoAvaliacao = avaliacaoCompetencia != null ? avaliacaoCompetencia.CARGOS : associado.CARGOS;

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
                    List<DescricoesCargoModel> listaDescricoesCompetencias = new CargosService().ObterDescricoesCompetencias(cargoAvaliacao.IdCargo, competencias, associado);
                    rpt_TabelaCabecalho_Titulos.DataSource = listaDescricoesCompetencias;
                    rpt_TabelaCabecalho_Titulos.DataBind();
                    rpt_TabelaCabecalho_CargoAtual.DataSource = listaDescricoesCompetencias;
                    rpt_TabelaCabecalho_CargoAtual.DataBind();
                    rpt_TabelaCabecalho_CargoProximo.DataSource = listaDescricoesCompetencias;
                    rpt_TabelaCabecalho_CargoProximo.DataBind();

                    // ESCONDE ACCORDIONS CASO MENTOR TENHA ENTRADO PELA OPÇÃO "FEEDBACK RH"
                    if (strExibir == "FeedbackRH")
                    {
                        accordionConsideracoesMentor.Visible = false;
                        divCompetencias.Visible = false;
                    }

                    CarregarCombosConsideracoes();
                    CarregarConsideracoesMentor();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", TIPO.Error, MessageBoxHandler);
                }
            }
        }

        private string JsonRadar(ResultadoProjetosModel projeto)
        {
            var listradar = new List<dynamic>();

            dynamic obj = new JObject();

            List<string> labels = new List<string>();
            List<decimal> datasetAvaliado = new List<decimal>();
            List<decimal> datasetGestor = new List<decimal>();
            List<decimal> datasetAtual = new List<decimal>();
            List<decimal> datasetProximoNivel = new List<decimal>();

            /*
              PercentualSomaNotaFinalN1N2 = somaPercentualN1N2,
              PercentualSomaNotaFinalN1N2Avaliado = somaPercentualN1N2Avaliado
             */

            foreach (var item in projeto.ListSomaCompetenciasN1N2)
            {
                labels.Add(item.Eixo);
                datasetGestor.Add(item.PercentualSomaNotaFinalN1N2.HasValue ? Math.Round(item.PercentualSomaNotaFinalN1N2.Value) : 0);
                datasetAvaliado.Add(item.PercentualSomaNotaFinalN1N2Avaliado.HasValue ? Math.Round(item.PercentualSomaNotaFinalN1N2Avaliado.Value) : 0);
                datasetAtual.Add(100);
                datasetProximoNivel.Add(200);
            }

            obj.labels = new JArray(labels);
            obj.datasetavaliado = new JArray(datasetAvaliado);
            obj.datasetgestor = new JArray(datasetGestor);
            obj.datasetatual = new JArray(datasetAtual);
            obj.datasetproximonivel = new JArray(datasetProximoNivel);

            listradar.Add(obj);

            return JsonConvert.SerializeObject(listradar);

        }

        protected void rptCompetencias_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                CarregarAvaliacao(e.Item);
            }
        }

        private void CarregarAvaliacao(RepeaterItem repeaterItem)
        {
            //Estáticos
            HiddenField hiddenCompetencia = (HiddenField)repeaterItem.FindControl("IdCompetenciaItem");
            HtmlTableCell lblNota1Avaliado = (HtmlTableCell)repeaterItem.FindControl("lblNota1Avaliado");
            HtmlTableCell lblNota1Cegas = (HtmlTableCell)repeaterItem.FindControl("lblNota1Cegas");
            HtmlTableCell lblNota1Gestor = (HtmlTableCell)repeaterItem.FindControl("lblNota1Gestor");
            HtmlTableCell lblNota1Feedback = (HtmlTableCell)repeaterItem.FindControl("lblNota1Feedback");

            HtmlTableCell lblNota2Avaliado = (HtmlTableCell)repeaterItem.FindControl("lblNota2Avaliado");
            HtmlTableCell lblNota2Cegas = (HtmlTableCell)repeaterItem.FindControl("lblNota2Cegas");
            HtmlTableCell lblNota2Gestor = (HtmlTableCell)repeaterItem.FindControl("lblNota2Gestor");
            HtmlTableCell lblNota2Feedback = (HtmlTableCell)repeaterItem.FindControl("lblNota2Feedback");

            Label lblObservacaoAvaliado = (Label)repeaterItem.FindControl("lblObservacaoAvaliado");
            Label lblObservacaoGestor = (Label)repeaterItem.FindControl("lblObservacaoGestor");

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
                var notasService = new NotasAvaliacaoService();

                //Estáticos
                var nota = notasService.ObterNotaCompetencia(avaliacao.IdNotaNivel1AutoAvaliacao);
                lblNota1Avaliado.InnerText = nota.CodigoNota;
                lblObservacaoAvaliado.Text = avaliacao.ComentariosAutoAvaliacao;

                nota = notasService.ObterNotaCompetencia(avaliacao.IdNotaNivel2AutoAvaliacao);
                lblNota2Avaliado.InnerText = nota.CodigoNota;

                nota = notasService.ObterNotaCompetencia(Convert.ToInt32(avaliacao.IdNotaNivel1AvaliacaoCegas));
                lblNota1Cegas.InnerText = nota.CodigoNotaAvaliador;
                lblObservacaoGestor.Text = avaliacao.ComentariosAvaliacaoCegas;

                nota = notasService.ObterNotaCompetencia(Convert.ToInt32(avaliacao.IdNotaNivel1AvaliacaoGestor));
                lblNota1Gestor.InnerText = nota.CodigoNotaAvaliador;
                lblObservacaoGestor.Text += " | " + avaliacao.ComentariosAvaliacaoGestor;

                nota = notasService.ObterNotaCompetencia(Convert.ToInt32(avaliacao.IdNotaNivel2AvaliacaoCegas));
                lblNota2Cegas.InnerText = nota.CodigoNotaAvaliador;

                nota = notasService.ObterNotaCompetencia(Convert.ToInt32(avaliacao.IdNotaNivel2AvaliacaoGestor));
                lblNota2Gestor.InnerText = nota.CodigoNotaAvaliador;

                nota = notasService.ObterNotaCompetencia(Convert.ToInt32(avaliacao.IdNotaNivel1Feedback));
                lblNota1Feedback.InnerText = nota.CodigoNotaAvaliador;
                lblObservacaoGestor.Text += " | " + avaliacao.ComentariosFeedback;
                
                nota = notasService.ObterNotaCompetencia(Convert.ToInt32(avaliacao.IdNotaNivel2Feedback));
                lblNota2Feedback.InnerText = nota.CodigoNotaAvaliador;
            }
            else
            {
                //MessageBox.Show("O Feedback da Avaliação Competência ainda não foi finalizado.", "Impossível Prosseguir", TIPO.Warning, MessageBoxHandler);
            }
        }

        public void CarregarCombosConsideracoes()
        {
            ddlElegivelPromocao.Items.Clear();
            ddlElegivelPromocao.Items.Insert(0, "Não");
            ddlElegivelPromocao.Items.Insert(1, "Sim");
            ddlInputPromocao.Items.Clear();
            ddlInputPromocao.Items.Insert(0, "Não");
            ddlInputPromocao.Items.Insert(1, "Sim");
        }
        public void CarregarConsideracoesMentor()
        {
            int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
            string TipoAvaliacao = WebStorage.Get("TipoAvaliacao", "0");
            string Escopo = WebStorage.Get("Escopo", "0");

            var associadosService = new AssociadosService();
            var cargosService = new CargosService();
            var consideracoesMentorService = new ConsideracoesMentorService();
            var avaliacoesService = new AvaliacoesService();
            var projetosService = new ProjetosService();
            var complexidadeService = new ComplexidadesService();

            var mentor = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);

            var consideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorPorAtributos(mentor.IdAssociado, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
            if (consideracoesMentor == null)
            {
                CONSIDERACOESMENTOR addConsideracoesMentor = new CONSIDERACOESMENTOR();
                addConsideracoesMentor.idMentor = mentor.IdAssociado;
                addConsideracoesMentor.idAssociado = idAssociado;
                addConsideracoesMentor.idPeriodo = idPeriodo;
                addConsideracoesMentor.TipoAvaliacao = TipoAvaliacao;
                addConsideracoesMentor.Escopo = Escopo;
                addConsideracoesMentor.LiberadoRH = false;
                addConsideracoesMentor.AcaoComite = "-";
                addConsideracoesMentor.PontosFortesRH = "-";
                addConsideracoesMentor.PontosFracosRH = "-";
                addConsideracoesMentor.SalarioAtual = 1;
                addConsideracoesMentor.SalarioNovo = 1;
                addConsideracoesMentor.RegimeContratacaoAtual = "-";
                addConsideracoesMentor.RegimeContratacaoNovo = "-";
                addConsideracoesMentor.MentoriaRealizada = false;

                CONSIDERACOESMENTOR ultimaConsideracoesMentor = new CONSIDERACOESMENTOR();
                ultimaConsideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorUltimaDoAvaliado(idAssociado, idPeriodo, TipoAvaliacao, Escopo);
                if (ultimaConsideracoesMentor != null && ultimaConsideracoesMentor.idConsideracoesMentor > 0)
                {
                    addConsideracoesMentor.SalarioAtual = ultimaConsideracoesMentor.SalarioAtual;
                    addConsideracoesMentor.RegimeContratacaoAtual = ultimaConsideracoesMentor.RegimeContratacaoAtual;
                }

                consideracoesMentorService.AdicionarConsideracoesMentor(addConsideracoesMentor);
                consideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorPorAtributos(mentor.IdAssociado, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
            }

            WebStorage.Set("idConsideracoesMentor", consideracoesMentor.idConsideracoesMentor.ToString());

            var avaliacoes = avaliacoesService.ObterAvaliacoesAssociadoSemestre(idAssociado, idPeriodo, TipoAvaliacao);
            string addTextProjetosEnvolvidos = "";
            foreach (var avaliacao in avaliacoes)
            {
                addTextProjetosEnvolvidos += projetosService.ObterProjeto(avaliacao.idProjeto).Projeto + " (" +
                    complexidadeService.ObterComplexidade(projetosService.ObterProjeto(avaliacao.idProjeto).IdComplexidade).Complexidade + ")";
                if (avaliacoes.IndexOf(avaliacao) < avaliacoes.Count - 1) { addTextProjetosEnvolvidos += "<br />"; }
            }

            var cargoAtual = cargosService.ObterCargo(associadosService.ObterAssociado(consideracoesMentor.idAssociado).IdCargo);
            var associado = associadosService.ObterAssociado(consideracoesMentor.idAssociado);
            var ultimaPromocao = cargosService.ObterUltimaPromocaoAssociado(associado.IdAssociado);

            // CONFIGURAR VISIVEIS SOMENTE APÓS LIBERAÇÃO DO RH
            if ((bool)!consideracoesMentor.LiberadoRH) { 
                accordionDadosRH.Visible = false; 
                rowMentoriaRealizada.Visible = false; }

            // ACCORDION CONSIDERAÇÕES MENTOR
            ddlElegivelPromocao.SelectedIndex = Convert.ToInt32(consideracoesMentor.ElegivelPromocao);
            ddlInputPromocao.SelectedIndex = Convert.ToInt32(consideracoesMentor.InputPromocao);
            textTrajetoria.Text = consideracoesMentor.TrajetoriaAssociado;
            textPontosFortes.Text = consideracoesMentor.PontosFortes;
            textPontosFracos.Text = consideracoesMentor.PontosFracos;
            labelAssociado.Text = associado.Nome;
            labelCargo.Text = cargoAtual.Cargo;
            labelVertical.Text = associado.Vertical;
            labelProjetosEnvolvidos.Text = addTextProjetosEnvolvidos;
            cboxMentoriaRealizada.Checked = consideracoesMentor.MentoriaRealizada ? (bool)consideracoesMentor.MentoriaRealizada : false;
            labelTempoDePeers.Text = "-";
            labelTempoDeCargo.Text = "-";
            if (ultimaPromocao != null){
                int promocaoAnos = DateTime.Now.Year - ((DateTime)ultimaPromocao.DataPromocao).Year;
                int promocaoMesesTotal = (promocaoAnos * 12) + DateTime.Now.Month - ((DateTime)ultimaPromocao.DataPromocao).Month;
                int promocaoMesesRestantes = promocaoMesesTotal - (promocaoAnos * 12);
                labelTempoDeCargo.Text =
                    (promocaoAnos > 0 ? promocaoAnos.ToString() + " ano" + (promocaoAnos > 1 ? "s" : "") : "") +
                    (promocaoAnos > 0 && promocaoMesesRestantes > 0 ? " e " : "") +
                    (promocaoMesesRestantes > 0 ? promocaoMesesRestantes.ToString() + 
                        (promocaoMesesRestantes > 1 ? " meses" : " mês") : "");
            }
            if (associado.DataAdmissao != null){
                int admissaoAnos = DateTime.Now.Year - ((DateTime)associado.DataAdmissao).Year;
                int admissaoMesesTotal = (admissaoAnos * 12) + DateTime.Now.Month - ((DateTime)associado.DataAdmissao).Month;
                int admissaoMesesRestantes = admissaoMesesTotal - (admissaoAnos * 12);

                labelTempoDePeers.Text =
                    (admissaoAnos > 0 ? admissaoAnos.ToString() + " ano" + (admissaoAnos > 1 ? "s" : "") : "") +
                    (admissaoAnos > 0 && admissaoMesesRestantes > 0 ? " e " : "") +
                    (admissaoMesesRestantes > 0 ? admissaoMesesRestantes.ToString() +
                        (admissaoMesesRestantes > 1 ? " meses" : " mês") : "");

                if (admissaoMesesTotal <= 12){
                    labelElegivelPromocao.Text = "Readequação";}
                else{
                    if (ultimaPromocao != null){
                        int promocaoAnos = DateTime.Now.Year - ((DateTime)ultimaPromocao.DataPromocao).Year;
                        int promocaoMesesTotal = (promocaoAnos * 12) + DateTime.Now.Month - ((DateTime)ultimaPromocao.DataPromocao).Month;
                        if (promocaoMesesTotal >= cargoAtual.TempoMinimoPromocao){
                            labelElegivelPromocao.Text = "Sim";}
                        else{labelElegivelPromocao.Text = "Não";}}
                    else{labelElegivelPromocao.Text = "Não há última promoção!";}}}
            else{labelElegivelPromocao.Text = "Não há data de admissão!";}

            // TEMPORÁRIO ENQUANTO O RH REFAZ O RACIONAL DE ELEBIGILIDADE
            labelElegivelPromocao.Text = consideracoesMentor.ElegivelPromocao ? "Sim" : "Não";
            
            // ACCORDION DADOS RH
            labelAcaoComite.Text = consideracoesMentor.AcaoComite;
            labelPontosFortesRH.Text = consideracoesMentor.PontosFortesRH;
            labelPontosFracosRH.Text = consideracoesMentor.PontosFracosRH;
            labelProximoCargo.Text = "-";
            labelSalarioAtual.Text = "R$ " + consideracoesMentor.SalarioAtual.ToString().Replace(".", ",");
            labelProximoSalario.Text = "R$ " + consideracoesMentor.SalarioNovo.ToString().Replace(".", ",");
            labelIncremento.Text = Math.Round((double)(((consideracoesMentor.SalarioNovo / consideracoesMentor.SalarioAtual) - 1) * 100), 2).ToString() + "%";
            labelRegimeContratacaoAtual.Text = consideracoesMentor.RegimeContratacaoAtual;
            labelRegimeContratacaoNovo.Text = consideracoesMentor.RegimeContratacaoNovo;
            if (cargoAtual.idProximoCargo != null){
                labelProximoCargo.Text = cargosService.ObterCargo((int)cargoAtual.idProximoCargo).Cargo;
            }
        }
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            var consideracoesMentorService = new ConsideracoesMentorService();

            int idConsideracoesMentor = Convert.ToInt32(WebStorage.Get("idConsideracoesMentor", "0"));
            var consideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorPorId(idConsideracoesMentor);

            if (consideracoesMentor != null)
            {
                CONSIDERACOESMENTOR updConsideracoesMentor = new CONSIDERACOESMENTOR();
                updConsideracoesMentor = consideracoesMentor;
                updConsideracoesMentor.ElegivelPromocao = Convert.ToBoolean(ddlElegivelPromocao.SelectedIndex);
                updConsideracoesMentor.InputPromocao = Convert.ToBoolean(ddlInputPromocao.SelectedIndex);
                updConsideracoesMentor.TrajetoriaAssociado = textTrajetoria.Text;
                updConsideracoesMentor.PontosFortes = textPontosFortes.Text;
                updConsideracoesMentor.PontosFracos = textPontosFracos.Text;
                updConsideracoesMentor.MentoriaRealizada = cboxMentoriaRealizada.Checked;
                updConsideracoesMentor.DataMentoriaRealizada = cboxMentoriaRealizada.Checked ? DateTime.Today : (DateTime?)null;
                string resultUpdate = consideracoesMentorService.AtualizarConsideracoesMentor(consideracoesMentor, updConsideracoesMentor);
                if (resultUpdate == "okay")
                {
                    MessageBox.Show("Considerações salvas!", "Salvar", TIPO.Default, MessageBoxHandler);
                }
                else
                {
                    MessageBox.Show(resultUpdate, "Salvar", TIPO.Error, MessageBoxHandler);
                }
            }
        }

        protected void CarregarResultados(int idAssociado, int idPeriodo, ASSOCIADOS associado, string strTipoAvaliacao, string strEscopo)
        {
            var resultadoAssociado = new ResultadoServices().ObterResultadoAssociado_OBSOLETO(idAssociado, idPeriodo, associado.IdCargo, strTipoAvaliacao, strEscopo);

            this.rptProjetos.ItemDataBound += RptProjetos_ItemDataBound;
            this.rptProjetos.DataSource = resultadoAssociado;
            this.rptProjetos.DataBind();

            var somaProjetos = new ResultadoServices().SomaResultadoProjetos(resultadoAssociado);

            this.rptSomaProjetos.ItemDataBound += RptSomaProjetos_ItemDataBound;
            this.rptSomaProjetos.DataSource = somaProjetos;
            this.rptSomaProjetos.DataBind();
        }
        private void RptSomaProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ResultadoSomaProjetosModel item = (ResultadoSomaProjetosModel)e.Item.DataItem;

            Repeater repn1 = (Repeater)e.Item.FindControl("rptSomaProjetosItems");
            if (repn1 != null)
            {
                repn1.DataSource = item.ListProjetosSomaPerfomance;
                repn1.DataBind();
            }

            Repeater repsc = (Repeater)e.Item.FindControl("rptProjetosSomaCompetencias");
            if (repsc != null)
            {
                repsc.DataSource = item.ListProjetosSomaCompetenciasN1N2;
                repsc.DataBind();
            }

            Repeater repradar = (Repeater)e.Item.FindControl("rptSomaProjetosRadar");
            if (repradar != null)
            {
                repradar.DataSource = item.ListProjetosSomaCompetenciasN1N2;
                repradar.DataBind();
            }


        }

        private void RptProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ResultadoProjetosModel item = (ResultadoProjetosModel)e.Item.DataItem;

            Repeater repn1 = (Repeater)e.Item.FindControl("rptCompetenciasN1");
            if (repn1 != null)
            {
                repn1.DataSource = item.ListCompetenciasNivel1;
                repn1.DataBind();
            }

            Repeater repn2 = (Repeater)e.Item.FindControl("rptCompetenciasN2");
            if (repn2 != null)
            {
                repn2.DataSource = item.ListCompetenciasNivel2;
                repn2.DataBind();
            }

            Repeater repperf = (Repeater)e.Item.FindControl("rptNotaPerfomance");
            if (repperf != null)
            {
                repperf.DataSource = item.ListPerfomance;
                repperf.DataBind();
            }

            Repeater repSomaComp = (Repeater)e.Item.FindControl("rptSomaCompetencias");
            if (repSomaComp != null)
            {
                repSomaComp.DataSource = item.ListSomaCompetenciasN1N2;
                repSomaComp.DataBind();
            }

            Repeater repRadar = (Repeater)e.Item.FindControl("rptRadar");
            if (repRadar != null)
            {
                repRadar.DataSource = item.ListSomaCompetenciasN1N2;
                repRadar.DataBind();
            }


        }

        #region MÉTODOS DE FORMATAÇÃO
        public string FormatPercentagem(decimal nota)
        {

            if (nota > 0)
            {
                return nota.ToString("##0") + "%";
            }

            return "0%";

        }

        public string FormatDecimal(decimal nota)
        {
            return Math.Round(nota, 2).ToString();

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

        #endregion
    }
}
