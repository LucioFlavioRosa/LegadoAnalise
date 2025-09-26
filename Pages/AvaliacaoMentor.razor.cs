using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Services.Mentoria;
using Services.Mentoria.Common;
using Services.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class AvaliacaoMentor : ComponentBase
{
    [Inject] public IMentoriaService MentoriaService { get; set; } = default!;
    [Inject] public IMentoriaHelper MentoriaHelper { get; set; } = default!;

    protected FiltroMentoriaModel Filtro { get; set; } = new();
    protected List<ComboItem> ProjetosCombo { get; set; } = new();
    protected List<ComboItem> ClientesCombo { get; set; } = new();
    protected List<ComboItem> PeriodosCombo { get; set; } = new();
    protected List<ComboItem> StatusCombo { get; set; } = new();
    protected List<MentoradosModel> Mentorados { get; set; } = new();
    protected List<ProjetoModel> ProjetosAvaliacoes { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await CarregarCombosAsync();
        await CarregarMentoradosAsync();
        await CarregarProjetosAvaliacoesAsync();
    }

    protected async Task CarregarCombosAsync()
    {
        ProjetosCombo = await MentoriaService.GetProjetosComboAsync();
        ClientesCombo = await MentoriaService.GetClientesComboAsync();
        PeriodosCombo = await MentoriaService.GetPeriodosComboAsync();
        StatusCombo = await MentoriaService.GetStatusComboAsync();
    }

    protected async Task CarregarMentoradosAsync()
    {
        Mentorados = await MentoriaService.GetMentoradosAsync();
    }

    protected async Task CarregarProjetosAvaliacoesAsync()
    {
        ProjetosAvaliacoes = await MentoriaService.GetProjetosAvaliacoesAsync(Filtro);
    }

    protected async Task OnFiltrar()
    {
        await CarregarProjetosAvaliacoesAsync();
    }
}
