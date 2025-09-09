using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.Scripts;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class avalizacao_gestor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboProjetos();
                CarregaComboClientes();
                CarregaComboPeriodos();
                CarregaComboStatus();
                CarregaComboEtapa();
                btnSearch_Click(sender, e);

                var strProjeto = Request.QueryString["IdProjeto"];
                var strPeriodo = Request.QueryString["IdPeriodo"];
                var strFinalizou = Request.QueryString["Finalizou"];
                var strTipoAvaliacao = Request.QueryString["TipoAvaliacao"];
                var strEscopo = Request.QueryString["Escopo"];

                var user = WebStorage.GetUsuarioLogado();
                if (user.IdPerfil <= 1) { text_Titulo.Text = "AVALIAÇÕES DE LIDERANÇA"; text_SubTitulo.Text = "Avaliação / Liderança"; }

                if (strProjeto != null && strPeriodo != null
                    && strProjeto != "" && strPeriodo != ""
                    && strProjeto != "0" && strPeriodo != "0")
                {
                    ddlProjetos.Value = strProjeto;
                    ddlPeriodos.Value = strPeriodo;
                    btnSearch_Click(sender, e);

                    if (strFinalizou == "Y")
                        MessageBox.Show("Avaliação Finalizada com Sucesso", "Próxima Etapa Liberada", TIPO.Info, MessageBoxHandler);
                }
            }
        }

        private void CarregaComboProjetos()
        {
            var usuario = new AssociadosService().ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var projetosList = new ProjetosService().ListaProjetosAtivosComoGestor(usuario);
            ddlProjetos.DataValueField = "IdProjeto";
            ddlProjetos.DataTextField = "Projeto";
            ddlProjetos.DataSource = projetosList;
            ddlProjetos.DataBind();
            ddlProjetos.Items.Insert(0, "[Selecionar]");
        }

        private void CarregaComboClientes()
        {
            var clienteList = new ClientesService().ListaClientesAtivos();
            ddlClientes.DataValueField = "IdCliente";
            ddlClientes.DataTextField = "Cliente";
            ddlClientes.DataSource = clienteList;
            ddlClientes.DataBind();
            ddlClientes.Items.Insert(0, "[Selecionar]");
        }

        private void CarregaComboPeriodos()
        {
            var PeriodoList = new PeriodoService().ListaPeriodos(WebStorage.GetUsuarioLogado().IdEmpresa);
            ddlPeriodos.DataValueField = "IdPeriodo";
            ddlPeriodos.DataTextField = "Periodo";
            ddlPeriodos.DataSource = PeriodoList;
            ddlPeriodos.DataBind();
            ddlPeriodos.Items.Insert(0, "[Selecionar]");
            ddlPeriodos.SelectedIndex = PeriodoList.Count;
        }

        private void CarregaComboStatus()
        {
            var statusList = new StatusService().ListaStatusAvaliacoes();
            ddlStatus.DataValueField = "IdStatus";
            ddlStatus.DataTextField = "Status";
            ddlStatus.DataSource = statusList;
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, "[Selecionar]");
        }
        private void CarregaComboEtapa()
        {
            ddlEtapa.Items.Clear();
            ddlEtapa.Items.Insert(0, "[Selecionar]");
            ddlEtapa.Items.Insert(1, "Preenchimento Todas");
            ddlEtapa.Items.Insert(2, "Preenchimento Gestor");
            ddlEtapa.Items.Insert(3, "Preenchimento Liderado");
            ddlEtapa.Items.Insert(4, "Finalizadas Todas");
            ddlEtapa.Items.Insert(5, "Finalizadas Gestor");
            ddlEtapa.Items.Insert(6, "Finalizadas Liderado");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            PROJETOS projeto = null;
            PERIODOSAVALIACOES periodo = null;
            PROJETOSSTATUS status = null;
            CLIENTES cliente = null;

            var projetoService = new ProjetosService();
            var periodoService = new PeriodoService();
            var statusService = new StatusService();
            var associadosService = new AssociadosService();
            var cargosService = new CargosService();
            var clienteService = new ClientesService();
            var avaliacaoService = new AvaliacoesService();
            var cargosNiveisServices = new CargosNiveisService();
            var fotoAssociadoService = new FotosAssociadosService();

            var strProjeto = Request.QueryString["IdProjeto"];
            var strPeriodo = Request.QueryString["IdPeriodo"];

            if (ddlProjetos.SelectedIndex > 0)
            {
                projeto = projetoService.ObterProjeto(Convert.ToInt32(ddlProjetos.Items[ddlProjetos.SelectedIndex].Value));
            }
            else if (!string.IsNullOrEmpty(strProjeto))
            {
                projeto = projetoService.ObterProjeto(Convert.ToInt32(strProjeto));
            }

            if (ddlClientes.SelectedIndex > 0)
                cliente = clienteService.ObterCliente(Convert.ToInt32(ddlClientes.Items[ddlClientes.SelectedIndex].Value));

            if (ddlPeriodos.SelectedIndex > 0)
            {
                periodo = periodoService.ObterPeriodo(Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value));
            }
            else if (!string.IsNullOrEmpty(strPeriodo))
            {
                periodo = periodoService.ObterPeriodo(Convert.ToInt32(strPeriodo));
            }

            if (ddlStatus.SelectedIndex > 0)
                status = statusService.ObterStatusProjeto(Convert.ToInt32(ddlStatus.Items[ddlStatus.SelectedIndex].Value));

            var gestor = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);

            #region AVALIAÇÕES DE DESEMPENHO (GESTOR) E LIDERANÇA (LIDERADO)
            var projetos = new ProjetosService().ListaProjetosAtivosComoGestor(gestor, projeto, status, periodo, cliente);

            List<ProjetoModel> listaProjetosAvaliacoes = new List<ProjetoModel>();

            var alocacoesTodas = projetoService.ObterListaAssociadosComoGestor(-1, gestor.IdAssociado, periodo);
            var listaProjetos = alocacoesTodas.Select(x => x.IdProjeto).Distinct().ToList();
            var listaAssociados = alocacoesTodas.Select(x => x.IdAssociado).Distinct().ToList();
            var avaliacoesTodas = avaliacaoService.ObterAvaliacoesListas(listaProjetos, listaAssociados);
            var respostasTodas = avaliacaoService.ObterAvaliacoesCompetenciasListas(listaProjetos, listaAssociados);

            foreach (var itemProjeto in projetos)
            {
                var umProjeto = new ProjetoModel();

                umProjeto.DataInicio = itemProjeto.DataInicio.ToString("dd/MM/yyyy");
                umProjeto.DataTermino = itemProjeto.DataFim?.ToString("dd/MM/yyyy");
                umProjeto.Gestor = associadosService.ObterAssociado(itemProjeto.IdAssociadoGestor);
                umProjeto.Id = itemProjeto.IdProjeto;
                umProjeto.Nome = itemProjeto.Projeto;
                umProjeto.Responsavel = associadosService.ObterAssociado(itemProjeto.IdAssociadoResponsavel);
                umProjeto.Status = itemProjeto.PROJETOSSTATUS;
                umProjeto.Cliente = clienteService.ObterCliente(itemProjeto.IdCliente);


                var projetoPessoas = alocacoesTodas.Where(p => p.IdProjeto == itemProjeto.IdProjeto);

                foreach (var itemProjetoPessoa in projetoPessoas)
                {
                    List<PERIODOSAVALIACOES> periodosAvaliacoes = new List<PERIODOSAVALIACOES>();

                    if (periodo != null)
                        periodosAvaliacoes.Add(periodo);
                    else
                        periodosAvaliacoes = periodoService.ListaTodosPeriodos(itemProjeto.IdEmpresa);

                    foreach (var periodoAvaliacao in periodosAvaliacoes)
                    {
                        var avaliacaoEmail = avaliacoesTodas.FirstOrDefault(x => x.idProjeto == itemProjetoPessoa.IdProjeto && x.idAssociado == itemProjetoPessoa.IdAssociado &&
                            x.idPeriodo == periodoAvaliacao.IdPeriodo && x.idEmpresa == itemProjeto.IdEmpresa && x.TipoAvaliacao == itemProjetoPessoa.TipoAvaliacao &&
                            x.Escopo == itemProjetoPessoa.Escopo && x.idGestor == itemProjetoPessoa.IdGestor);

                        if (itemProjetoPessoa.IdProjeto == 232)
                        {

                        }

                        var avaliacao = respostasTodas.Where(x => 
                            x.IdAssociado == itemProjetoPessoa.IdAssociado && 
                            x.IdProjeto == itemProjeto.IdProjeto &&
                            x.IdPeriodo == periodoAvaliacao.IdPeriodo && 
                            x.TipoAvaliacao == itemProjetoPessoa.TipoAvaliacao && 
                            x.Escopo == itemProjetoPessoa.Escopo &&
                            (avaliacaoEmail != null ? x.idAvaliacao == avaliacaoEmail.idAvaliacao : true)).ToList();

                        var avaliacao_finalizadas = avaliacao.Where(x => x.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoFinalizada).ToList();
                        var avaliacao_mentores = avaliacao.Where(x => x.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoMentor).ToList();
                        var avaliacao_feedback = avaliacao.Where(x => x.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaFeedback).ToList();

                        avaliacao_finalizadas.AddRange(avaliacao_mentores);
                        avaliacao_finalizadas.AddRange(avaliacao_feedback);

                        switch (ddlEtapa.SelectedIndex){
                            case 0: avaliacao.AddRange(avaliacao_finalizadas); break;
                            case 1: break;
                            case 2: avaliacao.Find(a => a.TipoAvaliacao == "desempenho"); break;
                            case 3: avaliacao.Find(a => a.TipoAvaliacao == "lideranca"); break;
                            case 4: avaliacao = avaliacao_finalizadas; break;
                            case 5: avaliacao = avaliacao_finalizadas; avaliacao.Find(a => a.TipoAvaliacao == "desempenho"); break;
                            case 6: avaliacao = avaliacao_finalizadas; avaliacao.Find(a => a.TipoAvaliacao == "lideranca"); break;}

                        var associado = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);

                        // Se avaliação > 0, indica que possui avaliação na Etapa Atual
                        if (avaliacao.Count > 0)
                        {
                            ProjetosAssociadosModel projetosAssociadosModel = new ProjetosAssociadosModel();

                            projetosAssociadosModel.Id = itemProjetoPessoa.IdProjetoAssociado;
                            projetosAssociadosModel.Projeto = projetoService.ObterProjeto(itemProjetoPessoa.IdProjeto);
                            projetosAssociadosModel.Associado = associadosService.ObterAssociado(itemProjetoPessoa.IdAssociado);
                            projetosAssociadosModel.Associado.ASSOCIADOS3 = associadosService.ObterAssociado(itemProjetoPessoa.ASSOCIADOS.IdAssociadoMentor);
                            projetosAssociadosModel.DataInicio = itemProjetoPessoa.DataInicio.ToString("dd/MM/yyyy");
                            projetosAssociadosModel.Periodo = periodoAvaliacao;
                            projetosAssociadosModel.TipoAvaliacao = itemProjetoPessoa.TipoAvaliacao;
                            projetosAssociadosModel.Escopo = itemProjetoPessoa.Escopo;
                            projetosAssociadosModel.FotoAssociado = fotoAssociadoService.ObterFotoPorAssociado(itemProjetoPessoa.IdAssociado);

                            if (projetosAssociadosModel.Projeto != null)
                                projetosAssociadosModel.Gestor = associadosService.ObterAssociado((int)itemProjetoPessoa.IdGestor);//projetosAssociadosModel.Projeto.IdAssociadoGestor);

                            if (projetosAssociadosModel.Associado != null)
                            {
                                if (projetosAssociadosModel.FotoAssociado == null)
                                {
                                    projetosAssociadosModel.FotoAssociado = new FOTOSASSOCIADOS()
                                    {
                                        IdAssociado = projetosAssociadosModel.Associado.IdAssociado,
                                        Imagem = "assets/images/users/usernophoto.jpg",
                                        NomeFoto = "usernophoto.jpg",
                                        AssociadoFoto = projetosAssociadosModel.Associado.Nome,
                                    };

                                }

                                AVALIACOESCOMPETENCIAS avaliacaoCompetencia = respostasTodas.FirstOrDefault(x =>
                                    x.IdAssociado == itemProjetoPessoa.IdAssociado &&
                                    x.IdProjeto == projetosAssociadosModel.Projeto.IdProjeto &&
                                    x.IdPeriodo == periodoAvaliacao.IdPeriodo &&
                                    x.TipoAvaliacao == itemProjetoPessoa.TipoAvaliacao &&
                                    x.Escopo == itemProjetoPessoa.Escopo &&
                                    (avaliacaoEmail != null ? x.idAvaliacao == avaliacaoEmail.idAvaliacao : true));
                                    
                                    //new AvaliacoesService().ObterAvaliacaoCompetencia(itemProjetoPessoa.IdAssociado, projetosAssociadosModel.Projeto.IdProjeto,
                                    //periodoAvaliacao.IdPeriodo, itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, avaliacaoEmail != null ? avaliacaoEmail.idAvaliacao : -1);

                                if (avaliacaoCompetencia != null)
                                    projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(avaliacaoCompetencia.IdCargo);
                                else
                                    projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(projetosAssociadosModel.Associado.IdCargo);


                                projetosAssociadosModel.Avaliador = associadosService.ObterAssociado(projetosAssociadosModel.Projeto.IdAssociadoGestor);
                                var projetosAssociados = new ProjetosService().ObterGestorEAvaliadorDeAssociado(itemProjeto.IdProjeto, projetosAssociadosModel.Associado.IdAssociado,
                                    itemProjetoPessoa.IdProjetoAssociado);

                                int idGestor = 0;
                                int idAvaliador = 0;

                                if (projetosAssociados.IdGestor != null)
                                {
                                    idGestor = Convert.ToInt32(projetosAssociados.IdGestor);
                                    projetosAssociadosModel.Gestor.Nome = associadosService.ObterAssociado(idGestor).Nome;
                                }


                                if (projetosAssociados.IdAvaliador != null)
                                {
                                    idAvaliador = Convert.ToInt32(projetosAssociados.IdAvaliador);
                                    projetosAssociadosModel.Avaliador.Nome = associadosService.ObterAssociado(idAvaliador).Nome;
                                }
                                else
                                {
                                    projetosAssociadosModel.Avaliador.Nome = projetosAssociadosModel.Gestor.Nome;
                                }

                            }

                            if (itemProjetoPessoa.DataFim == null)
                                projetosAssociadosModel.DataTermino = "";
                            else
                                projetosAssociadosModel.DataTermino = itemProjetoPessoa.DataFim?.ToString("dd/MM/yyyy");

                            projetosAssociadosModel.ExibirBotaoFinalizar = false;
                            projetosAssociadosModel.ExibirBotaoLiberarLider = false;

                            // Checa se já liberou a avaliação - RESPONDER AVALIAÇÃO DE DE DESEMPENHO(GESTOR) E LIDERANÇA(LIDERADO)
                            if (avaliacaoEmail != null)
                            {
                                projetosAssociadosModel.AvaliacaoLiberada = avaliacaoEmail.Liberado;
                                projetosAssociadosModel.IdEmail = avaliacaoEmail.idAvaliacao.ToString();

                                if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoGestor)
                                {
                                    projetosAssociadosModel.ExibirRotuloEtapa = "hidden";
                                    projetosAssociadosModel.Etapa = "Em Av. Gestor";

                                    if (avaliacao[0].DataHoraInicioAvaliacaoGestor != null)
                                    {
                                        projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Em andamento");
                                        projetosAssociadosModel.RotuloBotao = "Continuar Avaliação";
                                        projetosAssociadosModel.ExibirBotaoFinalizar = true;
                                    }
                                    else
                                    {
                                        projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Não iniciado");
                                        projetosAssociadosModel.RotuloBotao = "Iniciar Avaliação";
                                    }
                                }
                                else
                                {
                                    projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Concluído");
                                    projetosAssociadosModel.RotuloBotao = "Ver Avaliação";

                                    projetosAssociadosModel.ExibirRotuloEtapa = "";

                                    if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAutoAvaliacao || avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaNaoIniciada)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                        projetosAssociadosModel.ExibirBotaoVer = "hidden";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoAsCegas)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                        projetosAssociadosModel.ExibirBotaoVer = "hidden";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaEmParalelo)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                        projetosAssociadosModel.ExibirBotaoVer = "hidden";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaFeedback)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Feedback";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoMentor)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Cons. Mentor";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoFinalizada)
                                    {
                                        projetosAssociadosModel.Etapa = "Finalizada";
                                    }

                                    if (projetosAssociadosModel.TipoAvaliacao == "lideranca" && avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaFeedback)
                                    {
                                        // DESATIVADO - AV. DE LIDERANÇA SERÁ MANTIDA EM ANONIMATO
                                        //projetosAssociadosModel.ExibirBotaoLiberarLider = true;
                                    }
                                }
                            }
                            else
                            {
                                projetosAssociadosModel.AvaliacaoLiberada = false;
                                projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Não iniciado");
                                projetosAssociadosModel.RotuloBotao = "Iniciar Avaliação";
                            }

                            if (avaliacaoEmail != null)
                            {
                                if (projetosAssociadosModel.TipoAvaliacao == "desempenho")
                                    umProjeto.Associados.Add(projetosAssociadosModel);
                                else if (projetosAssociadosModel.TipoAvaliacao == "lideranca" && avaliacaoEmail.idGestor == gestor.IdAssociado)
                                    umProjeto.Lideres.Add(projetosAssociadosModel);
                            }
                        }
                    }
                }

                if (umProjeto.Associados.Count > 0 || umProjeto.Lideres.Count > 0)
                {
                    listaProjetosAvaliacoes.Add(umProjeto);
                }
            }

            rptProjetos.DataSource = listaProjetosAvaliacoes;
            rptProjetos.DataBind();
            #endregion

            #region AVALIAÇÕES DE LIDERANÇA RESPONDIDAS POR LIDERADOS (DESATIVADO DEVIDO A ANONIMIDADE)
            if (false == true)
            {
                var avaliacoes_AvaliacoesLiderados = new AvaliacoesService().ObterAvaliacoesAssociado(gestor.IdAssociado, avaliacaoService.etapaAvaliacaoFinalizada, "lideranca");
                List<ProjetoModel> listaAvaliacoesLiderados = new List<ProjetoModel>();

                var projetos_AvaliacoesLideranca = new List<PROJETOS>();
                var idsProjetos_AvaliacoesLideranca = new List<int>();
                foreach (var itemAvaliacao in avaliacoes_AvaliacoesLiderados)
                {
                    if (!idsProjetos_AvaliacoesLideranca.Contains(new ProjetosService().ObterProjeto(itemAvaliacao.idProjeto).IdProjeto))
                    {
                        projetos_AvaliacoesLideranca.Add(new ProjetosService().ObterProjeto(itemAvaliacao.idProjeto));
                        idsProjetos_AvaliacoesLideranca.Add(new ProjetosService().ObterProjeto(itemAvaliacao.idProjeto).IdProjeto);
                    }
                }
                foreach (var itemProjeto_ in projetos_AvaliacoesLideranca)
                {
                    var umProjeto = new ProjetoModel();

                    umProjeto.DataInicio = itemProjeto_.DataInicio.ToString("dd/MM/yyyy");
                    umProjeto.DataTermino = itemProjeto_.DataFim?.ToString("dd/MM/yyyy");
                    umProjeto.Gestor = associadosService.ObterAssociado(itemProjeto_.IdAssociadoGestor);
                    umProjeto.Id = itemProjeto_.IdProjeto;
                    umProjeto.Nome = itemProjeto_.Projeto;
                    umProjeto.Responsavel = associadosService.ObterAssociado(itemProjeto_.IdAssociadoResponsavel);
                    umProjeto.Status = new StatusService().ObterStatusProjeto(itemProjeto_.IdStatus);
                    umProjeto.Cliente = clienteService.ObterCliente(itemProjeto_.IdCliente);

                    var projetoPessoas = projetoService.ObterListaAssociadosNoPeriodo(itemProjeto_.IdProjeto, gestor.IdAssociado, periodo);

                    foreach (var itemProjetoPessoa in projetoPessoas)
                    {
                        List<PERIODOSAVALIACOES> periodosAvaliacoes = new List<PERIODOSAVALIACOES>();

                        if (periodo != null)
                            periodosAvaliacoes.Add(periodo);
                        else
                            periodosAvaliacoes = periodoService.ListaPeriodos(itemProjeto_.IdEmpresa, itemProjetoPessoa.DataInicio, itemProjetoPessoa.DataFim.Value);

                        foreach (var periodoAvaliacao in periodosAvaliacoes)
                        {
                            var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(itemProjetoPessoa.IdProjeto, itemProjetoPessoa.IdAssociado, periodoAvaliacao.IdPeriodo, itemProjeto_.IdEmpresa,
                                        itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, (int)itemProjetoPessoa.IdGestor);

                            var avaliacao = avaliacaoService.ObterAvaliacoesCompetencias(itemProjetoPessoa.IdAssociado, itemProjeto_.IdProjeto, periodoAvaliacao.IdPeriodo, avaliacaoService.etapaAvaliacaoFinalizada,
                                itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, avaliacaoEmail != null ? avaliacaoEmail.idAvaliacao : -1);

                            var associado = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);

                            // Se avaliação > 0, indica que possui avaliação na Etapa Atual
                            if (avaliacao.Count > 0)
                            {
                                ProjetosAssociadosModel projetosAssociadosModel = new ProjetosAssociadosModel();

                                projetosAssociadosModel.Id = itemProjetoPessoa.IdProjetoAssociado;
                                projetosAssociadosModel.Projeto = projetoService.ObterProjeto(itemProjetoPessoa.IdProjeto);
                                projetosAssociadosModel.Associado = associadosService.ObterAssociado(itemProjetoPessoa.IdAssociado);
                                projetosAssociadosModel.Associado.ASSOCIADOS3 = associadosService.ObterAssociado(itemProjetoPessoa.ASSOCIADOS.IdAssociadoMentor);
                                projetosAssociadosModel.DataInicio = itemProjetoPessoa.DataInicio.ToString("dd/MM/yyyy");
                                projetosAssociadosModel.Periodo = periodoAvaliacao;
                                projetosAssociadosModel.TipoAvaliacao = itemProjetoPessoa.TipoAvaliacao;
                                projetosAssociadosModel.Escopo = itemProjetoPessoa.Escopo;

                                if (projetosAssociadosModel.Projeto != null)
                                    projetosAssociadosModel.Gestor = associadosService.ObterAssociado((int)itemProjetoPessoa.IdGestor);//projetosAssociadosModel.Projeto.IdAssociadoGestor);

                                if (projetosAssociadosModel.Associado != null)
                                {
                                    if (projetosAssociadosModel.Gestor.FotoNome == null || projetosAssociadosModel.Gestor.FotoNome == "")
                                    {
                                        projetosAssociadosModel.Associado.FotoNome = "assets/images/users/usernophoto.jpg";
                                    }
                                    else
                                    {
                                        projetosAssociadosModel.Associado.FotoNome = projetosAssociadosModel.Gestor.FotoNome.Replace(" ", "%20");
                                    }

                                    AVALIACOESCOMPETENCIAS avaliacaoCompetencia = new AvaliacoesService().ObterAvaliacaoCompetencia(itemProjetoPessoa.IdAssociado, projetosAssociadosModel.Projeto.IdProjeto,
                                        periodoAvaliacao.IdPeriodo, itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, avaliacaoEmail.idAvaliacao);

                                    if (avaliacaoCompetencia != null)
                                        projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(avaliacaoCompetencia.IdCargo);
                                    else
                                        projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(projetosAssociadosModel.Associado.IdCargo);


                                    projetosAssociadosModel.Avaliador = associadosService.ObterAssociado(projetosAssociadosModel.Projeto.IdAssociadoGestor);
                                    var projetosAssociados = new ProjetosService().ObterGestorEAvaliadorDeAssociado(itemProjeto_.IdProjeto, projetosAssociadosModel.Associado.IdAssociado,
                                        itemProjetoPessoa.IdProjetoAssociado);

                                    int idGestor = 0;
                                    int idAvaliador = 0;

                                    if (projetosAssociados.IdGestor != null)
                                    {
                                        idGestor = Convert.ToInt32(projetosAssociados.IdGestor);
                                        projetosAssociadosModel.Gestor.Nome = associadosService.ObterAssociado(idGestor).Nome;
                                    }

                                    if (projetosAssociados.IdAvaliador != null)
                                    {
                                        idAvaliador = Convert.ToInt32(projetosAssociados.IdAvaliador);
                                        projetosAssociadosModel.Avaliador.Nome = associadosService.ObterAssociado(idAvaliador).Nome;
                                    }
                                    else
                                    {
                                        projetosAssociadosModel.Avaliador.Nome = projetosAssociadosModel.Gestor.Nome;
                                    }
                                }

                                if (itemProjetoPessoa.DataFim == null)
                                    projetosAssociadosModel.DataTermino = "";
                                else
                                    projetosAssociadosModel.DataTermino = itemProjetoPessoa.DataFim?.ToString("dd/MM/yyyy");

                                projetosAssociadosModel.ExibirBotaoFinalizar = false;
                                // Checa se já liberou a avaliação - RESPONDER AVALIAÇÃO DE LIDERANÇA
                                if (avaliacaoEmail != null)
                                {
                                    projetosAssociadosModel.AvaliacaoLiberada = avaliacaoEmail.Liberado;
                                    projetosAssociadosModel.IdEmail = avaliacaoEmail.idAvaliacao.ToString();
                                    projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Concluído");
                                    projetosAssociadosModel.RotuloBotao = "Ver Avaliação";
                                }
                                else
                                {
                                    projetosAssociadosModel.AvaliacaoLiberada = false;
                                    projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Não iniciado");
                                }

                                umProjeto.Associados.Add(projetosAssociadosModel);
                            }
                        }
                    }
                    //listaAvaliacoesLiderados.Add(umProjeto); // ANONIMO
                }
                rptAvaliacaoesLiderados.DataSource = listaAvaliacoesLiderados;
                rptAvaliacaoesLiderados.DataBind();

                if (listaAvaliacoesLiderados.Count <= 0)
                {
                    divcardAvaliacoesLiderados.Visible = false;
                }
            }
            divcardAvaliacoesLiderados.Visible = false;

            #endregion
        }

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            int idAvaliacao = Convert.ToInt32((sender as System.Web.UI.WebControls.Button).CommandArgument);
            var avaliacaoService = new AvaliacoesService();
            var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idAvaliacao);

            if (avaliacaoEmail != null)
            {
                //ALTERAR AQUI EM CADA MÓDULO
                string etapaAtual = avaliacaoService.etapaAvaliacaoGestor;

                var avaliacoesPerformance = avaliacaoService.ObterAvaliacoesPerformances(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual, true);
                if (avaliacaoEmail.TipoAvaliacao == "lideranca")
                {
                    avaliacoesPerformance = avaliacaoService.ObterAvaliacoesPerformancesLideranca(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo);
                }

                if (avaliacoesPerformance.Count == 0)
                {
                    MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                    return;
                }

                var avaliacoesCompetencia = avaliacaoService.ObterAvaliacoesCompetencias(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual, 
                    avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, avaliacaoEmail.idAvaliacao);

                if (avaliacoesCompetencia.Count == 0)
                {
                    MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                    return;
                }

                // Checa se Digitou tudo da avaliação performance
                foreach (var avaliacao in avaliacoesPerformance)
                {
                    if (avaliacao.IdNotaNivel1AvaliacaoGestor <= 0)
                    {
                        MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação",
                        "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                        return;
                    }
                }

                // Armazena quantidade de notas "Não se Aplica" para não considerar na média final
                var notaService = new NotasAvaliacaoService();
                Dictionary<int, int> qtdeNaoSeAplica = new Dictionary<int, int>();
                int idNotaNaoSeAplica = notaService.ObterNotaCompetencia("Não se Aplica").IdNota;

                // Checa se Digitou tudo da avaliação competencia
                foreach (var avaliacao in avaliacoesCompetencia)
                {
                    if (avaliacao.IdNotaNivel1AvaliacaoGestor <= 0 || avaliacao.IdNotaNivel2AvaliacaoGestor <= 0)
                    {
                        if (avaliacao.COMPETENCIAS.IdModo == 1)
                        {
                            MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação",
                            "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        else if (avaliacao.COMPETENCIAS.IdModo == 2)
                        {
                            avaliacao.IdNotaNivel1AvaliacaoGestor = avaliacao.COMPETENCIAS.IdNotaPadraoNivel1 ?? 5;
                            avaliacao.IdNotaNivel2AvaliacaoGestor = avaliacao.COMPETENCIAS.IdNotaPadraoNivel2 ?? 5;
                        }
                    }

                    COMPETENCIAS competencia = new CompetenciasService().ObterCompetencia(avaliacao.IdCompetencia);
                    SUBCOMPETENCIAS subCompetencia = new SubCompetenciasService().ObterCompetencia(competencia.IdSubCompetencia);
                    if (!qtdeNaoSeAplica.ContainsKey(subCompetencia.IdSubCompetencia))
                    {
                        qtdeNaoSeAplica.Add(subCompetencia.IdSubCompetencia, 0);
                    }

                    // Não considera essa avaliação no cálculo da média
                    if ((avaliacao.TipoAvaliacao == "desempenho" &&
                            (avaliacao.IdNotaNivel1AutoAvaliacao == idNotaNaoSeAplica ||
                            avaliacao.IdNotaNivel2AutoAvaliacao == idNotaNaoSeAplica ||
                            avaliacao.IdNotaNivel1AvaliacaoGestor == idNotaNaoSeAplica ||
                            avaliacao.IdNotaNivel2AvaliacaoGestor == idNotaNaoSeAplica))
                        || (avaliacao.TipoAvaliacao == "lideranca" &&
                            avaliacao.IdNotaNivel1AvaliacaoGestor == idNotaNaoSeAplica))
                    {
                        avaliacao.IdNotaNivel1AutoAvaliacao = avaliacao.IdNotaNivel2AutoAvaliacao = idNotaNaoSeAplica;
                        avaliacao.IdNotaNivel1AvaliacaoGestor = avaliacao.IdNotaNivel2AvaliacaoGestor = idNotaNaoSeAplica;

                        qtdeNaoSeAplica[subCompetencia.IdSubCompetencia] += 1;
                    }
                }

                DateTime dataHoraFinalizacao = DateTime.Now;
                
                // Finaliza a avaliação performance
                foreach (var avaliacao in avaliacoesPerformance)
                {
                    avaliacao.DataHoraFimAvaliacaoGestor = dataHoraFinalizacao;
                    avaliacaoService.AlterarAvaliacaoPerformance(avaliacao.IdAvaliacaoPerformance, avaliacao);
                    // Avança para a Próxima Etapa
                    avaliacaoService.AvancaProximaEtapaPerformance(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }

                // Finaliza a avaliação competência
                var competenciaService = new CompetenciasService();
                foreach (var avaliacao in avaliacoesCompetencia)
                {
                    var nota1Avaliado = notaService.ObterNotaCompetencia(avaliacao.IdNotaNivel1AutoAvaliacao).Peso;
                    var nota2Avaliado = notaService.ObterNotaCompetencia(avaliacao.IdNotaNivel2AutoAvaliacao).Peso;
                    var nota1Gestor = notaService.ObterNotaCompetencia(avaliacao.IdNotaNivel1AvaliacaoGestor.Value).Peso;
                    var nota2Gestor = notaService.ObterNotaCompetencia(avaliacao.IdNotaNivel2AvaliacaoGestor.Value).Peso;

                    COMPETENCIAS competencia = new CompetenciasService().ObterCompetencia(avaliacao.IdCompetencia);
                    SUBCOMPETENCIAS subCompetencia = new SubCompetenciasService().ObterCompetencia(competencia.IdSubCompetencia);

                    var qtdeForDivide = competenciaService.ObterQtdeSubCompetencias(avaliacao.IdCompetencia);
                    qtdeForDivide -= qtdeNaoSeAplica[subCompetencia.IdSubCompetencia];

                    avaliacao.NotaSubCompetenciaAvaliado = nota1Avaliado + nota2Avaliado;
                    avaliacao.NotaSubCompetenciaGestor = nota1Gestor + nota2Gestor;

                    if (qtdeForDivide != 0)
                    {
                        avaliacao.NotaCompetenciaAvaliado = avaliacao.NotaSubCompetenciaAvaliado / qtdeForDivide;
                        avaliacao.NotaCompetenciaGestor = avaliacao.NotaSubCompetenciaGestor / qtdeForDivide;
                    }
                    else
                    {
                        avaliacao.NotaCompetenciaAvaliado = avaliacao.NotaSubCompetenciaAvaliado;
                        avaliacao.NotaCompetenciaGestor = avaliacao.NotaSubCompetenciaGestor;
                    }

                    avaliacao.DataHoraFimAvaliacaoGestor = dataHoraFinalizacao;
                    avaliacaoService.AlterarAvaliacaoCompetencia(avaliacao.IdAvaliacaoCompetencia, avaliacao);
                    // Avança para a Próxima Etapa
                    avaliacaoService.AvancaProximaEtapaCompetencia(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }

                // Avança Etapa da Avaliação E-mail
                var enviarEmail = avaliacaoEmail.TipoAvaliacao == "desempenho"; // JUL2022 - ANONIMIZAÇÃO DA AV. LIDERANÇA, NÃO ENVIAR EMAIL AO GESTOR QUANDO O LIDERADO FINALIZA A AVALIAÇÃO
                avaliacaoService.AvancaProximaEtapaEmail(avaliacaoEmail, enviarEmail:enviarEmail);
                Response.Redirect($"~/avalizacao_gestor.aspx?IdProjeto={avaliacaoEmail.idProjeto}&IdPeriodo={avaliacaoEmail.idPeriodo}&Finalizou=Y");
            }
            else
            {
                MessageBox.Show("É obrigatório digitar todas as notas das Avaliações Competência e Performance antes de finalizar a Avaliação",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
            }
        }

        protected void btnLiberarLider_Click(object sender, EventArgs e)
        {
            int idAvaliacao = Convert.ToInt32((sender as System.Web.UI.WebControls.Button).CommandArgument);
            var avaliacaoService = new AvaliacoesService();
            var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idAvaliacao);

            if (avaliacaoEmail != null)
            {
                //ALTERAR AQUI EM CADA MÓDULO
                string etapaAtual = avaliacaoService.etapaFeedback;

                var avaliacoesPerformance = avaliacaoService.ObterAvaliacoesPerformances(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual, true);

                if (avaliacoesPerformance.Count == 0)
                {
                    MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                    return;
                }

                var avaliacoesCompetencia = avaliacaoService.ObterAvaliacoesCompetencias(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual,
                    avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, avaliacaoEmail.idAvaliacao);

                if (avaliacoesCompetencia.Count == 0)
                {
                    MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                    return;
                }

                // Avança a avaliação performance
                foreach (var avaliacao in avaliacoesPerformance)
                {
                    // Avança para a Próxima Etapa
                    avaliacaoService.AvancaProximaEtapaPerformance(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }

                // Avança a avaliação competência
                var competenciaService = new CompetenciasService();
                foreach (var avaliacao in avaliacoesCompetencia)
                {
                    // Avança para a Próxima Etapa
                    avaliacaoService.AvancaProximaEtapaCompetencia(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }

                // Avança Etapa da Avaliação E-mail
                avaliacaoService.AvancaProximaEtapaEmail(avaliacaoEmail);
                Response.Redirect($"~/avalizacao_gestor.aspx?IdProjeto={avaliacaoEmail.idProjeto}&IdPeriodo={avaliacaoEmail.idPeriodo}&Finalizou=Y");
            }
            else
            {
                MessageBox.Show("É obrigatório digitar todas as notas das Avaliações Competência e Performance antes de finalizar a Avaliação",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
            }
        }

        protected void colAvaliador_Init(object sender, EventArgs e)
        {
            //CargoColunas.ConferirCargoColuna(sender, "avaliador", "header");
        }
        protected void colMentor_Init(object sender, EventArgs e)
        {
            //CargoColunas.ConferirCargoColuna(sender, "mentor", "header");
        }
        protected void rowAvaliador_Init(object sender, EventArgs e)
        {

        }
        protected void rowMentor_Init(object sender, EventArgs e)
        {

        }
    }
}