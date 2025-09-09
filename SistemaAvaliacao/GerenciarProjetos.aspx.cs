using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using Microsoft.Ajax.Utilities;
using Microsoft.Graph;
using Microsoft.PowerBI.Api.Models;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace SistemaAvaliacao
{
    public partial class GerenciarProjetos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LimparCampos();
                Session["HIERARQUIA"] = new List<HierarquiaAvaliacoesProjetoModel>();
            }

            //listaHierarquia.DataSource = Session["HIERARQUIA"];
            //listaHierarquia.DataBind();
        }

        private void LimparCampos()
        {
            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            labelTituloHierarquia.InnerText = ultimoPerido.Codigo + " - de " + ((DateTime)ultimoPerido.DataInicio).ToString("dd/MM/yyyy") + " a " + ((DateTime)ultimoPerido.DataFim).ToString("dd/MM/yyyy");
            montaComboListaProjetos();
        }

        public void montaComboListaProjetos()
        {
            List<PROJETOS> listaprojetos = new ProjetosService().ObterListaProjetosAtributos(idGestor: WebStorage.GetUsuarioLogado().Id);
            List<ProjetoModel> listaProjetoModels = new List<ProjetoModel>();
            listaProjetoModels = (from item in listaprojetos
                                  select new ProjetoModel()
                                  {
                                      IdProjeto = item.IdProjeto,
                                      Nome = item.Projeto,
                                      DataInicio = item.DataInicio.ToString("dd/MM/yyyy"),
                                      DataTermino = ((DateTime)item.DataFim).ToString("dd/MM/yyyy")
                                  }
                                  ).ToList();

            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            foreach (var item in listaProjetoModels)
            {
                var alocacoes = new ProjetosService().ObterListaAssociadosNoPeriodo(item.IdProjeto, -1, ultimoPerido);
                item.ContagemAlocados = alocacoes.Select(p => p.IdAssociado).Distinct().Count();
            }

            this.rptProjetos.DataSource = listaProjetoModels;
            this.rptProjetos.DataBind();
        }

        protected void btnGerenciarProjeto_Click(object sender, EventArgs e)
        {
            var IdProjeto = int.Parse(((Button)sender).CommandArgument);

            WebStorage.Set("IdProjeto", IdProjeto.ToString());
            normalizaHierarquiaProjeto(IdProjeto);
            atualizaBlocoHierarquia(IdProjeto, true);
            atualizaFatoresProjeto(IdProjeto);
        }

        protected void normalizaHierarquiaProjeto(int IdProjeto)
        {
            var ultimoPeriodo = new PeriodoService().ObterPeriodoUltimo();

            var getProjetosAssociados = new ProjetosService().ObterListaAssociadosNoPeriodo(IdProjeto, -1, ultimoPeriodo);
            var projeto = new ProjetosService().ObterProjeto(IdProjeto);
            var IdGestor = projeto.IdAssociadoGestor;
            var IdResponsavel = projeto.IdAssociadoResponsavel;
            string desempenho = "desempenho", lideranca = "lideranca";
            string escopoProjeto = "projeto", escopoLider = "líder", escopoBackOffice = "backoffice";
            var dummyNSA = new AssociadosService().ObterAssociadoPeloNome("Não se aplica");
            var dummyValidacao = new AssociadosService().ObterAssociadoPeloNome("Validação Gestor");

            var existeLiderancaResponsavel = getProjetosAssociados.Where(x => x.IdAssociado == IdResponsavel && x.TipoAvaliacao == lideranca).ToList();
            if (existeLiderancaResponsavel == null || existeLiderancaResponsavel.Count == 0)
            {
                adicionaAlocacao(projeto.IdProjeto, IdResponsavel, IdGestor, ultimoPeriodo, dummyNSA.IdAssociado, lideranca, escopoLider);
            }

            var existeDesempenhoGestor = getProjetosAssociados.Where(x => x.IdAssociado == IdGestor && x.TipoAvaliacao == desempenho).ToList();
            if (existeDesempenhoGestor == null || existeDesempenhoGestor.Count == 0)
            {
                adicionaAlocacao(projeto.IdProjeto, IdGestor, IdGestor, ultimoPeriodo, IdResponsavel, desempenho, escopoProjeto);
            }

            var existeValidacaoLider = getProjetosAssociados.Where(x => x.IdAvaliador == dummyValidacao.IdAssociado ||
                x.IdAssociado == dummyValidacao.IdAssociado ||
                x.IdGestor == dummyValidacao.IdAssociado).ToList();
            if (existeValidacaoLider != null && existeValidacaoLider.Count > 0)
            {
                var existeDesempenhoValidacao = getProjetosAssociados.Where(x => x.IdAssociado == dummyValidacao.IdAssociado && x.TipoAvaliacao == desempenho).ToList();
                if (existeDesempenhoValidacao == null || existeDesempenhoValidacao.Count == 0)
                {
                    adicionaAlocacao(projeto.IdProjeto, dummyValidacao.IdAssociado, IdGestor, ultimoPeriodo, IdGestor, desempenho, escopoProjeto);
                }

                var existeLiderancaGestor = getProjetosAssociados.Where(x => x.IdAssociado == IdGestor && x.TipoAvaliacao == lideranca).ToList();
                if (existeLiderancaGestor == null || existeLiderancaGestor.Count == 0)
                {
                    adicionaAlocacao(projeto.IdProjeto, IdGestor, dummyValidacao.IdAssociado, ultimoPeriodo, dummyNSA.IdAssociado, lideranca, escopoLider);
                }

                foreach (var alocacao in existeValidacaoLider)
                {
                    var existeDesempenhoAssociado = getProjetosAssociados.Where(x => x.IdAssociado == alocacao.IdAssociado && x.TipoAvaliacao == desempenho).ToList();
                    if (existeDesempenhoAssociado == null || existeDesempenhoAssociado.Count == 0)
                    {
                        adicionaAlocacao(projeto.IdProjeto, alocacao.IdAssociado, IdGestor, ultimoPeriodo, dummyValidacao.IdAssociado, desempenho, escopoProjeto);
                    }

                    var existeLiderancaLiderado = getProjetosAssociados.Where(x => x.IdGestor == alocacao.IdAssociado && x.TipoAvaliacao == lideranca).ToList();
                    if (existeLiderancaLiderado == null || existeLiderancaLiderado.Count == 0)
                    {
                        adicionaAlocacao(projeto.IdProjeto, dummyValidacao.IdAssociado, alocacao.IdAssociado, ultimoPeriodo, dummyNSA.IdAssociado, lideranca, escopoProjeto);
                    }
                }
            }

        }

        protected void adicionaAlocacao(int IdProjeto, int IdAssociado, int IdGestor, PERIODOSAVALIACOES ultimoPeriodo, int IdAvaliador, string TipoAvaliacao, string Escopo)
        {
            var projetosService = new ProjetosService();

            var addAlocacao = new PROJETOSASSOCIADOS();
            addAlocacao.IdProjeto = IdProjeto;
            addAlocacao.IdAssociado = IdAssociado;
            addAlocacao.IdGestor = IdGestor;
            addAlocacao.DataInicio = (DateTime)ultimoPeriodo.DataInicio;
            addAlocacao.DataFim = ultimoPeriodo.DataFim;
            addAlocacao.Comentario = "Adicionado automaticamente";
            addAlocacao.USR = WebStorage.GetUsuarioLogado().Id;
            addAlocacao.DHC = DateTime.Now;
            addAlocacao.ATV = 1;
            addAlocacao.IdAvaliador = IdAvaliador;
            addAlocacao.TipoAvaliacao = TipoAvaliacao;
            addAlocacao.Escopo = Escopo;

            projetosService.InserirProjetoAssociado(addAlocacao);
        }

        public void atualizaBlocoHierarquia(int IdProjeto, bool sobePagina, string aba = "hierarquia")
        {
            var getProjeto = new ProjetosService().ObterProjeto(IdProjeto);
            txtCodigo.Text = getProjeto.Codigo;
            txtProjeto.Text = getProjeto.Projeto;
            txtCliente.Text = new ClientesService().ObterCliente(getProjeto.IdCliente).Cliente;
            txtDataInicio.Text = getProjeto.DataInicio.ToString("dd/MM/yyyy");
            txtDataTermino.Text = getProjeto.DataFim != null ? ((DateTime)(getProjeto.DataFim)).ToString("dd/MM/yyyy") : "";
            ddlResponsavel.Text = new AssociadosService().ObterAssociado(getProjeto.IdAssociadoResponsavel).Nome;
            ddlStatus.Text = new StatusService().ObterStatusProjeto(getProjeto.IdStatus).Status;
            ddlTipoProjeto.Text = new TipoProjetoService().ObterTipoProjeto(getProjeto.IdTipo).ProjetoTipo;
            ddlComplexidade.Text = new ComplexidadesService().ObterComplexidade(getProjeto.IdComplexidade).Complexidade;

            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            var getProjetosAssociados = new ProjetosService().ObterListaAssociadosNoPeriodo(getProjeto.IdProjeto, -1, ultimoPerido);
            getProjetosAssociados = getProjetosAssociados.Where(gpa => gpa.TipoAvaliacao == "desempenho").ToList();
            var returnHierarquia = new HierarquiaAvaliacoesProjetoModel();
            returnHierarquia.IdProjeto = getProjeto.IdProjeto;
            returnHierarquia.Projeto = getProjeto.Projeto;
            returnHierarquia.Associados = new List<HierarquiaAssociadoModel>();

            var lideresTopLevel = new List<int>();
            foreach (var item in getProjetosAssociados)
            {
                if (!lideresTopLevel.Contains((int)item.IdAvaliador) && !getProjetosAssociados.Select(x => x.IdAssociado).ToList().Contains((int)item.IdAvaliador))
                {
                    lideresTopLevel.Add((int)item.IdAvaliador);
                    returnHierarquia.Associados.Add(getHieraquiaAssociado(getProjetosAssociados, (int)item.IdAvaliador, 0, false, getProjeto.IdProjeto, getProjeto.IdAssociadoGestor, item));
                }
            }

            Session["HIERARQUIA"] = new List<HierarquiaAvaliacoesProjetoModel>() { returnHierarquia };
            listaHierarquia.DataSource = new List<HierarquiaAvaliacoesProjetoModel>() { returnHierarquia };
            listaHierarquia.DataBind();

            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "expandContract", "expandContract()", true);
            if (sobePagina)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "scrollTop", "scrollTop()", true);
            }
        }

        public HierarquiaAssociadoModel getHieraquiaAssociado(List<PROJETOSASSOCIADOS> getProjetosAssociados, int IdAvaliador, int MarginLeft, bool isLast, int IdProjeto, int IdGestor, PROJETOSASSOCIADOS alocacao)
        {
            var dummyValidacao = new AssociadosService().ObterAssociadoPeloNome("Validação Gestor");
            var associado = new AssociadosService().ObterAssociado(IdAvaliador);
            var returnHierarquiaAssociado = new HierarquiaAssociadoModel();
            returnHierarquiaAssociado.IdProjeto = IdProjeto;
            returnHierarquiaAssociado.IdAssociado = associado.IdAssociado;
            returnHierarquiaAssociado.Associado = associado.Nome;
            returnHierarquiaAssociado.FotoNome = associado.FotoNome;
            returnHierarquiaAssociado.IdAvaliador = IdGestor;
            returnHierarquiaAssociado.MarginLeft = MarginLeft;
            returnHierarquiaAssociado.MarginLeftText = "margin-left: " + MarginLeft.ToString() + "px";
            returnHierarquiaAssociado.Avaliados = new List<HierarquiaAssociadoModel>();
            returnHierarquiaAssociado.NovoAvaliado = new List<int>();

            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            var getAlocacaoDesempenho = new ProjetosService().ObterProjetoAssociadoHierarquia(associado.IdAssociado, IdProjeto, IdGestor, ultimoPerido.IdPeriodo, "desempenho");
            returnHierarquiaAssociado.RespondeDesempenho = getAlocacaoDesempenho != null;
            var getAlocacaoLideranca = new ProjetosService().ObterProjetoAssociadoHierarquia(IdGestor, IdProjeto, associado.IdAssociado, ultimoPerido.IdPeriodo, "lideranca");
            returnHierarquiaAssociado.RespondeLideranca = getAlocacaoLideranca != null;

            returnHierarquiaAssociado.DataInicioAlocacao = alocacao.DataInicio;
            returnHierarquiaAssociado.InicioAlocacao = alocacao.DataInicio.Date.ToString("yyyy-MM-dd");
            returnHierarquiaAssociado.DataTerminoAlocacao = alocacao.DataFim;
            returnHierarquiaAssociado.TerminoAlocacao = ((DateTime)alocacao.DataFim).Date.ToString("yyyy-MM-dd");

            if (alocacao.IdPeriodoSinalizado == -1)
            {
                returnHierarquiaAssociado.IdPeriodoSinalizado = -1;
                returnHierarquiaAssociado.PeriodoSinalizado = "Próximo";
                returnHierarquiaAssociado.IdPeriodoAvaliacao = -1;
                returnHierarquiaAssociado.PeriodoAvaliacao = "Próximo";
            }
            else if (alocacao.IdPeriodoSinalizado != null)
            {
                var periodoAvaliacao = new PeriodoService().ObterPeriodo((int)alocacao.IdPeriodoSinalizado);
                returnHierarquiaAssociado.IdPeriodoSinalizado = periodoAvaliacao.IdPeriodo;
                returnHierarquiaAssociado.PeriodoSinalizado = periodoAvaliacao.Codigo;
                returnHierarquiaAssociado.IdPeriodoAvaliacao = periodoAvaliacao.IdPeriodo;
                returnHierarquiaAssociado.PeriodoAvaliacao = periodoAvaliacao.Codigo;
            }
            else
            {
                var periodoAvaliacao = new PeriodoService().VerificaExistenciaPeriodo(alocacao.DataInicio, (DateTime)alocacao.DataFim, 1);
                returnHierarquiaAssociado.IdPeriodoAvaliacao = periodoAvaliacao.IdPeriodo;
                returnHierarquiaAssociado.PeriodoAvaliacao = periodoAvaliacao.Codigo;
            }

            var getAvaliacoes = new AvaliacoesService().ObterAvaliacaoHierarquia(associado.IdAssociado, IdProjeto, ultimoPerido.IdPeriodo);
            returnHierarquiaAssociado.HabilitaExcluir = getAvaliacoes.Count == 0;
            returnHierarquiaAssociado.TextoExcluir = returnHierarquiaAssociado.HabilitaExcluir ? "Excluir" : "Não é possível excluir uma alocação cujo associado já recebeu uma avaliação no ciclo";
            returnHierarquiaAssociado.HabilitaLimpar = getAvaliacoes.Count == 0;
            returnHierarquiaAssociado.TextoLimpar = returnHierarquiaAssociado.HabilitaLimpar ? "Alterar" : "Não é possível alterar uma alocação cujo associado já recebeu uma avaliação no ciclo";

            var meusAvaliados = getProjetosAssociados.Where(x => x.IdAvaliador == associado.IdAssociado).ToList();
            returnHierarquiaAssociado.LeftBorder = isLast ? "" : "border-color:black;border-left:2px solid";
            returnHierarquiaAssociado.LeftBorderLast = !isLast ? "" : "border-color:black;border-left:2px solid";

            if (meusAvaliados.Count > 0)
            {
                returnHierarquiaAssociado.HabilitaExcluir = false;
                returnHierarquiaAssociado.TextoExcluir = "Não é possível excluir uma alocação de um avaliador";

            }

            // VERIFICAR CARGO DO ASSOCIADO PARA HABILITAR O BOTÃO DE ADICIONAR LIDERADOS
            if (associado.CARGOS.Funcao != null && (associado.CARGOS.Funcao.ToLower().Contains("executar") || associado.CARGOS.Funcao.ToLower().Contains("aprender")))
            {
                returnHierarquiaAssociado.HabilitarAdicionarLiderado = false;
                returnHierarquiaAssociado.TextoAdicionarLiderado = "O cargo do associado não possui nível suficiente para ser um avaliador";
            }
            else
            {
                returnHierarquiaAssociado.HabilitarAdicionarLiderado = true;
                returnHierarquiaAssociado.TextoAdicionarLiderado = "Adicionar liderado";
            }

            foreach (var item in meusAvaliados)
            {
                returnHierarquiaAssociado.Avaliados.Add(getHieraquiaAssociado(getProjetosAssociados, item.IdAssociado, MarginLeft + 20,
                    meusAvaliados.IndexOf(item) == meusAvaliados.Count - 1, IdProjeto, associado.IdAssociado, item));
            }

            // ADICIONA DUMMY DE ADICIONAR LIDERADO
            if (txtLiderDummy.Text == associado.IdAssociado.ToString())
            {
                txtLiderDummy.Text = "";
                returnHierarquiaAssociado.NovoAvaliado = new List<int>();
                returnHierarquiaAssociado.NovoAvaliado.Add(1);
            }

            if (returnHierarquiaAssociado.IdAssociado == dummyValidacao.IdAssociado)
            {
                returnHierarquiaAssociado.ExibeAcoes = "hidden";
                returnHierarquiaAssociado.ExibeNome = false;
                returnHierarquiaAssociado.ExibeSelecionar = "";

                var todosAssociados = new AssociadosService().ObterAssociados();
                todosAssociados.Insert(0, new ASSOCIADOS { IdAssociado = 0, Nome = "[Selecionar]" });
                returnHierarquiaAssociado.AssociadosAlterar = todosAssociados;
            }
            else
            {
                returnHierarquiaAssociado.ExibeAcoes = "";
                returnHierarquiaAssociado.ExibeNome = true;
                returnHierarquiaAssociado.ExibeSelecionar = "hidden";
            }

            var existeDummy = getProjetosAssociados.Where(x => x.IdAssociado == dummyValidacao.IdAssociado).ToList();
            if (existeDummy != null && existeDummy.Count > 0)
            {
                returnHierarquiaAssociado.HabilitaLimpar = false;
                returnHierarquiaAssociado.TextoLimpar = "Finalize a alteração de associado em andamento.";
            }

            return returnHierarquiaAssociado;
        }
        protected void mainRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HierarquiaAssociadoModel currentItem = (HierarquiaAssociadoModel)e.Item.DataItem;
                PeriodoService periodoService = new PeriodoService();
                var ultimoPeriodo = periodoService.ObterPeriodoUltimo();

                if (currentItem.Avaliados.Count > 0)
                {
                    Repeater ChildRepeater = (Repeater)e.Item.FindControl("ChildRepeater");

                    ChildRepeater.DataSource = currentItem.Avaliados;
                    ChildRepeater.ItemTemplate = parentRepeater.ItemTemplate;
                    ChildRepeater.Visible = true;
                    ChildRepeater.DataBind();
                }

                if (currentItem.NovoAvaliado.Count > 0)
                {
                    Repeater ChildRepeater = (Repeater)e.Item.FindControl("NovoAvaliadoRepeater");

                    ChildRepeater.DataSource = currentItem.NovoAvaliado;
                    ChildRepeater.ItemTemplate = repeaterNovoAvaliado.ItemTemplate;
                    ChildRepeater.Visible = true;
                    ChildRepeater.DataBind();
                }

                DropDownList ddlCiclos = (DropDownList)e.Item.FindControl("ddlCicloAvaliacao");
                if (ddlCiclos != null)
                {
                    ddlCiclos.Items.Add(periodoService.ObterPeriodoUltimo().Codigo);
                    ddlCiclos.Items.Add("Próximo");
                    if (currentItem.IdPeriodoAvaliacao == -1)
                    {
                        ddlCiclos.SelectedIndex = 1;
                    }
                    else
                    {
                        ddlCiclos.SelectedIndex = 0;
                    }
                }
            }
        }

        protected void parentRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
        }
        protected void txtDummy_TextChanged(object sender, EventArgs e)
        {
            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }

        protected void repeaterNovoAvaliado_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddlNovoAvaliado = (DropDownList)e.Item.FindControl("ddlNovoAvaliado");
                var todosAssociados = new AssociadosService().ObterAssociados();
                ddlNovoAvaliado.DataValueField = "IdAssociado";
                ddlNovoAvaliado.DataTextField = "Nome";
                ddlNovoAvaliado.DataSource = todosAssociados;
                ddlNovoAvaliado.DataBind();
                ddlNovoAvaliado.Items.Insert(0, "[Selecionar]");
            }
        }

        protected void btnFinalizaAdicaoDummy_Click(object sender, EventArgs e)
        {
            if (txtAddLideradoDummy.Text == "")
            {
                MessageBox.Show("Selecione um associado", "ALERTA", TIPO.Error, MessageBoxHandler2);
                atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                return;
            }
            int addAssociadoId = int.Parse(txtAddLideradoDummy.Text);
            if (addAssociadoId <= 0)
            {
                MessageBox.Show("Selecione um associado", "ALERTA", TIPO.Error, MessageBoxHandler2);
                atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                return;
            }
            if (txtDummy.Text == "")
            {
                MessageBox.Show("Nenhum projeto selecionado", "ALERTA", TIPO.Error, MessageBoxHandler2);
                atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                return;
            }
            if (txtLiderDummy.Text == "")
            {
                MessageBox.Show("Nenhum líder selecionado", "ALERTA", TIPO.Error, MessageBoxHandler2);
                atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                return;
            }

            var projetosService = new ProjetosService();
            int liderId = int.Parse(txtLiderDummy.Text);
            int projetoId = int.Parse(txtDummy.Text);
            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            var existeAlocacaoDesempenho = projetosService.ObterProjetoAssociadoHierarquia(addAssociadoId, projetoId, liderId, ultimoPerido.IdPeriodo, "desempenho");
            var existeAlocacaoLideranca = projetosService.ObterProjetoAssociadoHierarquia(liderId, projetoId, addAssociadoId, ultimoPerido.IdPeriodo, "desempenho");

            if (existeAlocacaoDesempenho != null || existeAlocacaoLideranca != null)
            {
                MessageBox.Show("Já existe uma alocação deste associado a este líder", "ALERTA", TIPO.Error, MessageBoxHandler2);
                atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                return;
            }

            var getProjetosAssociados = new ProjetosService().ObterListaAssociadosNoPeriodo(projetoId, -1, ultimoPerido);
            var existeAssociado = getProjetosAssociados.Where(x => x.IdAssociado == addAssociadoId || x.IdAvaliador == addAssociadoId).ToList();

            if (existeAssociado != null && existeAssociado.Count > 0)
            {
                MessageBox.Show("O associado selecionado já existe na hierarquia de avaliações do projeto", "ALERTA", TIPO.Error, MessageBoxHandler2);
                atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                return;
            }

            // SE TUDO OKAY, ADICIONA ALOCAÇÃO COMO DESEMPENHO E LIDERANCA
            var addAlocacaoDesempenho = new PROJETOSASSOCIADOS();
            addAlocacaoDesempenho.IdProjeto = projetoId;
            addAlocacaoDesempenho.IdAssociado = addAssociadoId;
            addAlocacaoDesempenho.IdGestor = WebStorage.GetUsuarioLogado().Id;
            addAlocacaoDesempenho.DataInicio = (DateTime)ultimoPerido.DataInicio;
            addAlocacaoDesempenho.DataFim = ultimoPerido.DataFim;
            addAlocacaoDesempenho.Comentario = "Adicionado pelo gestor";
            addAlocacaoDesempenho.USR = WebStorage.GetUsuarioLogado().Id;
            addAlocacaoDesempenho.DHC = DateTime.Now;
            addAlocacaoDesempenho.ATV = 1;
            addAlocacaoDesempenho.IdAvaliador = liderId;
            addAlocacaoDesempenho.TipoAvaliacao = "desempenho";
            addAlocacaoDesempenho.Escopo = "projeto";
            var addAlocacaoLideranca = new PROJETOSASSOCIADOS();
            addAlocacaoLideranca.IdProjeto = projetoId;
            addAlocacaoLideranca.IdAssociado = liderId;
            addAlocacaoLideranca.IdGestor = addAssociadoId;
            addAlocacaoLideranca.DataInicio = (DateTime)ultimoPerido.DataInicio;
            addAlocacaoLideranca.DataFim = ultimoPerido.DataFim;
            addAlocacaoLideranca.Comentario = "Adicionado pelo gestor";
            addAlocacaoLideranca.USR = WebStorage.GetUsuarioLogado().Id;
            addAlocacaoLideranca.DHC = DateTime.Now;
            addAlocacaoLideranca.ATV = 1;
            addAlocacaoLideranca.IdAvaliador = 275; //NÃO SE APLICA
            addAlocacaoLideranca.TipoAvaliacao = "lideranca";
            addAlocacaoLideranca.Escopo = "projeto";

            projetosService.InserirProjetoAssociado(addAlocacaoDesempenho);
            projetosService.InserirProjetoAssociado(addAlocacaoLideranca);

            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }

        protected void btnExcluirLideradoDummy_Click(object sender, EventArgs e)
        {
            if (txtExcluiLideradoDummy.Text == "")
            {
                MessageBox.Show("Selecione um associado", "ALERTA", TIPO.Error, MessageBoxHandler2); atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false); return;
            }

            int excluiAssociadoId = int.Parse(txtExcluiLideradoDummy.Text);
            var projetosService = new ProjetosService();
            int liderId = int.Parse(txtExcluiAvaliadorDummy.Text);
            int projetoId = int.Parse(txtDummy.Text);
            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            var existeAlocacaoDesempenho = projetosService.ObterProjetoAssociadoHierarquia(excluiAssociadoId, projetoId, liderId, ultimoPerido.IdPeriodo, "desempenho");
            var existeAlocacaoLideranca = projetosService.ObterProjetoAssociadoHierarquia(liderId, projetoId, excluiAssociadoId, ultimoPerido.IdPeriodo, "lideranca");

            if (existeAlocacaoDesempenho != null)
            {
                projetosService.ExcluirProjetoAssociado(existeAlocacaoDesempenho);
            }
            if (existeAlocacaoLideranca != null)
            {
                projetosService.ExcluirProjetoAssociado(existeAlocacaoLideranca);
            }

            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }

        protected void btn_Alocacao_AlteraInicio_Click(object sender, EventArgs e)
        {
            var IdAssociado = int.Parse(txt_Alocacao_IdAssociado.Text);
            var IdAvaliador = int.Parse(txt_Alocacao_Avaliador.Text);
            var novaData = txt_Alocacao_Data.Text;
            var dataConvertida = DateTime.Parse(novaData);
            var dataTarget = txt_Alocacao_Target.Text;
            int projetoId = int.Parse(txtDummy.Text);
            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();

            var projetosService = new ProjetosService();
            var existeAlocacaoDesempenho = projetosService.ObterProjetoAssociadoHierarquia(IdAssociado, projetoId, IdAvaliador, ultimoPerido.IdPeriodo, "desempenho");
            var existeAlocacaoLideranca = projetosService.ObterProjetoAssociadoHierarquia(IdAvaliador, projetoId, IdAssociado, ultimoPerido.IdPeriodo, "lideranca");

            if (dataTarget == "inicio")
            {
                int? tempSinalizador = null;
                if (dataConvertida < ultimoPerido.DataInicio)
                {
                    tempSinalizador = ultimoPerido.IdPeriodo;
                }

                if (existeAlocacaoDesempenho != null)
                {
                    if (dataConvertida >= existeAlocacaoDesempenho.DataFim)
                    {
                        MessageBox.Show("A data de início da alocação não pode ser maior ou igual a de término da alocação.", "ALERTA", TIPO.Error, MessageBoxHandler2);
                        atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                        return;
                    }

                    existeAlocacaoDesempenho.DataInicio = dataConvertida;
                    existeAlocacaoDesempenho.IdPeriodoSinalizado = existeAlocacaoDesempenho.IdPeriodoSinalizado == null ? tempSinalizador : existeAlocacaoDesempenho.IdPeriodoSinalizado;
                    projetosService.AlterarProjetoAssociado(existeAlocacaoDesempenho);
                }

                if (existeAlocacaoLideranca != null)
                {
                    if (dataConvertida >= existeAlocacaoLideranca.DataFim)
                    {
                        MessageBox.Show("A data de início da alocação não pode ser maior ou igual a de término da alocação.", "ALERTA", TIPO.Error, MessageBoxHandler2);
                        atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false); return;
                    }

                    existeAlocacaoLideranca.DataInicio = dataConvertida;
                    existeAlocacaoLideranca.IdPeriodoSinalizado = existeAlocacaoLideranca.IdPeriodoSinalizado == null ? tempSinalizador : existeAlocacaoLideranca.IdPeriodoSinalizado;
                    projetosService.AlterarProjetoAssociado(existeAlocacaoLideranca);
                }
            }
            else if (dataTarget == "termino")
            {
                int? tempSinalizador = null;
                if (dataConvertida > ultimoPerido.DataFim)
                {
                    tempSinalizador = ultimoPerido.IdPeriodo;
                }

                if (existeAlocacaoDesempenho != null)
                {
                    if (dataConvertida <= existeAlocacaoDesempenho.DataInicio)
                    {
                        MessageBox.Show("A data de término da alocação não pode ser menor ou igual a de início da alocação.", "ALERTA", TIPO.Error, MessageBoxHandler2);
                        atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false); return;
                    }

                    existeAlocacaoDesempenho.DataFim = dataConvertida;
                    existeAlocacaoDesempenho.IdPeriodoSinalizado = existeAlocacaoDesempenho.IdPeriodoSinalizado == null ? tempSinalizador : existeAlocacaoDesempenho.IdPeriodoSinalizado;
                    projetosService.AlterarProjetoAssociado(existeAlocacaoDesempenho);
                }

                if (existeAlocacaoLideranca != null)
                {
                    if (dataConvertida <= existeAlocacaoLideranca.DataInicio)
                    {
                        MessageBox.Show("A data de término da alocação não pode ser maior ou igual a de início da alocação.", "ALERTA", TIPO.Error, MessageBoxHandler2);
                        atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false); return;
                    }

                    existeAlocacaoLideranca.DataFim = dataConvertida;
                    existeAlocacaoLideranca.IdPeriodoSinalizado = existeAlocacaoLideranca.IdPeriodoSinalizado == null ? tempSinalizador : existeAlocacaoLideranca.IdPeriodoSinalizado;
                    projetosService.AlterarProjetoAssociado(existeAlocacaoLideranca);
                }
            }

            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }

        protected void btn_Alocacao_AlteraSinalizador_Click(object sender, EventArgs e)
        {
            var IdAssociado = int.Parse(txt_Alocacao_IdAssociado.Text);
            var IdAvaliador = int.Parse(txt_Alocacao_Avaliador.Text);
            int projetoId = int.Parse(txtDummy.Text);
            string sinalizadorText = txt_Alocacao_Sinalizador.Text;

            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            var projetosService = new ProjetosService();
            var existeAlocacaoDesempenho = projetosService.ObterProjetoAssociadoHierarquia(IdAssociado, projetoId, IdAvaliador, ultimoPerido.IdPeriodo, "desempenho");
            var existeAlocacaoLideranca = projetosService.ObterProjetoAssociadoHierarquia(IdAvaliador, projetoId, IdAssociado, ultimoPerido.IdPeriodo, "lideranca");

            int? newIdSinalizador = null;
            if (sinalizadorText == "Próximo")
            {
                newIdSinalizador = -1;
            }
            else
            {
                var getPeriodoSinalizado = new PeriodoService().ObterPeriodoCodigo(sinalizadorText);
                if (getPeriodoSinalizado != null)
                {
                    newIdSinalizador = getPeriodoSinalizado.IdPeriodo;
                }
            }

            if (existeAlocacaoDesempenho != null)
            {
                existeAlocacaoDesempenho.IdPeriodoSinalizado = newIdSinalizador;
                projetosService.AlterarProjetoAssociado(existeAlocacaoDesempenho);
            }
            if (existeAlocacaoLideranca != null)
            {
                existeAlocacaoLideranca.IdPeriodoSinalizado = newIdSinalizador;
                projetosService.AlterarProjetoAssociado(existeAlocacaoLideranca);
            }

            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }

        protected void btn_LimpaAssociado_Limpar_Click(object sender, EventArgs e)
        {
            var IdAssociado = int.Parse(txt_LimpaAssociado_IdAssociado.Text);
            var IdProjeto = int.Parse(txtDummy.Text);
            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            var dummyValidacao = new AssociadosService().ObterAssociadoPeloNome("Validação Gestor");
            var projetosService = new ProjetosService();

            var getProjetosAssociados = new ProjetosService().ObterListaAssociadosNoPeriodo(IdProjeto, -1, ultimoPerido);

            var alocacoesLider = getProjetosAssociados.Where(x => x.IdAvaliador == IdAssociado && x.TipoAvaliacao == "desempenho").ToList();
            foreach (var item in alocacoesLider)
            {
                item.IdAvaliador = dummyValidacao.IdAssociado;
                projetosService.AlterarProjetoAssociado(item);
            }
            var alocacoesAssociado = getProjetosAssociados.Where(x => x.IdAssociado == IdAssociado).ToList();
            foreach (var item in alocacoesAssociado)
            {
                item.IdAssociado = dummyValidacao.IdAssociado;
                projetosService.AlterarProjetoAssociado(item);
            }
            var alocacoesLiderado = getProjetosAssociados.Where(x => x.IdGestor == IdAssociado && x.TipoAvaliacao == "lideranca").ToList();
            foreach (var item in alocacoesLiderado)
            {
                item.IdGestor = dummyValidacao.IdAssociado;
                projetosService.AlterarProjetoAssociado(item);
            }

            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }

        protected void btn_AlteraAssociado_Finaliza_Click(object sender, EventArgs e)
        {
            var IdAssociado = int.Parse(txt_LimpaAssociado_IdAssociado.Text);
            var IdProjeto = int.Parse(txtDummy.Text);
            var ultimoPerido = new PeriodoService().ObterPeriodoUltimo();
            var dummyValidacao = new AssociadosService().ObterAssociadoPeloNome("Validação Gestor");
            var projetosService = new ProjetosService();

            var getProjetosAssociados = new ProjetosService().ObterListaAssociadosNoPeriodo(IdProjeto, -1, ultimoPerido);
            var existeAssociado = getProjetosAssociados.Where(x => x.IdAssociado == IdAssociado || x.IdAvaliador == IdAssociado).ToList();

            if (existeAssociado != null && existeAssociado.Count > 0)
            {
                MessageBox.Show("O associado selecionado já existe na hierarquia de avaliações do projeto.", "ALERTA", TIPO.Error, MessageBoxHandler2);
                atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                return;
            }

            var alocacoesLider = getProjetosAssociados.Where(x => x.IdAvaliador == dummyValidacao.IdAssociado && x.TipoAvaliacao == "desempenho").ToList();
            foreach (var item in alocacoesLider)
            {
                item.IdAvaliador = IdAssociado;
                projetosService.AlterarProjetoAssociado(item);
            }
            var alocacoesAssociado = getProjetosAssociados.Where(x => x.IdAssociado == dummyValidacao.IdAssociado).ToList();
            foreach (var item in alocacoesAssociado)
            {
                item.IdAssociado = IdAssociado;
                projetosService.AlterarProjetoAssociado(item);
            }
            var alocacoesLiderado = getProjetosAssociados.Where(x => x.IdGestor == dummyValidacao.IdAssociado && x.TipoAvaliacao == "lideranca").ToList();
            foreach (var item in alocacoesLiderado)
            {
                item.IdGestor = IdAssociado;
                projetosService.AlterarProjetoAssociado(item);
            }

            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }
        protected void btn_Drag_Finaliza_Click(object sender, EventArgs e)
        {
            var IdProjeto = int.Parse(txtDummy.Text);
            var IdDraged = int.Parse(txt_Drag_IdAssociadoDraged.Text);
            var IdTarget = int.Parse(txt_Drag_IdAssociadoTarget.Text);
            if (IdDraged == IdTarget) { atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false); return; }

            var ultimoPeriodo = new PeriodoService().ObterPeriodoUltimo();
            var associadosService = new AssociadosService();
            var projetosService = new ProjetosService();
            var associadoDraged = associadosService.ObterAssociado(IdDraged);
            var associadoTarget = associadosService.ObterAssociado(IdTarget);
            var dummyValidacao = associadosService.ObterAssociadoPeloNome("Validação Gestor");

            var getProjetosAssociados = new ProjetosService().ObterListaAssociadosNoPeriodo(IdProjeto, -1, ultimoPeriodo);

            var alocacoesDraged = getProjetosAssociados.Where(x => x.IdAssociado == IdDraged).ToList();
            var alocacoesAvaliadorDraged = getProjetosAssociados.Where(x => x.IdAvaliador == IdDraged).ToList();
            var alocacoesLideradoDraged = getProjetosAssociados.Where(x => x.IdGestor == IdDraged && x.TipoAvaliacao == "lideranca").ToList();
            var alocacoesTarget = getProjetosAssociados.Where(x => x.IdAssociado == IdTarget).ToList();
            var alocacoesAvaliadorTarget = getProjetosAssociados.Where(x => x.IdAvaliador == IdTarget).ToList();
            var alocacoesLideradoTarget = getProjetosAssociados.Where(x => x.IdGestor == IdTarget && x.TipoAvaliacao == "lideranca").ToList();

            if (alocacoesAvaliadorDraged != null && alocacoesAvaliadorDraged.Count > 0)
            {
                if (associadoTarget.CARGOS.Funcao != null && (associadoTarget.CARGOS.Funcao.ToLower().Contains("executar") || associadoTarget.CARGOS.Funcao.ToLower().Contains("aprender")))
                {
                    MessageBox.Show("O cargo de " + associadoTarget.Nome + " não é suficiente para receber liderados.", "ALERTA", TIPO.Error, MessageBoxHandler2);
                    atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                    return;
                }
            }
            if (alocacoesAvaliadorTarget != null && alocacoesAvaliadorTarget.Count > 0)
            {
                if (associadoDraged.CARGOS.Funcao != null && (associadoDraged.CARGOS.Funcao.ToLower().Contains("executar") || associadoDraged.CARGOS.Funcao.ToLower().Contains("aprender")))
                {
                    MessageBox.Show("O cargo de " + associadoDraged.Nome + " não é suficiente para receber liderados.", "ALERTA", TIPO.Error, MessageBoxHandler2);
                    atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
                    return;
                }
            }

            foreach (var item in alocacoesDraged)
            {
                item.IdAssociado = IdTarget;
                projetosService.AlterarProjetoAssociado(item);
            }
            foreach (var item in alocacoesAvaliadorDraged)
            {
                item.IdAvaliador = IdTarget;
                projetosService.AlterarProjetoAssociado(item);
            }
            foreach (var item in alocacoesLideradoDraged)
            {
                item.IdGestor = IdTarget;
                projetosService.AlterarProjetoAssociado(item);
            }
            foreach (var item in alocacoesTarget)
            {
                item.IdAssociado = IdDraged;
                projetosService.AlterarProjetoAssociado(item);
            }
            foreach (var item in alocacoesAvaliadorTarget)
            {
                item.IdAvaliador = IdDraged;
                projetosService.AlterarProjetoAssociado(item);
            }
            foreach (var item in alocacoesLideradoTarget)
            {
                item.IdGestor = IdDraged;
                projetosService.AlterarProjetoAssociado(item);
            }

            if (IdTarget == dummyValidacao.IdAssociado || IdDraged == dummyValidacao.IdAssociado)
            {
                var getResultProjetosAssociados = new ProjetosService().ObterListaAssociadosNoPeriodo(IdProjeto, -1, ultimoPeriodo);
                var alocacoesDummy = getResultProjetosAssociados.Where(x => x.IdAssociado == dummyValidacao.IdAssociado || x.IdAvaliador == dummyValidacao.IdAssociado || x.IdGestor == dummyValidacao.IdAssociado);
                foreach (var item in alocacoesDummy)
                {
                    projetosService.ExcluirProjetoAssociado(item);
                }
            }

            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }

        protected void btn_Drag_Novo_Click(object sender, EventArgs e)
        {
            var IdProjeto = int.Parse(txtDummy.Text);
            var IdDraged = int.Parse(txt_Drag_IdAssociadoDraged.Text);
            var IdTarget = int.Parse(txt_Drag_IdAssociadoTarget.Text);
            if (IdDraged == IdTarget) { atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false); return; }

            var ultimoPeriodo = new PeriodoService().ObterPeriodoUltimo();
            var associadosService = new AssociadosService();
            var projetosService = new ProjetosService();
            var associadoDraged = associadosService.ObterAssociado(IdDraged);
            var associadoTarget = associadosService.ObterAssociado(IdTarget);

            var getProjetosAssociados = new ProjetosService().ObterListaAssociadosNoPeriodo(IdProjeto, -1, ultimoPeriodo);

            var alocacoesDraged = getProjetosAssociados.Where(x => x.IdAssociado == IdDraged).ToList();
            var alocacoesAvaliadorDraged = getProjetosAssociados.Where(x => x.IdAvaliador == IdDraged).ToList();
            var alocacoesLideradoDraged = getProjetosAssociados.Where(x => x.IdGestor == IdDraged && x.TipoAvaliacao == "lideranca").ToList();
            var alocacoesTarget = getProjetosAssociados.Where(x => x.IdAssociado == IdTarget).ToList();
            var alocacoesAvaliadorTarget = getProjetosAssociados.Where(x => x.IdAvaliador == IdTarget).ToList();
            var alocacoesLideradoTarget = getProjetosAssociados.Where(x => x.IdGestor == IdTarget && x.TipoAvaliacao == "lideranca").ToList();

            foreach (var item in alocacoesDraged)
            {
                if (item.TipoAvaliacao == "desempenho")
                {
                    item.IdAvaliador = IdTarget;
                    projetosService.AlterarProjetoAssociado(item);
                }
            }
            foreach (var item in alocacoesAvaliadorDraged)
            {
            }
            foreach (var item in alocacoesLideradoDraged)
            {
                item.IdAssociado = IdTarget;
                projetosService.AlterarProjetoAssociado(item);
            }

            atualizaBlocoHierarquia(int.Parse(txtDummy.Text), false);
        }
        protected void atualizaFatoresProjeto(int IdProjeto)
        {
            var complexidadesService = new ComplexidadesService();
            var fatoresProjeto = complexidadesService.ObterFatoresProjetos(idProjeto: IdProjeto);

            // CRIA FATORESPROJETOS PARA O PROJETO CASO NÃO HOUVER
            if (fatoresProjeto == null || fatoresProjeto.Count < 1)
            {
                var fatores = complexidadesService.ObterFatores();
                var notasFatores = complexidadesService.ObterNotasFatores();

                foreach (var fator in fatores)
                {
                    FATORESPROJETOS addFatorProjeto = new FATORESPROJETOS();
                    addFatorProjeto.idProjeto = IdProjeto;
                    addFatorProjeto.idFator = fator.idFator;
                    addFatorProjeto.idNotaFator = notasFatores[notasFatores.Count / 2].idNotaFator;
                    addFatorProjeto.DHC = DateTime.Now;
                    addFatorProjeto.USR = WebStorage.GetUsuarioLogado().Id;

                    complexidadesService.GerirFatorProjeto(addFatorProjeto);
                }

                fatoresProjeto = complexidadesService.ObterFatoresProjetos(idProjeto: IdProjeto);
            }

            var projeto = new ProjetosService().ObterProjeto(IdProjeto);
            var responsavelId = projeto.IdAssociadoResponsavel;
            var logadoId = WebStorage.GetUsuarioLogado().Id;

            if (fatoresProjeto != null)
            {
                List<FatorProjetoModel> listaFatoresProjeto = new List<FatorProjetoModel>();
                listaFatoresProjeto = (from fp in fatoresProjeto
                                       select new FatorProjetoModel
                                       {
                                           IdFatorProjeto = fp.idFatorProjeto,
                                           IdFator = fp.idFator,
                                           Fator = fp.FATORES.Titulo,
                                           FatorDescBaixa = fp.FATORES.DescricaoBaixa.Replace(Environment.NewLine, "<br>").Replace(char.ConvertFromUtf32(10), "<br>"),
                                           FatorDescMedia = fp.FATORES.DescricaoMedia.Replace(Environment.NewLine, "<br>").Replace(char.ConvertFromUtf32(10), "<br>"),
                                           FatorDescAlta = fp.FATORES.DescricaoAlta.Replace(Environment.NewLine, "<br>").Replace(char.ConvertFromUtf32(10), "<br>"),
                                           IdProjeto = fp.idProjeto,
                                           Projeto = fp.PROJETOS.Projeto,
                                           IdNotaFator = (int)fp.idNotaFator,
                                           NotaFator = fp.NOTASFATORES.DescricaoNota,
                                           ValorSlider = fp.NOTASFATORES.idNotaFator,
                                           ValidadoGestor = fp.ValidGestor != null ? (bool)fp.ValidGestor : false,
                                           DHCValidadoGestor = fp.ValidGestor != null ? fp.DHCValidGestor : null,
                                           ValidadoResponsavel = fp.ValidResponsavel != null ? (bool)fp.ValidResponsavel : false,
                                           DHCValidadoResponsavel = fp.ValidResponsavel != null ? fp.DHCValidResponsavel : null,
                                           ValidadorPessoa = logadoId == responsavelId ? "MD" : "gestor"
                                       }).ToList();

                foreach (var item in listaFatoresProjeto)
                {
                    if (item.DHCValidadoGestor != null || item.DHCValidadoResponsavel != null)
                    {
                        if (item.DHCValidadoResponsavel == null || item.DHCValidadoGestor >= item.DHCValidadoResponsavel)
                        {
                            item.ValidadoTexto = "Atualizado pelo gestor em: " + ((DateTime)item.DHCValidadoGestor).ToString("dd/MM/yyyy HH:mm");
                        }
                        else
                        {
                            item.ValidadoTexto = "Atualizado pelo MD em: " + ((DateTime)item.DHCValidadoResponsavel).ToString("dd/MM/yyyy HH:mm");
                        }
                    }
                }

                rptFatoresProjeto.DataSource = listaFatoresProjeto;
                rptFatoresProjeto.DataBind();
            }
        }

        protected void btn_Fatores_AtualizaFatoreProjeto_Click(object sender, EventArgs e)
        {
            var fatorProjeto = txt_Fatores_IdFatorProjeto.Text;
            var textoValorSlider = txt_Fatores_ValorSlider.Text;
            int IdFatorProjeto = int.Parse(fatorProjeto);
            int valorSlider = int.Parse(textoValorSlider);
            int IdProjeto = int.Parse(WebStorage.Get("IdProjeto", "0"));

            var complexidadesService = new ComplexidadesService();
            var notasFatores = complexidadesService.ObterNotasFatores();
            var novaNota = notasFatores[valorSlider - 1].idNotaFator;

            var updateFatorProjeto = complexidadesService.ObterFatoresProjetos(idFatorProjeto: IdFatorProjeto)[0];
            updateFatorProjeto.idNotaFator = novaNota;
            complexidadesService.GerirFatorProjeto(updateFatorProjeto);

            calculaComplexidade(IdProjeto);
        }

        protected void calculaComplexidade(int idProjeto)
        {
            var complexidadesService = new ComplexidadesService();
            var fatoresProjeto = complexidadesService.ObterFatoresProjetos(idProjeto: idProjeto);

            decimal? somaTotal = 0;
            foreach (var fatorProjeto in fatoresProjeto)
            {
                decimal? valorNota = decimal.Parse(fatorProjeto.NOTASFATORES.Valor.ToString());
                decimal? pesoPercent = decimal.Parse(fatorProjeto.FATORES.Peso.ToString()) / 100;
                decimal? somaNota = valorNota * pesoPercent;
                somaTotal += somaNota;
            }

            var complexidades = complexidadesService.ObterListaComplexidades();
            var complexidadeFinal = complexidades.OrderByDescending(x => x.SomaMinimaFator).FirstOrDefault(x => somaTotal >= x.SomaMinimaFator);

            var projetosService = new ProjetosService();
            var projeto = projetosService.ObterProjeto(idProjeto);
            projeto.IdComplexidade = complexidadeFinal.IdComplexidade;
            projetosService.AlterarProjeto(projeto);
            ddlComplexidade.Text = complexidadeFinal.Complexidade;
        }

        protected void btn_AtualizaValid_Item_Click(object sender, EventArgs e)
        {
            var complexidadeService = new ComplexidadesService();

            var stringIdItem = txt_Fatores_IdFatorProjeto.Text;
            var stringValidValue = txt_Fatores_ValorSlider.Text;
            var idItem = int.Parse(stringIdItem);
            var idValidValue = bool.Parse(stringValidValue);

            var atualizaItem = complexidadeService.ObterFatoresProjetos(idFatorProjeto: idItem)[0];
            atualizaItem.ValidGestor = idValidValue;
            atualizaItem.DHCValidGestor = DateTime.Now;

            complexidadeService.GerirFatorProjeto(atualizaItem);

        }
    }
}