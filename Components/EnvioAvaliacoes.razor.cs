using Microsoft.AspNetCore.Components;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Components;

public partial class EnvioAvaliacoes : ComponentBase
{
    [Inject] private IEnvioAvaliacoesService EnvioAvaliacoesService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;

    private FiltrosEnvioAvaliacao filtros = new();
    private List<ProjetoModel> avaliacoes = new();
    private List<PendenciaModel> pendencias = new();
    private List<PeriodoAvaliacao> periodos = new();
    private List<Projeto> projetos = new();
    private List<Cliente> clientes = new();
    private List<ProjetoStatus> statusList = new();
    private List<Associado> associados = new();
    private List<Prazo> disparos = new();
    private List<string> listaIdsEmails = new();

    private bool carregandoLista = false;
    private bool carregandoEnvioTodas = false;
    private bool carregandoEnvioUnico = false;
    private bool carregandoRedisparo = false;
    private bool mostrarBotaoEnviarTodas = false;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDadosIniciaisAsync();
        await CarregarPendenciasAsync();
    }

    private async Task CarregarDadosIniciaisAsync()
    {
        try
        {
            periodos = await EnvioAvaliacoesService.ObterPeriodosAsync();
            projetos = await EnvioAvaliacoesService.ObterProjetosAsync();
            clientes = await EnvioAvaliacoesService.ObterClientesAsync();
            statusList = await EnvioAvaliacoesService.ObterStatusAsync();
            associados = await EnvioAvaliacoesService.ObterAssociadosAsync();
            disparos = await EnvioAvaliacoesService.ObterDisparosAsync();

            if (disparos.Any())
            {
                filtros.IdDisparo = disparos.First().IdPrazo;
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar dados iniciais: {ex.Message}");
        }
    }

    private async Task ListarAvaliacoes()
    {
        if (!filtros.IdPeriodo.HasValue)
        {
            MessageBoxService.ShowWarning("Selecione o Período");
            return;
        }

        carregandoLista = true;
        StateHasChanged();

        try
        {
            avaliacoes = await EnvioAvaliacoesService.ListarAvaliacoesAsync(filtros);
            
            listaIdsEmails.Clear();
            foreach (var projeto in avaliacoes)
            {
                foreach (var associado in projeto.Associados)
                {
                    listaIdsEmails.Add(associado.IdEmail);
                    associado.DisparoSelecionado = filtros.IdDisparo;
                }
            }

            mostrarBotaoEnviarTodas = avaliacoes.Any();

            if (!avaliacoes.Any())
            {
                MessageBoxService.ShowWarning("Sua consulta não encontrou Avaliações disponíveis!");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao listar avaliações: {ex.Message}");
        }
        finally
        {
            carregandoLista = false;
            StateHasChanged();
        }
    }

    private async Task EnviarTodasAvaliacoes()
    {
        if (!listaIdsEmails.Any())
        {
            MessageBoxService.ShowWarning("Não existem Avaliações a serem Geradas/Enviadas.");
            return;
        }

        carregandoEnvioTodas = true;
        StateHasChanged();

        try
        {
            var resultado = await EnvioAvaliacoesService.EnviarTodasAvaliacoesAsync(listaIdsEmails, filtros.IdDisparo);
            
            var mensagem = $"Resumo do Processo de Geração/Envio de Avaliações:<br/><b>{resultado.enviadas}</b> Avaliações Enviadas";
            
            if (resultado.falhas > 0)
            {
                mensagem += $"<br/><br/><font color=red>{resultado.falhas}</font> Avaliações não foram Geradas";
            }
            
            if (resultado.naoEnviadas > 0)
            {
                mensagem += $"<br/><br/><font color=blue>{resultado.naoEnviadas}</font> Avaliações não foram Enviadas";
            }

            if (!string.IsNullOrEmpty(resultado.detalhes))
            {
                mensagem += resultado.detalhes;
            }

            MessageBoxService.ShowInfo(mensagem);
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao enviar avaliações: {ex.Message}");
        }
        finally
        {
            carregandoEnvioTodas = false;
            StateHasChanged();
        }
    }

    private async Task EnviarAvaliacaoUnica(string idCompleto, int idDisparo)
    {
        carregandoEnvioUnico = true;
        StateHasChanged();

        try
        {
            var partes = idCompleto.Split(';');
            if (partes.Length < 6)
            {
                MessageBoxService.ShowError("Dados da avaliação inválidos.");
                return;
            }

            var request = new EnvioAvaliacaoRequest
            {
                IdProjeto = int.Parse(partes[0]),
                IdAssociado = int.Parse(partes[1]),
                IdPeriodo = int.Parse(partes[2]),
                TipoAvaliacao = partes[3],
                Escopo = partes[4],
                IdGestor = int.Parse(partes[5]),
                IdPrazo = idDisparo
            };

            var resultado = await EnvioAvaliacoesService.EnviarAvaliacaoAsync(request);

            switch (resultado.Status)
            {
                case "OK":
                    MessageBoxService.ShowSuccess("Avaliação Gerada e Enviada com Sucesso.");
                    break;
                case "Fail":
                    MessageBoxService.ShowError("Ocorreu uma Falha ao Gerar a Avaliação.");
                    break;
                case "NotSend":
                    MessageBoxService.ShowWarning("A Avaliação já havia sido Gerada e Enviada anteriormente.");
                    break;
                case "SendNotEmail":
                    MessageBoxService.ShowWarning("A Avaliação foi Gerada. Ocorreu erro no envio do E-mail");
                    break;
                case "NoWorkflow":
                    MessageBoxService.ShowError("Não existe Workflow cadastrado para o Período selecionado.");
                    break;
                default:
                    MessageBoxService.ShowError(resultado.Mensagem);
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao enviar avaliação: {ex.Message}");
        }
        finally
        {
            carregandoEnvioUnico = false;
            StateHasChanged();
        }
    }

    private async Task CarregarPendenciasAsync()
    {
        try
        {
            pendencias = await EnvioAvaliacoesService.ListarPendenciasAsync();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar pendências: {ex.Message}");
        }
    }

    private async Task RedispararPendencias()
    {
        carregandoRedisparo = true;
        StateHasChanged();

        try
        {
            var resultado = await EnvioAvaliacoesService.RedispararPendenciasAsync();
            
            if (resultado.falhas == 0)
            {
                MessageBoxService.ShowSuccess("E-mails de pendências enviados com sucesso!");
            }
            else
            {
                if (resultado.sucesso > 0)
                {
                    var mensagem = $"E-mail enviado parcialmente!<br/>Total enviado: {resultado.sucesso}<br/>Total com falha: {resultado.falhas}";
                    MessageBoxService.ShowWarning(mensagem);
                }
                else
                {
                    MessageBoxService.ShowError("Falha ao enviar e-mails de pendências.");
                }
            }

            await CarregarPendenciasAsync();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao redisparar pendências: {ex.Message}");
        }
        finally
        {
            carregandoRedisparo = false;
            StateHasChanged();
        }
    }
}

public static class ProjetosAssociadosModelExtensions
{
    public static int DisparoSelecionado { get; set; }
}