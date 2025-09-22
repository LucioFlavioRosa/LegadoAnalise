using Business.Model;
using Business.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Peers.Moderno.Services;

namespace Peers.Moderno.Components.Pages
{
    public partial class EvolucaoAssociado : ComponentBase
    {
        private AssociadoInfo? associadoInfo;
        private List<EVOLUCAOASSOCIADO>? resultadoAssociado;
        private string tipoAvaliacaoSelecionado = "[Selecionar]";
        private string escopoSelecionado = "[Selecionar]";
        private bool isLoading = false;
        private string mensagemErro = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var usuarioLogado = GetUsuarioLogado();
                if (usuarioLogado != null)
                {
                    associadoInfo = await AssociadosService.ObterAssociadoMentorCargoAsync(usuarioLogado.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao carregar dados iniciais");
                mensagemErro = "Erro ao carregar dados iniciais: " + ex.Message;
            }
        }

        private async Task GerarEvolucao()
        {
            if (tipoAvaliacaoSelecionado == "[Selecionar]" || escopoSelecionado == "[Selecionar]")
            {
                mensagemErro = "Selecione o Tipo de Avaliações e Escopo";
                return;
            }

            try
            {
                isLoading = true;
                mensagemErro = string.Empty;
                StateHasChanged();

                var usuarioLogado = GetUsuarioLogado();
                if (usuarioLogado == null)
                {
                    mensagemErro = "Usuário não encontrado";
                    return;
                }

                resultadoAssociado = await EvolucaoService.EvolucaoAsync(
                    usuarioLogado.Id, 
                    tipoAvaliacaoSelecionado, 
                    escopoSelecionado);

                if (resultadoAssociado?.Any() == true && associadoInfo != null)
                {
                    var jsonRadar = await ViewModel.GerarJsonRadarAsync(resultadoAssociado, associadoInfo.IdCargo);
                    await JSRuntime.InvokeVoidAsync("renderRadarChart", jsonRadar);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao gerar evolução");
                mensagemErro = "Erro ao gerar evolução: " + ex.Message;
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private UsuarioLogado? GetUsuarioLogado()
        {
            return new UsuarioLogado { Id = 1, IdCargo = 1 };
        }
    }

    public class UsuarioLogado
    {
        public int Id { get; set; }
        public int IdCargo { get; set; }
    }
}