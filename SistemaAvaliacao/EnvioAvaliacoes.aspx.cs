using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Tria.Framework.Domain.Service;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;

namespace SistemaAvaliacao
{
    public partial class EnvioAvaliacoes : System.Web.UI.Page
    {
        Thread thread = null;

        const int idProjeto = 0;
        const int idAssociado = 1;
        const int idPeriodo = 2;
        const int TipoAvaliacao = 3;
        const int Escopo = 4;
        const int idGestor = 5;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboPeriodos();
                CarregaComboProjetos();
                CarregaComboClientes();
                CarregaComboStatus();
                CarregaComboAssociados();
                CarregaComboDisparos();
                carregaPendencias();
                WebStorage.Delete(WebStorage.EnumTipoLista.Emails);
            }
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

        private void CarregaComboProjetos()
        {
            var usuario = new AssociadosService().ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var projetosList = new ProjetosService().ListaProjetosAtivos();
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

        private void CarregaComboStatus()
        {
            var statusList = new StatusService().ListaStatusAvaliacoes();
            ddlStatus.DataValueField = "IdStatus";
            ddlStatus.DataTextField = "Status";
            ddlStatus.DataSource = statusList;
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, "[Selecionar]");
        }

        private void CarregaComboAssociados()
        {
            var profissionalList = new AssociadosService().ObterAssociados(true);
            ddlAssociados.DataValueField = "IdAssociado";
            ddlAssociados.DataTextField = "Nome";
            ddlAssociados.DataSource = profissionalList;
            ddlAssociados.DataBind();
            ddlAssociados.Items.Insert(0, "[Selecionar]");
        }

        private void CarregaComboDisparos()
        {
            var prazosAtivos = new WorkflowService().ObterTodosPrazosAtivos();
            ddlDisparos.DataValueField = "IdPrazo";
            ddlDisparos.DataTextField = "NomeDisparo";
            ddlDisparos.DataSource = prazosAtivos;
            ddlDisparos.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            WebStorage.Delete(WebStorage.EnumTipoLista.Emails);
            PERIODOSAVALIACOES periodo = null;
            PROJETOS projeto = null;
            PROJETOSSTATUS status = null;
            CLIENTES cliente = null;
            ASSOCIADOS associado = null;

            var projetoService = new ProjetosService();
            var periodoService = new PeriodoService();
            var associadosService = new AssociadosService();
            var cargosService = new CargosService();
            var clienteService = new ClientesService();
            var statusService = new StatusService();
            var cargosNiveisServices = new CargosNiveisService();

            if (ddlPeriodos.SelectedIndex > 0)
                periodo = periodoService.ObterPeriodo(Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value));
            else
            {
                MessageBox.Show("Selecione o Período", "Período Obrigatório", TIPO.Warning, MessageBoxHandler);
                return;
            }

            if (ddlProjetos.SelectedIndex > 0)
                projeto = projetoService.ObterProjeto(Convert.ToInt32(ddlProjetos.Items[ddlProjetos.SelectedIndex].Value));

            if (ddlClientes.SelectedIndex > 0)
                cliente = clienteService.ObterCliente(Convert.ToInt32(ddlClientes.Items[ddlClientes.SelectedIndex].Value));

            if (ddlStatus.SelectedIndex > 0)
                status = statusService.ObterStatusProjeto(Convert.ToInt32(ddlStatus.Items[ddlStatus.SelectedIndex].Value));

            if (ddlAssociados.SelectedIndex > 0)
                associado = associadosService.ObterAssociado(Convert.ToInt32(ddlAssociados.Items[ddlAssociados.SelectedIndex].Value));

            var projetos = new ProjetosService().ListaProjetosAtivos(associado, projeto, status, periodo, cliente);            
            List<ProjetoModel> listaProjetosAvaliacoes = new List<ProjetoModel>();
            List<string> listaIdsEmailsToSend = new List<string>();

            // DISPAROS
            var prazosService = new WorkflowService();
            var prazosAtivos = prazosService.ObterTodosPrazosAtivos();
            List<DisparosModel> listaDisparos = new List<DisparosModel>();
            foreach (var itemDisparo in prazosAtivos)
            {
                DisparosModel addDisparo = new DisparosModel();
                addDisparo.NomeDisparo = itemDisparo.NomeDisparo;
                addDisparo.IdPrazo = itemDisparo.IdPrazo;
                listaDisparos.Add(addDisparo);
            }

            var periodosTodos = new PeriodoService().ListaTodosPeriodos(1).Where(x => x.ATV == 1);
            periodosTodos = periodosTodos.OrderByDescending(x => x.IdPeriodo).Reverse();
            var periodoProximo = periodosTodos.FirstOrDefault(x => x.DataInicio > periodo.DataInicio);

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

                List<PROJETOSASSOCIADOS> projetoPessoas = null;

                if (associado != null)
                    projetoPessoas = projetoService.ObterListaAssociados(itemProjeto.IdProjeto, associado.IdAssociado);
                else
                    projetoPessoas = projetoService.ObterListaAssociados(itemProjeto.IdProjeto);

                var associadoLogado = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);

                foreach (var itemProjetoPessoa in projetoPessoas)
                {
                    ProjetosAssociadosModel projetosAssociadosModel = new ProjetosAssociadosModel();

                    // SKIP ALOCAÇÃO CASO SINALIZADA PELO GESTOR, DT INICIO ANTES DO PERIODO FILTRADO OU DT INICIO APÓS INICIO DO PRÓXIMO PERÍODO
                    if (itemProjetoPessoa.IdPeriodoSinalizado != -1 && itemProjetoPessoa.IdPeriodoSinalizado != null && itemProjetoPessoa.IdPeriodoSinalizado != periodo.IdPeriodo) { continue; }
                    if (itemProjetoPessoa.DataInicio < periodo.DataInicio) { continue; }
                    if (periodoProximo != null && itemProjetoPessoa.DataInicio >= periodoProximo.DataInicio) { continue; }

                    projetosAssociadosModel.Id = itemProjetoPessoa.IdProjetoAssociado;
                    projetosAssociadosModel.IdEmail = itemProjeto.IdProjeto + ";" + itemProjetoPessoa.IdAssociado + ";" + periodo.IdPeriodo +
                        ";" + itemProjetoPessoa.TipoAvaliacao + ";" + itemProjetoPessoa.Escopo + ";" + itemProjetoPessoa.IdGestor;

                    // Adiciona na lista para geração dos emails em massa
                    listaIdsEmailsToSend.Add(projetosAssociadosModel.IdEmail);

                    projetosAssociadosModel.Projeto = itemProjeto;
                    projetosAssociadosModel.Associado = itemProjetoPessoa.ASSOCIADOS;
                    projetosAssociadosModel.DataInicio = itemProjetoPessoa.DataInicio.ToString("dd/MM/yyyy");
                    projetosAssociadosModel.Periodo = periodo;
                    projetosAssociadosModel.TipoAvaliacao = itemProjetoPessoa.TipoAvaliacao;
                    projetosAssociadosModel.Escopo = itemProjetoPessoa.Escopo;

                    if (projetosAssociadosModel.Projeto != null)
                        projetosAssociadosModel.Gestor = associadosService.ObterAssociado(projetosAssociadosModel.Projeto.IdAssociadoGestor);

                    if (projetosAssociadosModel.Associado != null)
                    {
                        var avaliacaoEmail = new AvaliacoesService().ObterAvaliacaoEmail(itemProjetoPessoa.IdProjeto, itemProjetoPessoa.IdAssociado, periodo.IdPeriodo, itemProjeto.IdEmpresa,
                            itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, (int)itemProjetoPessoa.IdGestor);

                        int check_idAval = 0;
                        if (avaliacaoEmail != null){ check_idAval = avaliacaoEmail.idAvaliacao; }
                        AVALIACOESCOMPETENCIAS avaliacaoCompetencia = new AvaliacoesService().ObterAvaliacaoCompetencia(itemProjetoPessoa.IdAssociado,
                            projetosAssociadosModel.Projeto.IdProjeto, periodo.IdPeriodo, itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, check_idAval);

                        if (avaliacaoCompetencia != null)
                            projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(avaliacaoCompetencia.IdCargo);
                        else
                            projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(projetosAssociadosModel.Associado.IdCargo);

                        if (itemProjetoPessoa.TipoAvaliacao == "desempenho")
                            projetosAssociadosModel.Avaliador = associadosService.ObterAssociado(projetosAssociadosModel.Associado.IdAssociadoMentor);
                        else
                            projetosAssociadosModel.Avaliador = associadosService.ObterAssociado(projetosAssociadosModel.Associado.IdAssociado);

                        var projetosAssociados = new ProjetosService().ObterGestorEAvaliadorDeAssociado(itemProjeto.IdProjeto, itemProjetoPessoa.IdAssociado, 
                            itemProjetoPessoa.IdProjetoAssociado);

                        int idGestor = 0;
                        int idAvaliador = 0;

                        if (projetosAssociados.IdGestor != null)
                        {
                            idGestor = Convert.ToInt32(projetosAssociados.IdGestor);
                            projetosAssociadosModel.Gestor.Nome = associadosService.ObterAssociado(idGestor).Nome;
                        }


                        if (itemProjetoPessoa.IdAvaliador != null)
                        {
                            idAvaliador = Convert.ToInt32(itemProjetoPessoa.IdAvaliador);
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


                    // DISPAROS
                    projetosAssociadosModel.Disparos = listaDisparos;

                    umProjeto.Associados.Add(projetosAssociadosModel);
                    
                }

                listaProjetosAvaliacoes.Add(umProjeto);

            }

            WebStorage.SetList(WebStorage.EnumTipoLista.Emails, listaIdsEmailsToSend);

            if (listaProjetosAvaliacoes.Count > 0)
            {
                btnGerarTodos.Visible = true;
                rptAvaliacoes.DataSource = listaProjetosAvaliacoes;
                rptAvaliacoes.DataBind();
            }
            else
            {
                rptAvaliacoes.DataSource = null;
                rptAvaliacoes.DataBind();
                MessageBox.Show("Sua consulta não encontrou Avaliações disponíveis !!", "Consulta de avaliações", TIPO.Warning, MessageBoxHandler);

            }
        }

        protected void btnGerarTodos_Click(object sender, EventArgs e)
        {
            var listaIds = WebStorage.GetList<string>(WebStorage.EnumTipoLista.Emails);

            if (listaIds.Count > 0)
            {
                int totalEnviadas = 0;
                int totalFalhas = 0;
                string falhas = "";
                int totalNaoEnviadas = 0;
                string naoEnviadas = "";

                foreach (var item in listaIds)
                {
                    var listaIDs = item.Split(';').ToList();
                    var resposta = GerarEnviar(listaIDs, Convert.ToInt32(ddlDisparos.SelectedValue));

                    switch (resposta)
                    {
                        case "OK":
                            totalEnviadas++;
                            break;
                        case "Fail":
                            totalFalhas++;
                            falhas += $"<br/>Projeto: {listaIDs[idProjeto]} / Associado: { new AssociadosService().ObterAssociado(Convert.ToInt32(listaIDs[idAssociado])).Nome}";
                            break;
                        case "NotSend":
                            totalNaoEnviadas++;
                            naoEnviadas += $"<br/>Projeto: {listaIDs[idProjeto]} / Associado: { new AssociadosService().ObterAssociado(Convert.ToInt32(listaIDs[idAssociado])).Nome}";
                            break;
                        case "SendNotEmail":
                            totalNaoEnviadas++;
                            naoEnviadas += $"<br/>Erro ao enviar E-mail / Associado: { new AssociadosService().ObterAssociado(Convert.ToInt32(listaIDs[idAssociado])).Nome}";
                            break;
                        case "NoWorkflow":
                            MessageBox.Show("Não existe Workflow cadastrado para o Período selecionado.", "Geração da Avaliação Cancelada", TIPO.Warning, MessageBoxHandler);
                            return;
                    }
                }

                var msg = $"Resumo do Processo de Geração/Envio de Avaliações:<br/><b>{totalEnviadas}</b> Avaliações Enviadas";
                if (totalFalhas > 0)
                {
                    msg += $"<br/><br/><font color=red>{totalFalhas}</font> Avaliações não foram Geradas:{falhas}";
                }
                if (totalNaoEnviadas > 0)
                {
                    msg += $"<br/><br/><font color=blue>{totalNaoEnviadas}</font> Avaliações não foram Enviadas:{naoEnviadas}";
                }

                MessageBox.Show(msg, "Gerar/Enviar Avaliações", TIPO.Default, MessageBoxHandler);
            }
            else
                MessageBox.Show("Não existem Avaliações a serem Geradas/Enviadas.", "Avaliação Não Enviada", TIPO.Warning, MessageBoxHandler);
        }

        protected void btnGerarUnico_Click(object sender, EventArgs e)
        {
            string idCompleto = (sender as System.Web.UI.WebControls.LinkButton).CommandArgument;

            var listaIDs = idCompleto.Split(';').ToList();
            var resposta = GerarEnviar(listaIDs, 4);

            switch (resposta)
            {
                case "OK":
                    MessageBox.Show("Avaliação Gerada e Enviada com Sucesso.", "Enviar Avaliação", TIPO.Info, MessageBoxHandler);
                    break;
                case "Fail":
                    MessageBox.Show("Ocorreu uma Falha ao Gerar a Avaliação.", "Avaliação Não Enviada", TIPO.Error, MessageBoxHandler);
                    break;
                case "NotSend":
                    MessageBox.Show("A Avaliação já havia sido Gerada e Enviada anteriormente.", "Avaliação Não Enviada", TIPO.Warning, MessageBoxHandler);
                    break;
                case "SendNotEmail":
                    MessageBox.Show("A Avaliação foi Gerada. Ocorreu erro no envio do E-mail", "Avaliação Gerada e Não Enviada", TIPO.Warning, MessageBoxHandler);
                    break;
                case "NoWorkflow":
                    MessageBox.Show("Não existe Workflow cadastrado para o Período selecionado.", "Geração da Avaliação Cancelada", TIPO.Error, MessageBoxHandler);
                    break;
            }
        }

        private string GerarEnviar(List<string> listaIDs, int IdPrazo)
        {
            var prazoService = new WorkflowService();
            var prazo = prazoService.ObterPrazo(IdPrazo);

            int aval_IdPeriodo, aval_IdAssociado, aval_IdProjeto, aval_idGestor;
            aval_IdPeriodo = int.Parse(listaIDs[idPeriodo]);
            aval_IdAssociado = int.Parse(listaIDs[idAssociado]);
            aval_IdProjeto = int.Parse(listaIDs[idProjeto]);
            aval_idGestor = int.Parse(listaIDs[idGestor]);

            string aval_TipoAvaliacao, aval_Escopo;
            aval_TipoAvaliacao = listaIDs[TipoAvaliacao];
            aval_Escopo = listaIDs[Escopo];

            var workflow = new WorkflowService().ObterByPeriodo(aval_IdPeriodo);
            if (workflow == null)
                return "NoWorkflow";

            int idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            var avaliacaoService = new AvaliacoesService();
            var avaliacao = avaliacaoService.ObterAvaliacaoEmail(aval_IdProjeto, aval_IdAssociado, aval_IdPeriodo, idEmpresa, aval_TipoAvaliacao, aval_Escopo, aval_idGestor);

            var context = new DataModel();

            if (avaliacao == null)
            {
                avaliacao = new AVALIACAO();
                avaliacao.idEmpresa = idEmpresa;
                avaliacao.idProjeto = aval_IdProjeto;
                avaliacao.idAssociado = aval_IdAssociado;
                avaliacao.idPeriodo = aval_IdPeriodo;
                avaliacao.Liberado = false;
                avaliacao.idStatus = 1; // Não iniciado
                avaliacao.DHC = DateTime.Now;
                avaliacao.USR = WebStorage.GetUsuarioLogado().Id;
                avaliacao.TipoAvaliacao = aval_TipoAvaliacao;
                avaliacao.Escopo = aval_Escopo;
                avaliacao.idGestor = aval_idGestor;
                avaliacao.IdPrazo = IdPrazo;

                var associado = new AssociadosService().ObterAssociado(aval_IdAssociado);
                var cargoAvaliacao = aval_TipoAvaliacao == "lideranca" ? context.CARGOS.FirstOrDefault(c => c.Cargo == "Líder") : associado.CARGOS;
                // ENTENDER PARA ENVIAR EMAIL CORRETAMENTE
                if (aval_TipoAvaliacao == "desempenho")
                {
                    avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaNaoIniciada;
                    if (avaliacaoService.SalvarAvaliacaoEmail(avaliacao))
                        avaliacao = avaliacaoService.ObterAvaliacaoEmail(aval_IdProjeto, aval_IdAssociado, aval_IdPeriodo, idEmpresa, aval_TipoAvaliacao, aval_Escopo, aval_idGestor);
                    else
                        return "Fail";
                }
                else if (aval_TipoAvaliacao == "lideranca")
                {
                    avaliacao.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaAvaliacaoGestor;
                    if (avaliacaoService.SalvarAvaliacaoEmail(avaliacao))
                        avaliacao = avaliacaoService.ObterAvaliacaoEmail(aval_IdProjeto, aval_IdAssociado, aval_IdPeriodo, idEmpresa, aval_TipoAvaliacao, aval_Escopo, aval_idGestor);
                    else
                        return "Fail";

                    #region PERFORMANCE DEFAULT DE LIDERANÇA
                    var performanceDummyLider = context.PERFORMANCES.FirstOrDefault(p => p.Performance == "Performance Lideres");
                    var avaliacaoPerformance = new AVALIACOESPERFORMANCES();
                    avaliacaoPerformance.IdEmpresa = associado.IdEmpresa;
                    avaliacaoPerformance.IdAssociado = aval_IdAssociado;
                    avaliacaoPerformance.IdCargo = cargoAvaliacao.IdCargo;
                    avaliacaoPerformance.IdNivel = associado.IdNivel;
                    avaliacaoPerformance.IdProjeto = aval_IdProjeto;
                    avaliacaoPerformance.IdPeriodo = aval_IdPeriodo;
                    avaliacaoPerformance.IdPerformance = performanceDummyLider.IdPerformance;
                    avaliacaoPerformance.IdAvaliacaoStatus = 3; // CONCLUÍDO
                    avaliacaoPerformance.DataHoraInicio = DateTime.Now;
                    avaliacaoPerformance.DataHoraTermino = DateTime.Now;
                    avaliacaoPerformance.PosicaoAtualFluxoAvaliacao = avaliacaoService.etapaAvaliacaoGestor;
                    avaliacaoPerformance.IdNotaNivel1AutoAvaliacao = 3; // ID NOTA PERFORMANCE PARA 'ESPERADO'
                    avaliacaoPerformance.ComentariosAutoAvaliacao = "N/A";
                    avaliacaoPerformance.DHCAutoAvaliacao = DateTime.Now;
                    avaliacaoPerformance.USRAutoAvaliacao = WebStorage.GetUsuarioLogado().Id;
                    avaliacaoPerformance.IdNotaNivel1AvaliacaoCegas = 3;
                    avaliacaoPerformance.ComentariosAvaliacaoCegas = "N/A";
                    avaliacaoPerformance.DHCAvaliacaoCegas = DateTime.Now;
                    avaliacaoPerformance.USRAvaliacaoCegas = WebStorage.GetUsuarioLogado().Id;
                    avaliacaoPerformance.IdNotaNivel1AvaliacaoGestor = 3;
                    avaliacaoPerformance.ComentariosAvaliacaoGestor = "N/A";
                    avaliacaoPerformance.DHCAvaliacaoGestor = DateTime.Now;
                    avaliacaoPerformance.USRAvaliacaoGestor = WebStorage.GetUsuarioLogado().Id;
                    avaliacaoPerformance.USR = WebStorage.GetUsuarioLogado().Id;
                    avaliacaoPerformance.DHC = DateTime.Now;
                    avaliacaoPerformance.ATV = 1;
                    avaliacaoService.SalvarAvaliacaoPerformance(avaliacaoPerformance);
                    #endregion
                }

                var competencias = new List<COMPETENCIAS>();
                competencias = new CompetenciasService().ObterListaCompetencias(idEmpresa, associado.IdCargo, associado.IdNivel, aval_TipoAvaliacao, aval_Escopo, null);
                foreach (var item in competencias)
                {
                    var avaliacaoCompetencia = new AVALIACOESCOMPETENCIAS();
                    avaliacaoCompetencia.IdEmpresa = associado.IdEmpresa;
                    avaliacaoCompetencia.IdAssociado = aval_IdAssociado;
                    avaliacaoCompetencia.USRAutoAvaliacao = aval_IdAssociado;
                    avaliacaoCompetencia.IdCargo = cargoAvaliacao.IdCargo;
                    avaliacaoCompetencia.IdNivel = associado.IdNivel;
                    avaliacaoCompetencia.IdProjeto = aval_IdProjeto;
                    avaliacaoCompetencia.IdPeriodo = aval_IdPeriodo;
                    avaliacaoCompetencia.IdCompetencia = item.IdCompetencia;
                    avaliacaoCompetencia.IdAvaliacaoStatus = aval_TipoAvaliacao == "desempenho" ? 1 : 2; // "A INICIAR" para desempenho, "EM ANDAMENTO" para liderança
                    avaliacaoCompetencia.PosicaoAtualFluxoAvaliacao = aval_TipoAvaliacao == "desempenho" ? avaliacaoService.etapaNaoIniciada : avaliacaoService.etapaAvaliacaoGestor;
                    avaliacaoCompetencia.DataHoraInicio = DateTime.Now;
                    avaliacaoCompetencia.USR = WebStorage.GetUsuarioLogado().Id;
                    avaliacaoCompetencia.DHC = DateTime.Now;
                    avaliacaoCompetencia.ATV = 1;
                    avaliacaoCompetencia.IdNotaNivel1AutoAvaliacao = aval_TipoAvaliacao == "desempenho" ? 0 : 5; //AUTO AV. N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.IdNotaNivel2AutoAvaliacao = aval_TipoAvaliacao == "desempenho" ? 0 : 5; //AUTO AV. N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.ComentariosAutoAvaliacao = aval_TipoAvaliacao == "desempenho" ? "" : "N/A"; //AUTO AV. N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.DHCAutoAvaliacao = DateTime.Now;
                    avaliacaoCompetencia.IdNotaNivel1AvaliacaoCegas = aval_TipoAvaliacao == "desempenho" ? (int?)null : 5; //AV. CEGAS N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.IdNotaNivel2AvaliacaoCegas = aval_TipoAvaliacao == "desempenho" ? (int?)null : 5; //AV. CEGAS N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.ComentariosAvaliacaoCegas = aval_TipoAvaliacao == "desempenho" ? "" : "N/A"; //AV. CEGAS N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.DHCAvaliacaoCegas = aval_TipoAvaliacao == "desempenho" ? (DateTime?)null : DateTime.Now; //AV. CEGAS N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.USRAvaliacaoCegas = aval_TipoAvaliacao == "desempenho" ? (int?)null : WebStorage.GetUsuarioLogado().Id; //AV. CEGAS N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.IdNotaNivel2AvaliacaoGestor = aval_TipoAvaliacao == "desempenho" ? (int?)null : 5; //NOTA NV 2 DE AV. GESTOR N/A EM COMPETENCIAS DE LIDERANÇA
                    avaliacaoCompetencia.TipoAvaliacao = aval_TipoAvaliacao;
                    avaliacaoCompetencia.Escopo = aval_Escopo;
                    avaliacaoCompetencia.idAvaliacao = avaliacao.idAvaliacao;

                    if (item.IdNotaPadraoNivel1 != null)
                    {
                        avaliacaoCompetencia.IdNotaNivel1AutoAvaliacao = (int)item.IdNotaPadraoNivel1;
                        avaliacaoCompetencia.IdNotaNivel1AvaliacaoCegas = (int)item.IdNotaPadraoNivel1;
                        avaliacaoCompetencia.IdNotaNivel1AvaliacaoGestor = (int)item.IdNotaPadraoNivel1;
                        avaliacaoCompetencia.IdNotaNivel1Feedback = (int)item.IdNotaPadraoNivel1;
                    }

                    if (item.IdNotaPadraoNivel2 != null)
                    {
                        avaliacaoCompetencia.IdNotaNivel2AutoAvaliacao = (int)item.IdNotaPadraoNivel2;
                        avaliacaoCompetencia.IdNotaNivel2AvaliacaoCegas = (int)item.IdNotaPadraoNivel2;
                        avaliacaoCompetencia.IdNotaNivel2AvaliacaoGestor = (int)item.IdNotaPadraoNivel2;
                        avaliacaoCompetencia.IdNotaNivel2Feedback = (int)item.IdNotaPadraoNivel2;
                    }

                    avaliacaoService.SalvarAvaliacaoCompetencia(avaliacaoCompetencia);
                }
            }

            // Se ainda não está liberado, envia o Email
            if (!avaliacao.Liberado)
            {
                var paramEmail = new EmailParametroService().ObterParametro(avaliacao.idEmpresa);
                var configEmail = new ConfigEmail();
                configEmail.From = paramEmail.RemetenteEmail;
                configEmail.SmtpServer = paramEmail.SMTPServer;
                configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
                configEmail.Dominio = paramEmail.Dominio;
                configEmail.Senha = paramEmail.Password;
                configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
                configEmail.Remetente = paramEmail.RemetenteNome;

                string etapa = "AUTO AVALIAÇÃO";
                var gestor = new AssociadosService().ObterAssociado((int)avaliacao.idGestor); 
                var alocacao = new ProjetosService().ObterProjetoAssociadoAtributos(avaliacao.idAssociado, avaliacao.idProjeto, (int)avaliacao.idGestor, 
                    avaliacao.idPeriodo, avaliacao.TipoAvaliacao, avaliacao.Escopo);
                var avaliador = new AssociadosService().ObterAssociado((int)alocacao.IdAvaliador);

                var destinatario = new Destinatario
                {
                    Nome = avaliacao.ASSOCIADOS.Nome,
                    Email = avaliacao.ASSOCIADOS.Email
                };

                if (aval_TipoAvaliacao == "lideranca")
                {
                    destinatario.Nome = gestor.Nome;
                    destinatario.Email = gestor.Email;
                    etapa = "AVALIAÇÃO DE LIDERANÇA";
                }

                var dataFinal = DateTime.Now.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy"); //DateTime.Now.AddDays(workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");

                avaliacao.Liberado = true;
                avaliacao.DataLiberacao = DateTime.Now;
                avaliacaoService.AlterarAvaliacaoEmail(avaliacao.idAvaliacao, avaliacao);

                try
                {
                    // MONTA O EMAIL
                    var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
                    _corpo = ConfiguraBody(_corpo, avaliacao.PROJETOS, avaliacao.ASSOCIADOS, "", "", "", "", dataFinal, etapa, destinatario.Nome, avaliador);

                    var mensagem = new Mensagem
                    {
                        Titulo = "[RH Peers] - Processo de Avaliação - Etapa: Nova Avaliação - Período: " + avaliacao.PERIODOSAVALIACOES.Periodo,
                        Corpo = _corpo
                    };

                    var emailService = new EmailService(configEmail, destinatario, mensagem);

                    var thread = new Thread(new ThreadStart(emailService.Enviar));
                    thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
                    thread.IsBackground = true;
                    thread.Start();

                    EnviaEmailAvaliacaoAsCegas(avaliacao);
                    return "OK";

                }
                catch (Exception)
                {
                    return "SendNotEmail";
                }


            }
            else
                return "NotSend";
        }



        public void EnviaEmailAvaliacaoAsCegas (AVALIACAO avaliacao)
        {
            var paramEmail = new EmailParametroService().ObterParametro(avaliacao.idEmpresa);
            var configEmail = new ConfigEmail();
            configEmail.From = paramEmail.RemetenteEmail;
            configEmail.SmtpServer = paramEmail.SMTPServer;
            configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
            configEmail.Dominio = paramEmail.Dominio;
            configEmail.Senha = paramEmail.Password;
            configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
            configEmail.Remetente = paramEmail.RemetenteNome;

            var prazo = new WorkflowService().ObterPrazo(avaliacao.IdPrazo);

            string etapa = "Avaliação às Cegas";
            var gestor = new AssociadosService().ObterAssociado((int)avaliacao.idGestor);
            var alocacao = new ProjetosService().ObterProjetoAssociadoAtributos(avaliacao.idAssociado, avaliacao.idProjeto, (int)avaliacao.idGestor,
                avaliacao.idPeriodo, avaliacao.TipoAvaliacao, avaliacao.Escopo);
            var avaliador = new AssociadosService().ObterAssociado((int)alocacao.IdAvaliador);

            var destinatario = new Destinatario
            {
                Nome = avaliador.Nome,
                Email = avaliador.Email
            };

            var dataFinal = DateTime.Now.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");

            try
            {
                // MONTA O EMAIL
                var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
                _corpo = ConfiguraBody(_corpo, avaliacao.PROJETOS, avaliacao.ASSOCIADOS, "", "", "", "", dataFinal, etapa, destinatario.Nome, avaliador);

                var mensagem = new Mensagem
                {
                    Titulo = "[RH Peers] - Processo de Avaliação - " + etapa + " - Período: " + avaliacao.PERIODOSAVALIACOES.Periodo,
                    Corpo = _corpo
                };

                var emailService = new EmailService(configEmail, destinatario, mensagem);

                var thread = new Thread(new ThreadStart(emailService.Enviar));
                thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception)
            {
            }
        }

        private string ConfiguraBody(string corpo, PROJETOS projeto, ASSOCIADOS associado,
            string texto_inicial, string texto_principal, string info_final, string despedida, string prazoFinal, string etapa, string destinatario, ASSOCIADOS avaliador, 
            string descricao = "Uma nova avaliação está disponível para você.", bool removeDatas = false)
        {
            // Padrão Email
            var newCorpo = corpo.Replace("[INFO_INICIAL]", texto_inicial);
            newCorpo = newCorpo.Replace("[TEXTO_PRINCIPAL]", texto_principal);
            newCorpo = newCorpo.Replace("[INFO_FINAL]", info_final);
            newCorpo = newCorpo.Replace("[CUMPRIMENTOS]", despedida);

            // Dados Pessoais
            newCorpo = newCorpo.Replace("[NOME]", destinatario);
            newCorpo = newCorpo.Replace("[CARGO]", associado.CARGOS.Cargo);// +" - "+ associado.CARGOSNIVEIS.Nivel);
            newCorpo = newCorpo.Replace("[MENTOR]", associado.ASSOCIADOS2.Nome);
            newCorpo = newCorpo.Replace("[PROJETO]", projeto.Projeto);
            newCorpo = newCorpo.Replace("[GESTOR]", projeto.ASSOCIADOS1.Nome);
            newCorpo = newCorpo.Replace("[AVALIADOR]", avaliador.Nome);
            if (removeDatas)
            {
                newCorpo = newCorpo.Replace("<p><strong>Data de Início:</strong> [DATA_INICIO]</p>", "");
                newCorpo = newCorpo.Replace("<p><strong>Data de Término:</strong> [DATA_FINAL]</p>", "");
            }
            else
            {
                newCorpo = newCorpo.Replace("[DATA_INICIO]", projeto.DataInicio.ToString("dd/MM/yyyy"));
                newCorpo = newCorpo.Replace("[DATA_FINAL]", projeto.DataFim?.ToString("dd/MM/yyyy"));
            }
            newCorpo = newCorpo.Replace("[PRAZO_FINAL]", prazoFinal);
            newCorpo = newCorpo.Replace("[ETAPA_AVALIACAO]", etapa);

            // NomeAvaliaco e Descrição - Atualização RH - Setembro/2021
            newCorpo = newCorpo.Replace("[NOME_AVALIADO]", associado.Nome);
            newCorpo = newCorpo.Replace("[DESCRICAO]", descricao);

            return newCorpo;
        }

        protected void btnGerarUnico_CMD_Command(object sender, RepeaterCommandEventArgs e)
        {
            string idCompleto = e.CommandArgument.ToString();
            var listaIDs = idCompleto.Split(';').ToList();

            HtmlSelect cboxDisparo = (HtmlSelect)e.Item.FindControl("cboxDisparo");

            var resposta = GerarEnviar(listaIDs, Convert.ToInt32(cboxDisparo.Value));

            switch (resposta)
            {
                case "OK":
                    MessageBox.Show("Avaliação Gerada e Enviada com Sucesso.", "Enviar Avaliação", TIPO.Info, MessageBoxHandler);
                    break;
                case "Fail":
                    MessageBox.Show("Ocorreu uma Falha ao Gerar a Avaliação.", "Avaliação Não Enviada", TIPO.Error, MessageBoxHandler);
                    break;
                case "NotSend":
                    MessageBox.Show("A Avaliação já havia sido Gerada e Enviada anteriormente.", "Avaliação Não Enviada", TIPO.Warning, MessageBoxHandler);
                    break;
                case "SendNotEmail":
                    MessageBox.Show("A Avaliação foi Gerada. Ocorreu erro no envio do E-mail", "Avaliação Gerada e Não Enviada", TIPO.Warning, MessageBoxHandler);
                    break;
                case "NoWorkflow":
                    MessageBox.Show("Não existe Workflow cadastrado para o Período selecionado.", "Geração da Avaliação Cancelada", TIPO.Error, MessageBoxHandler);
                    break;
            }
        }

        protected void rptAssociados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                HtmlSelect cboxDisparo = (HtmlSelect)e.Item.FindControl("cboxDisparo");

                // DISPAROS
                var prazosService = new WorkflowService();
                var prazosAtivos = prazosService.ObterTodosPrazosAtivos();
                foreach (var itemDisparo in prazosAtivos)
                {
                    ListItem addItem = new ListItem();
                    addItem.Value = itemDisparo.IdPrazo.ToString();
                    addItem.Text = itemDisparo.NomeDisparo;
                    cboxDisparo.Items.Add(addItem);
                }
            }
        }

        public void carregaPendencias()
        {
            rptProjetos.DataSource = getListaPendencias();
            rptProjetos.DataBind();
        }

        protected void btnRedispararTodos_Click(object sender, EventArgs e)
        {
            var pendencias = getListaPendencias();
            var contaSucesso = 0;
            var contaFalha = 0;
            foreach (var itemProjeto in pendencias)
            {
                var avaliacaoService = new AvaliacoesService();
                var avaliacao = avaliacaoService.ObterAvaliacaoEmail(itemProjeto.IdProjeto, itemProjeto.IdAvaliado, itemProjeto.IdPeriodo, 1, itemProjeto.TipoAvaliacao, itemProjeto.Escopo, itemProjeto.IdGestor);

                var paramEmail = new EmailParametroService().ObterParametro(avaliacao.idEmpresa);
                var configEmail = new ConfigEmail();
                configEmail.From = paramEmail.RemetenteEmail;
                configEmail.SmtpServer = paramEmail.SMTPServer;
                configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
                configEmail.Dominio = paramEmail.Dominio;
                configEmail.Senha = paramEmail.Password;
                configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
                configEmail.Remetente = paramEmail.RemetenteNome;

                string etapa = itemProjeto.Pendencia;
                var gestor = new AssociadosService().ObterAssociado((int)avaliacao.idGestor);
                var alocacao = new ProjetosService().ObterProjetoAssociadoAtributos(avaliacao.idAssociado, avaliacao.idProjeto, (int)avaliacao.idGestor,
                    avaliacao.idPeriodo, avaliacao.TipoAvaliacao, avaliacao.Escopo);
                var avaliador = new AssociadosService().ObterAssociado((int)alocacao.IdAvaliador);

                var destinatario = new Destinatario();
                destinatario.Nome = itemProjeto.Respondente;
                destinatario.Email = itemProjeto.RespondenteEmail;

                var prazo = new WorkflowService().ObterPrazo(avaliacao.IdPrazo);
                var dataFinal = DateTime.Now.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");

                try
                {
                    // MONTA O EMAIL
                    var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
                    _corpo = ConfiguraBody(_corpo, avaliacao.PROJETOS, avaliacao.ASSOCIADOS, "", "", "", "", dataFinal, etapa, destinatario.Nome, avaliador, "Você tem uma avaliação pendente!", true);

                    var mensagem = new Mensagem
                    {
                        Titulo = "[RH Peers] - Processo de Avaliação - Etapa Pendente - Período: " + avaliacao.PERIODOSAVALIACOES.Periodo,
                        Corpo = _corpo
                    };

                    var emailService = new EmailService(configEmail, destinatario, mensagem);

                    var thread = new Thread(new ThreadStart(emailService.Enviar));
                    thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
                    thread.IsBackground = true;
                    thread.Start();
                    contaSucesso++;
                }
                catch (Exception)
                {
                    contaFalha--;
                }
            }
            if (contaFalha == 0)
            {
                MessageBox.Show("E-mails de pendências enviados", "OK", TIPO.Info, MessageBoxHandler);
            }
            else
            {
                if (contaSucesso > 0)
                {
                    var msg = $"E-mail enviado parcialmente!<br/>Total enviado: {contaSucesso}<br/>Total com falha: {contaFalha}";
                    MessageBox.Show("E-mails de pendências enviados", "OK", TIPO.Default, MessageBoxHandler); 
                }
                else
                {
                    MessageBox.Show("Falha ao enviar e-mails. ", "OK", TIPO.Default, MessageBoxHandler);
                }
            }
        }

        public List<ProjetoModel> getListaPendencias()
        {
            var associadosService = new AssociadosService();
            var avaliacaoService = new AvaliacoesService();
            var periodosService = new PeriodoService();
            var projetosService = new ProjetosService();
            var ultimoPeriodo = periodosService.ObterPeriodoUltimo();

            var projetos = projetosService.ObterListaProjetosAssociadosTodos();
            projetos = projetos.Where(pa => (pa.DataInicio >= ultimoPeriodo.DataInicio || pa.IdPeriodoSinalizado == ultimoPeriodo.IdPeriodo) && pa.ATV == 1).ToList();

            List<ProjetoModel> listaProjetosAvaliacoes = new List<ProjetoModel>();

            foreach (var itemProjeto in projetos)
            {
                var umProjeto = new ProjetoModel();
                var periodo = periodosService.VerificaExistenciaPeriodo(itemProjeto.DataInicio, (DateTime)itemProjeto.DataFim, 1);
                if (periodo == null) { continue; }
                if (periodo.IdPeriodo != ultimoPeriodo.IdPeriodo && itemProjeto.IdPeriodoSinalizado != ultimoPeriodo.IdPeriodo) { continue; }

                var avaliacao = avaliacaoService.ObterAvaliacaoEmail(itemProjeto.IdProjeto, itemProjeto.IdAssociado, periodo.IdPeriodo, 1, itemProjeto.TipoAvaliacao, itemProjeto.Escopo,
                    (int)itemProjeto.IdGestor);

                if (avaliacao == null) { continue; }
                if (avaliacao.PosicaoAtualFluxoAvaliacao == "AFI") { continue; }

                if (avaliacao.PROJETOS.ATV == 0) { continue; }

                umProjeto.FotoNome = avaliacao.ASSOCIADOS.FotoNome;
                if (avaliacao.ASSOCIADOS.FotoNome == null || avaliacao.ASSOCIADOS.FotoNome == "")
                {
                    umProjeto.FotoNome = "assets/images/users/usernophoto.jpg";
                }
                umProjeto.FotoNome = umProjeto.FotoNome.Replace(" ", "%20");

                umProjeto.Nome = itemProjeto.ASSOCIADOS.Nome;
                umProjeto.IdAvaliado = itemProjeto.ASSOCIADOS.IdAssociado;
                umProjeto.Projeto = itemProjeto.PROJETOS.Projeto;
                umProjeto.Periodo = ultimoPeriodo.Periodo;
                umProjeto.IdPeriodo = ultimoPeriodo.IdPeriodo;
                umProjeto.TipoAvaliacao = itemProjeto.TipoAvaliacao;
                umProjeto.Escopo = itemProjeto.Escopo;
                umProjeto.IdGestor = (int)itemProjeto.IdGestor;
                umProjeto.IdProjeto = itemProjeto.IdProjeto;

                PRAZOS prazo = avaliacao.PRAZOS;

                // CONFERE CADA CASO
                if (avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaNaoIniciada || avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAutoAvaliacao)
                {
                    umProjeto.Pendencia = "Auto Avaliação";
                    var dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");
                    umProjeto.DataLimite =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") :
                            dataFinal;
                    umProjeto.Respondente = associadosService.ObterAssociado(avaliacao.idAssociado).Nome;
                    umProjeto.RespondenteEmail = associadosService.ObterAssociado(avaliacao.idAssociado).Email;
                }
                else if (avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoAsCegas)
                {
                    umProjeto.Pendencia = "Avaliação as Cegas";
                    var dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");
                    umProjeto.DataLimite =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") :
                            dataFinal;
                    umProjeto.Respondente = associadosService.ObterAssociado((int)itemProjeto.IdAvaliador).Nome;
                    umProjeto.RespondenteEmail = associadosService.ObterAssociado((int)itemProjeto.IdAvaliador).Email;
                }
                else if (avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoGestor)
                {
                    if (avaliacao.TipoAvaliacao == "desempenho")
                    {
                        umProjeto.Pendencia = "Avaliação do Gestor";
                    }
                    else
                    {
                        umProjeto.Pendencia = "Avaliação de Liderança";
                    }

                    var dataFinal = avaliacao.DataLiberacao != null ? avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy") :
                            avaliacao.DHC.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");
                    umProjeto.DataLimite =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") :
                            dataFinal;
                    umProjeto.Respondente = associadosService.ObterAssociado((int)itemProjeto.IdGestor).Nome;
                    umProjeto.RespondenteEmail = associadosService.ObterAssociado((int)itemProjeto.IdGestor).Email;
                }
                else if (avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaFeedback)
                {
                    umProjeto.Pendencia = "Feedback";
                    var dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoFeedback).ToString("dd/MM/yyyy");
                    umProjeto.DataLimite =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorFeedback).ToString("dd/MM/yyyy") :
                            dataFinal;
                    umProjeto.Respondente = associadosService.ObterAssociado((int)itemProjeto.IdAvaliador).Nome + " ou " + associadosService.ObterAssociado((int)itemProjeto.IdGestor).Nome;
                    umProjeto.RespondenteEmail = associadosService.ObterAssociado((int)itemProjeto.IdGestor).Email;
                }
                else if (avaliacao.PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoMentor)
                {
                    umProjeto.Pendencia = "Mentoria";
                    var dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoMentor).ToString("dd/MM/yyyy");
                    umProjeto.DataLimite =
                        DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                            DateTime.Today.AddDays(prazo.CompensadorMentor).ToString("dd/MM/yyyy") :
                            dataFinal;
                    umProjeto.Respondente = associadosService.ObterAssociado(associadosService.ObterAssociado(avaliacao.idAssociado).IdAssociadoMentor).Nome;
                    umProjeto.RespondenteEmail = associadosService.ObterAssociado(associadosService.ObterAssociado(avaliacao.idAssociado).IdAssociadoMentor).Email;
                }
                else { continue; }

                listaProjetosAvaliacoes.Add(umProjeto);
            }

            return listaProjetosAvaliacoes;
        }
    }
}