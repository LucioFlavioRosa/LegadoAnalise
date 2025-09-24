using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Projetos.Common;

namespace Peers.Moderno.Components.Projetos;

public partial class TiposProjetos : ComponentBase
{
    [Inject] private ITiposProjetosService TiposProjetosService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;

    private List<TipoProjeto> TiposProjetos { get; set; } = new();
    private TipoProjeto TipoProjetoAtual { get; set; } = new();
    private bool IsCarregando { get; set; } = true;
    private bool IsProcessing { get; set; } = false;
    private bool IsEdicao => TipoProjetoAtual.IdTipo > 0;

    protected override async Task OnInitializedAsync()
    {
        await CarregarTiposProjetos();
    }

    private async Task CarregarTiposProjetos()
    {
        try
        {
            IsCarregando = true;
            TiposProjetos = await TiposProjetosService.ListarAsync();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar tipos de projeto: {ex.Message}");
        }
        finally
        {
            IsCarregando = false;
            StateHasChanged();
        }
    }

    private async Task SalvarTipoProjeto()
    {
        if (string.IsNullOrWhiteSpace(TipoProjetoAtual.Nome))
        {
            MessageBoxService.ShowWarning("Preencha o campo Tipo de Projeto");
            return;
        }

        try
        {
            IsProcessing = true;
            StateHasChanged();

            var existeNome = await TiposProjetosService.ExisteNomeAsync(
                TipoProjetoAtual.Nome, 
                IsEdicao ? TipoProjetoAtual.IdTipo : null);

            if (existeNome)
            {
                MessageBoxService.ShowWarning("Já existe um tipo de projeto com este nome");
                return;
            }

            bool sucesso;
            string mensagem;

            if (IsEdicao)
            {
                sucesso = await TiposProjetosService.AlterarAsync(TipoProjetoAtual);
                mensagem = "Tipo de projeto alterado com sucesso!!";
            }
            else
            {
                sucesso = await TiposProjetosService.InserirAsync(TipoProjetoAtual);
                mensagem = "Tipo de Projeto Inserido com sucesso!!";
            }

            if (sucesso)
            {
                MessageBoxService.ShowSuccess(mensagem);
                await LimparFormulario();
                await CarregarTiposProjetos();
            }
            else
            {
                MessageBoxService.ShowError("Erro ao salvar tipo de projeto");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao salvar: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    private void EditarTipoProjeto(TipoProjeto tipo)
    {
        TipoProjetoAtual = new TipoProjeto
        {
            IdTipo = tipo.IdTipo,
            Nome = tipo.Nome,
            ATV = tipo.ATV
        };
        StateHasChanged();
    }

    private async Task InativarTipoProjeto(int id)
    {
        try
        {
            IsProcessing = true;
            StateHasChanged();

            var sucesso = await TiposProjetosService.InativarAsync(id);

            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Tipo de Projeto Inativado com sucesso!!");
                await CarregarTiposProjetos();
            }
            else
            {
                MessageBoxService.ShowError("Erro ao Excluir um Tipo de Projeto!!");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao inativar: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    private async Task CancelarEdicao()
    {
        await LimparFormulario();
    }

    private async Task LimparFormulario()
    {
        TipoProjetoAtual = new TipoProjeto();
        StateHasChanged();
        await Task.CompletedTask;
    }
}