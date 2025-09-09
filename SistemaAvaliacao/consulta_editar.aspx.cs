using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using SistemaAvaliacao.UserControls;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using AjaxControlToolkit;
using SistemaAvaliacao.Scripts;

namespace SistemaAvaliacao
{
    public partial class consulta_editar : System.Web.UI.Page
    {
        const string etapaNaoIniciada = "AVM";
        const string etapaAutoAvaliacao = "AAV";
        const string etapaAvaliacaoAsCegas = "ACE";
        const string etapaAvaliacaoGestor = "AGE";
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

            //if (ddlFase.SelectedIndex > 0)
               // avaliacoes = avaliacoes.Where(x => x.PosicaoAtualFluxoAvaliacao == ddlFase.SelectedValue.ToString()).ToList();

            //Filtra pelo Cliente
            if (cliente != null)
                avaliacoes = avaliacoes.FindAll(a => a.PROJETOS.IdCliente == cliente.IdCliente);

            List<AvaliacaoEmailModel> listaAvaliacoes = new List<AvaliacaoEmailModel>();
            
            foreach (var avaliacao in avaliacoes)
            {
                var umaAvaliacao = new AvaliacaoEmailModel();

                umaAvaliacao.Associado = avaliacao.ASSOCIADOS;
                umaAvaliacao.Cliente = avaliacao.PROJETOS.CLIENTES;
                umaAvaliacao.DataInicio = avaliacao.DataLiberacao.Value.ToString("dd/MM/yyyy");
                umaAvaliacao.DataTermino = avaliacaoService.CalculaDataTermino(avaliacao, avaliacaoService.etapaFeedback);
                
                umaAvaliacao.Gestor = associadosService.ObterGestor(avaliacao.idProjeto, avaliacao.idAssociado);
                umaAvaliacao.Id = avaliacao.idAvaliacao;
                umaAvaliacao.Periodo = avaliacao.PERIODOSAVALIACOES;
                umaAvaliacao.Projeto = avaliacao.PROJETOS;
                umaAvaliacao.Status = avaliacao.AVALIACOESSTATUS;

                switch (avaliacao.PosicaoAtualFluxoAvaliacao)
                {
                    case etapaNaoIniciada:
                        umaAvaliacao.Fase = "Não Iniciado";
                        break;
                    case etapaAutoAvaliacao:
                        umaAvaliacao.Fase = "Iniciado";                        
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
            //btnEfetuarRetrocesso.Visible = false;
        }

        protected void btnRetroceder_Click(object sender, EventArgs e)
        {
            AVALIACAO avaliacao;
            int idAvaliacao = Convert.ToInt32((sender as System.Web.UI.WebControls.Button).CommandArgument);

            DataModel dm = new DataModel();
            avaliacao = dm.AVALIACAO.FirstOrDefault(x => x.idAvaliacao == idAvaliacao);
            if (avaliacao != null)
            {
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaNaoIniciada)
                {
                    MessageBox.Show("Não é possível retroceder uma avaliação que ainda não foi iniciada.", "Alerta", TIPO.Error, MessageBoxHandler);
                    //btnEfetuarRetrocesso.Visible = false;
                }
                else
                {
                    int idPeriodo = avaliacao.idPeriodo;
                    PERIODOSAVALIACOES periodoVigente;
                    periodoVigente = dm.PERIODOSAVALIACOES.OrderByDescending(p => p.IdPeriodo).FirstOrDefault(x => x.ATV == 1);

                    if (idPeriodo == periodoVigente.IdPeriodo)
                    {
                        if (periodoVigente.DataFim >= DateTime.Today)
                        {
                            ddlProjetos.Items[ddlProjetos.SelectedIndex].Text = new ProjetosService().ObterProjeto(avaliacao.idProjeto).Projeto;
                            ddlClientes.Items[ddlClientes.SelectedIndex].Text = new ClientesService().ObterCliente(
                                new ProjetosService().ObterProjeto(avaliacao.idProjeto).IdCliente).Cliente;
                            ddlProfissionais.Items[ddlProfissionais.SelectedIndex].Text = new AssociadosService().ObterAssociado(avaliacao.idAssociado).Nome;
                            ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Text = periodoVigente.Periodo;
                            //ddlFase.SelectedValue = avaliacao.PosicaoAtualFluxoAvaliacao;
                            ddlAvaliacaoId.Text = avaliacao.idAvaliacao.ToString();
                            //btnEfetuarRetrocesso.Visible = true;
                        }
                        else
                        {
                            MessageBox.Show("Não possível retroceder esta avaliação pois o período já encerrou.", "Alerta", TIPO.Error, MessageBoxHandler);
                            //btnEfetuarRetrocesso.Visible = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Não é possível retroceder uma avaliação de um período anterior ao vigente.", "Alerta", TIPO.Error, MessageBoxHandler);
                        //btnEfetuarRetrocesso.Visible = false;
                    }
                }
            }

        }
        protected void btnEfetuarRetrocesso_Click(object sender, EventArgs e)
        {
            AVALIACAO avaliacao;
            DataModel dm = new DataModel();
            int idAvaliacao = Convert.ToInt32(ddlAvaliacaoId.Text);

            avaliacao = dm.AVALIACAO.FirstOrDefault(x => x.idAvaliacao == idAvaliacao);

            if (avaliacao != null)
            {
                if (avaliacao.PosicaoAtualFluxoAvaliacao != "1")
                {
                    avaliacao.PosicaoAtualFluxoAvaliacao = "1";
                    avaliacao.idStatus = 2;
                    dm.SaveChanges();
                                        
                    var competencias = dm.AVALIACOESCOMPETENCIAS.Where(x => x.IdAssociado == avaliacao.idAssociado && x.IdProjeto == avaliacao.idProjeto && x.IdPeriodo == avaliacao.idPeriodo).ToList();

                    foreach (var item in competencias)
                    {
                        item.IdAvaliacaoStatus = 2;
                        item.PosicaoAtualFluxoAvaliacao = "1";                       
                        dm.SaveChanges();
                    }

                    var performances = dm.AVALIACOESPERFORMANCES.Where(x => x.IdAssociado == avaliacao.idAssociado && x.IdProjeto == avaliacao.idProjeto && x.IdPeriodo == avaliacao.idPeriodo).ToList();

                    foreach (var item in performances)
                    {
                        item.IdAvaliacaoStatus = 2;
                        item.PosicaoAtualFluxoAvaliacao = "1";                        
                        dm.SaveChanges();
                    }

                    MessageBox.Show("Fase da avaliação atualizada com sucesso!", "Alerta", TIPO.Info, MessageBoxHandler);
                    btnSearch_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("A avaliação já está na fase selecionada.", "Alerta", TIPO.Error, MessageBoxHandler);
                }
            }
        }
    }
}