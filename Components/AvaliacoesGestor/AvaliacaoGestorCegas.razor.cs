using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Services.AvaliacoesGestor.Common;
using Services.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Components.AvaliacoesGestor;

public partial class AvaliacaoGestorCegas : ComponentBase
{
    [Inject] public IAvaliacoesGestorService AvaliacoesGestorService { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] public ComboHelper ComboHelper { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    protected FiltroAvaliacaoGestorCegasModel Filtro { get; set; } = new();
    protected List<ComboItem> ProjetosCombo { get; set; } = new();
    protected List<ComboItem> ClientesCombo { get; set; } = new();
    protected List<ComboItem> PeriodosCombo { get; set; } = new();
    protected List<ComboItem> StatusCombo { get; set; } = new();
    protected List<ProjetoModel> ProjetosAvaliacoes { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await CarregarCombosAsync();
        await BuscarAvaliacoesAsync();
    }

    protected async Task CarregarCombosAsync()
    {
        ProjetosCombo = await ComboHelper.GetProjetosComboAsync(AvaliacoesGestorService.GetDbContext(), AvaliacoesGestorService.GetGestorId());
        ClientesCombo = await ComboHelper.GetClientesComboAsync(AvaliacoesGestorService.GetDbContext());
        PeriodosCombo = await ComboHelper.GetPeriodosComboAsync(AvaliacoesGestorService.GetDbContext(), AvaliacoesGestorService.GetEmpresaId());
        StatusCombo = await ComboHelper.GetStatusComboAsync(AvaliacoesGestorService.GetDbContext());
    }

    protected async Task OnFiltrarAvaliacoes()
    {
        await BuscarAvaliacoesAsync();
    }

    protected async Task BuscarAvaliacoesAsync()
    {
        ProjetosAvaliacoes = await AvaliacoesGestorService.BuscarAvaliacoesGestorCegasAsync(Filtro);
        StateHasChanged();
    }

    protected async Task FinalizarAvaliacao(string idEmail)
    {
        var resultado = await AvaliacoesGestorService.FinalizarAvaliacaoAsync(idEmail);
        if (resultado.Sucesso)
        {
            MessageBoxService.ShowSuccess(resultado.Mensagem, "Próxima Etapa Liberada");
            await BuscarAvaliacoesAsync();
        }
        else
        {
            MessageBoxService.ShowWarning(resultado.Mensagem, "Finalização Não Permitida");
        }
    }
}
