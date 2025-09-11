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
using System.Linq;

namespace SistemaAvaliacao
{
    public partial class autoavalizacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboProjetos();
                CarregaComboClientes();
                CarregaComboPeriodos();
                CarregaComboStatus();
                btnSearch_Click(sender, e);

                var strProjeto = Request.QueryString["IdProjeto"];
                var strPeriodo = Request.QueryString["IdPeriodo"];
                var strFinalizou = Request.QueryString["Finalizou"];

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

        #region CARREGA COMBOS
        private void CarregaComboProjetos()
        {
            var usuario = new AssociadosService().ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var projetosList = new ProjetosService().ListaProjetosAtivos(usuario);
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

        #endregion

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
            else if(!string.IsNullOrEmpty(strProjeto))
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

            var associado = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var projetos = new ProjetosService().ListaProjetosAtivos(associado, projeto, status, periodo, cliente);

            List<ProjetoModel> listaProjetosAvaliacoes = new List<ProjetoModel>();

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

                var projetoPessoas = projetoService.ObterListaAssociadosNoPeriodo(itemProjeto.IdProjeto, associado.IdAssociado, periodo);
                              
                foreach (var itemProjetoPessoa in projetoPessoas)
                {
                    List<PERIODOSAVALIACOES> periodosAvaliacoes = new List<PERIODOSAVALIACOES>();

                    if (periodo != null)
                        periodosAvaliacoes.Add(periodo);
                    else
                        periodosAvaliacoes = periodoService.ListaTodosPeriodos(itemProjeto.IdEmpresa);
                        //periodosAvaliacoes = periodoService.ListaPeriodos(itemProjeto.IdEmpresa, itemProjetoPessoa.DataInicio, itemProjetoPessoa.DataFim.Value);//periodoService.ListaTodosPeriodos(1);


                    if (itemProjetoPessoa.TipoAvaliacao != "desempenho") { continue; }

                    foreach (var periodoAvaliacao in periodosAvaliacoes)
                    {
                        var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(itemProjetoPessoa.IdProjeto, itemProjetoPessoa.IdAssociado, periodoAvaliacao.IdPeriodo, itemProjeto.IdEmpresa,
                            itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, (int)itemProjetoPessoa.IdGestor);

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
                            projetosAssociadosModel.Gestor = associadosService.ObterAssociado((int)itemProjetoPessoa.IdGestor);// projetosAssociadosModel.Projeto.IdAssociadoGestor);
                        
                        if (projetosAssociadosModel.Associado != null)
                        {
                            if (projetosAssociadosModel.Associado.FotoNome == null || projetosAssociadosModel.Associado.FotoNome == "")
                            {
                                projetosAssociadosModel.Associado.FotoNome = "assets/images/users/usernophoto.jpg";
                            }
                            projetosAssociadosModel.Associado.FotoNome = projetosAssociadosModel.Associado.FotoNome.Replace(" ", "%20");

                            AVALIACOESCOMPETENCIAS avaliacaoCompetencia =
                                new AvaliacoesService().ObterAvaliacaoCompetencia(itemProjetoPessoa.IdAssociado, projetosAssociadosModel.Projeto.IdProjeto, periodoAvaliacao.IdPeriodo,
                                    itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, avaliacaoEmail != null ? avaliacaoEmail.idAvaliacao : -1);

                            if(avaliacaoCompetencia != null)
                                projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(avaliacaoCompetencia.IdCargo);
                            else
                                projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(projetosAssociadosModel.Associado.IdCargo);

                            projetosAssociadosModel.Avaliador = associadosService.ObterAssociado(projetosAssociadosModel.Projeto.IdAssociadoGestor);


                            var projetosAssociados = new ProjetosService().ObterGestorEAvaliadorDeAssociado(itemProjeto.IdProjeto, associado.IdAssociado, itemProjetoPessoa.IdProjetoAssociado);

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

                        // Checa se já liberou a avaliação
                        if (avaliacaoEmail != null)
                        {
                            //PENSAR EM UMA FORMA SEGURA DE SABER QUE A AVALIAÇÃO FOI CONCLUIDA

                            projetosAssociadosModel.AvaliacaoLiberada = avaliacaoEmail.Liberado;
                            projetosAssociadosModel.IdEmail = avaliacaoEmail.idAvaliacao.ToString();
                            var avaliacaoCompetencia = avaliacaoService.ObterAvaliacaoCompetencia(itemProjetoPessoa.IdAssociado, projetosAssociadosModel.Projeto.IdProjeto, periodoAvaliacao.IdPeriodo,
                                    itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, avaliacaoEmail != null ? avaliacaoEmail.idAvaliacao : -1);
                            var avaliacaoPerformance = avaliacaoService.ObterAvaliacaoPerformance(itemProjetoPessoa.IdAssociado, projetosAssociadosModel.Projeto.IdProjeto, periodoAvaliacao.IdPeriodo,
                                true);

                            // Verfica se está na etapa de Avaliação às Cegas
                            var etapasPermitidas = new[] {
                                avaliacaoService.etapaAvaliacaoAsCegas,
                                avaliacaoService.etapaAutoAvaliacao,
                                avaliacaoService.etapaEmParalelo,
                                avaliacaoService.etapaNaoIniciada
                            };

                            if (etapasPermitidas.Contains(avaliacaoEmail.PosicaoAtualFluxoAvaliacao))
                            {


                                if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaNaoIniciada ||
                                    avaliacaoCompetencia.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaNaoIniciada
                                    )
                                {
                                    projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Não iniciado");
                                    projetosAssociadosModel.RotuloBotao = "Iniciar Avaliação";

                                    projetosAssociadosModel.ExibirRotuloEtapa = "hidden";
                                    projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                }
                                else if(avaliacaoCompetencia.DataHoraFimAutoAvaliacao != null)
                                {
                                    projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Concluído");
                                    projetosAssociadosModel.RotuloBotao = "Ver Avaliação";
                                }
                                else
                                {
                                    projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Em andamento");
                                    projetosAssociadosModel.RotuloBotao = "Continuar Avaliação";
                                    projetosAssociadosModel.ExibirBotaoFinalizar = true;

                                    projetosAssociadosModel.ExibirRotuloEtapa = "hidden";
                                    projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                }

                            }

                            else
                            {
                                projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Concluído");
                                projetosAssociadosModel.RotuloBotao = "Ver Avaliação";

                                projetosAssociadosModel.ExibirRotuloEtapa = "";
                                if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoAsCegas)
                                {
                                    projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                }
                                else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoGestor)
                                {
                                    projetosAssociadosModel.Etapa = "Em Av. Gestor";
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
                            }
                        }
                        else
                        {
                            projetosAssociadosModel.AvaliacaoLiberada = false;
                            projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Não iniciado");
                            projetosAssociadosModel.RotuloBotao = "Iniciar Avaliação";
                        }

                        if (avaliacaoEmail != null)
                            umProjeto.Associados.Add(projetosAssociadosModel);
                    }
                }

                if (umProjeto.Associados.Count > 0)
                {
                    listaProjetosAvaliacoes.Add(umProjeto);
                }
            }

            rptProjetos.DataSource = listaProjetosAvaliacoes;
            rptProjetos.DataBind();
        }

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            int idAvaliacao = Convert.ToInt32((sender as System.Web.UI.WebControls.Button).CommandArgument);
            var avaliacaoService = new AvaliacoesService();
            var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(idAvaliacao);

            if (avaliacaoEmail != null)
            {
                //ALTERAR AQUI EM CADA MÓDULO
                string etapaAtual = avaliacaoService.etapaAutoAvaliacao;

                var avaliacoesCompetencia = avaliacaoService.ObterAvaliacoesCompetencias(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual,
                        avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, avaliacaoEmail.idAvaliacao);

                if (avaliacoesCompetencia.Count == 0)
                {
                    MessageBox.Show("Não existem Competências parametrizadas para este cargo. Não é possível dar andamento na avaliação !!",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                    return;
                }

                var avaliacoesPerformance = avaliacaoService.ObterAvaliacoesPerformances(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual, true);

                if (avaliacoesPerformance.Count == 0)
                {
                    MessageBox.Show("Não existem Performances parametrizadas para este cargo ou você ainda não iniciou o preenchimento das Performances. Não é possível dar andamento na avaliação !!",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                    return;
                }

                // Checa se Digitou tudo da avaliação competencia
                foreach (var avaliacao in avaliacoesCompetencia)
                {
                    //if (avaliacao.IdNotaNivel1AutoAvaliacao <= 0 || avaliacao.IdNotaNivel2AutoAvaliacao <= 0 || avaliacao.ComentariosAutoAvaliacao?.Trim() == "")
                    if (avaliacao.IdNotaNivel1AutoAvaliacao <= 0 || avaliacao.IdNotaNivel2AutoAvaliacao <= 0)
                    {
                        if (avaliacao.COMPETENCIAS.IdModo == 1)
                        {
                            MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação",
                            "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        else if (avaliacao.COMPETENCIAS.IdModo == 2)
                        {
                            avaliacao.IdNotaNivel1AutoAvaliacao = avaliacao.COMPETENCIAS.IdNotaPadraoNivel1 != null ? (int)avaliacao.COMPETENCIAS.IdNotaPadraoNivel1 : 5;
                            avaliacao.IdNotaNivel2AutoAvaliacao = avaliacao.COMPETENCIAS.IdNotaPadraoNivel2 != null ? (int)avaliacao.COMPETENCIAS.IdNotaPadraoNivel2 : 5;
                        }
                    }
                }

                // Checa se Digitou tudo da avaliação performance
                foreach (var avaliacao in avaliacoesPerformance)
                {
                    //if (avaliacao.IdNotaNivel1AutoAvaliacao <= 0 || avaliacao.ComentariosAutoAvaliacao?.Trim() == "")
                    if (avaliacao.IdNotaNivel1AutoAvaliacao <= 0)
                    {
                        MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação",
                        "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                        return;
                    }
                }



                DateTime dataHoraFinalizacao = DateTime.Now;

                // Finaliza a avaliação performance
                foreach (var avaliacao in avaliacoesPerformance)
                {
                    avaliacao.DataHoraFimAutoAvaliacao = dataHoraFinalizacao;
                    avaliacaoService.AlterarAvaliacaoPerformance(avaliacao.IdAvaliacaoPerformance, avaliacao);
                    // Avança para a Próxima Etapa
                    avaliacaoService.AvancaProximaEtapaPerformance(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }

                // Finaliza a avaliação competência
                foreach (var avaliacao in avaliacoesCompetencia)
                {
                    avaliacao.DataHoraFimAutoAvaliacao = dataHoraFinalizacao;
                    avaliacaoService.AlterarAvaliacaoCompetencia(avaliacao.IdAvaliacaoCompetencia, avaliacao);
                    // Avança para a Próxima Etapa
                    avaliacaoService.AvancaProximaEtapaCompetencia(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }

                var avaliacoesPerformanceAtual = avaliacaoService.ObterAvaliacoesPerformances(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual, true);
                var avaliacoesCompetenciaAtual = avaliacaoService.ObterAvaliacoesCompetencias(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual,
                                            avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, avaliacaoEmail.idAvaliacao);

                bool performanceAsCegasFinalizada = avaliacoesPerformanceAtual.All(p => p.DataHoraFimAvaliacaoCegas != null);
                bool competenciaAsCegasFinalizada = avaliacoesCompetenciaAtual.All(c => c.DataHoraFimAvaliacaoCegas != null);

                
                // Avança Etapa da Avaliação E-mail se Avaliação às cegas estiver finalizada
                if (performanceAsCegasFinalizada && competenciaAsCegasFinalizada)
                {
                    avaliacaoService.AvancaProximaEtapaEmail(avaliacaoEmail);
                }

                WebStorage.Set("redirectUrl", $"~/autoavalizacao.aspx?IdProjeto={avaliacaoEmail.idProjeto}&IdPeriodo={avaliacaoEmail.idPeriodo}&Finalizou=Y");
                Response.Redirect("comentarios");
            }
            else
            {
                MessageBox.Show("É obrigatório digitar todas as notas das Avaliações Competência e Performance antes de finalizar a Avaliação",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
            }
        }

        
        protected void colAvaliador_Load(object sender, EventArgs e){
            CargoColunas.ConferirCargoColuna(sender, "avaliador", "header");
        }
        protected void colMentor_Init(object sender, EventArgs e)
        {
            CargoColunas.ConferirCargoColuna(sender, "mentor", "header");
        }
        protected void rowAvaliador_Init(object sender, EventArgs e)
        {
            CargoColunas.ConferirCargoColuna(sender, "avaliador", "row");
        }
        protected void rowMentor_Init(object sender, EventArgs e)
        {
            CargoColunas.ConferirCargoColuna(sender, "mentor", "row");
        }
    }
}