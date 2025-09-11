using Business.DataAccess;
using Business.Model;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class periodoavaliacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LimparCampos();
            }
        }

        private void LimparCampos()
        {
            MontaComboEmpresa();
            MontaListPeriodos();
            MontaListaAvaliacoesSinalizadas();
            hdIdPeriodo.Value = string.Empty;
            txtDataInicio.Text = "";
            txtDataTermino.Text = "";
            txtPeriodo.Text = "";
        }

        private void MontaListPeriodos()
        {
            int idempresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
            var periodos = new PeriodoService().ListaTodosPeriodos(idempresa);

            if (periodos != null)
            {
                this.rptPeriodo.DataSource = null;
                this.rptPeriodo.DataSource = periodos;
                this.rptPeriodo.DataBind();

            }
        }
        private void MontaListaAvaliacoesSinalizadas()
        {
            var projetosService = new ProjetosService();
            var associadosService = new AssociadosService();
            var avaliacoesSinalizadas = projetosService.ObterAvaliacoesSinalizadas();
            avaliacoesSinalizadas = avaliacoesSinalizadas.Where(av => av.TipoAvaliacao == "desempenho").ToList();
            List<ProjetoModel> listaAvaliacoesSinalizadas =
                    (from av in avaliacoesSinalizadas
                     select new ProjetoModel
                     {
                         Projeto = projetosService.ObterProjeto(av.IdProjeto).Projeto,
                         Respondente = associadosService.ObterAssociado(av.IdAssociado).Nome,
                         Avaliador = associadosService.ObterAssociado((int)av.IdAvaliador).Nome,
                         DataInicio = av.DataInicio.ToString("dd/MM/yyyy"),
                         DataTermino = ((DateTime)av.DataFim).ToString("dd/MM/yyyy"),
                     }).ToList();
                

            if (listaAvaliacoesSinalizadas != null)
            {
                this.rptAvaliacoesSinalizadas.DataSource = listaAvaliacoesSinalizadas;
                this.rptAvaliacoesSinalizadas.DataBind();
            }
        }

        public string FormatStatus(int atv)
        {
            if (atv == 1)
            {
                return "Ativo";
            }
            else
            {
                return "Inativo";
            }
        }

        private void MontaComboEmpresa()
        {
            List<EMPRESAS> empresas = new EmpresasService().ObterListaEmpresas(true);

            this.ddlEmpresa.DataSource = empresas;
            this.ddlEmpresa.DataValueField = "IdEmpresa";
            this.ddlEmpresa.DataTextField = "Empresa";
            this.ddlEmpresa.DataBind();
            this.ddlEmpresa.Items.Insert(0, "[Selecionar]");
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {

                PERIODOSAVALIACOES periodo = new PERIODOSAVALIACOES();
                periodo.DHC = DateTime.Now;
                periodo.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                periodo.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());

                if (ddlStatus.SelectedIndex == 0)
                {
                    periodo.ATV = 1;
                }
                else
                {
                    periodo.ATV = 0;
                }

                if (ValidaForm())
                {
                    DateTime datainicio = Convert.ToDateTime(txtDataInicio.Text);

                    periodo.DataInicio = datainicio;
                    periodo.DataFim = Convert.ToDateTime(txtDataTermino.Text);

                 
                    periodo.Codigo = txtPeriodo.Text;
                    periodo.Periodo = txtPeriodo.Text;

                    if (string.IsNullOrEmpty(hdIdPeriodo.Value))
                    {
                        new PeriodoService().InserirPeriodo(periodo);
                        AtualizaAvaliacoesSinalizdas();
                        MessageBox.Show("Período Inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                        var addWorkflow = new WORKFLOW();
                        addWorkflow.IdEmpresa = 1;
                        addWorkflow.IdPeriodo = periodo.IdPeriodo;
                        addWorkflow.DataInicio = (DateTime)periodo.DataInicio;
                        addWorkflow.DiasAutoAvaliacao = 6;
                        addWorkflow.DiasAvaliacaoCegas = 11;
                        addWorkflow.DiasAvaliacaoGestor = 2;
                        addWorkflow.DiasFeedback = 3;
                        addWorkflow.DiasAlertaSemAlteracao = 1;
                        addWorkflow.USR = periodo.USR;
                        addWorkflow.DHC = periodo.DHC;
                        addWorkflow.ATV = periodo.ATV;
                        var workflowService = new WorkflowService();
                        workflowService.Inserir(addWorkflow);
                    }
                    else
                    {
                        periodo.IdPeriodo = Convert.ToInt32(hdIdPeriodo.Value);
                        new PeriodoService().AlterarPeriodo(periodo);
                        MessageBox.Show("Período alterado com sucesso !!", "", TIPO.Info, MessageBoxHandler);         
                    }

                    LimparCampos();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar cadastrar/salvar o período: " + ex.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }

        public bool ValidaForm()
        {
            bool valida = true;
            StringBuilder sb = new StringBuilder();

            if (ddlEmpresa.SelectedIndex == 0)
            {
                sb.Append("Selecione a Empresa,");
            }

            if (string.IsNullOrEmpty(txtDataInicio.Text))
            {
                sb.Append("Informe o Campo Data de Início,");
            }

            if (string.IsNullOrEmpty(txtDataTermino.Text))
            {
                sb.Append("Informe o Campo Data de Término,");
            }

            if (string.IsNullOrEmpty(txtPeriodo.Text))
            {
                sb.Append("Informe o Campo Código NSEM-YYYY,");
            }

            if (!string.IsNullOrEmpty(txtDataInicio.Text) && !string.IsNullOrEmpty(txtDataTermino.Text))
            {
                if (Convert.ToDateTime(txtDataInicio.Text) > Convert.ToDateTime(txtDataTermino.Text))
                {
                    sb.Append("A Data Início é maior que a Data Término,");
                }

                var convertDataInicio = Convert.ToDateTime(txtDataInicio.Text);
                var convertDataFim = Convert.ToDateTime(txtDataTermino.Text);
                var existperiodo = new PeriodoService().VerificaExistenciaPeriodo(convertDataInicio, convertDataFim, Convert.ToInt32(Session["IDEMPRESA"].ToString()));

                if (existperiodo != null)
                {
                    if (string.IsNullOrEmpty(hdIdPeriodo.Value) && existperiodo.DataInicio > convertDataInicio)
                    {
                        sb.Append("Já existe um intervalo cadastrado com as datas informadas");
                    }
                    else
                    {
                        if (int.TryParse(hdIdPeriodo.Value, out int idperiodo))
                        {
                            if (idperiodo != existperiodo.IdPeriodo && convertDataInicio <= existperiodo.DataInicio)
                            {
                                sb.Append("Esta alteração não pode ser efetuada, já existe um intervalo cadastrado com as datas informadas");
                            }
                        }
                       
                    }

                }
            }

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "", TIPO.Warning, MessageBoxHandler);
                valida = false;
            }

            return valida;
        }

        public void AtualizaAvaliacoesSinalizdas()
        {
            var projetosService = new ProjetosService();
            var avaliacoesSinalizadas = projetosService.ObterAvaliacoesSinalizadas();
            var ultimoPeriodo = new PeriodoService().ObterPeriodoUltimo();
            foreach (var item in avaliacoesSinalizadas)
            {
                item.IdPeriodoSinalizado = ultimoPeriodo.IdPeriodo;
                projetosService.AlterarProjetoAssociado(item);
            }
        }

        protected void rptPeriodo_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var id = ((Label)e.Item.FindControl("lblIdPeriodo"));

            if (e.CommandName == "AlterarPeriodo")
            {
                if (int.TryParse(id.Text, out int idperiodo))
                {
                    var item = new PeriodoService().ObterPeriodo(idperiodo);

                    hdIdPeriodo.Value = item.IdPeriodo.ToString();
                    txtDataInicio.Text = item.DataInicio.Value.ToString("yyyy-MM-dd");
                    txtDataTermino.Text = item.DataFim.Value.ToString("yyyy-MM-dd");
                    txtPeriodo.Text = item.Periodo.ToString();
                    ddlEmpresa.SelectedValue = item.IdEmpresa.HasValue ? item.IdEmpresa.Value.ToString() : string.Empty;
                    ddlStatus.SelectedValue = item.ATV.HasValue && item.ATV.Value == 1 ? "1" : "0";
                }
            }
        }
    }
}