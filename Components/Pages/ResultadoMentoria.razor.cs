using Microsoft.AspNetCore.Components;
using Services.Mentoria;
using Services.Mentoria.Common;
using Services.Common;

public partial class ResultadoMentoria : ComponentBase
{
    [Inject] public IMentoriaService MentoriaService { get; set; } = default!;
    [Inject] public IMentoriaHelper MentoriaHelper { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] public ChartHelper ChartHelper { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    protected List<PDIPillsModel> Pills { get; set; } = new();
    protected List<MentoradoRespostaPill> PeriodosExibidos { get; set; } = new();
    protected int PeriodoSelecionadoId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CarregarPillsEPeriodos();
    }

    protected async Task CarregarPillsEPeriodos()
    {
        Pills = await MentoriaService.GetPeriodosLiberadosMentoriaAsync();
        if (Pills.Any())
        {
            PeriodoSelecionadoId = Pills.FirstOrDefault(x => x.Active == "active")?.IdPeriodo ?? Pills.First().IdPeriodo;
            await CarregarPeriodosExibidos();
        }
    }

    protected async Task CarregarPeriodosExibidos()
    {
        PeriodosExibidos = await MentoriaService.GetMentoradoRespostasPorPeriodoAsync(PeriodoSelecionadoId);
        StateHasChanged();
    }

    protected async Task SelecionarPeriodo(int idPeriodo)
    {
        PeriodoSelecionadoId = idPeriodo;
        await CarregarPeriodosExibidos();
    }
}
