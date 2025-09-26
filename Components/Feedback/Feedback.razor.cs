using Microsoft.AspNetCore.Components;
using Services.Feedback.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Components.Feedback
{
    public partial class Feedback : ComponentBase
    {
        [Inject] public IFeedbackService FeedbackService { get; set; }
        [Inject] public IFeedbackFinalizationService FeedbackFinalizationService { get; set; }
        [Inject] public IFeedbackComboHelper FeedbackComboHelper { get; set; }
        [Inject] public Services.Common.IMessageBoxService MessageBoxService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected Components.Common.MessageBox MessageBoxRef;

        protected List<ComboItem> ProjetosCombo = new();
        protected List<ComboItem> ClientesCombo = new();
        protected List<ComboItem> PeriodosCombo = new();
        protected List<ComboItem> StatusCombo = new();

        protected FeedbackFiltroModel Filtro = new();
        protected List<ProjetoModel> ProjetosAvaliacoes;

        protected override async Task OnInitializedAsync()
        {
            await CarregarCombosAsync();
            await CarregarAvaliacoesAsync();
        }

        protected async Task CarregarCombosAsync()
        {
            ProjetosCombo = await FeedbackComboHelper.GetProjetosAsync();
            ClientesCombo = await FeedbackComboHelper.GetClientesAsync();
            PeriodosCombo = await FeedbackComboHelper.GetPeriodosAsync();
            StatusCombo = await FeedbackComboHelper.GetStatusAsync();
        }

        protected async Task CarregarAvaliacoesAsync()
        {
            ProjetosAvaliacoes = await FeedbackService.ListarAvaliacoesAsync(Filtro);
            StateHasChanged();
        }

        protected async Task OnFilterSubmit()
        {
            await CarregarAvaliacoesAsync();
        }

        protected async Task OnFinalizarClick(ProjetosAssociadosModel associado)
        {
            var resultado = await FeedbackFinalizationService.FinalizarAvaliacaoAsync(associado.IdEmail);
            if (resultado.Sucesso)
            {
                MessageBoxRef.ShowInfo(resultado.Mensagem);
                await CarregarAvaliacoesAsync();
            }
            else
            {
                MessageBoxRef.ShowWarning(resultado.Mensagem);
            }
        }
    }
}
