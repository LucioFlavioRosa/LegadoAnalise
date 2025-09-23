using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Cargos.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Pages;

public partial class Contact : ComponentBase
{
    [Inject] public ICargoLookupService CargoLookupService { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;

    public string CodigoCargo { get; set; } = string.Empty;
    public Cargo? CargoEncontrado { get; set; }
    public bool IsLoading { get; set; }

    private async Task BuscarCargo()
    {
        if (string.IsNullOrWhiteSpace(CodigoCargo))
        {
            MessageBoxService.ShowWarning("Por favor, digite um código válido.");
            return;
        }

        IsLoading = true;
        CargoEncontrado = null;
        StateHasChanged();

        try
        {
            CargoEncontrado = await CargoLookupService.BuscarPorCodigoAsync(CodigoCargo);
            
            if (CargoEncontrado == null)
            {
                MessageBoxService.ShowError("Cargo não encontrado para o código informado.");
            }
            else
            {
                MessageBoxService.ShowSuccess($"Cargo encontrado: {CargoEncontrado.Nome}");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao buscar cargo: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}