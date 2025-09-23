using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Eixos;
using Peers.Moderno.Services.Common;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Components;

public partial class Eixos : ComponentBase
{
    [Inject] private IEixosService EixosService { get; set; } = null!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    private List<Eixo>? Eixos { get; set; }
    private EixoFormModel CurrentEixo { get; set; } = new();
    private bool IsLoading { get; set; }
    private bool IsLoadingList { get; set; }
    private bool IsExporting { get; set; }
    private bool IsEditing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadEixos();
    }

    private async Task LoadEixos()
    {
        IsLoadingList = true;
        try
        {
            Eixos = await EixosService.ListAsync();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar eixos: {ex.Message}");
        }
        finally
        {
            IsLoadingList = false;
            StateHasChanged();
        }
    }

    private async Task HandleSubmit()
    {
        if (string.IsNullOrWhiteSpace(CurrentEixo.Nome))
        {
            MessageBoxService.ShowWarning("Preencha o Campo Eixo");
            return;
        }

        IsLoading = true;
        try
        {
            var eixo = new Eixo
            {
                IdEixo = CurrentEixo.IdEixo,
                Nome = CurrentEixo.Nome.Trim(),
                ATV = CurrentEixo.ATV,
                USR = GetCurrentUserId(),
                DHC = DateTime.Now,
                TipoAvaliacao = "desempenho"
            };

            if (IsEditing)
            {
                await EixosService.UpdateAsync(eixo);
                MessageBoxService.ShowSuccess("Eixo alterado com sucesso!");
            }
            else
            {
                await EixosService.CreateAsync(eixo);
                MessageBoxService.ShowSuccess("Eixo cadastrado com sucesso!");
            }

            await LoadEixos();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao salvar eixo: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private void EditEixo(Eixo eixo)
    {
        CurrentEixo = new EixoFormModel
        {
            IdEixo = eixo.IdEixo,
            Nome = eixo.Nome,
            ATV = eixo.ATV
        };
        IsEditing = true;
        StateHasChanged();
    }

    private async Task InactivateEixo(int id)
    {
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Tem certeza que deseja inativar este eixo?");
        if (!confirmed) return;

        try
        {
            var success = await EixosService.InactivateAsync(id);
            if (success)
            {
                MessageBoxService.ShowSuccess("Eixo inativado com sucesso!");
                await LoadEixos();
            }
            else
            {
                MessageBoxService.ShowError("Erro ao inativar eixo!");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao inativar eixo: {ex.Message}");
        }
    }

    private async Task ExportEixos()
    {
        IsExporting = true;
        try
        {
            var fileBytes = await EixosService.ExportAsync();
            var fileName = $"Eixos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var base64 = Convert.ToBase64String(fileBytes);
            
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, base64);
            MessageBoxService.ShowSuccess("Arquivo exportado com sucesso!");
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao exportar eixos: {ex.Message}");
        }
        finally
        {
            IsExporting = false;
            StateHasChanged();
        }
    }

    private void CancelEdit()
    {
        ClearForm();
    }

    private void ClearForm()
    {
        CurrentEixo = new EixoFormModel();
        IsEditing = false;
        StateHasChanged();
    }

    private int GetCurrentUserId()
    {
        // TODO: Implementar obtenção do usuário atual da sessão/claims
        return 1;
    }

    public class EixoFormModel
    {
        public int IdEixo { get; set; }
        
        [Required(ErrorMessage = "O campo Eixo é obrigatório")]
        [StringLength(500, ErrorMessage = "O campo Eixo deve ter no máximo 500 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        public int ATV { get; set; } = 1;
    }
}