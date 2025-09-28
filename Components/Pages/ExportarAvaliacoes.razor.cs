using Microsoft.AspNetCore.Components;
using Services.AvaliacoesExportacao;
using Services.AvaliacoesExportacao.Common;
using Services.Common;
using Components.Shared;

namespace Components.Pages;

public partial class ExportarAvaliacoes : ComponentBase
{
    [Inject] public ExportarAvaliacoesService ExportarAvaliacoesService { get; set; } = default!;
    [Inject] public ExportarAvaliacoesHelper ExportarAvaliacoesHelper { get; set; } = default!;
    [Inject] public ExportarAvaliacoesExcelService ExportarAvaliacoesExcelService { get; set; } = default!;
    [Inject] public ComboHelper ComboHelper { get; set; } = default!;
    [Inject] public FormatHelper FormatHelper { get; set; } = default!;
    [Inject] public MessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] public TelemetryService TelemetryService { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    protected string FiltroPeriodo { get; set; } = string.Empty;
    protected string FiltroProjeto { get; set; } = string.Empty;
    protected string FiltroAssociado { get; set; } = string.Empty;

    protected List<ComboItem> PeriodosCombo = new();
    protected List<ComboItem> ProjetosCombo = new();
    protected List<ComboItem> AssociadosCombo = new();

    protected List<AvaliacoesExportacao.Models.AvaliacaoExportacaoPreviewModel> Avaliacoes = new();

    protected override async Task OnInitializedAsync()
    {
        await CarregarCombos();
    }

    protected async Task CarregarCombos()
    {
        PeriodosCombo = await ComboHelper.GetPeriodosComboAsync();
        ProjetosCombo = await ComboHelper.GetProjetosComboAsync();
        AssociadosCombo = await ComboHelper.GetProfissionaisComboAsync();
    }

    protected async Task BuscarAvaliacoes()
    {
        try
        {
            Avaliacoes = await ExportarAvaliacoesService.ObterAvaliacoesPreviewAsync(FiltroPeriodo, FiltroProjeto, FiltroAssociado);
            MessageBoxService.ShowSuccess($"{Avaliacoes.Count} avaliações encontradas.");
            TelemetryService.TrackEvent("ExportarAvaliacoes_Busca", new Dictionary<string, string> {
                { "Periodo", FiltroPeriodo },
                { "Projeto", FiltroProjeto },
                { "Associado", FiltroAssociado },
                { "QtdAvaliacoes", Avaliacoes.Count.ToString() }
            });
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError("Erro ao buscar avaliações.");
            TelemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Action", "BuscarAvaliacoes" }
            });
        }
    }

    protected async Task Exportar()
    {
        try
        {
            var fileResult = await ExportarAvaliacoesExcelService.ExportarParaExcelAsync(FiltroPeriodo, FiltroProjeto, FiltroAssociado);
            if (fileResult != null)
            {
                var fileName = fileResult.FileName;
                var fileBytes = fileResult.FileBytes;
                var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                await BlazorDownloadFile(fileName, fileType, fileBytes);
                MessageBoxService.ShowSuccess("Arquivo exportado com sucesso.");
                TelemetryService.TrackEvent("ExportarAvaliacoes_Exportacao", new Dictionary<string, string> {
                    { "Periodo", FiltroPeriodo },
                    { "Projeto", FiltroProjeto },
                    { "Associado", FiltroAssociado },
                    { "QtdAvaliacoes", Avaliacoes.Count.ToString() }
                });
            }
            else
            {
                MessageBoxService.ShowWarning("Nenhum dado para exportar.");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError("Erro ao exportar arquivo.");
            TelemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Action", "Exportar" }
            });
        }
    }

    protected async Task BlazorDownloadFile(string fileName, string contentType, byte[] fileBytes)
    {
        await JS.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, Convert.ToBase64String(fileBytes));
    }
}
