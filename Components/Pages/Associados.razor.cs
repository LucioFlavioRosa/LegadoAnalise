// Code-behind opcional para lógica adicional da página de Associados. Lógica principal está no arquivo .razor.

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.Collections.Generic;
using Peers.Moderno.Models;
using Peers.Moderno.Services;
using Peers.Moderno.Components.Shared;

namespace Peers.Moderno.Components.Pages
{
    public partial class Associados : ComponentBase
    {
        [Inject] public IAssociadosService AssociadosService { get; set; }
        [Inject] public ICargosService CargosService { get; set; }
        [Inject] public IPerfisService PerfisService { get; set; }
        [Inject] public IVerticalService VerticalService { get; set; }
        [Inject] public IFotosAssociadosService FotosAssociadosService { get; set; }

        protected MessageBox messageBox;

        public ASSOCIADOS AssociadoSelecionado { get; set; } = new ASSOCIADOS();
        public List<PERFIS> Perfis { get; set; } = new();
        public List<CARGOS> Cargos { get; set; } = new();
        public List<VERTICAL> Verticais { get; set; } = new();
        public List<ASSOCIADOS> Mentores { get; set; } = new();
        public List<PROMOCOES> PromocoesAssociado { get; set; } = new();
        public List<ASSOCIADOS> ListaAssociados { get; set; } = new();
        public string FotoBase64 { get; set; }
        public bool IsPromocaoChecked { get; set; }
        public System.DateTime? DataAdmissao { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CarregarCombosEGrids();
            await LimpaCampos();
        }

        private async Task CarregarCombosEGrids()
        {
            Perfis = await PerfisService.ObterListaPerfisAsync(true);
            Cargos = await CargosService.ObterListaCargosAsync(true);
            Verticais = await VerticalService.ListarVerticaisAsync();
            Mentores = await AssociadosService.ObterAssociadosAsync(true);
            ListaAssociados = await AssociadosService.ObterAssociadosAsync();
        }

        private async Task LimpaCampos()
        {
            AssociadoSelecionado = new ASSOCIADOS();
            FotoBase64 = null;
            IsPromocaoChecked = false;
            DataAdmissao = null;
            PromocoesAssociado = new List<PROMOCOES>();
        }

        private async Task OnAssociadoSalvo()
        {
            await CarregarCombosEGrids();
            await LimpaCampos();
            await messageBox.ShowAsync("Associado salvo com sucesso!", MessageBoxType.Info);
        }

        private async Task OnFotoUpload(string base64)
        {
            FotoBase64 = base64;
            StateHasChanged();
        }

        private async Task OnPromocaoCheckedChanged(bool isChecked)
        {
            IsPromocaoChecked = isChecked;
        }

        private async Task OnDataAdmissaoChanged(System.DateTime? data)
        {
            DataAdmissao = data;
        }

        private async Task OnComentarioEditado(int idPromocao, string comentario)
        {
            await CargosService.AlterarPromocaoComentarioAsync(idPromocao, comentario);
            await messageBox.ShowAsync("Comentário atualizado com sucesso!", MessageBoxType.Info);
        }

        private async Task OnAssociadosImportados()
        {
            await CarregarCombosEGrids();
            await messageBox.ShowAsync("Associados importados com sucesso!", MessageBoxType.Info);
        }

        private async Task OnPromocoesImportadas()
        {
            await CarregarCombosEGrids();
            await messageBox.ShowAsync("Promoções importadas com sucesso!", MessageBoxType.Info);
        }

        private async Task OnAlterarAssociado(int idAssociado)
        {
            var associado = await AssociadosService.ObterAssociadoAsync(idAssociado);
            if (associado != null)
            {
                AssociadoSelecionado = associado;
                PromocoesAssociado = await CargosService.ObterTodasPromocoesAssociadoAsync(idAssociado);
                FotoBase64 = (await FotosAssociadosService.ObterFotoPorAssociadoAsync(idAssociado))?.Imagem;
                DataAdmissao = associado.DataAdmissao;
                IsPromocaoChecked = false;
                StateHasChanged();
            }
        }

        private async Task OnInativarAssociado(int idAssociado)
        {
            var associado = await AssociadosService.ObterAssociadoAsync(idAssociado);
            if (associado != null)
            {
                associado.ATV = 0;
                associado.IdStatus = 0;
                await AssociadosService.ExcluiAssociadoAsync(idAssociado, associado);
                await CarregarCombosEGrids();
                await messageBox.ShowAsync("Associado inativado com sucesso!", MessageBoxType.Info);
            }
        }
    }
}