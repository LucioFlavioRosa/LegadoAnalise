using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Prazos;
using Peers.Moderno.Services.Prazos.Common;

namespace Peers.Moderno.Components.Pages;

public partial class Prazos : ComponentBase
{
    [Inject] private IPrazosService PrazosService { get; set; } = default!;
    [Inject] private IPrazosValidationHelper ValidationHelper { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private IUserContextService UserContextService { get; set; } = default!;

    private List<Prazo> Prazos { get; set; } = new();
    private PrazoFormModel FormModel { get; set; } = new();
    private bool IsLoading { get; set; }

    private List<GatilhoOption> GatilhosAutoAvaliacao { get; set; } = new();
    private List<GatilhoOption> GatilhosAvaliacaoAsCegas { get; set; } = new();
    private List<GatilhoOption> GatilhosAvaliacaoGestor { get; set; } = new();
    private List<GatilhoOption> GatilhosFeedback { get; set; } = new();
    private List<GatilhoOption> GatilhosMentor { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadGatilhoOptions();
        await LoadPrazos();
    }

    private void LoadGatilhoOptions()
    {
        GatilhosAutoAvaliacao = ValidationHelper.GetGatilhoOptions(TipoGatilho.AutoAvaliacao);
        GatilhosAvaliacaoAsCegas = ValidationHelper.GetGatilhoOptions(TipoGatilho.AvaliacaoAsCegas);
        GatilhosAvaliacaoGestor = ValidationHelper.GetGatilhoOptions(TipoGatilho.AvaliacaoGestor);
        GatilhosFeedback = ValidationHelper.GetGatilhoOptions(TipoGatilho.Feedback);
        GatilhosMentor = ValidationHelper.GetGatilhoOptions(TipoGatilho.Mentor);
    }

    private async Task LoadPrazos()
    {
        try
        {
            IsLoading = true;
            Prazos = await PrazosService.ObterTodosPrazosAsync();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar prazos: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task SalvarPrazo()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            var usuario = await UserContextService.GetUsuarioLogadoAsync();
            if (usuario == null)
            {
                MessageBoxService.ShowError("Usuário não autenticado");
                return;
            }

            var result = await PrazosService.CadastrarOuAtualizarPrazoAsync(FormModel, usuario.Id);

            if (result.IsSuccess)
            {
                MessageBoxService.ShowSuccess(result.Message);
                LimparFormulario();
                await LoadPrazos();
            }
            else
            {
                MessageBoxService.ShowError(result.Message);
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao salvar prazo: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task EditarPrazo(int id)
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            var prazo = await PrazosService.ObterPrazoPorIdAsync(id);
            if (prazo != null)
            {
                MapPrazoToFormModel(prazo);
            }
            else
            {
                MessageBoxService.ShowError("Prazo não encontrado");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar prazo: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task InativarPrazo(int id)
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            var result = await PrazosService.InativarPrazoAsync(id);

            if (result.IsSuccess)
            {
                MessageBoxService.ShowSuccess(result.Message);
                await LoadPrazos();
            }
            else
            {
                MessageBoxService.ShowError(result.Message);
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao inativar prazo: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private void LimparFormulario()
    {
        FormModel = new PrazoFormModel
        {
            Status = 1
        };
    }

    private void MapPrazoToFormModel(Prazo prazo)
    {
        FormModel = new PrazoFormModel
        {
            IdPrazo = prazo.IdPrazo,
            NomeDisparo = prazo.NomeDisparo,
            Status = prazo.ATV,
            DuracaoAutoAvaliacao = prazo.DuracaoAutoAvaliacao.ToString(),
            GatilhoAutoAvaliacao = prazo.GatilhoAutoAvaliacao,
            CompensacaoAutoAvaliacao = prazo.CompensadorAutoAvaliacao.ToString(),
            DuracaoAvaliacaoAsCegas = prazo.DuracaoAvaliacaoAsCegas.ToString(),
            GatilhoAvaliacaoAsCegas = prazo.GatilhoAvaliacaoAsCegas,
            CompensacaoAvaliacaoAsCegas = prazo.CompensadorAvaliacaoAsCegas.ToString(),
            DuracaoAvaliacaoGestor = prazo.DuracaoAvaliacaoGestor.ToString(),
            GatilhoAvaliacaoGestor = prazo.GatilhoAvaliacaoGestor,
            CompensacaoAvaliacaoGestor = prazo.CompensadorAvaliacaoGestor.ToString(),
            DuracaoFeedback = prazo.DuracaoFeedback.ToString(),
            GatilhoFeedback = prazo.GatilhoFeedback,
            CompensacaoFeedback = prazo.CompensadorFeedback.ToString(),
            DuracaoMentor = prazo.DuracaoMentor.ToString(),
            GatilhoMentor = prazo.GatilhoMentor,
            CompensacaoMentor = prazo.CompensadorMentor.ToString()
        };
    }
}