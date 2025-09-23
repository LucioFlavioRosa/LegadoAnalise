using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Complexidades.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Pages;

public partial class Complexidade : ComponentBase
{
    [Inject] private IComplexidadeService ComplexidadeService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;

    private List<ComplexidadeDto>? complexidades;
    private ComplexidadeDto complexidadeAtual = new();
    private bool isLoading = false;

    protected override async Task OnInitializedAsync()
    {
        await CarregarComplexidades();
    }

    private async Task CarregarComplexidades()
    {
        try
        {
            complexidades = await ComplexidadeService.ObterListaComplexidadesAsync();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar complexidades: {ex.Message}");
        }
    }

    private async Task SalvarComplexidade()
    {
        if (isLoading) return;

        try
        {
            isLoading = true;
            StateHasChanged();

            bool sucesso;
            string mensagem;

            if (complexidadeAtual.IdComplexidade == 0)
            {
                sucesso = await ComplexidadeService.InserirComplexidadeAsync(complexidadeAtual);
                mensagem = sucesso ? "Complexidade inserida com sucesso!" : "Erro ao inserir complexidade. Verifique os dados informados.";
            }
            else
            {
                sucesso = await ComplexidadeService.AlterarComplexidadeAsync(complexidadeAtual);
                mensagem = sucesso ? "Complexidade alterada com sucesso!" : "Erro ao alterar complexidade. Verifique os dados informados.";
            }

            if (sucesso)
            {
                MessageBoxService.ShowSuccess(mensagem);
                await CarregarComplexidades();
                LimparFormulario();
            }
            else
            {
                MessageBoxService.ShowError(mensagem);
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao salvar complexidade: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task EditarComplexidade(int id)
    {
        try
        {
            var complexidade = await ComplexidadeService.ObterComplexidadeAsync(id);
            if (complexidade != null)
            {
                complexidadeAtual = complexidade;
                StateHasChanged();
            }
            else
            {
                MessageBoxService.ShowError("Complexidade não encontrada.");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar complexidade: {ex.Message}");
        }
    }

    private async Task InativarComplexidade(int id)
    {
        try
        {
            var sucesso = await ComplexidadeService.ExcluirComplexidadeAsync(id);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Complexidade inativada com sucesso!");
                await CarregarComplexidades();
            }
            else
            {
                MessageBoxService.ShowError("Erro ao inativar complexidade.");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao inativar complexidade: {ex.Message}");
        }
    }

    private void LimparFormulario()
    {
        complexidadeAtual = new ComplexidadeDto
        {
            ATV = 1
        };
        StateHasChanged();
    }
}