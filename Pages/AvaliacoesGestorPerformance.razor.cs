using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.Collections.Generic;
using Services.AvaliacoesGestor.Common;
using Services.Common;

public partial class AvaliacoesGestorPerformance : ComponentBase
{
    [Inject] public IAvaliacoesGestorService AvaliacoesGestorService { get; set; }
    [Inject] public IAvaliacoesGestorHelper AvaliacoesGestorHelper { get; set; }
    [Inject] public IComboHelper ComboHelper { get; set; }
    [Inject] public IMessageBoxService MessageBoxService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    protected AvaliacaoGestorPerformanceDadosViewModel ModelDados { get; set; }
    protected List<AvaliacaoGestorPerformanceItemViewModel> ListaPerformances { get; set; }
    protected List<ComboItem> ComboNotas { get; set; }
    protected bool IsTudoFinalizado { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        ModelDados = await AvaliacoesGestorService.ObterDadosCabecalhoAsync();
        ListaPerformances = await AvaliacoesGestorService.ObterListaPerformancesAsync();
        ComboNotas = ComboHelper.GetNotasPerformanceItems();
        IsTudoFinalizado = AvaliacoesGestorHelper.VerificarTudoFinalizado(ListaPerformances);
    }

    protected async Task SalvarAvaliacao()
    {
        var resultado = await AvaliacoesGestorService.SalvarAvaliacaoAsync(ListaPerformances);
        if (resultado)
        {
            MessageBoxService.ShowSuccess("Avaliação Salva com Sucesso.");
            await CarregarDadosAsync();
        }
        else
        {
            MessageBoxService.ShowError("Falha ao Incluir Performance da Avaliação.");
        }
    }

    protected void IrFinalizacao()
    {
        NavigationManager.NavigateTo("/avalizacao_gestor");
    }

    protected void IrCompetencia()
    {
        NavigationManager.NavigateTo("/avalizacao_gestor_competencia");
    }

    protected void OnNotaGestorChange(AvaliacaoGestorPerformanceItemViewModel item, ChangeEventArgs e)
    {
        if (e?.Value != null)
        {
            item.NotaGestor = e.Value.ToString();
        }
    }
}
