using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Services.FeedbackPerformance;
using Services.FeedbackPerformance.Common;
using Services.Common;

namespace Peers.Moderno.Pages;

public partial class FeedbackPerformance : ComponentBase
{
    [Inject] public IFeedbackPerformanceService FeedbackPerformanceService { get; set; } = default!;
    [Inject] public FeedbackPerformanceHelper FeedbackPerformanceHelper { get; set; } = default!;
    [Inject] public FeedbackPerformanceValidator FeedbackPerformanceValidator { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    public FeedbackPerformanceModel FeedbackModel { get; set; } = new FeedbackPerformanceModel();
    public string ProjetoNome { get; set; } = string.Empty;
    public string AssociadoNome { get; set; } = string.Empty;
    public string PeriodoNome { get; set; } = string.Empty;
    public string GestorNome { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public bool IsFinalizado { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        var resultado = await FeedbackPerformanceService.GetFeedbackPerformanceAsync();
        if (resultado != null)
        {
            FeedbackModel = resultado;
            ProjetoNome = FeedbackModel.ProjetoNome;
            AssociadoNome = FeedbackModel.AssociadoNome;
            PeriodoNome = FeedbackModel.PeriodoNome;
            GestorNome = FeedbackModel.GestorNome;
            ClienteNome = FeedbackModel.ClienteNome;
            TempoRestante = FeedbackModel.TempoRestante;
            IsFinalizado = FeedbackModel.IsFinalizado;
        }
        else
        {
            Mensagem = "Não foi possível carregar os dados do feedback de performance.";
        }
    }

    private async Task SalvarFeedback()
    {
        var validacao = FeedbackPerformanceValidator.ValidarFeedback(FeedbackModel);
        if (!validacao.Sucesso)
        {
            Mensagem = validacao.Mensagem;
            MessageBoxService.ShowWarning(validacao.Mensagem);
            return;
        }
        var sucesso = await FeedbackPerformanceService.SaveFeedbackPerformanceAsync(FeedbackModel);
        if (sucesso)
        {
            Mensagem = "Feedback salvo com sucesso.";
            MessageBoxService.ShowSuccess("Feedback salvo com sucesso.");
        }
        else
        {
            Mensagem = "Falha ao salvar o feedback.";
            MessageBoxService.ShowError("Falha ao salvar o feedback.");
        }
    }

    private async Task FinalizarFeedback()
    {
        var validacao = FeedbackPerformanceValidator.ValidarFinalizacao(FeedbackModel);
        if (!validacao.Sucesso)
        {
            Mensagem = validacao.Mensagem;
            MessageBoxService.ShowWarning(validacao.Mensagem);
            return;
        }
        var sucesso = await FeedbackPerformanceService.FinalizeFeedbackAsync(FeedbackModel);
        if (sucesso)
        {
            Mensagem = "Feedback finalizado com sucesso.";
            MessageBoxService.ShowSuccess("Feedback finalizado com sucesso.");
            IsFinalizado = true;
        }
        else
        {
            Mensagem = "Falha ao finalizar o feedback.";
            MessageBoxService.ShowError("Falha ao finalizar o feedback.");
        }
    }

    private void IrParaCompetencia()
    {
        NavigationManager.NavigateTo("/feedback-competencia");
    }
}
