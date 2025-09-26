using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Performance;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Pages;

public partial class FeedbackPerformance : ComponentBase
{
    [Inject] public IFeedbackPerformanceService FeedbackService { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;

    protected FeedbackPerformanceViewModel? ViewModel { get; set; }

    protected override async Task OnInitializedAsync()
    {
        int projetoId = 1;
        int associadoId = 1;
        int periodoId = 1;
        ViewModel = await FeedbackService.GetFeedbackAsync(projetoId, associadoId, periodoId);
    }

    protected async Task SalvarAvaliacaoAsync()
    {
        if (ViewModel == null) return;
        var saveModel = new FeedbackPerformanceSaveModel
        {
            ProjetoId = ViewModel.ProjetoId,
            AssociadoId = ViewModel.AssociadoId,
            PeriodoId = ViewModel.PeriodoId,
            NotasObservacoes = ViewModel.PerformanceItems.Select(item => new PerformanceNotaObservacao
            {
                IdPerformance = item.IdPerformance,
                NotaSelecionada = item.NotaSelecionada ?? 0,
                ConsideracaoFeedback = item.ConsideracaoFeedback ?? string.Empty
            }).ToList()
        };
        var result = await FeedbackService.SaveFeedbackAsync(saveModel);
        if (result)
        {
            MessageBoxService.ShowSuccess("Avaliação Salva com Sucesso.");
        }
        else
        {
            MessageBoxService.ShowError("Erro ao salvar avaliação.");
        }
    }

    protected void GoToCompetencia()
    {
        // Navegação para página de competência
    }
    protected void GoToFinalizacao()
    {
        // Navegação para página de finalização
    }
}
