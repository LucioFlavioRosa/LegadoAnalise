using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Dimensoes.Common;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Components.Pages.Dimensoes;

public partial class Dimensoes : ComponentBase
{
    private List<Dimensao> dimensoes = new();
    private List<Dimensao> dimensoesFiltradas = new();
    private DimensaoFormModel dimensaoModel = new();
    private string filtroTexto = string.Empty;
    private bool isLoading = true;
    private bool isProcessing = false;
    private bool isExporting = false;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDimensoes();
    }

    private async Task CarregarDimensoes()
    {
        try
        {
            isLoading = true;
            StateHasChanged();
            
            dimensoes = await DimensoesService.ListarAsync();
            dimensoesFiltradas = dimensoes.ToList();
            
            TelemetryService.TrackEvent("Dimensoes_Carregadas", new Dictionary<string, string>
            {
                { "TotalDimensoes", dimensoes.Count.ToString() }
            });
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar dimensões: {ex.Message}");
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "CarregarDimensoes" }
            });
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void FiltrarDimensoes(ChangeEventArgs e)
    {
        filtroTexto = e.Value?.ToString() ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(filtroTexto))
        {
            dimensoesFiltradas = dimensoes.ToList();
        }
        else
        {
            var termos = filtroTexto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            dimensoesFiltradas = dimensoes.Where(d => 
                termos.All(termo => 
                    d.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                    d.IdDimensao.ToString().Contains(termo, StringComparison.OrdinalIgnoreCase)
                )
            ).ToList();
        }
        
        StateHasChanged();
    }

    private async Task HandleCadastrar()
    {
        if (isProcessing) return;
        
        try
        {
            isProcessing = true;
            StateHasChanged();
            
            var dimensao = new Dimensao
            {
                IdDimensao = dimensaoModel.IdDimensao,
                Nome = dimensaoModel.Nome,
                Ativo = dimensaoModel.Ativo,
                TipoAvaliacao = "desempenho",
                DataCriacao = dimensaoModel.IdDimensao == 0 ? DateTime.Now : dimensaoModel.DataCriacao,
                DataAlteracao = DateTime.Now
            };
            
            if (dimensaoModel.IdDimensao == 0)
            {
                await DimensoesService.InserirAsync(dimensao);
                MessageBoxService.ShowSuccess("Dimensão inserida com sucesso!");
                
                TelemetryService.TrackEvent("Dimensao_Inserida", new Dictionary<string, string>
                {
                    { "Nome", dimensao.Nome },
                    { "Ativo", dimensao.Ativo.ToString() }
                });
            }
            else
            {
                await DimensoesService.AlterarAsync(dimensao);
                MessageBoxService.ShowSuccess("Dimensão alterada com sucesso!");
                
                TelemetryService.TrackEvent("Dimensao_Alterada", new Dictionary<string, string>
                {
                    { "Id", dimensao.IdDimensao.ToString() },
                    { "Nome", dimensao.Nome }
                });
            }
            
            LimparFormulario();
            await CarregarDimensoes();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao salvar dimensão: {ex.Message}");
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "SalvarDimensao" },
                { "Id", dimensaoModel.IdDimensao.ToString() }
            });
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private void HandleAlterar(Dimensao dimensao)
    {
        dimensaoModel = new DimensaoFormModel
        {
            IdDimensao = dimensao.IdDimensao,
            Nome = dimensao.Nome,
            Ativo = dimensao.Ativo,
            DataCriacao = dimensao.DataCriacao
        };
        
        StateHasChanged();
    }

    private async Task HandleInativar(Dimensao dimensao)
    {
        try
        {
            var confirmacao = await JSRuntime.InvokeAsync<bool>("confirm", $"Deseja realmente inativar a dimensão '{dimensao.Nome}'?");
            
            if (!confirmacao) return;
            
            await DimensoesService.InativarAsync(dimensao.IdDimensao);
            MessageBoxService.ShowSuccess("Dimensão inativada com sucesso!");
            
            TelemetryService.TrackEvent("Dimensao_Inativada", new Dictionary<string, string>
            {
                { "Id", dimensao.IdDimensao.ToString() },
                { "Nome", dimensao.Nome }
            });
            
            await CarregarDimensoes();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao inativar dimensão: {ex.Message}");
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "InativarDimensao" },
                { "Id", dimensao.IdDimensao.ToString() }
            });
        }
    }

    private async Task HandleExportar()
    {
        if (isExporting) return;
        
        try
        {
            isExporting = true;
            StateHasChanged();
            
            var arquivoBytes = await ExportService.ExportarDimensoesAsync();
            var nomeArquivo = $"Dimensoes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var base64 = Convert.ToBase64String(arquivoBytes);
            
            await JSRuntime.InvokeVoidAsync("downloadFile", nomeArquivo, base64);
            
            MessageBoxService.ShowSuccess("Arquivo exportado com sucesso!");
            
            TelemetryService.TrackEvent("Dimensoes_Exportadas", new Dictionary<string, string>
            {
                { "TotalRegistros", dimensoes.Count.ToString() },
                { "NomeArquivo", nomeArquivo }
            });
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao exportar dimensões: {ex.Message}");
            TelemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "ExportarDimensoes" }
            });
        }
        finally
        {
            isExporting = false;
            StateHasChanged();
        }
    }

    private void LimparFormulario()
    {
        dimensaoModel = new DimensaoFormModel();
        StateHasChanged();
    }

    public class DimensaoFormModel
    {
        public int IdDimensao { get; set; }
        
        [Required(ErrorMessage = "O nome da dimensão é obrigatório")]
        [StringLength(500, ErrorMessage = "O nome da dimensão deve ter no máximo 500 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; }
    }
}