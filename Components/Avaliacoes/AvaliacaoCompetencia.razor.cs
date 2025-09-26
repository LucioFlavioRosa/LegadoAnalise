using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components.Avaliacoes;

public partial class AvaliacaoCompetencia : ComponentBase
{
    [Inject] public IAvaliacaoCompetenciaService AvaliacaoCompetenciaService { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;

    [Parameter] public int IdProjeto { get; set; }
    [Parameter] public int IdAssociado { get; set; }
    [Parameter] public int IdPeriodo { get; set; }
    [Parameter] public string TipoAvaliacao { get; set; } = string.Empty;
    [Parameter] public string Escopo { get; set; } = string.Empty;
    [Parameter] public int IdGestor { get; set; }

    public AvaliacaoCompetenciaDto? Avaliacao { get; set; }
    public List<CompetenciaLinhaDto> Linhas { get; set; } = new();
    public bool IsLoading { get; set; } = true;
    public bool IsSaving { get; set; } = false;
    public string MensagemErro { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        Avaliacao = await AvaliacaoCompetenciaService.ObterAvaliacaoCompetenciaAsync(IdProjeto, IdAssociado, IdPeriodo, TipoAvaliacao, Escopo, IdGestor);
        if (Avaliacao != null)
        {
            Linhas = await AvaliacaoCompetenciaService.ObterLinhasCompetenciaAsync(Avaliacao);
        }
        else
        {
            MessageBoxService.ShowError("Avaliação não encontrada.");
        }
        IsLoading = false;
    }

    public async Task SalvarAsync(bool finalizar = false)
    {
        IsSaving = true;
        if (Avaliacao == null)
        {
            MessageBoxService.ShowError("Avaliação não encontrada.");
            IsSaving = false;
            return;
        }
        if (!await AvaliacaoCompetenciaService.ValidarAvaliacaoAsync(Avaliacao, Linhas, out var mensagemErro))
        {
            MessageBoxService.ShowError(mensagemErro);
            IsSaving = false;
            return;
        }
        var sucesso = await AvaliacaoCompetenciaService.SalvarAvaliacaoAsync(Avaliacao, Linhas, finalizar);
        if (sucesso)
        {
            MessageBoxService.ShowSuccess(finalizar ? "Avaliação Finalizada com Sucesso" : "Avaliação Salva com Sucesso.");
        }
        else
        {
            MessageBoxService.ShowError("Falha ao salvar avaliação.");
        }
        IsSaving = false;
    }
}
