using Microsoft.AspNetCore.Components;
using Services.Comentarios.Common;
using Services.Common;

namespace Components.Pages;

public partial class Comentarios : ComponentBase
{
    [Inject]
    protected IComentariosService ComentariosService { get; set; } = default!;
    [Inject]
    protected IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IUserContextService UserContextService { get; set; } = default!;

    protected string Comentario { get; set; } = string.Empty;
    protected bool IsLoading { get; set; } = false;
    protected string? MensagemErro { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CarregarComentarioAsync();
    }

    protected async Task CarregarComentarioAsync()
    {
        try
        {
            IsLoading = true;
            MensagemErro = null;
            var usuario = await UserContextService.GetUsuarioLogadoAsync();
            var periodo = await ComentariosService.ObterPeriodoUltimoAsync();
            var comentario = await ComentariosService.ObterComentarioAsync(usuario.Id, periodo.IdPeriodo);
            Comentario = comentario?.Comentario ?? string.Empty;
        }
        catch (Exception)
        {
            MensagemErro = "Erro ao carregar comentário. Tente novamente.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task EnviarComentario()
    {
        try
        {
            IsLoading = true;
            MensagemErro = null;
            var usuario = await UserContextService.GetUsuarioLogadoAsync();
            var periodo = await ComentariosService.ObterPeriodoUltimoAsync();
            await ComentariosService.CriarOuAtualizarComentarioAsync(usuario.Id, periodo.IdPeriodo, Comentario);
            MessageBoxService.ShowSuccess("Comentários enviados.<br/>Obrigado 😃", "Sucesso");
            var redirectUrl = await UserContextService.GetRedirectUrlAsync() ?? "index";
            NavigationManager.NavigateTo(redirectUrl, forceLoad: true);
        }
        catch (Exception)
        {
            MensagemErro = "Erro ao enviar comentário. Tente novamente.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
