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
    private bool isCarregando = true;
    private bool isProcessing = false;
    private bool isEdicao = false;

    protected override async Task OnInitializedAsync()
    {
        await CarregarComplexidades();
    }

    private async Task CarregarComplexidades()
    {
        try
        {
            isCarregando = true;
            complexidades = await ComplexidadeService.ObterListaComplexidadesAsync();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar complexidades: {ex.Message}");
        }
        finally
        {
            isCarregando = false;
            StateHasChanged();
        }
    }

    private async Task SalvarComplexidade()
    {
        if (!ValidarFormulario())
            return;

        try
        {
            isProcessing = true;
            StateHasChanged();

            bool sucesso;
            string mensagem;

            if (isEdicao)
            {
                sucesso = await ComplexidadeService.AlterarComplexidadeAsync(complexidadeAtual);
                mensagem = sucesso ? "Complexidade alterada com sucesso!" : "Erro ao alterar complexidade";
            }
            else
            {
                sucesso = await ComplexidadeService.InserirComplexidadeAsync(complexidadeAtual);
                mensagem = sucesso ? "Complexidade inserida com sucesso!" : "Erro ao inserir complexidade";
            }

            if (sucesso)
            {
                MessageBoxService.ShowSuccess(mensagem);
                LimparFormulario();
                await CarregarComplexidades();
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
            isProcessing = false;
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
                isEdicao = true;
                StateHasChanged();
            }
            else
            {
                MessageBoxService.ShowError("Complexidade não encontrada");
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
            isProcessing = true;
            StateHasChanged();

            var sucesso = await ComplexidadeService.ExcluirComplexidadeAsync(id);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Complexidade inativada com sucesso!");
                await CarregarComplexidades();
            }
            else
            {
                MessageBoxService.ShowError("Erro ao inativar complexidade");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao inativar complexidade: {ex.Message}");
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private void LimparFormulario()
    {
        complexidadeAtual = new ComplexidadeDto { ATV = 1 };
        isEdicao = false;
        StateHasChanged();
    }

    private bool ValidarFormulario()
    {
        if (string.IsNullOrWhiteSpace(complexidadeAtual.Complexidade))
        {
            MessageBoxService.ShowWarning("Favor preencher a Complexidade");
            return false;
        }

        if (complexidadeAtual.Peso <= 0)
        {
            MessageBoxService.ShowWarning("Favor preencher o Peso");
            return false;
        }

        if (complexidadeAtual.PesoPonderado <= 0)
        {
            MessageBoxService.ShowWarning("Favor preencher o Peso Ponderado");
            return false;
        }

        if (complexidadeAtual.Ponderacao <= 0)
        {
            MessageBoxService.ShowWarning("Favor preencher a Ponderação");
            return false;
        }

        if (complexidadeAtual.FaixaInicial < 0)
        {
            MessageBoxService.ShowWarning("Favor preencher a Faixa Inicial");
            return false;
        }

        if (complexidadeAtual.FaixaFinal <= 0)
        {
            MessageBoxService.ShowWarning("Favor preencher a Faixa Final");
            return false;
        }

        if (complexidadeAtual.FaixaInicial >= complexidadeAtual.FaixaFinal)
        {
            MessageBoxService.ShowWarning("A Faixa Inicial deve ser menor que a Faixa Final");
            return false;
        }

        return true;
    }
}