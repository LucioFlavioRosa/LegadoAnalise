using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Cargos;
using Peers.Moderno.Services.Cargos.Common;
using Peers.Moderno.Services.Common;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Components.Cargos;

public partial class Cargos : ComponentBase, IDisposable
{
    private List<Cargo>? cargos;
    private List<DropdownItem> proximosCargos = new();
    private CargoFormModel cargoModel = new();
    private bool isLoading = false;
    private bool isExporting = false;
    private bool showToast = false;
    private string toastMessage = string.Empty;
    private string toastTitle = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        MessageBoxService.OnMessageReceived += HandleMessage;
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        cargos = await CargosService.ObterListaCargosAsync();
        proximosCargos = await DropdownService.ObterCargosAtivosAsync();
        StateHasChanged();
    }

    private async Task SalvarCargo()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            var cargo = new Cargo
            {
                IdCargo = cargoModel.IdCargo,
                Nome = cargoModel.Nome,
                IdProximoCargo = cargoModel.IdProximoCargo > 0 ? cargoModel.IdProximoCargo : null,
                TempoMinimoPromocao = cargoModel.TempoMinimoPromocao,
                Funcao = cargoModel.Funcao,
                Autonomia = cargoModel.Autonomia,
                EscopoDeAtuacao = cargoModel.EscopoDeAtuacao,
                NivelInterlocucao = cargoModel.NivelInterlocucao,
                ATV = cargoModel.ATV
            };

            bool sucesso;
            if (cargoModel.IdCargo == 0)
            {
                sucesso = await CargosService.InserirCargoAsync(cargo);
                if (sucesso)
                {
                    MessageBoxService.ShowSuccess("Cargo inserido com sucesso!");
                    LimparFormulario();
                }
                else
                {
                    MessageBoxService.ShowError("Erro ao inserir cargo. Verifique se já existe um cargo com este nome.");
                }
            }
            else
            {
                sucesso = await CargosService.AlterarCargoAsync(cargo);
                if (sucesso)
                {
                    MessageBoxService.ShowSuccess("Cargo alterado com sucesso!");
                    LimparFormulario();
                }
                else
                {
                    MessageBoxService.ShowError("Erro ao alterar cargo. Verifique se já existe um cargo com este nome.");
                }
            }

            if (sucesso)
            {
                await CarregarDados();
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro inesperado: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void EditarCargo(Cargo cargo)
    {
        cargoModel = new CargoFormModel
        {
            IdCargo = cargo.IdCargo,
            Nome = cargo.Nome,
            IdProximoCargo = cargo.IdProximoCargo ?? 0,
            TempoMinimoPromocao = cargo.TempoMinimoPromocao,
            Funcao = cargo.Funcao,
            Autonomia = cargo.Autonomia,
            EscopoDeAtuacao = cargo.EscopoDeAtuacao,
            NivelInterlocucao = cargo.NivelInterlocucao,
            ATV = cargo.ATV
        };
        StateHasChanged();
    }

    private async Task InativarCargo(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Tem certeza que deseja inativar este cargo?"))
        {
            var sucesso = await CargosService.InativarCargoAsync(id);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Cargo inativado com sucesso!");
                await CarregarDados();
            }
            else
            {
                MessageBoxService.ShowError("Erro ao inativar cargo!");
            }
        }
    }

    private void LimparFormulario()
    {
        cargoModel = new CargoFormModel();
        StateHasChanged();
    }

    private async Task ExportarCargos()
    {
        isExporting = true;
        StateHasChanged();

        try
        {
            var (bytes, fileName) = await ExportService.ExportarCargosAsync();
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(bytes));
            MessageBoxService.ShowSuccess("Arquivo exportado com sucesso!");
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao exportar: {ex.Message}");
        }
        finally
        {
            isExporting = false;
            StateHasChanged();
        }
    }

    private void HandleMessage(MessageBoxEventArgs args)
    {
        toastTitle = args.Type switch
        {
            MessageBoxType.Success => "Sucesso",
            MessageBoxType.Error => "Erro",
            MessageBoxType.Warning => "Aviso",
            MessageBoxType.Info => "Informação",
            _ => "Mensagem"
        };
        toastMessage = args.Message;
        showToast = true;
        StateHasChanged();

        // Auto-hide toast after 5 seconds
        Task.Delay(5000).ContinueWith(_ =>
        {
            showToast = false;
            InvokeAsync(StateHasChanged);
        });
    }

    public void Dispose()
    {
        MessageBoxService.OnMessageReceived -= HandleMessage;
    }

    public class CargoFormModel
    {
        public int IdCargo { get; set; }
        
        [Required(ErrorMessage = "O nome do cargo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do cargo deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        public int IdProximoCargo { get; set; }
        
        [Range(1, 120, ErrorMessage = "O tempo mínimo deve ser entre 1 e 120 meses")]
        public int TempoMinimoPromocao { get; set; } = 12;
        
        public string? Funcao { get; set; }
        public string? Autonomia { get; set; }
        public string? EscopoDeAtuacao { get; set; }
        public string? NivelInterlocucao { get; set; }
        
        public int ATV { get; set; } = 1;
    }
}