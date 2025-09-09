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
    public partial class avaliacao : System.Web.UI.Page
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

                var strProjeto = WebStorage.Get("IdProjeto", "0");
                var strPeriodo = WebStorage.Get("IdPeriodo", "0");
                var strFinalizou = WebStorage.Get("Finalizou", "0");

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
            var statusList = new PeriodoService().ListaPeriodos(WebStorage.GetUsuarioLogado().IdEmpresa);
            ddlPeriodos.DataValueField = "IdPeriodo";
            ddlPeriodos.DataTextField = "Periodo";
            ddlPeriodos.DataSource = statusList;
            ddlPeriodos.DataBind();
            ddlPeriodos.Items.Insert(0, "[Selecionar]");
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
            PROJETOS filtroProjeto = null;
            PERIODOSAVALIACOES filtroPeriodo = null;
            PROJETOSSTATUS filtroStatus = null;
            CLIENTES filtroCliente = null;

            var projetoService = new ProjetosService();
            var periodoService = new PeriodoService();
            var statusService = new StatusService();
            var associadosService = new AssociadosService();
            var cargosService = new CargosService();
            var clienteService = new ClientesService();
            var avaliacaoService = new AvaliacoesService();
            var cargosNiveisServices = new CargosNiveisService();
            var fotosAssociadoService = new FotosAssociadosService();

            var strProjeto = WebStorage.Get("IdProjeto", "0");
            var strPeriodo = WebStorage.Get("IdPeriodo", "0");

            #region FILTROS
            if (ddlProjetos.SelectedIndex > 0)
            {
                filtroProjeto = projetoService.ObterProjeto(Convert.ToInt32(ddlProjetos.Items[ddlProjetos.SelectedIndex].Value));
            }
            else if(!string.IsNullOrEmpty(strProjeto) && strProjeto != "0")
            {
                filtroProjeto = projetoService.ObterProjeto(Convert.ToInt32(strProjeto));
            }

            if (ddlClientes.SelectedIndex > 0)
                filtroCliente = clienteService.ObterCliente(Convert.ToInt32(ddlClientes.Items[ddlClientes.SelectedIndex].Value));

            if (ddlPeriodos.SelectedIndex > 0)
            {
                filtroPeriodo = periodoService.ObterPeriodo(Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value));
            }
            else if (!string.IsNullOrEmpty(strPeriodo) && strPeriodo != "0")
            {
                filtroPeriodo = periodoService.ObterPeriodo(Convert.ToInt32(strPeriodo));
            }
            else
            {
                filtroPeriodo = periodoService.ObterPeriodoUltimo();
            }

            if (ddlStatus.SelectedIndex > 0)
                filtroStatus = statusService.ObterStatusProjeto(Convert.ToInt32(ddlStatus.Items[ddlStatus.SelectedIndex].Value));
            #endregion

            #region LEITOR DE AVALIAÇÕES
            List<ProjetoModel> listaProjetosAvaliacoes = new List<ProjetoModel>();
            var associadoLogado = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var listAssociacoes = avaliacaoService.ObterAssociacoesRelacionadas(associadoLogado.IdAssociado);
            var listIdProjetos = listAssociacoes.Select(av => av.IdProjeto).ToList();
            var fotoAssociado = fotosAssociadoService.ObterFotoPorAssociado(associadoLogado.IdAssociado);
            if (filtroProjeto != null) { listIdProjetos = listIdProjetos.Where(id => id == filtroProjeto.IdProjeto).ToList(); }
            var stackIdProjetos = new List<int>();

            foreach (var IdProjeto in listIdProjetos)
            {
                if (!stackIdProjetos.Contains(IdProjeto))
                {
                    stackIdProjetos.Add(IdProjeto);

                    var getProjeto = projetoService.ObterProjeto(IdProjeto);
                    if (filtroCliente != null && filtroCliente.IdCliente != getProjeto.IdCliente ) { continue; }

                    var linhaProjeto = new ProjetoModel();
                    linhaProjeto.Id = getProjeto.IdProjeto;
                    linhaProjeto.DataInicio = getProjeto.DataInicio.ToString("dd/MM/yyyy");
                    linhaProjeto.DataTermino = getProjeto.DataFim?.ToString("dd/MM/yyyy");
                    linhaProjeto.Gestor = associadosService.ObterAssociado(getProjeto.IdAssociadoGestor);
                    linhaProjeto.Responsavel = associadosService.ObterAssociado(getProjeto.IdAssociadoResponsavel);
                    linhaProjeto.Status = statusService.ObterStatusProjeto(getProjeto.IdStatus);
                    linhaProjeto.Cliente = clienteService.ObterCliente(getProjeto.IdCliente);

                    var listAssociacoesProjeto = listAssociacoes.Where(av => av.IdProjeto == IdProjeto).ToList();
                    foreach (var associacao in listAssociacoesProjeto)
                    {
                        var getPeriodo = periodoService.VerificaExistenciaPeriodo(associacao.DataInicio, (DateTime)associacao.DataFim, 1);
                        if (getPeriodo == null) { continue; }
                        var getAvaliacao = avaliacaoService.ObterAvaliacaoEmail(associacao.IdProjeto, associacao.IdAssociado,
                            getPeriodo.IdPeriodo, 1, associacao.TipoAvaliacao, associacao.Escopo, (int)associacao.IdGestor);
                        if (getAvaliacao == null) { continue; }

                        if (filtroStatus != null && filtroStatus.IdStatus != getAvaliacao.idStatus) { continue; }
                        if (filtroPeriodo != null && filtroPeriodo.IdPeriodo != getAvaliacao.idPeriodo) { continue; }

                        var addAvaliacao = new ProjetosAssociadosModel();
                        addAvaliacao.IdEmail = getAvaliacao.idAvaliacao.ToString();
                        addAvaliacao.Projeto = projetoService.ObterProjeto(getAvaliacao.idProjeto);
                        addAvaliacao.Associado = associadosService.ObterAssociado(getAvaliacao.idAssociado);
                        addAvaliacao.Gestor = associadosService.ObterAssociado((int)getAvaliacao.idGestor);
                        addAvaliacao.Avaliador = associadosService.ObterAssociado((int)associacao.IdAvaliador);
                        addAvaliacao.Mentor = associadosService.ObterAssociado(addAvaliacao.Associado.IdAssociadoMentor);
                        addAvaliacao.FotoAssociado = fotosAssociadoService.ObterFotoPorAssociado(addAvaliacao.Associado.IdAssociado);
                        addAvaliacao.Periodo = getAvaliacao.PERIODOSAVALIACOES;
                        addAvaliacao.Status = getAvaliacao.AVALIACOESSTATUS;
                        addAvaliacao.DataInicio = associacao.DataInicio.ToString("dd/MM/yyyy");
                        addAvaliacao.DataTermino = associacao.DataFim?.ToString("dd/MM/yyyy");
                        addAvaliacao.TipoAvaliacao = getAvaliacao.TipoAvaliacao;
                        addAvaliacao.TipoAvaliacaoShow = getAvaliacao.TipoAvaliacao == "desempenho" ? 
                            "Av. Desempenho" : "Av. Liderança";
                        addAvaliacao.Escopo = getAvaliacao.Escopo;
                        addAvaliacao.AvaliacaoLiberada = getAvaliacao.Liberado;
                        addAvaliacao.RotuloBotao = "Responder";
                        addAvaliacao.ExibirBotaoResponder = "hidden";

                        switch (getAvaliacao.PosicaoAtualFluxoAvaliacao)
                        {
                            case "AVM":
                                {
                                    addAvaliacao.Etapa = "Não iniciada";
                                    if (getAvaliacao.idAssociado == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                }
                                break;

                            case "AAV":
                                {
                                    addAvaliacao.Etapa = "Autoavaliação";
                                    if (getAvaliacao.idAssociado == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                }
                                break;

                            case "ACE":
                                {
                                    addAvaliacao.Etapa = "Av. as Cegas";
                                    if (addAvaliacao.Avaliador.IdAssociado == associadoLogado.IdAssociado ||
                                        getAvaliacao.idGestor == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                    else if (getAvaliacao.idAssociado == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.RotuloBotao = "Ver respostas";
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                }
                                break;

                            case "AGE":
                                {
                                    if (getAvaliacao.TipoAvaliacao == "desempenho")
                                    {
                                        addAvaliacao.Etapa = "Av. Gestor";
                                    }
                                    else
                                    {
                                        addAvaliacao.Etapa = "Av. Liderança";
                                        if (getAvaliacao.idAssociado == associadoLogado.IdAssociado){ continue; }
                                        if (getAvaliacao.idGestor != associadoLogado.IdAssociado) { continue; }
                                    }

                                    if (addAvaliacao.Avaliador.IdAssociado == associadoLogado.IdAssociado ||
                                        getAvaliacao.idGestor == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                    else if (getAvaliacao.idAssociado == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.RotuloBotao = "Ver respostas";
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                }
                                break;

                            case "FED":
                                {
                                    addAvaliacao.Etapa = "Feedback";
                                    if (addAvaliacao.Avaliador.IdAssociado == associadoLogado.IdAssociado ||
                                        getAvaliacao.idGestor == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                    else if (getAvaliacao.idAssociado == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.RotuloBotao = "Ver respostas";
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                }
                                break;

                            case "AME":
                                {
                                    addAvaliacao.Etapa = "Mentoria";
                                    if (addAvaliacao.Associado.IdAssociadoMentor == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.ExibirBotaoResponder = "";
                                        addAvaliacao.RotuloBotao = "Mentoria";
                                    }
                                    else if (getAvaliacao.idAssociado == associadoLogado.IdAssociado || 
                                        addAvaliacao.Avaliador.IdAssociado == associadoLogado.IdAssociado ||
                                        getAvaliacao.idGestor == associadoLogado.IdAssociado)
                                    {
                                        addAvaliacao.RotuloBotao = "Ver respostas";
                                        addAvaliacao.ExibirBotaoResponder = "";
                                    }
                                }
                                break;

                            case "AFI":
                                {
                                    addAvaliacao.Etapa = "Finalizada";

                                    if (getAvaliacao.TipoAvaliacao == "lideranca")
                                    {
                                        if (getAvaliacao.idAssociado == associadoLogado.IdAssociado) { continue; }
                                        if (getAvaliacao.idGestor != associadoLogado.IdAssociado) { continue; }
                                    }

                                    addAvaliacao.RotuloBotao = "Ver respostas";
                                    addAvaliacao.ExibirBotaoResponder = "";
                                }
                                break;
                        }

                        if (getAvaliacao.TipoAvaliacao == "desempenho")
                        {
                            linhaProjeto.Associados.Add(addAvaliacao);
                        }
                        else
                        {
                            linhaProjeto.Lideres.Add(addAvaliacao);
                        }
                    }

                    if (linhaProjeto.Associados != null && linhaProjeto.Associados.Count > 0)
                    {
                        listaProjetosAvaliacoes.Add(linhaProjeto);
                    }
                }
            }
            #endregion

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
                        MessageBox.Show("É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação",
                        "Finalização Não Permitida", TIPO.Warning, MessageBoxHandler);
                        return;
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

                // Avança Etapa da Avaliação E-mail
                avaliacaoService.AvancaProximaEtapaEmail(avaliacaoEmail);
                Response.Redirect($"~/autoavalizacao.aspx?IdProjeto={avaliacaoEmail.idProjeto}&IdPeriodo={avaliacaoEmail.idPeriodo}&Finalizou=Y");
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