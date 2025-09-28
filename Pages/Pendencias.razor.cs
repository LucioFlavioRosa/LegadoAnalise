using Microsoft.AspNetCore.Components;
using Services.Pendencias.Common;
using Services.Common;

namespace Pages;

public partial class Pendencias : ComponentBase
{
    [Inject]
    public IPendenciasService PendenciasService { get; set; } = default!;
    [Inject]
    public IMessageBoxService MessageBoxService { get; set; } = default!;

    protected List<ProjetoPendenciaModel>? Pendencias { get; set; }
    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsLoading = true;
            // Aqui, em um cenário real, obteríamos o userId do contexto autenticado
            // Para exemplo, vamos simular um userId = 0 (ajuste conforme integração real)
            int userId = await GetUsuarioLogadoIdAsync();
            Pendencias = await PendenciasService.ObterPendenciasAsync(userId);
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar pendências: {ex.Message}");
            Pendencias = new List<ProjetoPendenciaModel>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<int> GetUsuarioLogadoIdAsync()
    {
        // TODO: Integrar com o contexto de autenticação real para obter o Id do usuário logado
        // Exemplo: await UserContextService.GetUserIdAsync();
        return 0;
    }
}
