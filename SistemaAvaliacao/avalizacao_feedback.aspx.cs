using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaAvaliacao
{
    public partial class avalizacao_feedback : System.Web.UI.Page
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
            var fotosAssociadosService = new FotosAssociadosService();

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
            var projetos = new ProjetosService().ListaProjetosAtivosComoGestor(gestor, projeto, status, periodo, cliente);


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

                var projetoPessoas = projetoService.ObterListaAssociadosComoGestor(itemProjeto.IdProjeto, gestor.IdAssociado,periodo);
                projetoPessoas = projetoPessoas.Where(p => p.TipoAvaliacao == "desempenho").ToList();

                foreach (var itemProjetoPessoa in projetoPessoas)
                {
                    List<PERIODOSAVALIACOES> periodosAvaliacoes = new List<PERIODOSAVALIACOES>();

                    if (periodo != null)
                        periodosAvaliacoes.Add(periodo);
                    else
                        periodosAvaliacoes = periodoService.ListaTodosPeriodos(itemProjeto.IdEmpresa);
                        //periodosAvaliacoes = periodoService.ListaPeriodos(itemProjeto.IdEmpresa, itemProjetoPessoa.DataInicio, itemProjetoPessoa.DataFim.Value);

                    if (itemProjetoPessoa.TipoAvaliacao != "desempenho") 
                    { 
                        continue; 
                    }

                    foreach (var periodoAvaliacao in periodosAvaliacoes)
                    {
                        var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(itemProjetoPessoa.IdProjeto, itemProjetoPessoa.IdAssociado, periodoAvaliacao.IdPeriodo, itemProjeto.IdEmpresa,
                            itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, (int)itemProjetoPessoa.IdGestor);
                        var avaliacao = avaliacaoService.ObterAvaliacoesCompetencias(itemProjetoPessoa.IdAssociado, itemProjeto.IdProjeto, periodoAvaliacao.IdPeriodo);
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
                            projetosAssociadosModel.FotoAssociado = fotosAssociadosService.ObterFotoPorAssociado(itemProjetoPessoa.IdAssociado);

                            if (projetosAssociadosModel.Projeto != null)
                                projetosAssociadosModel.Gestor = associadosService.ObterAssociado((int)itemProjetoPessoa.IdGestor);//projetosAssociadosModel.Projeto.IdAssociadoGestor);

                            if (projetosAssociadosModel.Associado != null)
                            {
                                if (projetosAssociadosModel.Associado.FotoNome == null || projetosAssociadosModel.Associado.FotoNome == "")
                                {
                                    projetosAssociadosModel.Associado.FotoNome = "assets/images/users/usernophoto.jpg";
                                }
                                projetosAssociadosModel.Associado.FotoNome = projetosAssociadosModel.Associado.FotoNome.Replace(" ", "%20");

                                AVALIACOESCOMPETENCIAS avaliacaoCompetencia = new AvaliacoesService().ObterAvaliacaoCompetencia(itemProjetoPessoa.IdAssociado, projetosAssociadosModel.Projeto.IdProjeto,
                                    periodoAvaliacao.IdPeriodo, itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, avaliacaoEmail != null ? avaliacaoEmail.idAvaliacao : -1);

                                if (avaliacaoCompetencia != null)
                                    projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(avaliacaoCompetencia.IdCargo);
                                else
                                    projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(projetosAssociadosModel.Associado.IdCargo);



                                projetosAssociadosModel.Avaliador = associadosService.ObterAssociado(projetosAssociadosModel.Projeto.IdAssociadoGestor);
                                var projetosAssociados = new ProjetosService().ObterGestorEAvaliadorDeAssociadoDesempenho(itemProjeto.IdProjeto, projetosAssociadosModel.Associado.IdAssociado);

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
                                projetosAssociadosModel.AvaliacaoLiberada = avaliacaoEmail.Liberado;
                                projetosAssociadosModel.IdEmail = avaliacaoEmail.idAvaliacao.ToString();
                                projetosAssociadosModel.ExibirFeedback = "hidden";

                                if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaFeedback)
                                {
                                    projetosAssociadosModel.ExibirRotuloEtapa = "hidden";
                                    projetosAssociadosModel.Etapa = "Feedback";
                                    projetosAssociadosModel.ExibirFeedback = "";

                                    if (avaliacao[0].DataHoraInicioFeedback != null)
                                    {
                                        projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Em andamento");
                                        projetosAssociadosModel.RotuloBotao = "Continuar Feedback";
                                        projetosAssociadosModel.ExibirBotaoFinalizar = true;
                                    }
                                    else
                                    {
                                        projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Não iniciado");
                                        projetosAssociadosModel.RotuloBotao = "Iniciar Feedback";
                                    }
                                }
                                else
                                {
                                    projetosAssociadosModel.ExibirRotuloEtapa = "";
                                    projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Concluído");
                                    projetosAssociadosModel.RotuloBotao = "Ver Feedback";

                                    if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAutoAvaliacao || avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaNaoIniciada)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoGestor)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Av. Gestor";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaEmParalelo)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoAsCegas)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoMentor)
                                    {
                                        projetosAssociadosModel.Etapa = "Em Cons. Mentor";
                                        projetosAssociadosModel.ExibirFeedback = "";
                                    }
                                    else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoFinalizada)
                                    {
                                        projetosAssociadosModel.Etapa = "Finalizada";
                                        projetosAssociadosModel.ExibirFeedback = "";
                                    }
                                }
                            }
                            else
                            {
                                projetosAssociadosModel.AvaliacaoLiberada = false;
                                projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Não iniciado");
                                projetosAssociadosModel.RotuloBotao = "Iniciar Feedback";
                            }

                            if (avaliacaoEmail != null)
                                umProjeto.Associados.Add(projetosAssociadosModel);
                        }
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

                // Checa se Digitou tudo da avaliação performance
                foreach (var avaliacao in avaliacoesPerformance)
                {
                    if (avaliacao.IdNotaNivel1Feedback <= 0)
                    {
                        MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação",
                        "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                        return;
                    }
                }

                // Checa se Digitou tudo da avaliação competencia
                foreach (var avaliacao in avaliacoesCompetencia)
                {
                    if (avaliacao.IdNotaNivel1Feedback <= 0 || avaliacao.IdNotaNivel2Feedback <= 0)
                    {
                        if (avaliacao.COMPETENCIAS.IdModo == 1)
                        {
                            MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação",
                            "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        else if (avaliacao.COMPETENCIAS.IdModo == 2)
                        {
                            avaliacao.IdNotaNivel1Feedback = avaliacao.COMPETENCIAS.IdNotaPadraoNivel1 ?? 5;
                            avaliacao.IdNotaNivel2Feedback = avaliacao.COMPETENCIAS.IdNotaPadraoNivel2 ?? 5;
                        }
                    }
                }

                DateTime dataHoraFinalizacao = DateTime.Now;

                // Finaliza a avaliação performance
                foreach (var avaliacao in avaliacoesPerformance)
                {
                    avaliacao.DataHoraFimFeedback = dataHoraFinalizacao;
                    avaliacaoService.AlterarAvaliacaoPerformance(avaliacao.IdAvaliacaoPerformance, avaliacao);
                    // Avança para a Próxima Etapa
                    avaliacaoService.AvancaProximaEtapaPerformance(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }

                // Finaliza a avaliação competência
                foreach (var avaliacao in avaliacoesCompetencia)
                {
                    avaliacao.DataHoraFimFeedback = dataHoraFinalizacao;
                    avaliacaoService.AlterarAvaliacaoCompetencia(avaliacao.IdAvaliacaoCompetencia, avaliacao);
                    // Avança para a Próxima Etapa
                    avaliacaoService.AvancaProximaEtapaCompetencia(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }

                // Avança Etapa da Avaliação E-mail
                avaliacaoService.AvancaProximaEtapaEmail(avaliacaoEmail);
                Response.Redirect($"~/avalizacao_feedback.aspx?IdProjeto={avaliacaoEmail.idProjeto}&IdPeriodo={avaliacaoEmail.idPeriodo}&Finalizou=Y");
            }
            else
            {
                MessageBox.Show("É obrigatório digitar todas as notas das Avaliações Competência e Performance antes de finalizar a Avaliação",
                    "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
            }
        }
    }
}