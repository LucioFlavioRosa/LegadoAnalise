using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Consolidacao;
using Peers.Moderno.Services.Consolidacao.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Pages;

public partial class Consolidacao : ComponentBase
{
    [Inject] private IConsolidacaoService ConsolidacaoService { get; set; } = default!;
    [Inject] private IExportFileService ExportFileService { get; set; } = default!;
    [Inject] private IImportFileService ImportFileService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<ConsolidacaoItem> consolidacaoItems = new();
    private List<DropdownItem> periodos = new();
    private List<DropdownItem> projetos = new();
    private List<DropdownItem> associados = new();

    private int selectedPeriodo = 0;
    private int selectedProjeto = 0;
    private int selectedAssociado = 0;
    private int selectedExportPeriodo = 0;
    private int selectedExportAssociado = 0;

    private bool isLoading = false;
    private bool isExporting = false;
    private bool isImporting = false;
    private bool isExportingNotas = false;
    private bool isImportingNotas = false;

    private InputFile? importConsideracoesFileInput;
    private InputFile? importNotasFileInput;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDadosIniciaisAsync();
    }

    private async Task CarregarDadosIniciaisAsync()
    {
        try
        {
            // Mock data - substituir por chamadas reais aos serviços
            periodos = new List<DropdownItem>
            {
                new() { Id = 1, Nome = "2024 - 1º Semestre" },
                new() { Id = 2, Nome = "2024 - 2º Semestre" }
            };

            projetos = new List<DropdownItem>
            {
                new() { Id = 1, Nome = "Projeto Alpha" },
                new() { Id = 2, Nome = "Projeto Beta" }
            };

            associados = new List<DropdownItem>
            {
                new() { Id = 1, Nome = "João Silva" },
                new() { Id = 2, Nome = "Maria Santos" }
            };

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar dados iniciais: {ex.Message}");
        }
    }

    private async Task BuscarAvaliacoes()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            consolidacaoItems = await ConsolidacaoService.ObterListConsolidacaoAsync(
                selectedProjeto, selectedAssociado, selectedPeriodo, "desempenho", "projeto");

            if (!consolidacaoItems.Any())
            {
                MessageBoxService.ShowInfo("Nenhuma avaliação encontrada com os filtros selecionados.");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao buscar avaliações: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task ExportarConsideracoes()
    {
        try
        {
            isExporting = true;
            StateHasChanged();

            // Mock data - substituir por chamada real ao serviço
            var consideracoes = new List<ConsideracoesMentorModel>
            {
                new()
                {
                    idConsideracoesMentor = 1,
                    idMentor = 1,
                    Mentor = "João Silva",
                    idAssociado = 2,
                    Associado = "Maria Santos",
                    idPeriodo = selectedExportPeriodo,
                    Periodo = periodos.FirstOrDefault(p => p.Id == selectedExportPeriodo)?.Nome ?? "",
                    TipoAvaliacao = "desempenho",
                    Escopo = "projeto",
                    ElegivelPromocao = "Sim",
                    InputPromocao = "Não",
                    TrajetoriaAssociado = "Trajetória exemplar",
                    PontosFortes = "Liderança, comunicação",
                    PontosFracos = "Gestão de tempo",
                    MentorPodeVer = "Sim",
                    AcaoComite = "Promover",
                    PontosFortesRH = "Excelente desempenho",
                    PontosFracosRH = "Necessita treinamento",
                    SalarioAtual = "5000.00",
                    SalarioNovo = "6000.00",
                    RegimeContratacaoAtual = "CLT",
                    RegimeContratacaoNovo = "CLT",
                    MentoriaRealizada = "Sim"
                }
            };

            var fileName = $"ConsideracoesMentor_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var fileBytes = await ExportFileService.GenerateExcelConsideracoesMentorAsync(fileName, consideracoes);
            var base64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, base64);
            MessageBoxService.ShowSuccess("Arquivo exportado com sucesso!");
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao exportar considerações: {ex.Message}");
        }
        finally
        {
            isExporting = false;
            StateHasChanged();
        }
    }

    private async Task ImportarConsideracoes(InputFileChangeEventArgs e)
    {
        try
        {
            isImporting = true;
            StateHasChanged();

            var file = e.File;
            if (file == null)
            {
                MessageBoxService.ShowWarning("Nenhum arquivo selecionado.");
                return;
            }

            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB
            var result = await ImportFileService.ImportConsideracoesMentorAsync(stream);

            if (result.HasErrors)
            {
                MessageBoxService.ShowError($"Erros encontrados: {string.Join(", ", result.Errors)}");
            }
            else
            {
                var message = $"Importação concluída! Válidos: {result.ValidItems.Count}, Inválidos: {result.InvalidItems.Count}";
                if (result.HasWarnings)
                {
                    message += $"\nAvisos: {string.Join(", ", result.Warnings)}";
                }
                MessageBoxService.ShowSuccess(message);
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao importar considerações: {ex.Message}");
        }
        finally
        {
            isImporting = false;
            StateHasChanged();
        }
    }

    private async Task ExportarNotasPerformance()
    {
        try
        {
            isExportingNotas = true;
            StateHasChanged();

            // Mock data - substituir por chamada real ao serviço
            var notas = new List<PerformanceNotasModelExport>
            {
                new()
                {
                    IdNotaPerformance = 1,
                    IdAssociado = 1,
                    Associado = "João Silva",
                    IdCargo = 1,
                    Cargo = "Desenvolvedor Senior",
                    IdProjeto = 1,
                    Projeto = "Projeto Alpha",
                    IdPeriodo = 1,
                    Periodo = "2024 - 1º Semestre",
                    IdPerformance = 1,
                    Performance = "Liderança Técnica",
                    IdNotaAutoAvaliacao = 1,
                    NotaAutoAvaliacao = "4",
                    ComentariosAutoAvaliacao = "Bom desempenho",
                    IdNotaAvaliacaoAsCegas = 1,
                    NotaAvaliacaoAsCegas = "4",
                    ComentariosAvaliacaoAsCegas = "Consistente",
                    IdNotaAvaliacaoGestor = 1,
                    NotaAvaliacaoGestor = "5",
                    ComentariosAvaliacaoGestor = "Excelente",
                    IdNotaFeedback = 1,
                    NotaFeedback = "4",
                    ComentariosFeedback = "Muito bom",
                    IdNotaComite = 1,
                    NotaComite = "4",
                    NotaFinal = 4
                }
            };

            var fileName = $"NotasPerformances_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var fileBytes = await ExportFileService.GenerateExcelPerformanceNotasAsync(fileName, notas);
            var base64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, base64);
            MessageBoxService.ShowSuccess("Arquivo exportado com sucesso!");
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao exportar notas de performance: {ex.Message}");
        }
        finally
        {
            isExportingNotas = false;
            StateHasChanged();
        }
    }

    private async Task ImportarNotasPerformance(InputFileChangeEventArgs e)
    {
        try
        {
            isImportingNotas = true;
            StateHasChanged();

            var file = e.File;
            if (file == null)
            {
                MessageBoxService.ShowWarning("Nenhum arquivo selecionado.");
                return;
            }

            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB
            var result = await ImportFileService.ImportPerformanceNotasAsync(stream);

            if (result.HasErrors)
            {
                MessageBoxService.ShowError($"Erros encontrados: {string.Join(", ", result.Errors)}");
            }
            else
            {
                var message = $"Importação concluída! Válidos: {result.ValidItems.Count}, Inválidos: {result.InvalidItems.Count}";
                if (result.HasWarnings)
                {
                    message += $"\nAvisos: {string.Join(", ", result.Warnings)}";
                }
                MessageBoxService.ShowSuccess(message);
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao importar notas de performance: {ex.Message}");
        }
        finally
        {
            isImportingNotas = false;
            StateHasChanged();
        }
    }
}

public class DropdownItem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}