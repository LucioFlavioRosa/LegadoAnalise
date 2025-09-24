using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Performance;
using Peers.Moderno.Services.Cargos;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Performance;

public partial class Performance : ComponentBase
{
    [Inject] private IPerformanceService PerformanceService { get; set; } = default!;
    [Inject] private ICargosService CargosService { get; set; } = default!;
    [Inject] private IAvaliacoesService AvaliacoesService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private ITelemetryService TelemetryService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<Cargo> Cargos = new();
    private List<Models.Performance> Performances = new();
    private List<AvaliacaoPerformanceNota> NotasPerformance = new();
    private IBrowserFile? SelectedFile;

    private int SelectedCargoId = 0;
    private int SelectedStatus = 1;
    private string PerformanceText = string.Empty;
    private string SelectedAbrangencia = "Individual";
    private string DescricaoAbaixo = string.Empty;
    private string DescricaoEsperado = string.Empty;
    private string DescricaoAcima = string.Empty;
    private bool InputAutoAvaliacao = true;
    private bool InputAvaliacaoAsCegas = true;
    private bool InputAvaliacaoGestor = true;
    private int NotaPadraoAutoAvaliacaoId = 0;
    private int NotaPadraoAvaliacaoAsCegasId = 0;
    private int NotaPadraoAvaliacaoGestorId = 0;
    private int EditingPerformanceId = 0;

    private bool IsProcessing = false;
    private bool ShowMessageBox = false;
    private string MessageBoxText = string.Empty;
    private MessageBoxType MessageBoxType = MessageBoxType.Info;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadInitialData();
            MessageBoxService.OnMessageReceived += OnMessageReceived;
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "OnInitializedAsync" },
                { "Component", "Performance" }
            });
            ShowMessage("Erro ao carregar dados iniciais", MessageBoxType.Error);
        }
    }

    private async Task LoadInitialData()
    {
        await Task.WhenAll(
            LoadCargos(),
            LoadPerformances(),
            LoadNotasPerformance()
        );
        LimparCampos();
    }

    private async Task LoadCargos()
    {
        try
        {
            Cargos = await CargosService.ObterListaCargosAsync(true);
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "LoadCargos" },
                { "Component", "Performance" }
            });
            Cargos = new List<Cargo>();
        }
    }

    private async Task LoadPerformances()
    {
        try
        {
            Performances = await PerformanceService.ObterListaPerformancesAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "LoadPerformances" },
                { "Component", "Performance" }
            });
            Performances = new List<Models.Performance>();
        }
    }

    private async Task LoadNotasPerformance()
    {
        try
        {
            NotasPerformance = await AvaliacoesService.ListaNotasPerformancesAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "LoadNotasPerformance" },
                { "Component", "Performance" }
            });
            NotasPerformance = new List<AvaliacaoPerformanceNota>();
        }
    }

    private void LimparCampos()
    {
        SelectedCargoId = 0;
        SelectedStatus = 1;
        PerformanceText = string.Empty;
        SelectedAbrangencia = "Individual";
        DescricaoAbaixo = string.Empty;
        DescricaoEsperado = string.Empty;
        DescricaoAcima = string.Empty;
        InputAutoAvaliacao = true;
        InputAvaliacaoAsCegas = true;
        InputAvaliacaoGestor = true;
        NotaPadraoAutoAvaliacaoId = 0;
        NotaPadraoAvaliacaoAsCegasId = 0;
        NotaPadraoAvaliacaoGestorId = 0;
        EditingPerformanceId = 0;
    }

    private async Task CadastrarSalvar()
    {
        if (IsProcessing) return;

        try
        {
            IsProcessing = true;
            StateHasChanged();

            var validationResult = ValidateForm();
            if (!validationResult.IsValid)
            {
                ShowMessage(validationResult.ErrorMessage, MessageBoxType.Warning);
                return;
            }

            var performance = new Models.Performance
            {
                IdCargo = SelectedCargoId,
                Nome = PerformanceText,
                PerformanceAbaixo = DescricaoAbaixo,
                PerformanceEsperado = DescricaoEsperado,
                PerformanceAcima = DescricaoAcima,
                Abrangencia = SelectedAbrangencia,
                Ativo = SelectedStatus == 1,
                InputAutoavaliacao = InputAutoAvaliacao,
                InputAvaliacaoAsCegas = InputAvaliacaoAsCegas,
                InputAvaliacaoGestor = InputAvaliacaoGestor,
                NotaPadraoAutoAvaliacao = InputAutoAvaliacao ? null : (NotaPadraoAutoAvaliacaoId == 0 ? null : NotaPadraoAutoAvaliacaoId),
                NotaPadraoAvaliacaoAsCegas = InputAvaliacaoAsCegas ? null : (NotaPadraoAvaliacaoAsCegasId == 0 ? null : NotaPadraoAvaliacaoAsCegasId),
                NotaPadraoAvaliacaoGestor = InputAvaliacaoGestor ? null : (NotaPadraoAvaliacaoGestorId == 0 ? null : NotaPadraoAvaliacaoGestorId)
            };

            if (EditingPerformanceId == 0)
            {
                await PerformanceService.InserirPerformanceAsync(performance);
                ShowMessage("Performance inserida com sucesso!", MessageBoxType.Success);
            }
            else
            {
                performance.IdPerformance = EditingPerformanceId;
                await PerformanceService.AlterarPerformanceAsync(performance);
                ShowMessage("Performance alterada com sucesso!", MessageBoxType.Success);
            }

            await LoadPerformances();
            LimparCampos();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CadastrarSalvar" },
                { "Component", "Performance" },
                { "PerformanceId", EditingPerformanceId.ToString() }
            });
            ShowMessage("Erro ao salvar performance", MessageBoxType.Error);
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    private ValidationResult ValidateForm()
    {
        if (SelectedCargoId == 0)
            return ValidationResult.Error("Selecione o campo Cargo");

        if (string.IsNullOrWhiteSpace(PerformanceText))
            return ValidationResult.Error("Preencha o campo Performance");

        if (string.IsNullOrWhiteSpace(DescricaoAbaixo))
            return ValidationResult.Error("Preencha o campo Performance Abaixo");

        if (string.IsNullOrWhiteSpace(DescricaoEsperado))
            return ValidationResult.Error("Preencha o campo Performance Esperado");

        if (string.IsNullOrWhiteSpace(DescricaoAcima))
            return ValidationResult.Error("Preencha o campo Performance Acima");

        if (!InputAutoAvaliacao && NotaPadraoAutoAvaliacaoId == 0)
            return ValidationResult.Error("Selecione a nota padrão para auto avaliação");

        if (!InputAvaliacaoAsCegas && NotaPadraoAvaliacaoAsCegasId == 0)
            return ValidationResult.Error("Selecione a nota padrão para avaliação as cegas");

        if (!InputAvaliacaoGestor && NotaPadraoAvaliacaoGestorId == 0)
            return ValidationResult.Error("Selecione a nota padrão para avaliação do gestor");

        return ValidationResult.Success();
    }

    private async Task AlterarPerformance(int performanceId)
    {
        try
        {
            var performance = await PerformanceService.ObterPerformanceAsync(performanceId);
            if (performance == null)
            {
                ShowMessage("Performance não encontrada", MessageBoxType.Warning);
                return;
            }

            EditingPerformanceId = performance.IdPerformance;
            SelectedCargoId = performance.IdCargo;
            SelectedStatus = performance.Ativo ? 1 : 0;
            PerformanceText = performance.Nome;
            SelectedAbrangencia = performance.Abrangencia;
            DescricaoAbaixo = performance.PerformanceAbaixo;
            DescricaoEsperado = performance.PerformanceEsperado;
            DescricaoAcima = performance.PerformanceAcima;
            InputAutoAvaliacao = performance.InputAutoavaliacao;
            InputAvaliacaoAsCegas = performance.InputAvaliacaoAsCegas;
            InputAvaliacaoGestor = performance.InputAvaliacaoGestor;
            NotaPadraoAutoAvaliacaoId = performance.NotaPadraoAutoAvaliacao ?? 0;
            NotaPadraoAvaliacaoAsCegasId = performance.NotaPadraoAvaliacaoAsCegas ?? 0;
            NotaPadraoAvaliacaoGestorId = performance.NotaPadraoAvaliacaoGestor ?? 0;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AlterarPerformance" },
                { "Component", "Performance" },
                { "PerformanceId", performanceId.ToString() }
            });
            ShowMessage("Erro ao carregar performance para edição", MessageBoxType.Error);
        }
    }

    private async Task InativarPerformance(int performanceId)
    {
        try
        {
            var success = await PerformanceService.ExcluirPerformanceAsync(performanceId);
            if (success)
            {
                ShowMessage("Performance inativada com sucesso!", MessageBoxType.Success);
                await LoadPerformances();
            }
            else
            {
                ShowMessage("Erro ao inativar performance", MessageBoxType.Warning);
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InativarPerformance" },
                { "Component", "Performance" },
                { "PerformanceId", performanceId.ToString() }
            });
            ShowMessage("Erro ao inativar performance", MessageBoxType.Error);
        }
    }

    private async Task ExportarPerformances()
    {
        if (IsProcessing) return;

        try
        {
            IsProcessing = true;
            StateHasChanged();

            var fileBytes = await PerformanceService.ExportarPerformancesAsync();
            var fileName = $"Performance_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(fileBytes));
            ShowMessage("Arquivo exportado com sucesso!", MessageBoxType.Success);
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExportarPerformances" },
                { "Component", "Performance" }
            });
            ShowMessage("Erro ao exportar performances", MessageBoxType.Error);
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    private async Task ImportarPerformances()
    {
        if (IsProcessing || SelectedFile == null) return;

        try
        {
            IsProcessing = true;
            StateHasChanged();

            using var stream = SelectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            var result = await PerformanceService.ImportarPerformancesAsync(stream);

            ShowMessage($"Performances importadas com sucesso<br>Inseridas: {result.Inseridas}<br>Alteradas: {result.Alteradas}<br>Desconsideradas: {result.Desconsideradas}", MessageBoxType.Success);
            await LoadPerformances();
            SelectedFile = null;
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ImportarPerformances" },
                { "Component", "Performance" }
            });
            ShowMessage("Erro ao importar performances", MessageBoxType.Error);
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        SelectedFile = e.File;
        StateHasChanged();
    }

    private void OnInputAutoAvaliacaoChanged()
    {
        if (InputAutoAvaliacao)
        {
            NotaPadraoAutoAvaliacaoId = 0;
        }
        StateHasChanged();
    }

    private void OnInputAvaliacaoAsCegasChanged()
    {
        if (InputAvaliacaoAsCegas)
        {
            NotaPadraoAvaliacaoAsCegasId = 0;
        }
        StateHasChanged();
    }

    private void OnInputAvaliacaoGestorChanged()
    {
        if (InputAvaliacaoGestor)
        {
            NotaPadraoAvaliacaoGestorId = 0;
        }
        StateHasChanged();
    }

    private string GetNotaPadraoLabelStyle(bool show)
    {
        return show ? "display: inline; font-style: italic; color: gray;" : "display: none;";
    }

    private string GetNotaPadraoDropdownStyle(bool show)
    {
        return show ? "display: block;" : "display: none;";
    }

    private void OnMessageReceived(MessageBoxEventArgs args)
    {
        MessageBoxText = args.Message;
        MessageBoxType = args.Type;
        ShowMessageBox = true;
        StateHasChanged();
    }

    private void ShowMessage(string message, MessageBoxType type)
    {
        MessageBoxText = message;
        MessageBoxType = type;
        ShowMessageBox = true;
        StateHasChanged();
    }

    private void CloseMessageBox()
    {
        ShowMessageBox = false;
        StateHasChanged();
    }

    private string GetMessageBoxTitle()
    {
        return MessageBoxType switch
        {
            MessageBoxType.Success => "Sucesso",
            MessageBoxType.Error => "Erro",
            MessageBoxType.Warning => "Aviso",
            MessageBoxType.Info => "Informação",
            _ => "Mensagem"
        };
    }

    public void Dispose()
    {
        if (MessageBoxService != null)
        {
            MessageBoxService.OnMessageReceived -= OnMessageReceived;
        }
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    private ValidationResult() { }

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Error(string errorMessage)
    {
        return new ValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }
}