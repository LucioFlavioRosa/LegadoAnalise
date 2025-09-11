using Business.DataAccess;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;

namespace SistemaAvaliacao
{
    public partial class workflowdeprojetos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboPeriodos();
                CarregaWorkflows();
                WebStorage.Delete("IdWorkflow");
            }
        }

        private void CarregaWorkflows()
        {
            var ws = new WorkflowService();
            rptWorkflow.DataSource = ws.ObterLista();
            rptWorkflow.DataBind();
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

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            if (ddlPeriodos.SelectedIndex <= 0)
            {
                MessageBox.Show("Selecione o período das avaliações", "Período Obrigatório", TIPO.Warning, MessageBoxHandler);
                return;
            }

            int idPeriodo = Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value);
            DateTime dataInicio;

            if (!DateTime.TryParse(txtDataInicio.Value, out dataInicio))
            {
                MessageBox.Show("Selecione uma data para início das avaliações", "Data Inválida", TIPO.Warning, MessageBoxHandler);
                return;
            }

            if (dataInicio < DateTime.Now)
            {
                MessageBox.Show("A data de início das avaliações não pode ser anterior à data atual", "Data Inválida", TIPO.Warning, MessageBoxHandler);
                return;
            }

            if (txtAuto.Value == "" || txtCegas.Value == "" || txtGestor.Value == "" || txtFeedback.Value == "" || txtAlerta.Value == "")
            {
                MessageBox.Show("Digite todos os prazos em dias", "Prazo Obrigatório", TIPO.Warning, MessageBoxHandler);
                return;
            }

            int diasAuto = int.Parse(txtAuto.Value);
            int diasCegas = int.Parse(txtCegas.Value);
            int diasGestor = int.Parse(txtGestor.Value);
            int diasFeedback = int.Parse(txtFeedback.Value);
            int diasAlerta = int.Parse(txtAlerta.Value);

            if (diasAuto <= 0 || diasCegas <= 0 || diasGestor <= 0 || diasFeedback <= 0 || diasAlerta <= 0)
            {
                MessageBox.Show("Os prazos devem ser maiores que zero", "Prazo Inválido", TIPO.Warning, MessageBoxHandler);
                return;
            }

            var ws = new WorkflowService();
            var idWorkflow = Convert.ToInt32(WebStorage.Get("IdWorkflow", "0"));

            if (idWorkflow == 0) // É Inclusão
            {
                var wf = ws.ObterByPeriodo(idPeriodo);

                if (wf != null)
                {
                    MessageBox.Show("Já existe um Workflow cadastrado para esse Período", "Workflow Duplicado", TIPO.Warning, MessageBoxHandler);
                    return;
                }

                wf = new WORKFLOW();
                wf.DataInicio = dataInicio;
                wf.DiasAutoAvaliacao = diasAuto;
                wf.DiasAvaliacaoCegas = diasCegas;
                wf.DiasAvaliacaoGestor = diasGestor;
                wf.DiasFeedback = diasFeedback;
                wf.DiasAlertaSemAlteracao = diasAlerta;
                wf.IdPeriodo = idPeriodo;
                
                if (ws.Inserir(wf))
                {
                    CarregaWorkflows();
                    LimpaCampos();                    
                    MessageBox.Show("Workflow inserido com sucesso", "OK", TIPO.Info, MessageBoxHandler);
                }
                else
                    MessageBox.Show("Falha ao inserir o Workflow", "Erro", TIPO.Error, MessageBoxHandler);
            }
            else // É Alteração
            {
                var wf = ws.Obter(idWorkflow);

                if (wf == null)
                {
                    MessageBox.Show("Workflow não encontrado para Alteração", "Alteração não Permitida", TIPO.Warning, MessageBoxHandler);
                    return;
                }

                wf.DataInicio = dataInicio;
                wf.DiasAutoAvaliacao = diasAuto;
                wf.DiasAvaliacaoCegas = diasCegas;
                wf.DiasAvaliacaoGestor = diasGestor;
                wf.DiasFeedback = diasFeedback;
                wf.DiasAlertaSemAlteracao = diasAlerta;
                wf.IdPeriodo = idPeriodo;

                if (ws.Alterar(idPeriodo, wf))
                {
                    CarregaWorkflows();
                    LimpaCampos();
                    MessageBox.Show("Workflow alterado com sucesso", "OK", TIPO.Info, MessageBoxHandler);
                }
                else
                    MessageBox.Show("Falha ao alterar o Workflow", "Erro", TIPO.Error, MessageBoxHandler);
            }
        }

        private void LimpaCampos()
        {
            WebStorage.Delete("IdWorkflow");
            ddlPeriodos.SelectedIndex = 0;
            txtAuto.Value = txtCegas.Value = txtGestor.Value = txtFeedback.Value = txtDataInicio.Value = txtAlerta.Value = "";
        }

        protected void btnAlterar_Click(object sender, EventArgs e)
        {
            WebStorage.Delete("IdWorkflow");            
            int idWorkflow = Convert.ToInt32((sender as System.Web.UI.WebControls.LinkButton).CommandArgument);

            var ws = new WorkflowService();
            var wf = ws.Obter(idWorkflow);
            
            if (wf != null)
            {
                ddlPeriodos.Value = wf.IdPeriodo.ToString();
                txtDataInicio.Value = wf.DataInicio.ToString("yyyy-MM-dd");
                txtAuto.Value = wf.DiasAutoAvaliacao.ToString();
                txtCegas.Value = wf.DiasAvaliacaoCegas.ToString();
                txtGestor.Value = wf.DiasAvaliacaoGestor.ToString();
                txtFeedback.Value = wf.DiasFeedback.ToString();
                txtAlerta.Value = wf.DiasAlertaSemAlteracao.ToString();
                WebStorage.Set("IdWorkflow", idWorkflow.ToString());
            }
        }

        protected void btnDeletar_Click(object sender, EventArgs e)
        {
            WebStorage.Delete("IdWorkflow");            
            var ws = new WorkflowService();
            int idWorkflow = Convert.ToInt32((sender as System.Web.UI.WebControls.LinkButton).CommandArgument);

            if (ws.Deletar(idWorkflow))
            {
                MessageBox.Show("Workflow excluído com sucesso", "Exclusão OK", TIPO.Info, MessageBoxHandler);
                CarregaWorkflows();
                LimpaCampos();
            }
            else
                MessageBox.Show("Falha na tentativa de exclusão do Workflow", "Falha na Exclusão", TIPO.Error, MessageBoxHandler);
        }
    }
}