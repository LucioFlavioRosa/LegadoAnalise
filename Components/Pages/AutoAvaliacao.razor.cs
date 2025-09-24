using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Services.Avaliacoes.Common;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Models;

namespace Peers.Moderno.Components.Pages;

public partial class AutoAvaliacao : ComponentBase
{
    [Inject] private IAutoAvaliacaoService AutoAvaliacaoService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    protected List<ComboItem> Projetos { get; set; } = new();
    protected List<ComboItem> Clientes { get; set; } = new();
    protected List<ComboItem> Periodos { get; set; } = new();
    protected List<ComboItem> StatusList { get; set; } = new();
    protected List<ProjetoModel> ProjetosAvaliacao { get; set; } = new();

    protected string ProjetoSelecionado { get; set; } = string.Empty;
    protected string ClienteSelecionado { get; set; } = string.Empty;
    protected string PeriodoSelecionado { get; set; } = string.Empty;
    protected string StatusSelecionado { get; set; } = string.Empty;

    protected bool IsLoading { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await CarregarCombos();
        await ProcessarParametrosQueryString();
        await FiltrarAvaliacoes();
    }

    private async Task CarregarCombos()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            var tasks = new Task[4]
            {
                CarregarProjetos(),
                CarregarClientes(),
                CarregarPeriodos(),
                CarregarStatus()
            };

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar combos: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task CarregarProjetos()
    {
        Projetos = await AutoAvaliacaoService.ObterProjetosComboAsync();
    }

    private async Task CarregarClientes()
    {
        Clientes = await AutoAvaliacaoService.ObterClientesComboAsync();
    }

    private async Task CarregarPeriodos()
    {
        Periodos = await AutoAvaliacaoService.ObterPeriodosComboAsync();
        if (Periodos.Any())
        {
            PeriodoSelecionado = Periodos.Last().Value;
        }
    }

    private async Task CarregarStatus()
    {
        StatusList = await AutoAvaliacaoService.ObterStatusComboAsync();
    }

    private async Task ProcessarParametrosQueryString()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue("IdProjeto", out var idProjeto) && !string.IsNullOrEmpty(idProjeto))
        {
            ProjetoSelecionado = idProjeto.ToString();
        }

        if (queryParams.TryGetValue("IdPeriodo", out var idPeriodo) && !string.IsNullOrEmpty(idPeriodo))
        {
            PeriodoSelecionado = idPeriodo.ToString();
        }

        if (queryParams.TryGetValue("Finalizou", out var finalizou) && finalizou == "Y")
        {
            MessageBoxService.ShowInfo("Avaliação Finalizada com Sucesso - Próxima Etapa Liberada");
        }
    }

    protected async Task FiltrarAvaliacoes()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            var filtro = new FiltroAutoAvaliacaoModel
            {
                IdProjeto = string.IsNullOrEmpty(ProjetoSelecionado) ? null : int.Parse(ProjetoSelecionado),
                IdCliente = string.IsNullOrEmpty(ClienteSelecionado) ? null : int.Parse(ClienteSelecionado),
                IdPeriodo = string.IsNullOrEmpty(PeriodoSelecionado) ? null : int.Parse(PeriodoSelecionado),
                IdStatus = string.IsNullOrEmpty(StatusSelecionado) ? null : int.Parse(StatusSelecionado)
            };

            ProjetosAvaliacao = await AutoAvaliacaoService.ObterProjetosAvaliacaoAsync(filtro);
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao filtrar avaliações: {ex.Message}");
            ProjetosAvaliacao = new List<ProjetoModel>();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    protected async Task FinalizarAvaliacao(string idEmail)
    {
        try
        {
            if (string.IsNullOrEmpty(idEmail))
            {
                MessageBoxService.ShowError("ID da avaliação não encontrado");
                return;
            }

            var resultado = await AutoAvaliacaoService.FinalizarAvaliacaoAsync(int.Parse(idEmail));

            if (resultado.Sucesso)
            {
                if (!string.IsNullOrEmpty(resultado.RedirectUrl))
                {
                    Navigation.NavigateTo(resultado.RedirectUrl);
                }
                else
                {
                    await FiltrarAvaliacoes();
                    MessageBoxService.ShowSuccess("Avaliação finalizada com sucesso");
                }
            }
            else
            {
                MessageBoxService.ShowWarning(resultado.MensagemErro ?? "Erro ao finalizar avaliação");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao finalizar avaliação: {ex.Message}");
        }
    }
}