using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Services.AvaliacoesGestor;
using Peers.Moderno.Services.Common;

namespace Components.AvaliacoesGestor
{
    public partial class CompetenciaGestor : ComponentBase
    {
        [Parameter] public int IdProjeto { get; set; }
        [Parameter] public int IdAssociado { get; set; }
        [Parameter] public int IdPeriodo { get; set; }
        [Parameter] public int IdGestor { get; set; }

        [Inject] public IAvaliacoesGestorService AvaliacoesGestorService { get; set; } = default!;
        [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;

        protected AvaliacoesGestorService.AvaliacaoGestorDto? Avaliacao { get; set; }
        protected bool IsLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Avaliacao = await AvaliacoesGestorService.CarregarAvaliacaoGestorAsync(IdProjeto, IdAssociado, IdPeriodo, IdGestor, "desempenho", "projeto");
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"Erro ao carregar avaliação: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected async Task SalvarAvaliacao(EditContext context)
        {
            if (Avaliacao == null)
                return;
            var valido = await AvaliacoesGestorService.ValidarAvaliacaoAsync(Avaliacao);
            if (!valido)
            {
                MessageBoxService.ShowError("Preencha todas as notas obrigatórias e corrija inconsistências antes de salvar.");
                return;
            }
            var sucesso = await AvaliacoesGestorService.SalvarAvaliacaoCompetenciasAsync(Avaliacao, false);
            if (sucesso)
                MessageBoxService.ShowSuccess("Avaliação salva com sucesso.");
            else
                MessageBoxService.ShowError("Erro ao salvar avaliação.");
        }

        protected async Task FinalizarAvaliacao()
        {
            if (Avaliacao == null)
                return;
            var valido = await AvaliacoesGestorService.ValidarAvaliacaoAsync(Avaliacao);
            if (!valido)
            {
                MessageBoxService.ShowError("Preencha todas as notas obrigatórias e corrija inconsistências antes de finalizar.");
                return;
            }
            var sucesso = await AvaliacoesGestorService.SalvarAvaliacaoCompetenciasAsync(Avaliacao, true);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Avaliação finalizada com sucesso.");
                NavigationManager.NavigateTo($"/avalizacao_gestor/{IdProjeto}/{IdAssociado}/{IdPeriodo}");
            }
            else
                MessageBoxService.ShowError("Erro ao finalizar avaliação.");
        }
    }
}
