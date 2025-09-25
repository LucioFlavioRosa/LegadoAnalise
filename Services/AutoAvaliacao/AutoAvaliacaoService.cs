using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Services.Competencias;
using Peers.Moderno.Services.Cargos;
using Peers.Moderno.Services.Clientes;
using Peers.Moderno.Services.Projetos;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Peers.Moderno.Services.AutoAvaliacao;

public interface IAutoAvaliacaoService
{
    Task<AutoAvaliacaoDto> CarregarAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor);
    Task<bool> SalvarAvaliacaoAsync(AutoAvaliacaoDto avaliacao, bool finalizarAvaliacao = false);
    Task<bool> ValidarParametrosAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor);
    Task<List<CompetenciaAvaliacaoDto>> CarregarCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao);
    Task<CabecalhoAvaliacaoDto> CarregarCabecalhoAsync(int idProjeto, int idAssociado, int idPeriodo);
    Task<string> CalcularTempoRestanteAsync(int idAvaliacao);
    string TruncarTexto(string texto, int qtdCaracteres);
}

public class AutoAvaliacaoService : IAutoAvaliacaoService
{
    private readonly IAssociadosService _associadosService;
    private readonly IAvaliacoesService _avaliacoesService;
    private readonly ICompetenciasService _competenciasService;
    private readonly ICargosService _cargosService;
    private readonly IClientesService _clientesService;
    private readonly IProjetosService _projetosService;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IConfiguration _configuration;

    public AutoAvaliacaoService(
        IAssociadosService associadosService,
        IAvaliacoesService avaliacoesService,
        ICompetenciasService competenciasService,
        ICargosService cargosService,
        IClientesService clientesService,
        IProjetosService projetosService,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService,
        IConfiguration configuration)
    {
        _associadosService = associadosService;
        _avaliacoesService = avaliacoesService;
        _competenciasService = competenciasService;
        _cargosService = cargosService;
        _clientesService = clientesService;
        _projetosService = projetosService;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
        _configuration = configuration;
    }

    public async Task<AutoAvaliacaoDto> CarregarAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor)
    {
        try
        {
            var validacao = await ValidarParametrosAsync(idProjeto, idAssociado, idPeriodo, tipoAvaliacao, escopo, idGestor);
            if (!validacao)
            {
                return new AutoAvaliacaoDto { IsValid = false };
            }

            var cabecalho = await CarregarCabecalhoAsync(idProjeto, idAssociado, idPeriodo);
            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idProjeto, idAssociado, idPeriodo, cabecalho.Projeto.IdEmpresa, tipoAvaliacao, escopo, idGestor);
            
            if (avaliacaoEmail == null)
            {
                _messageBoxService.ShowError("Avaliação não encontrada");
                return new AutoAvaliacaoDto { IsValid = false };
            }

            var competencias = await CarregarCompetenciasAsync(idAssociado, idProjeto, idPeriodo, tipoAvaliacao, escopo, avaliacaoEmail.IdAvaliacao);
            var tempoRestante = await CalcularTempoRestanteAsync(avaliacaoEmail.IdAvaliacao);

            var dto = new AutoAvaliacaoDto
            {
                IdProjeto = idProjeto,
                IdAssociado = idAssociado,
                IdPeriodo = idPeriodo,
                TipoAvaliacao = tipoAvaliacao,
                Escopo = escopo,
                IdGestor = idGestor,
                IdAvaliacao = avaliacaoEmail.IdAvaliacao,
                Cabecalho = cabecalho,
                Competencias = competencias,
                TempoRestante = tempoRestante,
                IsValid = true
            };

            _telemetryService.TrackEvent("AutoAvaliacaoCarregada", new Dictionary<string, string>
            {
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() },
                { "TipoAvaliacao", tipoAvaliacao },
                { "Escopo", escopo }
            });

            return dto;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarAvaliacaoAsync" },
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() }
            });
            _messageBoxService.ShowError("Erro ao carregar avaliação");
            return new AutoAvaliacaoDto { IsValid = false };
        }
    }

    public async Task<bool> SalvarAvaliacaoAsync(AutoAvaliacaoDto avaliacao, bool finalizarAvaliacao = false)
    {
        try
        {
            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(avaliacao.IdAvaliacao);
            if (avaliacaoEmail == null)
            {
                _messageBoxService.ShowError("Avaliação não encontrada");
                return false;
            }

            var etapasPermitidas = new[] { "avaliacao_as_cegas", "auto_avaliacao", "em_paralelo", "nao_iniciada" };
            if (!etapasPermitidas.Contains(avaliacaoEmail.PosicaoAtualFluxoAvaliacao))
            {
                return true;
            }

            var associado = await _associadosService.ObterAssociadoAsync(avaliacao.IdAssociado);
            if (associado == null)
            {
                _messageBoxService.ShowError("Associado não encontrado");
                return false;
            }

            foreach (var competencia in avaliacao.Competencias)
            {
                var avaliacaoCompetencia = await _avaliacoesService.ObterAvaliacaoCompetenciaAsync(
                    avaliacao.IdAssociado, avaliacao.IdProjeto, competencia.IdCompetencia, 
                    avaliacao.IdPeriodo, avaliacao.TipoAvaliacao, avaliacao.Escopo, avaliacao.IdAvaliacao);

                if (avaliacaoCompetencia == null)
                {
                    avaliacaoCompetencia = new AvaliacaoCompetencia
                    {
                        IdEmpresa = associado.IdEmpresa,
                        IdAssociado = avaliacao.IdAssociado,
                        USRAutoAvaliacao = avaliacao.IdAssociado,
                        IdCargo = associado.IdCargo,
                        IdNivel = associado.IdNivel,
                        IdProjeto = avaliacao.IdProjeto,
                        IdPeriodo = avaliacao.IdPeriodo,
                        IdCompetencia = competencia.IdCompetencia,
                        IdAvaliacaoStatus = 2,
                        PosicaoAtualFluxoAvaliacao = "em_paralelo",
                        DataHoraInicio = DateTime.Now,
                        DHCAutoAvaliacao = DateTime.Now,
                        USR = avaliacao.IdAssociado,
                        DHC = DateTime.Now,
                        ATV = true,
                        IdNotaNivel1AutoAvaliacao = competencia.NotaNivel1,
                        IdNotaNivel2AutoAvaliacao = competencia.NotaNivel2,
                        ComentariosAutoAvaliacao = competencia.Consideracoes,
                        DataHoraInicioAutoAvaliacao = DateTime.Now,
                        TipoAvaliacao = avaliacao.TipoAvaliacao,
                        Escopo = avaliacao.Escopo,
                        IdAvaliacao = avaliacao.IdAvaliacao
                    };

                    var sucesso = await _avaliacoesService.SalvarAvaliacaoCompetenciaAsync(avaliacaoCompetencia);
                    if (!sucesso)
                    {
                        _messageBoxService.ShowError("Erro ao salvar competência");
                        return false;
                    }
                }
                else
                {
                    avaliacaoCompetencia.IdNotaNivel1AutoAvaliacao = competencia.NotaNivel1;
                    avaliacaoCompetencia.IdNotaNivel2AutoAvaliacao = competencia.NotaNivel2;
                    avaliacaoCompetencia.ComentariosAutoAvaliacao = competencia.Consideracoes;
                    avaliacaoCompetencia.PosicaoAtualFluxoAvaliacao = "em_paralelo";

                    if (avaliacaoCompetencia.IdAvaliacaoStatus == 1)
                    {
                        avaliacaoCompetencia.IdAvaliacaoStatus = 2;
                        if (avaliacaoCompetencia.DataHoraInicio == DateTime.MinValue)
                            avaliacaoCompetencia.DataHoraInicio = DateTime.Now;
                        avaliacaoCompetencia.DataHoraInicioAutoAvaliacao = DateTime.Now;
                    }

                    if (finalizarAvaliacao)
                        avaliacaoCompetencia.DataHoraFimAutoAvaliacao = DateTime.Now;

                    var sucesso = await _avaliacoesService.AlterarAvaliacaoCompetenciaAsync(avaliacaoCompetencia.IdAvaliacaoCompetencia, avaliacaoCompetencia);
                    if (!sucesso)
                    {
                        _messageBoxService.ShowError("Erro ao alterar competência");
                        return false;
                    }
                }

                avaliacaoEmail.IdStatus = avaliacaoCompetencia.IdAvaliacaoStatus;
                avaliacaoEmail.PosicaoAtualFluxoAvaliacao = avaliacaoCompetencia.PosicaoAtualFluxoAvaliacao;
                await _avaliacoesService.AlterarAvaliacaoEmailAsync(avaliacaoEmail.IdAvaliacao, avaliacaoEmail);

                if (finalizarAvaliacao)
                {
                    await _avaliacoesService.AvancaProximaEtapaCompetenciaAsync(avaliacaoCompetencia, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
                }
            }

            _telemetryService.TrackEvent("AutoAvaliacaoSalva", new Dictionary<string, string>
            {
                { "IdAvaliacao", avaliacao.IdAvaliacao.ToString() },
                { "Finalizada", finalizarAvaliacao.ToString() },
                { "TotalCompetencias", avaliacao.Competencias.Count.ToString() }
            });

            _messageBoxService.ShowSuccess("Avaliação salva com sucesso");
            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SalvarAvaliacaoAsync" },
                { "IdAvaliacao", avaliacao.IdAvaliacao.ToString() }
            });
            _messageBoxService.ShowError("Erro ao salvar avaliação");
            return false;
        }
    }

    public async Task<bool> ValidarParametrosAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor)
    {
        var mensagens = _configuration.GetSection("AutoAvaliacao:ValidationMessages");

        if (idProjeto <= 0)
        {
            _messageBoxService.ShowInfo(mensagens["ProjetoObrigatorio"] ?? "É obrigatório a seleção de um Projeto.");
            return false;
        }

        if (idAssociado <= 0)
        {
            _messageBoxService.ShowInfo(mensagens["AssociadoObrigatorio"] ?? "É obrigatório a seleção de um Associado.");
            return false;
        }

        if (idPeriodo <= 0)
        {
            _messageBoxService.ShowInfo(mensagens["PeriodoObrigatorio"] ?? "É obrigatório a seleção de um Período.");
            return false;
        }

        if (idGestor <= 0)
        {
            _messageBoxService.ShowInfo(mensagens["GestorObrigatorio"] ?? "É obrigatório a seleção de um Gestor.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(tipoAvaliacao))
        {
            _messageBoxService.ShowInfo(mensagens["TipoAvaliacaoObrigatorio"] ?? "É obrigatório a seleção de um tipo de avaliação.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(escopo))
        {
            _messageBoxService.ShowInfo(mensagens["EscopoObrigatorio"] ?? "É obrigatório a seleção de um escopo.");
            return false;
        }

        return true;
    }

    public async Task<List<CompetenciaAvaliacaoDto>> CarregarCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao)
    {
        var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
        var avaliacaoCompetencia = await _avaliacoesService.ObterAvaliacaoCompetenciaAsync(idAssociado, idProjeto, idPeriodo, tipoAvaliacao, escopo, idAvaliacao);
        
        var competencias = new List<Competencia>();
        var listaCompetencias = new List<AvaliacaoCompetencia>();

        if (avaliacaoCompetencia != null)
        {
            listaCompetencias = await _avaliacoesService.ObterAvaliacoesCompetenciasAsync(idAssociado, idProjeto, idPeriodo, tipoAvaliacao, escopo, idAvaliacao);
            var listaIdCompetencias = listaCompetencias.Select(comp => comp.IdCompetencia).ToList();
            competencias = await _competenciasService.ObterListaCompetenciasAsync(associado.IdEmpresa, avaliacaoCompetencia.IdCargo, associado.IdNivel, avaliacaoCompetencia.TipoAvaliacao, avaliacaoCompetencia.Escopo, listaIdCompetencias);
        }
        else
        {
            competencias = await _competenciasService.ObterListaCompetenciasAsync(associado.IdEmpresa, associado.IdCargo, associado.IdNivel, tipoAvaliacao, escopo, null);
        }

        var competenciasDto = new List<CompetenciaAvaliacaoDto>();
        
        foreach (var competencia in competencias)
        {
            var avaliacaoExistente = listaCompetencias.FirstOrDefault(x => x.IdCompetencia == competencia.IdCompetencia);
            
            var dto = new CompetenciaAvaliacaoDto
            {
                IdCompetencia = competencia.IdCompetencia,
                SubCompetencia = competencia.SubCompetencia?.Nome ?? "",
                PalavrasChave = competencia.PalavrasChave?.Replace("\n", "<br>") ?? "",
                DetalheNivelAtual = GetDetalheNivelAtual(competencia, associado.IdNivel),
                DetalheProximoNivel = GetDetalheProximoNivel(competencia, associado),
                NotaNivel1 = avaliacaoExistente?.IdNotaNivel1AutoAvaliacao ?? 0,
                NotaNivel2 = avaliacaoExistente?.IdNotaNivel2AutoAvaliacao ?? 0,
                Consideracoes = avaliacaoExistente?.ComentariosAutoAvaliacao ?? "",
                IsReadOnly = avaliacaoExistente?.DataHoraFimAutoAvaliacao != null,
                InputEtapaAtual = competencia.InputAutoAvaliacao,
                VisivelEtapaAtual = competencia.VisivelAutoAvaliacao,
                InputNivel1 = competencia.InputNivel1,
                InputNivel2 = competencia.InputNivel2,
                VisivelNivel1 = competencia.VisivelNivel1,
                VisivelNivel2 = competencia.VisivelNivel2,
                IdModo = competencia.IdModo
            };

            competenciasDto.Add(dto);
        }

        return competenciasDto;
    }

    public async Task<CabecalhoAvaliacaoDto> CarregarCabecalhoAsync(int idProjeto, int idAssociado, int idPeriodo)
    {
        var projeto = await _projetosService.ObterProjetoAsync(idProjeto);
        var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
        var periodo = await _avaliacoesService.ObterPeriodoAsync(idPeriodo);
        var cliente = await _clientesService.ObterClienteAsync(projeto.IdCliente);
        
        var tempoPeers = await _associadosService.CalcularTempoAssociadoAsync(idAssociado, "TempoDePeers");
        var tempoCargo = await _associadosService.CalcularTempoAssociadoAsync(idAssociado, "TempoDeCargo");

        return new CabecalhoAvaliacaoDto
        {
            Projeto = projeto,
            Associado = associado,
            Periodo = periodo,
            Cliente = cliente,
            TempoPeers = tempoPeers,
            TempoCargo = tempoCargo
        };
    }

    public async Task<string> CalcularTempoRestanteAsync(int idAvaliacao)
    {
        try
        {
            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idAvaliacao);
            if (avaliacaoEmail?.DataLiberacao == null || avaliacaoEmail.Prazos == null)
                return "Não definido";

            var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(avaliacaoEmail.Prazos.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");
            
            return DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR")) ?
                DateTime.Today.AddDays(avaliacaoEmail.Prazos.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") :
                dataFinal;
        }
        catch
        {
            return "Erro ao calcular";
        }
    }

    public string TruncarTexto(string texto, int qtdCaracteres)
    {
        if (!string.IsNullOrEmpty(texto) && texto.Length > qtdCaracteres)
        {
            return $"{texto.Substring(0, qtdCaracteres)}...";
        }
        return texto ?? string.Empty;
    }

    private string GetDetalheNivelAtual(Competencia competencia, int nivelAssociado)
    {
        return nivelAssociado switch
        {
            1 => competencia.CompetenciaJRDetalhe ?? "",
            2 => competencia.CompetenciaPLDetalhe ?? "",
            3 => competencia.CompetenciaSRDetalhe ?? "",
            _ => ""
        };
    }

    private string GetDetalheProximoNivel(Competencia competencia, Associado associado)
    {
        if (associado.Cargo?.ProximoCargo != null)
        {
            return competencia.CompetenciaJRDetalhe ?? "";
        }
        return "";
    }
}

public class AutoAvaliacaoDto
{
    public int IdProjeto { get; set; }
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int IdGestor { get; set; }
    public int IdAvaliacao { get; set; }
    public CabecalhoAvaliacaoDto Cabecalho { get; set; } = new();
    public List<CompetenciaAvaliacaoDto> Competencias { get; set; } = new();
    public string TempoRestante { get; set; } = string.Empty;
    public bool IsValid { get; set; }
}

public class CabecalhoAvaliacaoDto
{
    public Projeto Projeto { get; set; } = new();
    public Associado Associado { get; set; } = new();
    public Periodo Periodo { get; set; } = new();
    public Cliente Cliente { get; set; } = new();
    public string TempoPeers { get; set; } = string.Empty;
    public string TempoCargo { get; set; } = string.Empty;
}

public class CompetenciaAvaliacaoDto
{
    public int IdCompetencia { get; set; }
    public string SubCompetencia { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string DetalheProximoNivel { get; set; } = string.Empty;
    public int NotaNivel1 { get; set; }
    public int NotaNivel2 { get; set; }
    public string Consideracoes { get; set; } = string.Empty;
    public bool IsReadOnly { get; set; }
    public bool InputEtapaAtual { get; set; }
    public bool VisivelEtapaAtual { get; set; }
    public bool InputNivel1 { get; set; }
    public bool InputNivel2 { get; set; }
    public bool VisivelNivel1 { get; set; }
    public bool VisivelNivel2 { get; set; }
    public int IdModo { get; set; }
}