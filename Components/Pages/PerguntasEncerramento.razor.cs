using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Components.Pages;

public partial class PerguntasEncerramento : ComponentBase
{
    [Inject] public IPerguntasEncerramentoService PerguntasService { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;

    private List<PerguntaEncerramento> perguntas = new();
    private List<PerguntaEncerramento> perguntasFiltradas = new();
    private PerguntaEncerramentoModel perguntaAtual = new();
    private PerguntaEncerramento? perguntaParaRemover;
    
    private bool isLoading = false;
    private bool isLoadingList = true;
    private bool modoEdicao = false;
    private bool showDeleteConfirmation = false;
    private string filtroTexto = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CarregarPerguntas();
    }

    private async Task CarregarPerguntas()
    {
        try
        {
            isLoadingList = true;
            StateHasChanged();
            
            perguntas = await PerguntasService.ListarAsync();
            perguntasFiltradas = perguntas;
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar perguntas: {ex.Message}");
        }
        finally
        {
            isLoadingList = false;
            StateHasChanged();
        }
    }

    private async Task SalvarPergunta()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            if (await ValidarPergunta())
            {
                var pergunta = new PerguntaEncerramento
                {
                    Id = perguntaAtual.Id,
                    Codigo = perguntaAtual.Codigo.Trim(),
                    Descricao = perguntaAtual.Descricao.Trim(),
                    PossuiComentario = perguntaAtual.PossuiComentario
                };

                bool sucesso;
                if (modoEdicao)
                {
                    sucesso = await PerguntasService.AtualizarAsync(pergunta);
                    if (sucesso)
                    {
                        MessageBoxService.ShowSuccess("Pergunta atualizada com sucesso!");
                    }
                }
                else
                {
                    sucesso = await PerguntasService.AdicionarAsync(pergunta);
                    if (sucesso)
                    {
                        MessageBoxService.ShowSuccess("Pergunta cadastrada com sucesso!");
                    }
                }

                if (sucesso)
                {
                    LimparFormulario();
                    await CarregarPerguntas();
                }
                else
                {
                    MessageBoxService.ShowError("Erro ao salvar pergunta. Tente novamente.");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao salvar pergunta: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task<bool> ValidarPergunta()
    {
        if (string.IsNullOrWhiteSpace(perguntaAtual.Codigo))
        {
            MessageBoxService.ShowError("O código é obrigatório.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(perguntaAtual.Descricao))
        {
            MessageBoxService.ShowError("A descrição da pergunta é obrigatória.");
            return false;
        }

        var codigoExiste = await PerguntasService.ExisteCodigoAsync(perguntaAtual.Codigo.Trim(), modoEdicao ? perguntaAtual.Id : null);
        if (codigoExiste)
        {
            MessageBoxService.ShowError("Já existe uma pergunta com este código.");
            return false;
        }

        return true;
    }

    private void EditarPergunta(PerguntaEncerramento pergunta)
    {
        perguntaAtual = new PerguntaEncerramentoModel
        {
            Id = pergunta.Id,
            Codigo = pergunta.Codigo,
            Descricao = pergunta.Descricao,
            PossuiComentario = pergunta.PossuiComentario
        };
        modoEdicao = true;
        StateHasChanged();
    }

    private void RemoverPergunta(int id)
    {
        perguntaParaRemover = perguntas.FirstOrDefault(p => p.Id == id);
        if (perguntaParaRemover != null)
        {
            showDeleteConfirmation = true;
            StateHasChanged();
        }
    }

    private async Task ConfirmarRemocao()
    {
        if (perguntaParaRemover == null) return;

        try
        {
            isLoading = true;
            StateHasChanged();

            var sucesso = await PerguntasService.RemoverAsync(perguntaParaRemover.Id);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Pergunta removida com sucesso!");
                await CarregarPerguntas();
            }
            else
            {
                MessageBoxService.ShowError("Erro ao remover pergunta. Tente novamente.");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao remover pergunta: {ex.Message}");
        }
        finally
        {
            CancelarRemocao();
            isLoading = false;
            StateHasChanged();
        }
    }

    private void CancelarRemocao()
    {
        showDeleteConfirmation = false;
        perguntaParaRemover = null;
        StateHasChanged();
    }

    private void LimparFormulario()
    {
        perguntaAtual = new PerguntaEncerramentoModel();
        modoEdicao = false;
        StateHasChanged();
    }

    private void CancelarEdicao()
    {
        LimparFormulario();
    }

    private void FiltrarPerguntas()
    {
        if (string.IsNullOrWhiteSpace(filtroTexto))
        {
            perguntasFiltradas = perguntas;
        }
        else
        {
            var filtro = filtroTexto.ToLower();
            perguntasFiltradas = perguntas.Where(p =>
                p.Codigo.ToLower().Contains(filtro) ||
                p.Descricao.ToLower().Contains(filtro) ||
                (p.PossuiComentario ? "sim" : "não").Contains(filtro)
            ).ToList();
        }
        StateHasChanged();
    }

    public class PerguntaEncerramentoModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O código é obrigatório")]
        [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres")]
        public string Codigo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
        public string Descricao { get; set; } = string.Empty;
        
        public bool PossuiComentario { get; set; } = false;
    }
}