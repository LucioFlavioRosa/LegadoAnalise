using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;

namespace SistemaAvaliacao
{
    public partial class avalizacao_mentor : System.Web.UI.Page
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

                // MENTORADOS VISÍVEIS AO LONGO DO SEMESTRE
                CarregaMentorados();
            }
        }

        #region CARREGA COMBOS
        private void CarregaComboProjetos()
        {
            var usuario = new AssociadosService().ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var projetosList = new ProjetosService().ListaProjetosAtivosComoMentor(usuario);
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
            var fotosAssociadosService = new FotosAssociadosService();

            if (ddlProjetos.SelectedIndex > 0)
                projeto = projetoService.ObterProjeto(Convert.ToInt32(ddlProjetos.Items[ddlProjetos.SelectedIndex].Value));

            if (ddlClientes.SelectedIndex > 0)
                cliente = clienteService.ObterCliente(Convert.ToInt32(ddlClientes.Items[ddlClientes.SelectedIndex].Value));

            if (ddlPeriodos.SelectedIndex > 0)
                periodo = periodoService.ObterPeriodo(Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value));

            if (ddlStatus.SelectedIndex > 0)
                status = statusService.ObterStatusProjeto(Convert.ToInt32(ddlStatus.Items[ddlStatus.SelectedIndex].Value));

            var mentor = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            var projetos = new ProjetosService().ListaProjetosAtivosComoMentor(mentor, projeto, status, periodo, cliente, "desempenho");

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

                var associado = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);

                var projetoPessoas = projetoService.ObterListaAssociadosComoMentor(itemProjeto.IdProjeto, mentor.IdAssociado);
                projetoPessoas = projetoPessoas.FindAll(pp => pp.TipoAvaliacao == "desempenho");

                foreach (var itemProjetoPessoa in projetoPessoas)
                {
                    List<PERIODOSAVALIACOES> periodosAvaliacoes = new List<PERIODOSAVALIACOES>();

                    if (periodo != null)
                        periodosAvaliacoes.Add(periodo);
                    else
                        periodosAvaliacoes = periodoService.ListaTodosPeriodos(itemProjeto.IdEmpresa);
                        //periodosAvaliacoes = periodoService.ListaPeriodos(itemProjeto.IdEmpresa, itemProjetoPessoa.DataInicio, itemProjetoPessoa.DataFim.Value);

                    foreach (var periodoAvaliacao in periodosAvaliacoes)
                    {
                        var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(itemProjetoPessoa.IdProjeto, itemProjetoPessoa.IdAssociado, periodoAvaliacao.IdPeriodo, associado.IdEmpresa,
                            itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, (int)itemProjetoPessoa.IdGestor);

                        var avaliacao = avaliacaoService.ObterAvaliacoesCompetencias(itemProjetoPessoa.IdAssociado, itemProjeto.IdProjeto, periodoAvaliacao.IdPeriodo,
                            itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo, avaliacaoEmail != null ? avaliacaoEmail.idAvaliacao : -1);

                        // Se avaliação > 0, indica que possui avaliação na Etapa Atual
                        if (avaliacao.Count > 0)
                        {
                            ProjetosAssociadosModel projetosAssociadosModel = new ProjetosAssociadosModel();

                            // Checa se já liberou a avaliação
                            //var avaliacaoEmail = avaliacaoService.ObterAvaliacaoEmail(itemProjetoPessoa.IdProjeto, itemProjetoPessoa.IdAssociado, periodoAvaliacao.IdPeriodo, itemProjeto.IdEmpresa);
                            //if (avaliacaoEmail != null)
                            //    projetosAssociadosModel.AvaliacaoLiberada = avaliacaoEmail.Liberado;
                            //else
                            //    projetosAssociadosModel.AvaliacaoLiberada = false;

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
                                projetosAssociadosModel.Gestor = associadosService.ObterAssociado((int)itemProjetoPessoa.IdGestor); //projetosAssociadosModel.Projeto.IdAssociadoGestor);

                            
                            if (projetosAssociadosModel.Associado != null)
                            {
                                if (projetosAssociadosModel.FotoAssociado == null)
                                {
                                    projetosAssociadosModel.Associado.FotoNome = "assets/images/users/usernophoto.jpg";
                                }
                                else
                                {
                                    projetosAssociadosModel.Associado.FotoNome = projetosAssociadosModel.FotoAssociado.Imagem;
                                }

                                projetosAssociadosModel.Associado.CARGOS = cargosService.ObterCargo(projetosAssociadosModel.Associado.IdCargo);
                                projetosAssociadosModel.Associado.CARGOS.Cargo = projetosAssociadosModel.Associado.CARGOS.Cargo;

                                //if (projetosAssociadosModel.Associado.CARGOSNIVEIS.IdNivel == cargosNiveisServices.nivelJunior)
                                //projetosAssociadosModel.Associado.CARGOS.Cargo = projetosAssociadosModel.Associado.CARGOS.Cargo + " - JR";
                                //if (projetosAssociadosModel.Associado.CARGOSNIVEIS.IdNivel == cargosNiveisServices.nivelPleno)
                                //projetosAssociadosModel.Associado.CARGOS.Cargo = projetosAssociadosModel.Associado.CARGOS.Cargo + " - PL";
                                //if (projetosAssociadosModel.Associado.CARGOSNIVEIS.IdNivel == cargosNiveisServices.nivelSenior)
                                //projetosAssociadosModel.Associado.CARGOS.Cargo = projetosAssociadosModel.Associado.CARGOS.Cargo + " - SR";


                                projetosAssociadosModel.Avaliador = associadosService.ObterAssociado(projetosAssociadosModel.Projeto.IdAssociadoGestor);
                                
                                var projetosAssociados = new ProjetosService().ObterGestorEAvaliadorDeAssociado(itemProjeto.IdProjeto, projetosAssociadosModel.Associado.IdAssociado,
                                    itemProjetoPessoa.IdProjetoAssociado);

                                int idGestor = 0;
                                int idAvaliador = 0;

                                if (projetosAssociados != null && projetosAssociados.IdGestor != null)
                                {
                                    idGestor = Convert.ToInt32(projetosAssociados.IdGestor);
                                    projetosAssociadosModel.Gestor.Nome = associadosService.ObterAssociado(idGestor).Nome;
                                }


                                if (projetosAssociados != null && projetosAssociados.IdAvaliador != null)
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

                            // Busca a primeira avaliação para obter o status (esta lógica está muito ruim. Deveria haver uma tabela Avaliação com os dados comuns que hoje estão duplicados no banco)
                            //projetosAssociadosModel.Status = avaliacao[0].AVALIACOESSTATUS;

                            if (avaliacao[0].PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAvaliacaoMentor && avaliacao[0].PosicaoAtualFluxoAvaliacao != avaliacaoService.etapaAvaliacaoFinalizada)
                            {
                                projetosAssociadosModel.ExibirBotaoVerMentor = "hidden";
                                projetosAssociadosModel.ExibirRotuloEtapa = "";
                                if (avaliacao[0].PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaNaoIniciada)
                                    projetosAssociadosModel.Etapa = "Não iniciado";
                                else if (avaliacao[0].PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAutoAvaliacao)
                                    projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                else if (avaliacao[0].PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoAsCegas)
                                    projetosAssociadosModel.Etapa = "Em auto avaliação e avaliação as cegas";
                                else if (avaliacao[0].PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaEmParalelo)
                                    projetosAssociadosModel.Etapa = "Em Auto-avaliação e Av. às Cegas";
                                else if (avaliacao[0].PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaAvaliacaoGestor)
                                    projetosAssociadosModel.Etapa = "Em avaliação do gestor";
                                else if (avaliacao[0].PosicaoAtualFluxoAvaliacao == avaliacaoService.etapaFeedback)
                                    projetosAssociadosModel.Etapa = "Em feedback";
                            }
                            else
                            {
                                projetosAssociadosModel.ExibirBotaoVerMentor = "";
                                projetosAssociadosModel.ExibirRotuloEtapa = "";
                                projetosAssociadosModel.Etapa = "Em Cons. Mentor";
                            }

                            projetosAssociadosModel.ExibirBotaoFinalizar = false;
                            projetosAssociadosModel.RotuloBotao = "Ver Avaliação";
                            projetosAssociadosModel.Status = statusService.ObterStatusAvaliacao("Concluído");

                            // VERIFICA SE FEEDBACK RH EXISTE E ESTA DISPONIVEL
                            var consideracoesMentorService = new ConsideracoesMentorService();
                            var consideracoesMentor = 
                                consideracoesMentorService.ObterConsideracoesMentorPorAtributos(mentor.IdAssociado, itemProjetoPessoa.IdAssociado, periodoAvaliacao.IdPeriodo,
                                itemProjetoPessoa.TipoAvaliacao, itemProjetoPessoa.Escopo);
                            if (consideracoesMentor == null || consideracoesMentor.LiberadoRH != true)
                            {
                                projetosAssociadosModel.FeedbackRHVisible = "hidden";
                            }

                            if (avaliacaoEmail != null)
                                umProjeto.Associados.Add(projetosAssociadosModel);
                        }
                    }
                }

                listaProjetosAvaliacoes.Add(umProjeto);
            }

            rptProjetos.DataSource = listaProjetosAvaliacoes;
            rptProjetos.DataBind();
        }

        protected void CarregaMentorados()
        {
            var associadosService = new AssociadosService();
            var consideracoesService = new ConsideracoesMentorService();
            var periodosService = new PeriodoService();
            var fotosAssociadosService = new FotosAssociadosService();

            if (periodosService.ObterPeriodoUltimo() != null)
            {
                var associados = associadosService.ObterAssociados();
                var mentor = associadosService.ObterAssociado(WebStorage.GetUsuarioLogado().Id);
                var fotosAssociados = fotosAssociadosService.ListarFotos();

                var mentoradosAtivos = (from item in associados
                                        where item.ATV == 1
                                        && item.IdAssociadoMentor == mentor.IdAssociado
                                        select new MentoradosModel()
                                        {
                                            idAssociado = item.IdAssociado,
                                            Associado = item.Nome,
                                            idMentor = item.IdAssociadoMentor,
                                            Mentor = item.ASSOCIADOS2.Nome,
                                            idCargo = item.IdCargo,
                                            Cargo = item.CARGOS.Cargo,
                                            FotoNome = fotosAssociados.FirstOrDefault(f => f.IdAssociado == item.IdAssociado)?.Imagem ?? "assets/images/users/usernophoto.jpg",
                                            idProjeto = -1,
                                            idGestor = -1,
                                            idPeriodo = periodosService.ObterPeriodoUltimo().IdPeriodo,
                                            TipoAvaliacao = "desempenho",
                                            Escopo = "projeto"
                                        });

                rptMentorados.DataSource = mentoradosAtivos;
                rptMentorados.DataBind();
            }
        }
    }
}