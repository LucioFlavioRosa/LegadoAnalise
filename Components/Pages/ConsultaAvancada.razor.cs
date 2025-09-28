using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Services.Common;

public class ConsultaAvancadaBase : ComponentBase
{
    [Inject] public Services.Common.IConsultaAvancadaService ConsultaAvancadaService { get; set; } = default!;
    [Inject] public ComboHelper ComboHelper { get; set; } = default!;
    [Inject] public Peers.Moderno.Services.Common.IMessageBoxService MessageBoxService { get; set; } = default!;

    protected List<ComboItem> ProjetosCombo { get; set; } = new();
    protected List<ComboItem> ClientesCombo { get; set; } = new();
    protected List<ComboItem> ProfissionaisCombo { get; set; } = new();
    protected List<ComboItem> PeriodosCombo { get; set; } = new();
    protected List<ComboItem> FasesCombo { get; set; } = new();

    protected string? ProjetoId { get; set; }
    protected string? ClienteId { get; set; }
    protected string? AssociadoId { get; set; }
    protected string? PeriodoId { get; set; }
    protected string? Fase { get; set; }

    protected List<AvaliacaoEmailModel>? Avaliacoes { get; set; }
    protected bool IsLoading { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadCombosAsync();
    }

    protected async Task LoadCombosAsync()
    {
        ProjetosCombo = await ComboHelper.GetProjetosComboAsync(ConsultaAvancadaService.DbContext);
        ClientesCombo = await ComboHelper.GetClientesComboAsync(ConsultaAvancadaService.DbContext);
        ProfissionaisCombo = await ConsultaAvancadaService.GetProfissionaisComboAsync();
        PeriodosCombo = await ComboHelper.GetPeriodosComboAsync(ConsultaAvancadaService.DbContext);
        FasesCombo = GetFasesCombo();
        StateHasChanged();
    }

    protected List<ComboItem> GetFasesCombo()
    {
        return new List<ComboItem>
        {
            new ComboItem { Value = "", Text = "[Selecionar]" },
            new ComboItem { Value = "AVM", Text = "Não Iniciado" },
            new ComboItem { Value = "AAV", Text = "Auto Avaliação" },
            new ComboItem { Value = "ACE", Text = "As Cegas" },
            new ComboItem { Value = "AGE", Text = "Gestor" },
            new ComboItem { Value = "FED", Text = "Feedback" },
            new ComboItem { Value = "AME", Text = "Mentor" },
            new ComboItem { Value = "AFI", Text = "Finalizada" }
        };
    }

    protected async Task OnSearchAsync()
    {
        IsLoading = true;
        StateHasChanged();
        try
        {
            var filtro = new ConsultaAvancadaFiltroModel
            {
                ProjetoId = int.TryParse(ProjetoId, out var pid) && pid > 0 ? pid : (int?)null,
                ClienteId = int.TryParse(ClienteId, out var cid) && cid > 0 ? cid : (int?)null,
                AssociadoId = int.TryParse(AssociadoId, out var aid) && aid > 0 ? aid : (int?)null,
                PeriodoId = int.TryParse(PeriodoId, out var peid) && peid > 0 ? peid : (int?)null,
                Fase = string.IsNullOrWhiteSpace(Fase) ? null : Fase
            };
            Avaliacoes = await ConsultaAvancadaService.ConsultarAvaliacoesAsync(filtro);
            if (Avaliacoes == null || !Avaliacoes.Any())
            {
                MessageBoxService.ShowInfo("Nenhuma avaliação encontrada para os filtros selecionados.");
            }
        }
        catch
        {
            MessageBoxService.ShowError("Erro ao buscar avaliações. Tente novamente mais tarde.");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}
