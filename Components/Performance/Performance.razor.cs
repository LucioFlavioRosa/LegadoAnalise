using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Performance;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Cargos;
using Peers.Moderno.Services.Avaliacoes;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Components.Performance;

public partial class PerformanceBase : ComponentBase
{
    [Inject] protected IPerformanceService PerformanceService { get; set; } = null!;
    [Inject] protected ICargosService CargosService { get; set; } = null!;
    [Inject] protected IAvaliacoesService AvaliacoesService { get; set; } = null!;
    [Inject] protected IMessageBoxService MessageBoxService { get; set; } = null!;
    [Inject] protected ITelemetryService TelemetryService { get; set; } = null!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] protected IUserContextService UserContextService { get; set; } = null!;

    protected PerformanceModel CurrentPerformance { get; set; } = new();
    protected List<Models.Performance> Performances { get; set; } = new();
    protected List<Cargo> Cargos { get; set; } = new();
    protected List<AvaliacaoPerformanceNota> NotasPerformance { get; set; } = new();
    
    protected bool IsLoading { get; set; }
    protected bool IsLoadingList { get; set; }
    protected bool IsExporting { get; set; }
    protected bool IsEditing { get; set; }
    protected int? EditingId { get; set; }
    
    protected InputFile? fileUploadInput;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            TelemetryService.TrackEvent("PerformancePageLoaded");
            await LoadInitialDataAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "OnInitializedAsync" },
                { "Component", "Performance" }
            });
            MessageBoxService.ShowError("Erro ao carregar a página: " + ex.Message);
        }
    }

    protected async Task LoadInitialDataAsync()
    {
        IsLoadingList = true;
        StateHasChanged();

        try
        {
            var tasks = new List<Task>
            {
                LoadCargosAsync(),
                LoadNotasPerformanceAsync(),
                LoadPerformancesAsync()
            };

            await Task.WhenAll(tasks);
        }
        finally
        {
            IsLoadingList = false;
            StateHasChanged();
        }
    }

    protected async Task LoadCargosAsync()
    {
        try
        {
            Cargos = await CargosService.ObterListaCargosAsync(true);
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "LoadCargosAsync" },
                { "Component", "Performance" }
            });
            MessageBoxService.ShowError("Erro ao carregar cargos");
        }
    }

    protected async Task LoadNotasPerformanceAsync()
    {
        try
        {
            NotasPerformance = await AvaliacoesService.ObterNotasPerformanceAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "LoadNotasPerformanceAsync" },
                { "Component", "Performance" }
            });
            MessageBoxService.ShowError("Erro ao carregar notas de performance");
        }
    }

    protected async Task LoadPerformancesAsync()
    {
        try
        {
            Performances = await PerformanceService.ObterListaPerformancesAsync();
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "LoadPerformancesAsync" },
                { "Component", "Performance" }
            });
            MessageBoxService.ShowError("Erro ao carregar performances");
        }
    }

    protected async Task HandleSubmit()
    {
        if (IsLoading) return;

        IsLoading = true;
        StateHasChanged();

        try
        {
            var validationResult = ValidatePerformance();
            if (!validationResult.IsValid)
            {
                MessageBoxService.ShowWarning(validationResult.ErrorMessage);
                return;
            }

            var usuario = await UserContextService.GetUsuarioLogadoAsync();
            if (usuario == null)
            {
                MessageBoxService.ShowError("Usuário não autenticado");
                return;
            }

            var performance = MapToPerformanceEntity();
            performance.IdEmpresa = 1; // TODO: Obter da sessão
            performance.USR = usuario.Id;
            performance.DHC = DateTime.Now;

            bool success;
            string successMessage;

            if (IsEditing && EditingId.HasValue)
            {
                performance.IdPerformance = EditingId.Value;
                success = await PerformanceService.AlterarPerformanceAsync(performance);
                successMessage = "Performance alterada com sucesso!";
            }
            else
            {
                success = await PerformanceService.InserirPerformanceAsync(performance);
                successMessage = "Performance inserida com sucesso!";
            }

            if (success)
            {
                MessageBoxService.ShowSuccess(successMessage);
                await ClearFormAsync();
                await LoadPerformancesAsync();
                
                TelemetryService.TrackEvent(IsEditing ? "PerformanceUpdated" : "PerformanceCreated", new Dictionary<string, string>
                {
                    { "PerformanceId", performance.IdPerformance.ToString() },
                    { "UserId", usuario.Id.ToString() }
                });
            }
            else
            {
                MessageBoxService.ShowError("Erro ao salvar performance");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "HandleSubmit" },
                { "Component", "Performance" }
            });
            MessageBoxService.ShowError("Erro ao salvar: " + ex.Message);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    protected ValidationResult ValidatePerformance()
    {
        if (CurrentPerformance.IdCargo == 0)
            return ValidationResult.Error("Selecione o campo Cargo");

        if (string.IsNullOrWhiteSpace(CurrentPerformance.Nome))
            return ValidationResult.Error("Preencha o campo Performance");

        if (string.IsNullOrWhiteSpace(CurrentPerformance.PerformanceAbaixo))
            return ValidationResult.Error("Preencha o campo Performance Abaixo");

        if (string.IsNullOrWhiteSpace(CurrentPerformance.PerformanceEsperado))
            return ValidationResult.Error("Preencha o campo Performance Esperado");

        if (string.IsNullOrWhiteSpace(CurrentPerformance.PerformanceAcima))
            return ValidationResult.Error("Preencha o campo Performance Acima");

        // Validar notas padrão quando input está desabilitado
        if (!CurrentPerformance.InputAutoavaliacao && !CurrentPerformance.NotaPadraoAutoAvaliacao.HasValue)
            return ValidationResult.Error("Selecione a nota padrão para auto avaliação");

        if (!CurrentPerformance.InputAvaliacaoAsCegas && !CurrentPerformance.NotaPadraoAvaliacaoAsCegas.HasValue)
            return ValidationResult.Error("Selecione a nota padrão para avaliação às cegas");

        if (!CurrentPerformance.InputAvaliacaoGestor && !CurrentPerformance.NotaPadraoAvaliacaoGestor.HasValue)
            return ValidationResult.Error("Selecione a nota padrão para avaliação do gestor");

        return ValidationResult.Success();
    }

    protected Models.Performance MapToPerformanceEntity()
    {
        return new Models.Performance
        {
            IdPerformance = EditingId ?? 0,
            IdCargo = CurrentPerformance.IdCargo,
            IdNivel = 1, // TODO: Implementar seleção de nível
            Nome = CurrentPerformance.Nome?.Trim() ?? string.Empty,
            PerformanceAbaixo = CurrentPerformance.PerformanceAbaixo?.Trim() ?? string.Empty,
            PerformanceEsperado = CurrentPerformance.PerformanceEsperado?.Trim() ?? string.Empty,
            PerformanceAcima = CurrentPerformance.PerformanceAcima?.Trim() ?? string.Empty,
            Abrangencia = CurrentPerformance.Abrangencia ?? "Individual",
            InputAutoavaliacao = CurrentPerformance.InputAutoavaliacao,
            NotaPadraoAutoAvaliacao = CurrentPerformance.InputAutoavaliacao ? null : CurrentPerformance.NotaPadraoAutoAvaliacao,
            InputAvaliacaoAsCegas = CurrentPerformance.InputAvaliacaoAsCegas,
            NotaPadraoAvaliacaoAsCegas = CurrentPerformance.InputAvaliacaoAsCegas ? null : CurrentPerformance.NotaPadraoAvaliacaoAsCegas,
            InputAvaliacaoGestor = CurrentPerformance.InputAvaliacaoGestor,
            NotaPadraoAvaliacaoGestor = CurrentPerformance.InputAvaliacaoGestor ? null : CurrentPerformance.NotaPadraoAvaliacaoGestor,
            ATV = CurrentPerformance.ATV
        };
    }

    protected async Task EditPerformance(int performanceId)
    {
        try
        {
            var performance = await PerformanceService.ObterPerformanceAsync(performanceId);
            if (performance == null)
            {
                MessageBoxService.ShowError("Performance não encontrada");
                return;
            }

            CurrentPerformance = new PerformanceModel
            {
                IdCargo = performance.IdCargo,
                Nome = performance.Nome,
                PerformanceAbaixo = performance.PerformanceAbaixo,
                PerformanceEsperado = performance.PerformanceEsperado,
                PerformanceAcima = performance.PerformanceAcima,
                Abrangencia = performance.Abrangencia ?? "Individual",
                InputAutoavaliacao = performance.InputAutoavaliacao,
                NotaPadraoAutoAvaliacao = performance.NotaPadraoAutoAvaliacao,
                InputAvaliacaoAsCegas = performance.InputAvaliacaoAsCegas,
                NotaPadraoAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas,
                InputAvaliacaoGestor = performance.InputAvaliacaoGestor,
                NotaPadraoAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor,
                ATV = performance.ATV
            };

            IsEditing = true;
            EditingId = performanceId;
            StateHasChanged();

            // Scroll para o topo do formulário
            await JSRuntime.InvokeVoidAsync("window.scrollTo", 0, 0);
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "EditPerformance" },
                { "Component", "Performance" },
                { "PerformanceId", performanceId.ToString() }
            });
            MessageBoxService.ShowError("Erro ao carregar performance para edição");
        }
    }

    protected async Task InactivatePerformance(int performanceId)
    {
        try
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Tem certeza que deseja inativar esta performance?");
            if (!confirmed) return;

            var success = await PerformanceService.ExcluirPerformanceAsync(performanceId);
            if (success)
            {
                MessageBoxService.ShowSuccess("Performance inativada com sucesso!");
                await LoadPerformancesAsync();
                
                TelemetryService.TrackEvent("PerformanceInactivated", new Dictionary<string, string>
                {
                    { "PerformanceId", performanceId.ToString() }
                });
            }
            else
            {
                MessageBoxService.ShowError("Erro ao inativar performance");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InactivatePerformance" },
                { "Component", "Performance" },
                { "PerformanceId", performanceId.ToString() }
            });
            MessageBoxService.ShowError("Erro ao inativar performance");
        }
    }

    protected async Task CancelEdit()
    {
        await ClearFormAsync();
    }

    protected async Task ClearFormAsync()
    {
        CurrentPerformance = new PerformanceModel();
        IsEditing = false;
        EditingId = null;
        StateHasChanged();
        await Task.CompletedTask;
    }

    protected void OnInputCheckboxChanged(int inputType, bool isChecked)
    {
        switch (inputType)
        {
            case 0: // Auto Avaliação
                CurrentPerformance.InputAutoavaliacao = isChecked;
                if (isChecked)
                    CurrentPerformance.NotaPadraoAutoAvaliacao = null;
                break;
            case 1: // Avaliação às Cegas
                CurrentPerformance.InputAvaliacaoAsCegas = isChecked;
                if (isChecked)
                    CurrentPerformance.NotaPadraoAvaliacaoAsCegas = null;
                break;
            case 2: // Avaliação do Gestor
                CurrentPerformance.InputAvaliacaoGestor = isChecked;
                if (isChecked)
                    CurrentPerformance.NotaPadraoAvaliacaoGestor = null;
                break;
        }
        StateHasChanged();
    }

    protected string GetNotaPadraoLabelStyle(bool inputEnabled)
    {
        return inputEnabled ? "display: none; font-style: italic; color: gray;" : "display: inline; font-style: italic; color: gray;";
    }

    protected string GetNotaPadraoDivStyle(bool inputEnabled)
    {
        return inputEnabled ? "margin-left:-140px; margin-top:-6px; display:none" : "margin-left:-140px; margin-top:-6px; display:block";
    }

    protected async Task ExportPerformances()
    {
        if (IsExporting) return;

        IsExporting = true;
        StateHasChanged();

        try
        {
            var fileBytes = await PerformanceService.ExportarPerformancesAsync();
            var fileName = $"Performance_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, fileBytes);
            
            MessageBoxService.ShowSuccess("Arquivo exportado com sucesso!");
            
            TelemetryService.TrackEvent("PerformancesExported", new Dictionary<string, string>
            {
                { "FileName", fileName },
                { "RecordCount", Performances.Count.ToString() }
            });
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExportPerformances" },
                { "Component", "Performance" }
            });
            MessageBoxService.ShowError("Erro ao exportar arquivo: " + ex.Message);
        }
        finally
        {
            IsExporting = false;
            StateHasChanged();
        }
    }

    protected async Task TriggerFileUpload()
    {
        if (fileUploadInput != null)
        {
            await JSRuntime.InvokeVoidAsync("eval", $"document.querySelector('input[type=file]').click()");
        }
    }

    protected async Task HandleFileUpload(InputFileChangeEventArgs e)
    {
        if (e.FileCount == 0) return;

        var file = e.File;
        if (file == null) return;

        if (!file.Name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            MessageBoxService.ShowWarning("Apenas arquivos .xlsx são permitidos");
            return;
        }

        const long maxFileSize = 10 * 1024 * 1024; // 10MB
        if (file.Size > maxFileSize)
        {
            MessageBoxService.ShowWarning("Arquivo muito grande. Tamanho máximo: 10MB");
            return;
        }

        IsLoading = true;
        StateHasChanged();

        try
        {
            using var stream = file.OpenReadStream(maxFileSize);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            
            var result = await PerformanceService.ImportarPerformancesAsync(memoryStream.ToArray());
            
            if (result.Success)
            {
                MessageBoxService.ShowSuccess($"Performances importadas com sucesso!<br>Inseridas: {result.Inserted}<br>Alteradas: {result.Updated}<br>Desconsideradas: {result.Skipped}");
                await LoadPerformancesAsync();
                
                TelemetryService.TrackEvent("PerformancesImported", new Dictionary<string, string>
                {
                    { "FileName", file.Name },
                    { "Inserted", result.Inserted.ToString() },
                    { "Updated", result.Updated.ToString() },
                    { "Skipped", result.Skipped.ToString() }
                });
            }
            else
            {
                MessageBoxService.ShowError(result.ErrorMessage ?? "Erro ao importar arquivo");
            }
        }
        catch (Exception ex)
        {
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "HandleFileUpload" },
                { "Component", "Performance" },
                { "FileName", file.Name }
            });
            MessageBoxService.ShowError("Erro ao importar arquivo: " + ex.Message);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}

public class PerformanceModel
{
    [Required(ErrorMessage = "Cargo é obrigatório")]
    public int IdCargo { get; set; }
    
    [Required(ErrorMessage = "Performance é obrigatória")]
    [StringLength(500, ErrorMessage = "Performance deve ter no máximo 500 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Performance Abaixo é obrigatória")]
    [StringLength(2000, ErrorMessage = "Performance Abaixo deve ter no máximo 2000 caracteres")]
    public string PerformanceAbaixo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Performance Esperado é obrigatória")]
    [StringLength(2000, ErrorMessage = "Performance Esperado deve ter no máximo 2000 caracteres")]
    public string PerformanceEsperado { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Performance Acima é obrigatória")]
    [StringLength(2000, ErrorMessage = "Performance Acima deve ter no máximo 2000 caracteres")]
    public string PerformanceAcima { get; set; } = string.Empty;
    
    public string Abrangencia { get; set; } = "Individual";
    public bool InputAutoavaliacao { get; set; } = true;
    public int? NotaPadraoAutoAvaliacao { get; set; }
    public bool InputAvaliacaoAsCegas { get; set; } = true;
    public int? NotaPadraoAvaliacaoAsCegas { get; set; }
    public bool InputAvaliacaoGestor { get; set; } = true;
    public int? NotaPadraoAvaliacaoGestor { get; set; }
    public int ATV { get; set; } = 1;
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