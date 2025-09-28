using Microsoft.AspNetCore.Components;
using Services.Avaliacoes;
using Services.Common;

namespace Components.Avaliacoes;

public partial class EnvioEvolucao : ComponentBase
{
    [Inject] public IEvolucaoAssociadoService EvolucaoAssociadoService { get; set; } = default!;
    [Inject] public ComboHelper ComboHelper { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;

    public List<ComboItem> Periodos { get; set; } = new();
    public List<ComboItem> Associados { get; set; } = new();
    public List<ComboItem> Verticais { get; set; } = new();
    public List<EvolucaoAssociadoDto> Projetos { get; set; } = new();

    public string SelectedPeriodoId { get; set; } = string.Empty;
    public string SelectedAssociadoId { get; set; } = string.Empty;
    public string SelectedVerticalId { get; set; } = string.Empty;
    public bool ShowSecEnvio { get; set; } = false;
    public bool Reenviar { get; set; } = false;
    public string Message { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        Periodos = await ComboHelper.GetPeriodosComboAsync();
        Associados = await ComboHelper.GetProfissionaisComboAsync();
        Verticais = await ComboHelper.GetVerticaisComboAsync();
        if (Periodos.Count > 0) SelectedPeriodoId = Periodos[0].Value;
        if (Associados.Count > 0) SelectedAssociadoId = Associados[0].Value;
        if (Verticais.Count > 0) SelectedVerticalId = Verticais[0].Value;
    }

    public async Task OnSearch()
    {
        if (string.IsNullOrEmpty(SelectedVerticalId) || SelectedVerticalId == "")
        {
            MessageBoxService.ShowWarning("O campo Vertical deve ser preenchido!");
            return;
        }
        int? idAssociado = null;
        if (!string.IsNullOrEmpty(SelectedAssociadoId) && SelectedAssociadoId != "")
            idAssociado = int.Parse(SelectedAssociadoId);
        int idPeriodo = int.Parse(SelectedPeriodoId);
        int idVertical = int.Parse(SelectedVerticalId);
        Projetos = await EvolucaoAssociadoService.ListAssociadosEvolucaoAsync(idAssociado, idPeriodo, idVertical);
        ShowSecEnvio = true;
        StateHasChanged();
    }

    public async Task OnGerarTodos()
    {
        if (string.IsNullOrEmpty(SelectedVerticalId) || SelectedVerticalId == "")
        {
            MessageBoxService.ShowWarning("O campo Vertical deve ser preenchido!");
            return;
        }
        int idPeriodo = int.Parse(SelectedPeriodoId);
        int idVertical = int.Parse(SelectedVerticalId);
        var evolucoes = await EvolucaoAssociadoService.ListAssociadosEvolucaoAsync(null, idPeriodo, idVertical);
        int enviados = 0;
        foreach (var item in evolucoes)
        {
            if (!Reenviar && item.Enviado == "Sim")
                continue;
            // Simula geração e envio
            await EvolucaoAssociadoService.GerarEvolucaoAssociadoOtimizadoAsync(
                new List<Avaliacao>(), new List<AvaliacaoCompetencia>(), new List<AvaliacaoPerformance>(),
                new List<Competencia>(), new List<Performance>(),
                item.IdAssociado, item.IdPeriodo, 0, item.TipoAvaliacao, item.Escopo);
            enviados++;
        }
        if (enviados > 0)
        {
            MessageBoxService.ShowSuccess($"Evolução da Avaliação Gerada e Enviada com Sucesso. Vertical: {Verticais.FirstOrDefault(v => v.Value == SelectedVerticalId)?.Text} Quantidade enviado: {enviados}");
        }
        else
        {
            MessageBoxService.ShowInfo($"As Evoluções da Avaliação já foram geradas e enviadas. Vertical: {Verticais.FirstOrDefault(v => v.Value == SelectedVerticalId)?.Text}");
        }
    }

    public async Task OnGerarUnico(EvolucaoAssociadoDto item)
    {
        try
        {
            await EvolucaoAssociadoService.GerarEvolucaoAssociadoOtimizadoAsync(
                new List<Avaliacao>(), new List<AvaliacaoCompetencia>(), new List<AvaliacaoPerformance>(),
                new List<Competencia>(), new List<Performance>(),
                item.IdAssociado, item.IdPeriodo, 0, item.TipoAvaliacao, item.Escopo);
            MessageBoxService.ShowSuccess("Evolução da Avaliação Gerada e Enviada com Sucesso.");
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao tentar enviar a evolução ao associado: {ex.Message}");
        }
    }
}
