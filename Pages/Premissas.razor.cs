using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Peers.Moderno.Services;

namespace Peers.Moderno.Pages;

public partial class Premissas : ComponentBase
{
    [Inject] private IPremissasService PremissasService { get; set; } = default!;
    
    private List<PremissasRadar> premissas = new();
    private List<Eixo> eixos = new();
    private List<Cargo> cargos = new();
    private List<CargoNivel> niveis = new();
    
    private PremissasRadar premissaAtual = new();
    private bool modoEdicao = false;
    private bool salvando = false;
    private bool carregandoLista = true;
    
    private string mensagem = string.Empty;
    private string tipoMensagem = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        carregandoLista = true;
        StateHasChanged();
        
        try
        {
            var tasks = new Task[4]
            {
                CarregarPremissas(),
                CarregarEixos(),
                CarregarCargos(),
                CarregarNiveis()
            };
            
            await Task.WhenAll(tasks);
        }
        finally
        {
            carregandoLista = false;
            StateHasChanged();
        }
    }

    private async Task CarregarPremissas()
    {
        premissas = await PremissasService.ObterListaAsync();
    }

    private async Task CarregarEixos()
    {
        eixos = await PremissasService.ObterEixosAsync();
    }

    private async Task CarregarCargos()
    {
        cargos = await PremissasService.ObterCargosAsync();
    }

    private async Task CarregarNiveis()
    {
        niveis = await PremissasService.ObterNiveisAsync();
    }

    private async Task SalvarPremissa()
    {
        if (!ValidarCampos())
        {
            ExibirMensagem("Preencha todos os campos obrigatórios.", "warning");
            return;
        }

        salvando = true;
        StateHasChanged();

        try
        {
            bool sucesso;
            
            if (modoEdicao)
            {
                premissaAtual.DHC = DateTime.Now;
                sucesso = await PremissasService.AlterarAsync(premissaAtual);
                
                if (sucesso)
                {
                    ExibirMensagem("Premissa alterada com sucesso.", "success");
                    CancelarEdicao();
                }
                else
                {
                    ExibirMensagem("Houve uma falha na tentativa de alteração.", "error");
                }
            }
            else
            {
                premissaAtual.IdEmpresa = 1; // TODO: Obter da sessão do usuário
                premissaAtual.USR = 1; // TODO: Obter da sessão do usuário
                premissaAtual.DHC = DateTime.Now;
                premissaAtual.ATV = 1;
                
                sucesso = await PremissasService.InserirAsync(premissaAtual);
                
                if (sucesso)
                {
                    ExibirMensagem("Premissa cadastrada com sucesso.", "success");
                    LimparFormulario();
                }
                else
                {
                    ExibirMensagem("Houve uma falha na tentativa de cadastro.", "error");
                }
            }
            
            if (sucesso)
            {
                await CarregarPremissas();
            }
        }
        catch (Exception)
        {
            ExibirMensagem("Erro inesperado ao salvar a premissa.", "error");
        }
        finally
        {
            salvando = false;
            StateHasChanged();
        }
    }

    private async Task EditarPremissa(int id)
    {
        var premissa = await PremissasService.ObterAsync(id);
        if (premissa != null)
        {
            premissaAtual = premissa;
            modoEdicao = true;
            StateHasChanged();
        }
    }

    private async Task InativarPremissa(int id)
    {
        var sucesso = await PremissasService.InativarAsync(id);
        
        if (sucesso)
        {
            ExibirMensagem("Status da premissa alterado com sucesso.", "success");
            await CarregarPremissas();
        }
        else
        {
            ExibirMensagem("Erro ao alterar o status da premissa.", "error");
        }
    }

    private void CancelarEdicao()
    {
        modoEdicao = false;
        LimparFormulario();
    }

    private void LimparFormulario()
    {
        premissaAtual = new PremissasRadar();
        StateHasChanged();
    }

    private bool ValidarCampos()
    {
        return premissaAtual.IdEixo > 0 &&
               premissaAtual.IdCargo > 0 &&
               premissaAtual.IdNivel > 0 &&
               premissaAtual.ValorRadarPeers > 0 &&
               premissaAtual.ValorBaseAutoAvaliacao > 0 &&
               premissaAtual.ValorBaseAvaliacaoGestor > 0;
    }

    private void ExibirMensagem(string msg, string tipo)
    {
        mensagem = msg;
        tipoMensagem = tipo;
        StateHasChanged();
        
        // Limpar mensagem após 5 segundos
        Task.Delay(5000).ContinueWith(_ =>
        {
            InvokeAsync(() =>
            {
                mensagem = string.Empty;
                tipoMensagem = string.Empty;
                StateHasChanged();
            });
        });
    }
}