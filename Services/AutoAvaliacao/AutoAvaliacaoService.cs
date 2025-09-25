using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Competencias;
using Peers.Moderno.Services.Cargos;
using Peers.Moderno.Services.Projetos;
using Peers.Moderno.Services.Clientes;
using Peers.Moderno.Services.Avaliacoes;
using System.Globalization;

namespace Peers.Moderno.Services.AutoAvaliacao;

public interface IAutoAvaliacaoService
{
    Task<AutoAvaliacaoViewModel> CarregarAutoAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor);
    Task<List<CompetenciaAutoAvaliacaoModel>> CarregarCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao);
    Task<List<DescricoesCargoModel>> CarregarDescricoesCompetenciasAsync(int idCargo, List<Competencia> competencias, Associado associado);
    Task<ValidationResult> ValidarAvaliacaoAsync(List<CompetenciaAutoAvaliacaoModel> competencias);
    Task<bool> SalvarAvaliacaoAsync(List<CompetenciaAutoAvaliacaoModel> competencias, int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao, bool finalizarAvaliacao = false);
    Task<string> CalcularTempoRestanteAsync(int idAvaliacao);
    Task<bool> PodeEditarAvaliacaoAsync(int idAvaliacao);
    string TruncarTexto(string texto, int qtdCaracteres);
}

public class AutoAvaliacaoService : IAutoAvaliacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly IAssociadosService _associadosService;
    private readonly ICompetenciasService _competenciasService;
    private readonly ICargosService _cargosService;
    private readonly IProjetosService _projetosService;
    private readonly IClientesService _clientesService;
    private readonly IAvaliacoesService _avaliacoesService;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IUserContextService _userContextService;

    public AutoAvaliacaoService(
        ApplicationDbContext context,
        IAssociadosService associadosService,
        ICompetenciasService competenciasService,
        ICargosService cargosService,
        IProjetosService projetosService,
        IClientesService clientesService,
        IAvaliacoesService avaliacoesService,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService,
        IUserContextService userContextService)
    {
        _context = context;
        _associadosService = associadosService;
        _competenciasService = competenciasService;
        _cargosService = cargosService;
        _projetosService = projetosService;
        _clientesService = clientesService;
        _avaliacoesService = avaliacoesService;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
        _userContextService = userContextService;
    }

    public async Task<AutoAvaliacaoViewModel> CarregarAutoAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor)
    {
        try
        {
            var projeto = await _projetosService.ObterProjetoAsync(idProjeto);
            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            var periodo = await _avaliacoesService.ObterPeriodoAsync(idPeriodo);
            var cliente = await _clientesService.ObterClienteAsync(projeto.IdCliente);

            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idProjeto, idAssociado, idPeriodo, projeto.IdEmpresa, tipoAvaliacao, escopo, idGestor);

            var viewModel = new AutoAvaliacaoViewModel
            {
                Projeto = projeto.Nome,
                Associado = associado.Nome,
                Periodo = periodo.Nome,
                Cliente = cliente.Nome,
                TempoPeers = await CalcularTempoPeersAsync(associado.Id),
                TempoCargo = await CalcularTempoCargoAsync(associado.Id),
                TempoRestante = await CalcularTempoRestanteAsync(avaliacaoEmail.IdAvaliacao)
            };

            _telemetryService.TrackEvent("AutoAvaliacaoCarregada", new Dictionary<string, string>
            {
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() },
                { "TipoAvaliacao", tipoAvaliacao }
            });

            return viewModel;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarAutoAvaliacaoAsync" },
                { "IdProjeto", idProjeto.ToString() },
                { "IdAssociado", idAssociado.ToString() }
            });
            throw;
        }
    }

    public async Task<List<CompetenciaAutoAvaliacaoModel>> CarregarCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            var avaliacaoCompetencia = await _avaliacoesService.ObterAvaliacaoCompetenciaAsync(idAssociado, idProjeto, idPeriodo, tipoAvaliacao, escopo, idAvaliacao);

            var competencias = new List<Competencia>();
            var listaCompetencias = new List<AvaliacaoCompetencia>();

            if (avaliacaoCompetencia != null)
            {
                listaCompetencias = await _avaliacoesService.ObterAvaliacoesCompetenciasAsync(idAssociado, idProjeto, idPeriodo, tipoAvaliacao, escopo, idAvaliacao);
                var listaIdCompetencias = listaCompetencias.Select(comp => comp.IdCompetencia).ToList();
                competencias = await _competenciasService.ObterListaCompetenciasAsync(associado.IdEmpresa, avaliacaoCompetencia.IdCargo, associado.IdNivel, tipoAvaliacao, escopo, listaIdCompetencias);
            }
            else
            {
                competencias = await _competenciasService.ObterListaCompetenciasAsync(associado.IdEmpresa, associado.IdCargo, associado.IdNivel, tipoAvaliacao, escopo, null);
            }

            var cargo = avaliacaoCompetencia?.Cargo ?? associado.Cargo;
            var listaCompetenciasModel = new List<CompetenciaAutoAvaliacaoModel>();

            int lastEixo = -1;
            int lastSub = -1;
            int lastDimensao = -1;
            int idEixo = -1;
            int IdSubCompetencia = -1;
            int lastEixoLideranca = -1;

            bool trocaTitulos = (associado.Vertical != null && associado.Vertical.ToLower() == "backoffice");
            trocaTitulos = false;

            foreach (var item in competencias)
            {
                if (IdSubCompetencia != item.IdSubCompetencia)
                {
                    lastEixo = -1;
                    lastSub = -1;
                    lastDimensao = -1;
                }

                idEixo = item.IdEixo;
                IdSubCompetencia = item.IdSubCompetencia;

                var linhaCompetencia = new CompetenciaAutoAvaliacaoModel
                {
                    IdCompetencia = item.IdCompetencia,
                    PalavrasChave = item.PalavrasChave?.Replace(Convert.ToChar(10).ToString(), "<br>") ?? ""
                };

                if (trocaTitulos)
                {
                    linhaCompetencia.PalavrasChave = item.SubCompetencia.Nome;
                }

                // Verifica a Troca da linha do Eixo
                if (lastEixo != item.IdEixo)
                {
                    var eixo = await _competenciasService.ObterEixoAsync(item.IdEixo);
                    linhaCompetencia.Eixo = eixo.Nome;
                }
                else
                    linhaCompetencia.Eixo = "";

                // Verifica a Troca da linha da SubCompetencia
                if (lastEixo != item.IdEixo && lastSub != item.IdSubCompetencia)
                {
                    var subCompetencia = await _competenciasService.ObterSubCompetenciaAsync(item.IdSubCompetencia);
                    linhaCompetencia.SubCompetencia = subCompetencia.Nome;
                    if (trocaTitulos)
                    {
                        if (lastEixoLideranca != item.IdEixo)
                        {
                            linhaCompetencia.SubCompetencia = item.Eixo.Nome;
                            lastEixoLideranca = item.IdEixo;
                        }
                        else
                        {
                            linhaCompetencia.Eixo = "";
                            linhaCompetencia.SubCompetencia = "";
                            linhaCompetencia.Dimensao = "";
                        }
                    }
                }
                else
                    linhaCompetencia.SubCompetencia = "";

                // Verifica a Troca da linha da Dimensão
                if (lastEixo != item.IdEixo && lastSub != item.IdSubCompetencia && lastDimensao != item.IdDimensao && !trocaTitulos)
                {
                    var dimensao = await _competenciasService.ObterDimensaoAsync(item.IdDimensao);
                    linhaCompetencia.Dimensao = dimensao.Nome;
                    lastDimensao = item.IdDimensao;
                    lastSub = item.IdSubCompetencia;
                    lastEixo = item.IdEixo;
                }
                else
                    linhaCompetencia.Dimensao = "";

                // Próximo nível - competência do próximo cargo
                Competencia competenciaProximoCargo = null;
                if (cargo.IdProximoCargo != null)
                {
                    competenciaProximoCargo = await _competenciasService.ObterCompetenciaCargoSubAsync(cargo.IdProximoCargo.Value, item.IdSubCompetencia, item.PalavrasChave);
                }

                switch (associado.IdNivel)
                {
                    case 1: // Junior
                        linhaCompetencia.DetalheNivelAtual = item.CompetenciaJRDetalhe;
                        linhaCompetencia.CompetenciaAtual = item.CompetenciaJR;
                        linhaCompetencia.CompetenciaProximo = item.CompetenciaPL;
                        linhaCompetencia.DetalheProximoNivel = competenciaProximoCargo?.CompetenciaJRDetalhe ?? "";
                        linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "hidden";
                        break;
                }

                // Regras de autopreenchimento
                var inputEtapaAtual = item.InputAutoAvaliacao;
                var visivelEtapaAtual = item.VisivelAutoAvaliacao;

                linhaCompetencia.DisableSelectNivel1 = inputEtapaAtual && item.InputNivel1 ? "" : "disabled";
                linhaCompetencia.DisableSelectNivel2 = inputEtapaAtual && item.InputNivel2 ? "" : "disabled";
                linhaCompetencia.HiddenSelectNivel1 = inputEtapaAtual && item.InputNivel1 ? "" : "hidden";
                linhaCompetencia.HiddenSelectNivel2 = inputEtapaAtual && item.InputNivel2 ? "" : "hidden";
                linhaCompetencia.HiddenTextBoxNivel1 = !inputEtapaAtual || !item.InputNivel1 ? "" : "hidden";
                linhaCompetencia.HiddenTextBoxNivel2 = !inputEtapaAtual || !item.InputNivel2 ? "" : "hidden";

                if (item.IdModo == 1)
                {
                    var getLinhaAvaliacao = listaCompetencias.Where(x => x.IdCompetencia == item.IdCompetencia).FirstOrDefault();
                    if (getLinhaAvaliacao != null)
                    {
                        var getNotaNivel1 = await _avaliacoesService.ObterAvaliacaoCompetenciaNotaAsync(getLinhaAvaliacao.IdNotaNivel1AutoAvaliacao);
                        var getNotaNivel2 = await _avaliacoesService.ObterAvaliacaoCompetenciaNotaAsync(getLinhaAvaliacao.IdNotaNivel2AutoAvaliacao);
                        linhaCompetencia.TextoNotaNivel1 = getNotaNivel1?.DescricaoNota ?? "";
                        linhaCompetencia.TextoNotaNivel2 = getNotaNivel2?.DescricaoNota ?? "";
                    }
                }
                else if (item.IdModo == 2)
                {
                    var getResultado = await _avaliacoesService.ObterResultadoLiderComoCompetenciaAsync(idAssociado, idPeriodo);
                    linhaCompetencia.TextoNotaNivel1 = getResultado?.TextoMediaTotal ?? "";
                    linhaCompetencia.TextoNotaNivel2 = getResultado?.TextoMediaTotal ?? "";
                }

                linhaCompetencia.TextoNotaNivel1 = visivelEtapaAtual && item.VisivelNivel1 ? linhaCompetencia.TextoNotaNivel1 : "Nota não disponível nesta etapa.";
                linhaCompetencia.TextoNotaNivel2 = visivelEtapaAtual && item.VisivelNivel2 ? linhaCompetencia.TextoNotaNivel2 : "Nota não disponível nesta etapa.";

                // Carrega avaliação existente
                var avaliacaoExistente = listaCompetencias.FirstOrDefault(x => x.IdCompetencia == item.IdCompetencia);
                if (avaliacaoExistente != null)
                {
                    linhaCompetencia.NotaNivel1Selecionada = avaliacaoExistente.IdNotaNivel1AutoAvaliacao;
                    linhaCompetencia.NotaNivel2Selecionada = avaliacaoExistente.IdNotaNivel2AutoAvaliacao;
                    linhaCompetencia.ComentariosAutoAvaliacao = avaliacaoExistente.ComentariosAutoAvaliacao;
                    linhaCompetencia.IsFinalizadaAutoAvaliacao = avaliacaoExistente.DataHoraFimAutoAvaliacao != null;
                }

                listaCompetenciasModel.Add(linhaCompetencia);
            }

            return listaCompetenciasModel;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarCompetenciasAsync" },
                { "IdAssociado", idAssociado.ToString() },
                { "IdProjeto", idProjeto.ToString() }
            });
            throw;
        }
    }

    public async Task<List<DescricoesCargoModel>> CarregarDescricoesCompetenciasAsync(int idCargo, List<Competencia> competencias, Associado associado)
    {
        try
        {
            return await _cargosService.ObterDescricoesCompetenciasAsync(idCargo, competencias, associado);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarDescricoesCompetenciasAsync" },
                { "IdCargo", idCargo.ToString() }
            });
            throw;
        }
    }

    public async Task<ValidationResult> ValidarAvaliacaoAsync(List<CompetenciaAutoAvaliacaoModel> competencias)
    {
        try
        {
            var result = new ValidationResult { IsValid = true };
            var listaRespostas = new List<CompetenciaValidacaoModel>();

            foreach (var competencia in competencias)
            {
                var competenciaDb = await _competenciasService.ObterCompetenciaAsync(competencia.IdCompetencia);
                var addCompetencia = new CompetenciaValidacaoModel
                {
                    NotaValidaNivel1 = competencia.NotaNivel1Selecionada,
                    NotaValidaNivel2 = competencia.NotaNivel2Selecionada,
                    IdModo = competenciaDb.IdModo,
                    SubCompetencia = competenciaDb.SubCompetencia.Nome
                };

                // Validação padrão
                if (competenciaDb.IdModo == 1)
                {
                    // Valida "Não se aplica"
                    if (competencia.NotaNivel1Selecionada == 5)
                    {
                        if (competencia.NotaNivel2Selecionada != 5)
                        {
                            result.IsValid = false;
                            result.ErrorMessage = "ATENÇÃO! Quando a nota de Competência do Nível Atual for = Não Se Aplica, o Próximo Nível deve ser Não Se Aplica.";
                            return result;
                        }
                    }

                    // Valida preenchimento obrigatório
                    if ((competencia.NotaNivel1Selecionada == 0 && competencia.NotaNivel2Selecionada > 0) ||
                        (competencia.NotaNivel1Selecionada > 0 && competencia.NotaNivel2Selecionada == 0))
                    {
                        result.IsValid = false;
                        result.ErrorMessage = "É obrigatório selecionar uma nota para cada nível.";
                        return result;
                    }

                    // Valida consistência entre níveis
                    if (competencia.NotaNivel1Selecionada > 0 && competencia.NotaNivel2Selecionada > 0)
                    {
                        var statusList = await _avaliacoesService.ObterNotasCompetenciasAsync(false);
                        var pesoDdl1 = statusList.FirstOrDefault(x => x.IdNota == competencia.NotaNivel1Selecionada)?.Peso ?? 0;
                        var pesoDdl2 = statusList.FirstOrDefault(x => x.IdNota == competencia.NotaNivel2Selecionada)?.Peso ?? 0;

                        if (pesoDdl2 > pesoDdl1)
                        {
                            result.IsValid = false;
                            result.ErrorMessage = "A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).";
                            return result;
                        }
                    }
                }
                else if (competenciaDb.IdModo == 2) // Média total av. liderança
                {
                    addCompetencia.NotaValidaNivel1 = competenciaDb.IdNotaPadraoNivel1 ?? 5;
                    addCompetencia.NotaValidaNivel2 = competenciaDb.IdNotaPadraoNivel2 ?? 5;
                }

                listaRespostas.Add(addCompetencia);
            }

            // Valida pilares (subcompetências)
            var listPilares = listaRespostas.Select(r => r.SubCompetencia).Distinct().ToList();
            var pilarVazio = false;

            foreach (var pilar in listPilares)
            {
                var countCompetenciasPilar = listaRespostas.Where(r => r.SubCompetencia == pilar).ToList();
                var countCompetenciasPadrao = countCompetenciasPilar.Where(r => r.IdModo != 2).ToList();
                var countCompetenciasAuto = countCompetenciasPilar.Where(r => r.IdModo == 2).ToList();
                var countRespostasVaziasNivel1 = countCompetenciasPadrao.Where(r => r.NotaValidaNivel1 == 5).ToList();
                var countRespostasVaziasNivel2 = countCompetenciasPadrao.Where(r => r.NotaValidaNivel2 == 5).ToList();

                if (countCompetenciasPilar.Count != countCompetenciasAuto.Count)
                {
                    if (countCompetenciasPadrao.Count == countRespostasVaziasNivel1.Count ||
                        countCompetenciasPadrao.Count == countRespostasVaziasNivel2.Count)
                    {
                        pilarVazio = true;
                        break;
                    }
                }
            }

            if (pilarVazio)
            {
                result.IsValid = false;
                result.ErrorMessage = "Cada pilar precisa receber ao mínimo 1 nota mensurável em cada nível (diferente de não se aplica)";
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarAvaliacaoAsync" }
            });
            return new ValidationResult { IsValid = false, ErrorMessage = "Erro interno ao validar avaliação" };
        }
    }

    public async Task<bool> SalvarAvaliacaoAsync(List<CompetenciaAutoAvaliacaoModel> competencias, int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao, bool finalizarAvaliacao = false)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idAvaliacao);

            // Cancela salvamento caso avaliação esteja em etapa posterior
            var etapasPermitidas = new[] { "avaliacao_as_cegas", "auto_avaliacao", "em_paralelo", "nao_iniciada" };
            if (!etapasPermitidas.Contains(avaliacaoEmail.PosicaoAtualFluxoAvaliacao))
            {
                return true;
            }

            var usuarioLogado = await _userContextService.GetUsuarioLogadoAsync();

            foreach (var competencia in competencias)
            {
                var avaliacao = await _avaliacoesService.ObterAvaliacaoCompetenciaAsync(idAssociado, idProjeto, competencia.IdCompetencia, idPeriodo, tipoAvaliacao, escopo, idAvaliacao);

                if (avaliacao == null)
                {
                    // Cria nova avaliação
                    avaliacao = new AvaliacaoCompetencia
                    {
                        IdEmpresa = associado.IdEmpresa,
                        IdAssociado = idAssociado,
                        USRAutoAvaliacao = idAssociado,
                        IdCargo = associado.IdCargo,
                        IdNivel = associado.IdNivel,
                        IdProjeto = idProjeto,
                        IdPeriodo = idPeriodo,
                        IdCompetencia = competencia.IdCompetencia,
                        IdAvaliacaoStatus = 2, // Em Andamento
                        PosicaoAtualFluxoAvaliacao = "em_paralelo",
                        DataHoraInicio = DateTime.Now,
                        DHCAutoAvaliacao = DateTime.Now,
                        USR = usuarioLogado?.Id ?? 0,
                        DHC = DateTime.Now,
                        ATV = 1,
                        IdNotaNivel1AutoAvaliacao = competencia.NotaNivel1Selecionada,
                        IdNotaNivel2AutoAvaliacao = competencia.NotaNivel2Selecionada,
                        ComentariosAutoAvaliacao = competencia.ComentariosAutoAvaliacao?.Trim() ?? "",
                        DataHoraInicioAutoAvaliacao = DateTime.Now,
                        TipoAvaliacao = tipoAvaliacao,
                        Escopo = escopo,
                        IdAvaliacao = idAvaliacao
                    };

                    await _avaliacoesService.SalvarAvaliacaoCompetenciaAsync(avaliacao);
                }
                else
                {
                    // Atualiza avaliação existente
                    avaliacao.IdNotaNivel1AutoAvaliacao = competencia.NotaNivel1Selecionada;
                    avaliacao.IdNotaNivel2AutoAvaliacao = competencia.NotaNivel2Selecionada;
                    avaliacao.ComentariosAutoAvaliacao = competencia.ComentariosAutoAvaliacao?.Trim() ?? "";
                    avaliacao.PosicaoAtualFluxoAvaliacao = "em_paralelo";

                    if (avaliacao.IdAvaliacaoStatus == 1)
                    {
                        avaliacao.IdAvaliacaoStatus = 2;
                        if (avaliacao.DataHoraInicio == DateTime.MinValue)
                            avaliacao.DataHoraInicio = DateTime.Now;
                        avaliacao.DataHoraInicioAutoAvaliacao = DateTime.Now;
                    }

                    if (finalizarAvaliacao)
                        avaliacao.DataHoraFimAutoAvaliacao = DateTime.Now;

                    await _avaliacoesService.AlterarAvaliacaoCompetenciaAsync(avaliacao.IdAvaliacaoCompetencia, avaliacao, true);
                }

                // Atualiza status da avaliação email
                avaliacaoEmail.IdStatus = avaliacao.IdAvaliacaoStatus;
                avaliacaoEmail.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;
                await _avaliacoesService.AlterarAvaliacaoEmailAsync(avaliacaoEmail.IdAvaliacao, avaliacaoEmail);

                if (finalizarAvaliacao)
                {
                    await _avaliacoesService.AvancaProximaEtapaCompetenciaAsync(avaliacao, avaliacaoEmail, tipoAvaliacao);
                }
            }

            _telemetryService.TrackEvent("AutoAvaliacaoSalva", new Dictionary<string, string>
            {
                { "IdAssociado", idAssociado.ToString() },
                { "IdProjeto", idProjeto.ToString() },
                { "Finalizada", finalizarAvaliacao.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SalvarAvaliacaoAsync" },
                { "IdAssociado", idAssociado.ToString() },
                { "IdProjeto", idProjeto.ToString() }
            });
            return false;
        }
    }

    public async Task<string> CalcularTempoRestanteAsync(int idAvaliacao)
    {
        try
        {
            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idAvaliacao);
            if (avaliacaoEmail?.PRAZOS != null && avaliacaoEmail.DataLiberacao.HasValue)
            {
                var prazo = avaliacaoEmail.PRAZOS;
                var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");
                return DateTime.Today >= Convert.ToDateTime(dataFinal, CultureInfo.GetCultureInfo("pt-BR"))
                    ? DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy")
                    : dataFinal;
            }
            return "";
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CalcularTempoRestanteAsync" },
                { "IdAvaliacao", idAvaliacao.ToString() }
            });
            return "";
        }
    }

    public async Task<bool> PodeEditarAvaliacaoAsync(int idAvaliacao)
    {
        try
        {
            var avaliacaoEmail = await _avaliacoesService.ObterAvaliacaoEmailAsync(idAvaliacao);
            var etapasPermitidas = new[] { "avaliacao_as_cegas", "auto_avaliacao", "em_paralelo", "nao_iniciada" };
            return avaliacaoEmail != null && etapasPermitidas.Contains(avaliacaoEmail.PosicaoAtualFluxoAvaliacao);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "PodeEditarAvaliacaoAsync" },
                { "IdAvaliacao", idAvaliacao.ToString() }
            });
            return false;
        }
    }

    public string TruncarTexto(string texto, int qtdCaracteres)
    {
        if (!string.IsNullOrEmpty(texto) && texto.Length > qtdCaracteres)
        {
            return $"{texto.Substring(0, qtdCaracteres)}...";
        }
        return texto;
    }

    private async Task<string> CalcularTempoPeersAsync(int idAssociado)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(idAssociado);
            if (associado?.DataAdmissao != null)
            {
                var tempo = DateTime.Now - associado.DataAdmissao.Value;
                var anos = tempo.Days / 365;
                var meses = (tempo.Days % 365) / 30;
                return $"{anos} anos e {meses} meses";
            }
            return "";
        }
        catch
        {
            return "";
        }
    }

    private async Task<string> CalcularTempoCargoAsync(int idAssociado)
    {
        try
        {
            var promocoes = await _associadosService.ObterPromocoesAssociadoAsync(idAssociado);
            var ultimaPromocao = promocoes.OrderByDescending(p => p.DataPromocao).FirstOrDefault();
            
            if (ultimaPromocao?.DataPromocao != null)
            {
                var tempo = DateTime.Now - ultimaPromocao.DataPromocao;
                var anos = tempo.Days / 365;
                var meses = (tempo.Days % 365) / 30;
                return $"{anos} anos e {meses} meses";
            }
            
            return await CalcularTempoPeersAsync(idAssociado);
        }
        catch
        {
            return "";
        }
    }
}

public class AutoAvaliacaoViewModel
{
    public string Projeto { get; set; } = string.Empty;
    public string Associado { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string TempoPeers { get; set; } = string.Empty;
    public string TempoCargo { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
}

public class CompetenciaAutoAvaliacaoModel
{
    public int IdCompetencia { get; set; }
    public string Eixo { get; set; } = string.Empty;
    public string SubCompetencia { get; set; } = string.Empty;
    public string Dimensao { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string CompetenciaAtual { get; set; } = string.Empty;
    public string CompetenciaProximo { get; set; } = string.Empty;
    public string DetalheProximoNivel { get; set; } = string.Empty;
    public string IsHiddenDetalhamentoProximoNivel { get; set; } = string.Empty;
    public string DisableSelectNivel1 { get; set; } = string.Empty;
    public string DisableSelectNivel2 { get; set; } = string.Empty;
    public string HiddenSelectNivel1 { get; set; } = string.Empty;
    public string HiddenSelectNivel2 { get; set; } = string.Empty;
    public string HiddenTextBoxNivel1 { get; set; } = string.Empty;
    public string HiddenTextBoxNivel2 { get; set; } = string.Empty;
    public string TextoNotaNivel1 { get; set; } = string.Empty;
    public string TextoNotaNivel2 { get; set; } = string.Empty;
    public int NotaNivel1Selecionada { get; set; }
    public int NotaNivel2Selecionada { get; set; }
    public string ComentariosAutoAvaliacao { get; set; } = string.Empty;
    public bool IsFinalizadaAutoAvaliacao { get; set; }
}

public class CompetenciaValidacaoModel
{
    public int NotaValidaNivel1 { get; set; }
    public int NotaValidaNivel2 { get; set; }
    public int IdModo { get; set; }
    public string SubCompetencia { get; set; } = string.Empty;
}

public class DescricoesCargoModel
{
    public string TituloExibicao { get; set; } = string.Empty;
    public string DescricaoCompetencia_Atual { get; set; } = string.Empty;
    public string DescricaoCompetencia_Proximo { get; set; } = string.Empty;
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}