using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Services.Workflow;
using Services.Common;
using System.Globalization;

namespace Pages;

public partial class WorkflowDeProjetos : ComponentBase
{
    [Inject] public IWorkflowService WorkflowService { get; set; } = default!;
    [Inject] public IWorkflowComboHelper WorkflowComboHelper { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;

    protected List<ComboItem> Periodos { get; set; } = new();
    protected List<WorkflowDto> Workflows { get; set; } = new();

    protected int WorkflowEditId { get; set; } = 0;
    protected string? PeriodoSelecionadoId { get; set; }
    protected DateTime? DataInicio { get; set; }
    protected int? DiasAutoAvaliacao { get; set; }
    protected int? DiasAvaliacaoCegas { get; set; }
    protected int? DiasAvaliacaoGestor { get; set; }
    protected int? DiasFeedback { get; set; }
    protected int? DiasAlertaSemAlteracao { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CarregarCombosAsync();
        await CarregarWorkflowsAsync();
        LimparCampos();
    }

    protected async Task CarregarCombosAsync()
    {
        Periodos = await WorkflowComboHelper.GetPeriodosComboAsync();
    }

    protected async Task CarregarWorkflowsAsync()
    {
        Workflows = await WorkflowService.ObterListaAsync();
    }

    protected void LimparCampos()
    {
        WorkflowEditId = 0;
        PeriodoSelecionadoId = string.Empty;
        DataInicio = null;
        DiasAutoAvaliacao = null;
        DiasAvaliacaoCegas = null;
        DiasAvaliacaoGestor = null;
        DiasFeedback = null;
        DiasAlertaSemAlteracao = null;
        StateHasChanged();
    }

    protected async Task SalvarWorkflow()
    {
        if (string.IsNullOrEmpty(PeriodoSelecionadoId) || PeriodoSelecionadoId == "")
        {
            MessageBoxService.ShowWarning("Selecione o período das avaliações", "Período Obrigatório");
            return;
        }
        if (!DataInicio.HasValue)
        {
            MessageBoxService.ShowWarning("Selecione uma data para início das avaliações", "Data Inválida");
            return;
        }
        if (DataInicio.Value.Date < DateTime.Now.Date)
        {
            MessageBoxService.ShowWarning("A data de início das avaliações não pode ser anterior à data atual", "Data Inválida");
            return;
        }
        if (!DiasAutoAvaliacao.HasValue || !DiasAvaliacaoCegas.HasValue || !DiasAvaliacaoGestor.HasValue || !DiasFeedback.HasValue || !DiasAlertaSemAlteracao.HasValue)
        {
            MessageBoxService.ShowWarning("Digite todos os prazos em dias", "Prazo Obrigatório");
            return;
        }
        if (DiasAutoAvaliacao <= 0 || DiasAvaliacaoCegas <= 0 || DiasAvaliacaoGestor <= 0 || DiasFeedback <= 0 || DiasAlertaSemAlteracao <= 0)
        {
            MessageBoxService.ShowWarning("Os prazos devem ser maiores que zero", "Prazo Inválido");
            return;
        }

        var workflow = new WorkflowDto
        {
            IdWorkflow = WorkflowEditId,
            IdPeriodo = int.Parse(PeriodoSelecionadoId!),
            DataInicio = DataInicio.Value,
            DiasAutoAvaliacao = DiasAutoAvaliacao.Value,
            DiasAvaliacaoCegas = DiasAvaliacaoCegas.Value,
            DiasAvaliacaoGestor = DiasAvaliacaoGestor.Value,
            DiasFeedback = DiasFeedback.Value,
            DiasAlertaSemAlteracao = DiasAlertaSemAlteracao.Value
        };

        if (WorkflowEditId == 0)
        {
            var existente = await WorkflowService.ObterByPeriodoAsync(workflow.IdPeriodo);
            if (existente != null)
            {
                MessageBoxService.ShowWarning("Já existe um Workflow cadastrado para esse Período", "Workflow Duplicado");
                return;
            }
            var sucesso = await WorkflowService.InserirAsync(workflow);
            if (sucesso)
            {
                await CarregarWorkflowsAsync();
                LimparCampos();
                MessageBoxService.ShowInfo("Workflow inserido com sucesso", "OK");
            }
            else
            {
                MessageBoxService.ShowError("Falha ao inserir o Workflow", "Erro");
            }
        }
        else
        {
            var wf = await WorkflowService.ObterAsync(WorkflowEditId);
            if (wf == null)
            {
                MessageBoxService.ShowWarning("Workflow não encontrado para Alteração", "Alteração não Permitida");
                return;
            }
            workflow.IdWorkflow = WorkflowEditId;
            var sucesso = await WorkflowService.AlterarAsync(workflow);
            if (sucesso)
            {
                await CarregarWorkflowsAsync();
                LimparCampos();
                MessageBoxService.ShowInfo("Workflow alterado com sucesso", "OK");
            }
            else
            {
                MessageBoxService.ShowError("Falha ao alterar o Workflow", "Erro");
            }
        }
    }

    protected void EditarWorkflow(WorkflowDto wf)
    {
        WorkflowEditId = wf.IdWorkflow;
        PeriodoSelecionadoId = wf.IdPeriodo.ToString();
        DataInicio = wf.DataInicio;
        DiasAutoAvaliacao = wf.DiasAutoAvaliacao;
        DiasAvaliacaoCegas = wf.DiasAvaliacaoCegas;
        DiasAvaliacaoGestor = wf.DiasAvaliacaoGestor;
        DiasFeedback = wf.DiasFeedback;
        DiasAlertaSemAlteracao = wf.DiasAlertaSemAlteracao;
        StateHasChanged();
    }

    protected async Task DeletarWorkflow(int idWorkflow)
    {
        var sucesso = await WorkflowService.DeletarAsync(idWorkflow);
        if (sucesso)
        {
            MessageBoxService.ShowInfo("Workflow excluído com sucesso", "Exclusão OK");
            await CarregarWorkflowsAsync();
            LimparCampos();
        }
        else
        {
            MessageBoxService.ShowError("Falha na tentativa de exclusão do Workflow", "Falha na Exclusão");
        }
    }
}
