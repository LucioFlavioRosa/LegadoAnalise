using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Projetos;
using Peers.Moderno.Services.Clientes;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Services.Performance;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Peers.Moderno.Services.AutoAvaliacao;

public interface IAutoAvaliacaoService
{
    Task<AutoAvaliacaoPerformanceViewModel> CarregarDadosAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
    Task<bool> SalvarAvaliacaoAsync(AutoAvaliacaoPerformanceViewModel model, bool finalizarAvaliacao = false);
    Task<bool> FinalizarAvaliacaoAsync(AutoAvaliacaoPerformanceViewModel model);
    Task<List<PerformanceItemModel>> ObterPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<string> CalcularTempoRestanteAsync(int idAvaliacao);
    Task<bool> ValidarPermissoesAsync(int idAssociado, int idProjeto, int idPeriodo);
    Task<bool> PodeEditarAvaliacaoAsync(int idAssociado, int idProjeto, int idPeriodo);
}

public class AutoAvaliacaoService : IAutoAvaliacaoService
{
    private readonly IAssociadosService _associadosService;
    private readonly IProjetosService _projetosService;
    private readonly IClientesService _clientesService;
    private readonly IAvaliacoesService _avaliacoesService;
    private readonly IPerformanceService _performanceService;
    private readonly IUserContextService _userContextService;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IConfiguration _configuration;

    public AutoAvaliacaoService(
        IAssociadosService associadosService,
        IProjetosService projetosService,
        IClientesService clientesService,
        IAvaliacoesService avaliacoesService,
        IPerformanceService performanceService,
        IUserContextService userContextService,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService,
        IConfiguration configuration)
    {
        _associadosService = associadosService;
        _projetosService = projetosService;
        _clientesService = clientesService;
        _avaliacoesService = avaliacoesService;
        _performanceService = performanceService;
        _userContextService = userContextService;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
        _configuration = configuration;
    }

    public async Task<AutoAvaliacaoPerformanceViewModel> CarregarDadosAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
    {
        try
        {
            var model = new AutoAvaliacaoPerformanceViewModel();

            if (!await ValidarParametrosObrigatoriosAsync(idProjeto, idAssociado, idPeriodo))
            {
                return model;
            }

            var projeto = await _projetosService.ObterProjetoAsync(idProjeto);
            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            var gestor = await _associadosService.ObterAssociadoAsync(projeto.IdAssociadoGestor);
            var cliente = await _clientesService.ObterClienteAsync(projeto.IdCliente);
            var periodo = await _avaliacoesService.ObterPeriodoAsync(idPeriodo);

            model.IdProjeto = idProjeto;
            model.IdAssociado = idAssociado;
            model.IdPeriodo = idPeriodo;
            model.IdAvaliacao = idAvaliacao;
            model.NomeAssociado = associado.Nome;
            model.NomePeriodo = periodo.Periodo;
            model.NomeProjeto = projeto.Projeto;
            model.NomeGestor = gestor.Nome;
            model.NomeCliente = cliente.Cliente;
            model.TempoPeers = await CalcularTempoPeersAsync(associado.Id);
            model.TempoCargo = await CalcularTempoCargoAsync(associado.Id);
            model.TempoRestante = await CalcularTempoRestanteAsync(idAvaliacao);

            model.Performances = await ObterPerformancesAsync(idAssociado, idProjeto, idPeriodo);
            model.PodeEditar = await PodeEditarAvaliacaoAsync(idAssociado, idProjeto, idPeriodo);

            _telemetryService.TrackEvent("AutoAvaliacaoPerformanceCarregada", new Dictionary<string, string>
            {
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() },
                { "IdPeriodo", idPeriodo.ToString() },
                { "QuantidadePerformances", model.Performances.Count.ToString() }
            });

            return model;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarDadosAsync" },
                { "Component", "AutoAvaliacaoService" },
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> SalvarAvaliacaoAsync(AutoAvaliacaoPerformanceViewModel model, bool finalizarAvaliacao = false)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(model.IdAssociado);
            var usuarioLogado = await _userContextService.GetUsuarioLogadoAsync();

            foreach (var performance in model.Performances)
            {
                var avaliacao = await _avaliacoesService.ObterAvaliacaoPerformanceAsync(
                    model.IdAssociado, model.IdProjeto, performance.IdPerformance, model.IdPeriodo);

                if (avaliacao == null)
                {
                    avaliacao = CriarNovaAvaliacaoPerformance(model, performance, associado, usuarioLogado.Id);
                    var sucesso = await _avaliacoesService.SalvarAvaliacaoPerformanceAsync(avaliacao);
                    if (!sucesso) return false;
                }
                else
                {
                    AtualizarAvaliacaoPerformance(avaliacao, performance, finalizarAvaliacao);
                    var sucesso = await _avaliacoesService.AlterarAvaliacaoPerformanceAsync(avaliacao.IdAvaliacaoPerformance, avaliacao);
                    if (!sucesso) return false;
                }

                await AtualizarStatusAvaliacaoEmailAsync(model.IdAvaliacao, avaliacao, finalizarAvaliacao);
            }

            var mensagem = finalizarAvaliacao 
                ? _configuration["AutoAvaliacao:ValidationMessages:SucessoFinalizacao"]
                : _configuration["AutoAvaliacao:ValidationMessages:SucessoSalvamento"];
            
            _messageBoxService.ShowSuccess(mensagem);

            _telemetryService.TrackEvent("AutoAvaliacaoPerformanceSalva", new Dictionary<string, string>
            {
                { "IdProjeto", model.IdProjeto.ToString() },
                { "IdAssociado", model.IdAssociado.ToString() },
                { "Finalizada", finalizarAvaliacao.ToString() },
                { "QuantidadePerformances", model.Performances.Count.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SalvarAvaliacaoAsync" },
                { "Component", "AutoAvaliacaoService" },
                { "IdProjeto", model.IdProjeto.ToString() },
                { "IdAssociado", model.IdAssociado.ToString() }
            });

            var mensagem = finalizarAvaliacao 
                ? _configuration["AutoAvaliacao:ValidationMessages:ErroFinalizacao"]
                : _configuration["AutoAvaliacao:ValidationMessages:ErroSalvamento"];
            
            _messageBoxService.ShowError(mensagem);
            return false;
        }
    }

    public async Task<bool> FinalizarAvaliacaoAsync(AutoAvaliacaoPerformanceViewModel model)
    {
        return await SalvarAvaliacaoAsync(model, true);
    }

    public async Task<List<PerformanceItemModel>> ObterPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            var avaliacaoPerformance = await _avaliacoesService.ObterAvaliacaoPerformanceAsync(idAssociado, idProjeto, idPeriodo, true);

            List<Performance> performances;
            if (avaliacaoPerformance != null)
            {
                var listaPerformances = await _avaliacoesService.ObterAvaliacoesPerformancesAsync(idAssociado, idProjeto, idPeriodo, true);
                var listaIdPerformances = listaPerformances.Select(perf => perf.IdPerformance).ToList();
                performances = await _performanceService.ObterListaPerformancesAsync(associado.IdEmpresa, avaliacaoPerformance.IdCargo, associado.IdNivel, listaIdPerformances);
            }
            else
            {
                performances = await _performanceService.ObterListaPerformancesAsync(associado.IdEmpresa, associado.IdCargo, associado.IdNivel, null);
            }

            return await OrganizarPerformancesPorAbrangenciaAsync(performances, idAssociado, idProjeto, idPeriodo);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPerformancesAsync" },
                { "Component", "AutoAvaliacaoService" },
                { "IdAssociado", idAssociado.ToString() }
            });
            return new List<PerformanceItemModel>();
        }
    }

    public async Task<string> CalcularTempoRestanteAsync(int idAvaliacao)
    {
        try
        {
            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idAvaliacao);
            if (avaliacaoEmail?.PRAZOS == null || !avaliacaoEmail.DataLiberacao.HasValue)
            {
                return "N/A";
            }

            var prazo = avaliacaoEmail.PRAZOS;
            var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");
            
            return DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) 
                ? DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy")
                : dataFinal;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CalcularTempoRestanteAsync" },
                { "Component", "AutoAvaliacaoService" },
                { "IdAvaliacao", idAvaliacao.ToString() }
            });
            return "N/A";
        }
    }

    public async Task<bool> ValidarPermissoesAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        try
        {
            var usuarioLogado = await _userContextService.GetUsuarioLogadoAsync();
            if (usuarioLogado == null)
            {
                _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:PermissaoNegada"]);
                return false;
            }

            return await ValidarParametrosObrigatoriosAsync(idProjeto, idAssociado, idPeriodo);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPermissoesAsync" },
                { "Component", "AutoAvaliacaoService" }
            });
            return false;
        }
    }

    public async Task<bool> PodeEditarAvaliacaoAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        try
        {
            var performances = await ObterPerformancesAsync(idAssociado, idProjeto, idPeriodo);
            return performances.Any(p => p.PodeEditar);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "PodeEditarAvaliacaoAsync" },
                { "Component", "AutoAvaliacaoService" }
            });
            return false;
        }
    }

    private async Task<bool> ValidarParametrosObrigatoriosAsync(int idProjeto, int idAssociado, int idPeriodo)
    {
        if (idProjeto <= 0)
        {
            _messageBoxService.ShowInfo(_configuration["AutoAvaliacao:ValidationMessages:ProjetoObrigatorio"]);
            return false;
        }

        if (idAssociado <= 0)
        {
            _messageBoxService.ShowInfo(_configuration["AutoAvaliacao:ValidationMessages:AssociadoObrigatorio"]);
            return false;
        }

        if (idPeriodo <= 0)
        {
            _messageBoxService.ShowInfo(_configuration["AutoAvaliacao:ValidationMessages:PeriodoObrigatorio"]);
            return false;
        }

        return true;
    }

    private async Task<string> CalcularTempoPeersAsync(int idAssociado)
    {
        try
        {
            return await _associadosService.CalcularTempoPeersAsync(idAssociado);
        }
        catch
        {
            return "N/A";
        }
    }

    private async Task<string> CalcularTempoCargoAsync(int idAssociado)
    {
        try
        {
            return await _associadosService.CalcularTempoCargoAsync(idAssociado);
        }
        catch
        {
            return "N/A";
        }
    }

    private async Task<List<PerformanceItemModel>> OrganizarPerformancesPorAbrangenciaAsync(
        List<Performance> performances, int idAssociado, int idProjeto, int idPeriodo)
    {
        var listaPerformancesModel = new List<PerformanceItemModel>();
        var abrangenciasContadas = new List<string>();
        bool novaAbrangencia = true;
        string setAbrangencia = "";

        while (novaAbrangencia)
        {
            novaAbrangencia = false;
            setAbrangencia = "";

            foreach (var item in performances)
            {
                bool addItem = false;
                bool headerSeparador = false;

                if (!string.IsNullOrEmpty(setAbrangencia))
                {
                    if (item.Abrangencia == setAbrangencia)
                    {
                        addItem = true;
                        headerSeparador = false;
                    }
                }
                else if (!abrangenciasContadas.Contains(item.Abrangencia ?? ""))
                {
                    abrangenciasContadas.Add(item.Abrangencia ?? "");
                    setAbrangencia = item.Abrangencia ?? "";
                    novaAbrangencia = true;
                    headerSeparador = true;
                    addItem = true;
                }

                if (addItem)
                {
                    var performanceModel = await CriarPerformanceItemModelAsync(item, headerSeparador, idAssociado, idProjeto, idPeriodo);
                    listaPerformancesModel.Add(performanceModel);
                }
            }
        }

        return listaPerformancesModel;
    }

    private async Task<PerformanceItemModel> CriarPerformanceItemModelAsync(
        Performance performance, bool headerSeparador, int idAssociado, int idProjeto, int idPeriodo)
    {
        var model = new PerformanceItemModel
        {
            IdPerformance = performance.IdPerformance,
            Descricao = performance.Performance1,
            Abaixo = performance.PerformanceAbaixo,
            Esperado = performance.PerformanceEsperado,
            Acima = performance.PerformanceAcima,
            Abrangencia = performance.Abrangencia?.ToUpper() ?? "",
            MostrarSeparadorAbrangencia = headerSeparador,
            PodeEditar = performance.InputAutoavaliacao,
            DisclaimerInput = performance.InputAutoavaliacao ? "" : "Esta nota não requer preenchimento do avaliado"
        };

        var avaliacao = await _avaliacoesService.ObterAvaliacaoPerformanceAsync(idAssociado, idProjeto, performance.IdPerformance, idPeriodo);
        if (avaliacao != null)
        {
            model.NotaSelecionada = avaliacao.IdNotaNivel1AutoAvaliacao;
            model.Observacoes = avaliacao.ComentariosAutoAvaliacao;
            model.PodeEditar = model.PodeEditar && avaliacao.DataHoraFimAutoAvaliacao == null;
        }
        else
        {
            model.NotaSelecionada = performance.NotaPadraoAutoAvaliacao ?? 0;
        }

        return model;
    }

    private AvaliacaoPerformance CriarNovaAvaliacaoPerformance(
        AutoAvaliacaoPerformanceViewModel model, PerformanceItemModel performance, Associado associado, int usuarioId)
    {
        return new AvaliacaoPerformance
        {
            IdEmpresa = associado.IdEmpresa,
            IdAssociado = model.IdAssociado,
            USRAutoAvaliacao = model.IdAssociado,
            IdCargo = associado.IdCargo,
            IdNivel = associado.IdNivel,
            IdProjeto = model.IdProjeto,
            IdPeriodo = model.IdPeriodo,
            IdPerformance = performance.IdPerformance,
            IdAvaliacaoStatus = 2, // Em Andamento
            PosicaoAtualFluxoAvaliacao = "em_paralelo",
            DataHoraInicio = DateTime.Now,
            DHCAutoAvaliacao = DateTime.Now,
            USR = usuarioId,
            DHC = DateTime.Now,
            ATV = 1,
            IdNotaNivel1AutoAvaliacao = performance.NotaSelecionada,
            ComentariosAutoAvaliacao = performance.Observacoes?.Trim() ?? "",
            DataHoraInicioAutoAvaliacao = DateTime.Now
        };
    }

    private void AtualizarAvaliacaoPerformance(AvaliacaoPerformance avaliacao, PerformanceItemModel performance, bool finalizarAvaliacao)
    {
        avaliacao.IdNotaNivel1AutoAvaliacao = performance.NotaSelecionada;
        avaliacao.ComentariosAutoAvaliacao = performance.Observacoes?.Trim() ?? "";

        if (avaliacao.IdAvaliacaoStatus == 1) // Não Iniciada
        {
            avaliacao.IdAvaliacaoStatus = 2; // Em Andamento
            avaliacao.PosicaoAtualFluxoAvaliacao = "em_paralelo";
            if (avaliacao.DataHoraInicio == DateTime.MinValue)
                avaliacao.DataHoraInicio = DateTime.Now;
            avaliacao.DataHoraInicioAutoAvaliacao = DateTime.Now;
        }

        if (finalizarAvaliacao)
        {
            avaliacao.DataHoraFimAutoAvaliacao = DateTime.Now;
        }
    }

    private async Task AtualizarStatusAvaliacaoEmailAsync(int idAvaliacao, AvaliacaoPerformance avaliacao, bool finalizarAvaliacao)
    {
        var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idAvaliacao);
        if (avaliacaoEmail != null)
        {
            avaliacaoEmail.idStatus = avaliacao.IdAvaliacaoStatus;
            avaliacaoEmail.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;
            await _avaliacoesService.AlterarAvaliacaoEmailAsync(avaliacaoEmail.idAvaliacao, avaliacaoEmail);

            if (finalizarAvaliacao)
            {
                await _avaliacoesService.AvancaProximaEtapaPerformanceAsync(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
            }
        }
    }
}