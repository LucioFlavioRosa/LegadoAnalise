using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Peers.Moderno.Services.AvaliacoesGestor;
using Peers.Moderno.Services.AvaliacoesGestor.Common;
using Peers.Moderno.Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Components.AvaliacoesGestor
{
    public partial class AvaliacoesGestor : ComponentBase
    {
        [Inject] public IAvaliacoesGestorService AvaliacoesGestorService { get; set; } = default!;
        [Inject] public IAvaliacoesGestorHelper AvaliacoesGestorHelper { get; set; } = default!;
        [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;

        protected string TituloPagina { get; set; } = "AVALIAÇÃO DO GESTOR";
        protected string SubTituloPagina { get; set; } = "Avaliação / Gestor";

        protected AvaliacoesGestorFiltroModel Filtro { get; set; } = new();
        protected List<ComboItem> ProjetosCombo { get; set; } = new();
        protected List<ComboItem> ClientesCombo { get; set; } = new();
        protected List<ComboItem> PeriodosCombo { get; set; } = new();
        protected List<ComboItem> StatusCombo { get; set; } = new();
        protected List<ComboItem> EtapasCombo { get; set; } = new();

        protected List<ProjetoModel> ProjetosAvaliacoes { get; set; } = new();
        protected List<ProjetoModel> AvaliacoesLiderados { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await CarregarCombosAsync();
            await BuscarAvaliacoes();
        }

        protected async Task CarregarCombosAsync()
        {
            ProjetosCombo = await AvaliacoesGestorHelper.GetProjetosComboAsync();
            ClientesCombo = await AvaliacoesGestorHelper.GetClientesComboAsync();
            PeriodosCombo = await AvaliacoesGestorHelper.GetPeriodosComboAsync();
            StatusCombo = await AvaliacoesGestorHelper.GetStatusComboAsync();
            EtapasCombo = AvaliacoesGestorHelper.GetEtapasCombo();
        }

        protected async Task BuscarAvaliacoes()
        {
            ProjetosAvaliacoes = await AvaliacoesGestorService.BuscarProjetosAvaliacoesAsync(Filtro);
            AvaliacoesLiderados = await AvaliacoesGestorService.BuscarAvaliacoesLideradosAsync(Filtro);
        }

        protected async Task FinalizarAvaliacao(string idAvaliacao)
        {
            var resultado = await AvaliacoesGestorService.FinalizarAvaliacaoAsync(idAvaliacao);
            if (resultado.Sucesso)
            {
                MessageBoxService.ShowSuccess(resultado.Mensagem);
                await BuscarAvaliacoes();
            }
            else
            {
                MessageBoxService.ShowWarning(resultado.Mensagem);
            }
        }

        protected async Task LiberarLider(string idAvaliacao)
        {
            var resultado = await AvaliacoesGestorService.LiberarLiderAsync(idAvaliacao);
            if (resultado.Sucesso)
            {
                MessageBoxService.ShowSuccess(resultado.Mensagem);
                await BuscarAvaliacoes();
            }
            else
            {
                MessageBoxService.ShowWarning(resultado.Mensagem);
            }
        }
    }
}
