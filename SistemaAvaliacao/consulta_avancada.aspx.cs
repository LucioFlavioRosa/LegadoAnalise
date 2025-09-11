using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaAvaliacao
{
    public partial class consulta_avancada : System.Web.UI.Page
    {
        const string etapaNaoIniciada = "AVM";
        const string etapaAutoAvaliacao = "AAV";
        const string etapaAvaliacaoAsCegas = "ACE";
        const string etapaAvaliacaoGestor = "AGE";
        const string etapaEmParalelo = "AEP";
        const string etapaFeedback = "FED";
        const string etapaAvaliacaoMentor = "AME";
        const string etapaAvaliacaoFinalizada = "AFI";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboProjetos();
                CarregaComboClientes();
                CarregaComboProfissionais();
                CarregaComboPeriodos();
            }
        }

        private void CarregaComboProjetos()
        {
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

        private void CarregaComboProfissionais()
        {
            var profissaList = new AssociadosService().ObterAssociados(true);
            ddlProfissionais.DataValueField = "IdAssociado";
            ddlProfissionais.DataTextField = "Nome";
            ddlProfissionais.DataSource = profissaList;
            ddlProfissionais.DataBind();
            ddlProfissionais.Items.Insert(0, "[Selecionar]");
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


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Filtros
            PROJETOS projeto = null;
            CLIENTES cliente = null;
            ASSOCIADOS associado = null;
            PERIODOSAVALIACOES periodo = null;
                        
            var projetoService = new ProjetosService();
            var clienteService = new ClientesService();
            var associadosService = new AssociadosService();
            var periodoService = new PeriodoService();
            var statusService = new StatusService();            
            
            var cargosService = new CargosService();            
            var avaliacaoService = new AvaliacoesService();

            
            if (ddlProjetos.SelectedIndex > 0)
                projeto = projetoService.ObterProjeto(Convert.ToInt32(ddlProjetos.Items[ddlProjetos.SelectedIndex].Value));
  
            if (ddlProfissionais.SelectedIndex > 0)
                associado = associadosService.ObterAssociado(Convert.ToInt32(ddlProfissionais.Items[ddlProfissionais.SelectedIndex].Value));

            if (ddlPeriodos.SelectedIndex > 0)
                periodo = periodoService.ObterPeriodo(Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value));

            if (ddlClientes.SelectedIndex > 0)
                cliente = clienteService.ObterCliente(Convert.ToInt32(ddlClientes.Items[ddlClientes.SelectedIndex].Value));

            var avaliacoes = avaliacaoService.ObterAvaliacoesEmail(projeto, associado, periodo, null);

            if (ddlFase.SelectedIndex > 0)
                avaliacoes = avaliacoes.Where(x => x.PosicaoAtualFluxoAvaliacao == ddlFase.SelectedValue.ToString()).ToList();

            //Filtra pelo Cliente
            if (cliente != null)
                avaliacoes = avaliacoes.FindAll(a => a.PROJETOS.IdCliente == cliente.IdCliente);

            List<AvaliacaoEmailModel> listaAvaliacoes = new List<AvaliacaoEmailModel>();
            DataModel dm = new DataModel();

            foreach (var avaliacao in avaliacoes)
            {
                var umaAvaliacao = new AvaliacaoEmailModel();

                umaAvaliacao.Associado = avaliacao.ASSOCIADOS;
                umaAvaliacao.Cliente = avaliacao.PROJETOS.CLIENTES;
                umaAvaliacao.DataInicio = avaliacao.DataLiberacao.Value.ToString("dd/MM/yyyy");
                umaAvaliacao.DataTermino = avaliacaoService.CalculaDataTermino(avaliacao, avaliacaoService.etapaFeedback);
                umaAvaliacao.TipoAvaliacao = avaliacao.TipoAvaliacao;
                umaAvaliacao.Escopo = avaliacao.Escopo;
                
                umaAvaliacao.Gestor = associadosService.ObterGestor(avaliacao.idProjeto, avaliacao.idAssociado);
                umaAvaliacao.Id = avaliacao.idAvaliacao;
                umaAvaliacao.Periodo = avaliacao.PERIODOSAVALIACOES;
                umaAvaliacao.Projeto = avaliacao.PROJETOS;
                umaAvaliacao.Status = avaliacao.AVALIACOESSTATUS;

                var competencias = dm.AVALIACOESCOMPETENCIAS.Where(x => x.IdAssociado == avaliacao.idAssociado && x.IdProjeto == avaliacao.idProjeto && x.IdPeriodo == avaliacao.idPeriodo).ToList();

                var performances = dm.AVALIACOESPERFORMANCES.Where(x => x.IdAssociado == avaliacao.idAssociado && x.IdProjeto == avaliacao.idProjeto && x.IdPeriodo == avaliacao.idPeriodo).ToList();

                var competenciasAutoAvaliacaoFinalizada = competencias.Any(c => c.DataHoraFimAutoAvaliacao != null);
                var competenciasAsCegasFinalizada = competencias.Any(c => c.DataHoraFimAvaliacaoCegas != null);

                var performancesAutoAvaliacaoFinalizada = competencias.Any(c => c.DataHoraFimAutoAvaliacao != null);
                var performancesAsCegasFinalizada = competencias.Any(c => c.DataHoraFimAvaliacaoCegas != null);

                string statusAuto = competenciasAutoAvaliacaoFinalizada && performancesAutoAvaliacaoFinalizada ? "OK" : "Não OK";
                string statusAsCegas = competenciasAsCegasFinalizada && performancesAsCegasFinalizada ? "OK" : "Não OK";

                switch (avaliacao.PosicaoAtualFluxoAvaliacao)
                {
                    case etapaNaoIniciada:
                        umaAvaliacao.Fase = "Não Iniciado";
                        break;
                    case etapaAutoAvaliacao:
                        umaAvaliacao.Fase = "Iniciado";                        
                        break;
                    case etapaEmParalelo:
                        umaAvaliacao.Fase = $"Em Auto Avalição e às Cegas (Auto avaliação: {statusAuto}) - (Às cegas: {statusAsCegas}";
                        break;
                    case etapaAvaliacaoAsCegas:
                        umaAvaliacao.Fase = "As Cegas";                        
                        break;
                    case etapaAvaliacaoGestor:
                        umaAvaliacao.Fase = "Gestor";                        
                        break;
                    case etapaFeedback:
                        umaAvaliacao.Fase = "Feedback";
                        break;
                    case etapaAvaliacaoMentor:
                        umaAvaliacao.Fase = "Mentor";
                        break;
                    case etapaAvaliacaoFinalizada:
                        umaAvaliacao.Fase = "Finalizada";
                        break;
                }


                listaAvaliacoes.Add(umaAvaliacao);
            }

            rptProjetos.DataSource = listaAvaliacoes;
            rptProjetos.DataBind();
        }
    }
}