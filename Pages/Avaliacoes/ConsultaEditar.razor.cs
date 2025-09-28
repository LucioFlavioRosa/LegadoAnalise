using Microsoft.AspNetCore.Components;
using Services.Avaliacoes;
using Services.Common;

namespace Pages.Avaliacoes;

public partial class ConsultaEditar : ComponentBase
{
    [Inject] public IConsultaAvaliacaoService ConsultaAvaliacaoService { get; set; } = default!;
    [Inject] public ComboHelper ComboHelper { get; set; } = default!;
    [Inject] public FormatHelper FormatHelper { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;

    protected string AvaliacaoId { get; set; } = string.Empty;
    protected string ProjetoId { get; set; } = string.Empty;
    protected string ClienteId { get; set; } = string.Empty;
    protected string AssociadoId { get; set; } = string.Empty;
    protected string PeriodoId { get; set; } = string.Empty;

    protected List<ComboItem> ProjetosCombo { get; set; } = new();
    protected List<ComboItem> ClientesCombo { get; set; } = new();
    protected List<ComboItem> ProfissionaisCombo { get; set; } = new();
    protected List<ComboItem> PeriodosCombo { get; set; } = new();

    protected bool ExibirDetalhes { get; set; } = false;
    protected string DetalhesAvaliacaoId { get; set; } = string.Empty;
    protected string DetalhesProjeto { get; set; } = string.Empty;
    protected string DetalhesCliente { get; set; } = string.Empty;
    protected string DetalhesAssociado { get; set; } = string.Empty;
    protected string DetalhesPeriodo { get; set; } = string.Empty;

    protected List<AvaliacaoEmailModel> Avaliacoes { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await CarregarCombosAsync();
    }

    protected async Task CarregarCombosAsync()
    {
        ProjetosCombo = await ComboHelper.GetProjetosComboAsync();
        ClientesCombo = await ComboHelper.GetClientesComboAsync();
        ProfissionaisCombo = await ComboHelper.GetProfissionaisComboAsync();
        PeriodosCombo = await ComboHelper.GetPeriodosComboAsync();
    }

    protected async Task BuscarAvaliacoes()
    {
        Avaliacoes = await ConsultaAvaliacaoService.BuscarAvaliacoesAsync(ProjetoId, ClienteId, AssociadoId, PeriodoId);
        ExibirDetalhes = false;
        StateHasChanged();
    }

    protected async Task RetrocederAvaliacao(int avaliacaoId)
    {
        var resultado = await ConsultaAvaliacaoService.RetrocederAvaliacaoAsync(avaliacaoId);
        if (resultado.Sucesso)
        {
            await BuscarAvaliacoes();
            MessageBoxService.ShowSuccess(resultado.Mensagem);
        }
        else
        {
            MessageBoxService.ShowError(resultado.Mensagem);
        }
    }
}
