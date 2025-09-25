using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Projetos;
using Peers.Moderno.Services.Clientes;
using Peers.Moderno.Services.Performance;
using Peers.Moderno.Services.Avaliacoes;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Peers.Moderno.Services.AutoAvaliacao;

public interface IAutoAvaliacaoPerformanceService
{
    Task<AutoAvaliacaoPerformanceViewModel> CarregarDadosIniciais(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
    Task<List<PerformanceItemViewModel>> CarregarPerformances(int idAssociado, int idProjeto, int idPeriodo);
    Task<bool> SalvarAvaliacao(int idProjeto, int idAssociado, int idPeriodo, List<PerformanceAvaliacaoDto> avaliacoes, bool finalizarAvaliacao = false);
    Task<bool> ValidarDadosObrigatorios(int idProjeto, int idAssociado, int idPeriodo);
    Task<string> CalcularTempoRestante(int idAvaliacao);
    Task<bool> VerificarPermissaoEdicao(int idAssociado, int idProjeto, int idPeriodo);
    string TruncarTexto(string texto, int qtdCaracter);
}

public class AutoAvaliacaoPerformanceService : IAutoAvaliacaoPerformanceService
{
    private readonly IAssociadosService _associadosService;
    private readonly IProjetosService _projetosService;
    private readonly IClientesService _clientesService;
    private readonly IPerformanceService _performanceService;
    private readonly IAvaliacoesService _avaliacoesService;
    private readonly IUserContextService _userContextService;
    private readonly ITelemetryService _telemetryService;
    private readonly IConfiguration _configuration;
    private readonly IMessageBoxService _messageBoxService;

    public AutoAvaliacaoPerformanceService(
        IAssociadosService associadosService,
        IProjetosService projetosService,
        IClientesService clientesService,
        IPerformanceService performanceService,
        IAvaliacoesService avaliacoesService,
        IUserContextService userContextService,
        ITelemetryService telemetryService,
        IConfiguration configuration,
        IMessageBoxService messageBoxService)
    {
        _associadosService = associadosService;
        _projetosService = projetosService;
        _clientesService = clientesService;
        _performanceService = performanceService;
        _avaliacoesService = avaliacoesService;
        _userContextService = userContextService;
        _telemetryService = telemetryService;
        _configuration = configuration;
        _messageBoxService = messageBoxService;
    }

    public async Task<AutoAvaliacaoPerformanceViewModel> CarregarDadosIniciais(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
    {
        try
        {
            var projeto = await _projetosService.ObterProjetoAsync(idProjeto);
            if (projeto == null)
            {
                throw new InvalidOperationException(_configuration["AutoAvaliacao:ValidationMessages:ProjetoNaoEncontrado"]);
            }

            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            if (associado == null)
            {
                throw new InvalidOperationException(_configuration["AutoAvaliacao:ValidationMessages:AssociadoNaoEncontrado"]);
            }

            var periodo = await _avaliacoesService.ObterPeriodoAsync(idPeriodo);
            if (periodo == null)
            {
                throw new InvalidOperationException(_configuration["AutoAvaliacao:ValidationMessages:PeriodoNaoEncontrado"]);
            }

            var gestor = await _associadosService.ObterAssociadoAsync(projeto.IdAssociadoGestor);
            var cliente = await _clientesService.ObterClienteAsync(projeto.IdCliente);

            var tempoRestante = await CalcularTempoRestante(idAvaliacao);
            var tempoPeers = await _associadosService.CalcularTempoPeersAsync(idAssociado);
            var tempoCargo = await _associadosService.CalcularTempoCargoAsync(idAssociado);

            var viewModel = new AutoAvaliacaoPerformanceViewModel
            {
                IdProjeto = idProjeto,
                IdAssociado = idAssociado,
                IdPeriodo = idPeriodo,
                IdAvaliacao = idAvaliacao,
                NomeAssociado = associado.Nome,
                NomePeriodo = periodo.Periodo,
                NomeProjeto = projeto.Projeto,
                NomeGestor = gestor?.Nome ?? "Não informado",
                NomeCliente = cliente?.Cliente ?? "Não informado",
                TempoRestante = tempoRestante,
                TempoPeers = tempoPeers,
                TempoCargo = tempoCargo,
                PermiteEdicao = await VerificarPermissaoEdicao(idAssociado, idProjeto, idPeriodo)
            };

            _telemetryService.TrackEvent("AutoAvaliacaoPerformance_DadosCarregados", new Dictionary<string, string>
            {
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() },
                { "IdPeriodo", idPeriodo.ToString() }
            });

            return viewModel;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarDadosIniciais" },
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() }
            });
            throw;
        }
    }

    public async Task<List<PerformanceItemViewModel>> CarregarPerformances(int idAssociado, int idProjeto, int idPeriodo)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            if (associado == null)
            {
                throw new InvalidOperationException(_configuration["AutoAvaliacao:ValidationMessages:AssociadoNaoEncontrado"]);
            }

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

            var listaPerformancesModel = new List<PerformanceItemViewModel>();
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
                        var avaliacao = await _avaliacoesService.ObterAvaliacaoPerformanceAsync(idAssociado, idProjeto, item.IdPerformance, idPeriodo);
                        
                        var linhaPerformance = new PerformanceItemViewModel
                        {
                            IdPerformance = item.IdPerformance,
                            Descricao = item.Performance,
                            Abaixo = item.PerformanceAbaixo,
                            Esperado = item.PerformanceEsperado,
                            Acima = item.PerformanceAcima,
                            Abrangencia = (item.Abrangencia ?? "").ToUpper(),
                            MostrarSeparadorAbrangencia = headerSeparador,
                            PermiteInput = item.InputAutoavaliacao,
                            DisclaimerInput = item.InputAutoavaliacao ? "" : "Esta nota não requer preenchimento do avaliado",
                            NotaSelecionada = avaliacao?.IdNotaNivel1AutoAvaliacao ?? (item.InputAutoavaliacao ? 0 : item.NotaPadraoAutoAvaliacao),
                            Observacao = avaliacao?.ComentariosAutoAvaliacao ?? "",
                            Desabilitado = avaliacao?.DataHoraFimAutoAvaliacao != null ||
                                         (avaliacao?.PosicaoAtualFluxoAvaliacao != null &&
                                          avaliacao.PosicaoAtualFluxoAvaliacao != 0 &&
                                          avaliacao.PosicaoAtualFluxoAvaliacao != 1 &&
                                          avaliacao.PosicaoAtualFluxoAvaliacao != 2 &&
                                          avaliacao.PosicaoAtualFluxoAvaliacao != 3)
                        };

                        listaPerformancesModel.Add(linhaPerformance);
                    }
                }
            }

            _telemetryService.TrackEvent("AutoAvaliacaoPerformance_PerformancesCarregadas", new Dictionary<string, string>
            {
                { "IdAssociado", idAssociado.ToString() },
                { "QuantidadePerformances", listaPerformancesModel.Count.ToString() }
            });

            return listaPerformancesModel;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarPerformances" },
                { "IdAssociado", idAssociado.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> SalvarAvaliacao(int idProjeto, int idAssociado, int idPeriodo, List<PerformanceAvaliacaoDto> avaliacoes, bool finalizarAvaliacao = false)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            if (associado == null)
            {
                _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:AssociadoNaoEncontrado"]);
                return false;
            }

            var usuario = await _userContextService.GetUsuarioLogadoAsync();
            if (usuario == null)
            {
                _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:PermissaoNegada"]);
                return false;
            }

            foreach (var avaliacaoDto in avaliacoes)
            {
                var avaliacao = await _avaliacoesService.ObterAvaliacaoPerformanceAsync(idAssociado, idProjeto, avaliacaoDto.IdPerformance, idPeriodo);

                if (avaliacao == null)
                {
                    avaliacao = new AvaliacaoPerformance
                    {
                        IdEmpresa = associado.IdEmpresa,
                        IdAssociado = idAssociado,
                        USRAutoAvaliacao = idAssociado,
                        IdCargo = associado.IdCargo,
                        IdNivel = associado.IdNivel,
                        IdProjeto = idProjeto,
                        IdPeriodo = idPeriodo,
                        IdPerformance = avaliacaoDto.IdPerformance,
                        IdAvaliacaoStatus = 2,
                        PosicaoAtualFluxoAvaliacao = 1,
                        DataHoraInicio = DateTime.Now,
                        DHCAutoAvaliacao = DateTime.Now,
                        USR = usuario.Id,
                        DHC = DateTime.Now,
                        ATV = 1,
                        IdNotaNivel1AutoAvaliacao = avaliacaoDto.NotaSelecionada,
                        ComentariosAutoAvaliacao = avaliacaoDto.Observacao?.Trim() ?? "",
                        DataHoraInicioAutoAvaliacao = DateTime.Now
                    };

                    if (!await _avaliacoesService.SalvarAvaliacaoPerformanceAsync(avaliacao))
                    {
                        _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:ErroSalvamento"]);
                        return false;
                    }
                }
                else
                {
                    avaliacao.IdNotaNivel1AutoAvaliacao = avaliacaoDto.NotaSelecionada;
                    avaliacao.ComentariosAutoAvaliacao = avaliacaoDto.Observacao?.Trim() ?? "";

                    if (avaliacao.IdAvaliacaoStatus == 1)
                    {
                        avaliacao.IdAvaliacaoStatus = 2;
                        avaliacao.PosicaoAtualFluxoAvaliacao = 1;

                        if (avaliacao.DataHoraInicio == DateTime.MinValue)
                            avaliacao.DataHoraInicio = DateTime.Now;
                        avaliacao.DataHoraInicioAutoAvaliacao = DateTime.Now;
                    }

                    if (finalizarAvaliacao)
                        avaliacao.DataHoraFimAutoAvaliacao = DateTime.Now;

                    if (!await _avaliacoesService.AlterarAvaliacaoPerformanceAsync(avaliacao.IdAvaliacaoPerformance, avaliacao))
                    {
                        _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:ErroSalvamento"]);
                        return false;
                    }
                }

                var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(avaliacao.IdAvaliacaoPerformance);
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

            _telemetryService.TrackEvent("AutoAvaliacaoPerformance_AvaliacaoSalva", new Dictionary<string, string>
            {
                { "IdAssociado", idAssociado.ToString() },
                { "IdProjeto", idProjeto.ToString() },
                { "Finalizada", finalizarAvaliacao.ToString() },
                { "QuantidadeAvaliacoes", avaliacoes.Count.ToString() }
            });

            var mensagem = finalizarAvaliacao 
                ? _configuration["AutoAvaliacao:ValidationMessages:SucessoFinalizacao"]
                : _configuration["AutoAvaliacao:ValidationMessages:SucessoSalvamento"];
            
            _messageBoxService.ShowSuccess(mensagem);
            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SalvarAvaliacao" },
                { "IdAssociado", idAssociado.ToString() },
                { "IdProjeto", idProjeto.ToString() }
            });
            _messageBoxService.ShowError(_configuration["AutoAvaliacao:ValidationMessages:ErroInterno"]);
            return false;
        }
    }

    public async Task<bool> ValidarDadosObrigatorios(int idProjeto, int idAssociado, int idPeriodo)
    {
        try
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

            var projeto = await _projetosService.ObterProjetoAsync(idProjeto);
            if (projeto == null)
            {
                _messageBoxService.ShowInfo(_configuration["AutoAvaliacao:ValidationMessages:ProjetoNaoEncontrado"]);
                return false;
            }

            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            if (associado == null)
            {
                _messageBoxService.ShowInfo(_configuration["AutoAvaliacao:ValidationMessages:AssociadoNaoEncontrado"]);
                return false;
            }

            var periodo = await _avaliacoesService.ObterPeriodoAsync(idPeriodo);
            if (periodo == null)
            {
                _messageBoxService.ShowInfo(_configuration["AutoAvaliacao:ValidationMessages:PeriodoNaoEncontrado"]);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarDadosObrigatorios" },
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() }
            });
            return false;
        }
    }

    public async Task<string> CalcularTempoRestante(int idAvaliacao)
    {
        try
        {
            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idAvaliacao);
            if (avaliacaoEmail?.PRAZOS == null || !avaliacaoEmail.DataLiberacao.HasValue)
            {
                return "Não definido";
            }

            var prazo = avaliacaoEmail.PRAZOS;
            var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao);
            
            if (DateTime.Today >= dataFinal)
            {
                return DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy");
            }
            
            return dataFinal.ToString("dd/MM/yyyy");
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CalcularTempoRestante" },
                { "IdAvaliacao", idAvaliacao.ToString() }
            });
            return "Erro ao calcular";
        }
    }

    public async Task<bool> VerificarPermissaoEdicao(int idAssociado, int idProjeto, int idPeriodo)
    {
        try
        {
            var performances = await CarregarPerformances(idAssociado, idProjeto, idPeriodo);
            return performances.Any(p => !p.Desabilitado);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "VerificarPermissaoEdicao" },
                { "IdAssociado", idAssociado.ToString() }
            });
            return false;
        }
    }

    public string TruncarTexto(string texto, int qtdCaracter)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;

        if (texto.Length > qtdCaracter)
            return texto.Substring(0, qtdCaracter) + "...";
            
        return texto;
    }
}

public class AutoAvaliacaoPerformanceViewModel
{
    public int IdProjeto { get; set; }
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public int IdAvaliacao { get; set; }
    public string NomeAssociado { get; set; } = string.Empty;
    public string NomePeriodo { get; set; } = string.Empty;
    public string NomeProjeto { get; set; } = string.Empty;
    public string NomeGestor { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
    public string TempoPeers { get; set; } = string.Empty;
    public string TempoCargo { get; set; } = string.Empty;
    public bool PermiteEdicao { get; set; }
}

public class PerformanceItemViewModel
{
    public int IdPerformance { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Abaixo { get; set; } = string.Empty;
    public string Esperado { get; set; } = string.Empty;
    public string Acima { get; set; } = string.Empty;
    public string Abrangencia { get; set; } = string.Empty;
    public bool MostrarSeparadorAbrangencia { get; set; }
    public bool PermiteInput { get; set; }
    public string DisclaimerInput { get; set; } = string.Empty;
    public int NotaSelecionada { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public bool Desabilitado { get; set; }
}

public class PerformanceAvaliacaoDto
{
    public int IdPerformance { get; set; }
    public int NotaSelecionada { get; set; }
    public string? Observacao { get; set; }
}

public class AvaliacaoPerformance
{
    public int IdAvaliacaoPerformance { get; set; }
    public int IdEmpresa { get; set; }
    public int IdAssociado { get; set; }
    public int USRAutoAvaliacao { get; set; }
    public int IdCargo { get; set; }
    public int IdNivel { get; set; }
    public int IdProjeto { get; set; }
    public int IdPeriodo { get; set; }
    public int IdPerformance { get; set; }
    public int IdAvaliacaoStatus { get; set; }
    public int PosicaoAtualFluxoAvaliacao { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime DHCAutoAvaliacao { get; set; }
    public int USR { get; set; }
    public DateTime DHC { get; set; }
    public int ATV { get; set; }
    public int IdNotaNivel1AutoAvaliacao { get; set; }
    public string ComentariosAutoAvaliacao { get; set; } = string.Empty;
    public DateTime DataHoraInicioAutoAvaliacao { get; set; }
    public DateTime? DataHoraFimAutoAvaliacao { get; set; }
}