using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Peers.Moderno.Services.Avaliacoes.Common;
using Peers.Moderno.Services.Common;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Peers.Moderno.Services.Avaliacoes;

public interface IEnvioAvaliacoesService
{
    Task<List<ProjetoModel>> ListarAvaliacoesAsync(FiltrosEnvioAvaliacao filtros);
    Task<EnvioAvaliacaoResult> EnviarAvaliacaoAsync(EnvioAvaliacaoRequest request);
    Task<(int enviadas, int falhas, int naoEnviadas, string detalhes)> EnviarTodasAvaliacoesAsync(List<string> listaIds, int idDisparo);
    Task<List<PendenciaModel>> ListarPendenciasAsync();
    Task<(int sucesso, int falhas)> RedispararPendenciasAsync();
    Task<List<PeriodoAvaliacao>> ObterPeriodosAsync();
    Task<List<Projeto>> ObterProjetosAsync();
    Task<List<Cliente>> ObterClientesAsync();
    Task<List<ProjetoStatus>> ObterStatusAsync();
    Task<List<Associado>> ObterAssociadosAsync();
    Task<List<Prazo>> ObterDisparosAsync();
}

public class EnvioAvaliacoesService : IEnvioAvaliacoesService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailUtils _emailUtils;
    private readonly IWorkflowUtils _workflowUtils;
    private readonly ITelemetryService _telemetryService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EnvioAvaliacoesService(
        ApplicationDbContext context,
        IEmailUtils emailUtils,
        IWorkflowUtils workflowUtils,
        ITelemetryService telemetryService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _emailUtils = emailUtils;
        _workflowUtils = workflowUtils;
        _telemetryService = telemetryService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ProjetoModel>> ListarAvaliacoesAsync(FiltrosEnvioAvaliacao filtros)
    {
        try
        {
            var query = _context.Set<Projeto>()
                .Include(p => p.AssociadoGestor)
                .Include(p => p.AssociadoResponsavel)
                .Where(p => p.ATV);

            if (filtros.IdProjeto.HasValue)
                query = query.Where(p => p.IdProjeto == filtros.IdProjeto.Value);

            if (filtros.IdCliente.HasValue)
                query = query.Where(p => p.IdCliente == filtros.IdCliente.Value);

            var projetos = await query.ToListAsync();
            var resultado = new List<ProjetoModel>();

            foreach (var projeto in projetos)
            {
                var projetoModel = new ProjetoModel
                {
                    Id = projeto.IdProjeto,
                    Nome = projeto.Projeto,
                    DataInicio = projeto.DataInicio.ToString("dd/MM/yyyy"),
                    DataTermino = projeto.DataFim?.ToString("dd/MM/yyyy") ?? "",
                    Gestor = projeto.AssociadoGestor,
                    Responsavel = projeto.AssociadoResponsavel
                };

                var associados = await ObterAssociadosProjetoAsync(projeto.IdProjeto, filtros);
                projetoModel.Associados = associados;

                resultado.Add(projetoModel);
            }

            return resultado;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<EnvioAvaliacaoResult> EnviarAvaliacaoAsync(EnvioAvaliacaoRequest request)
    {
        try
        {
            var workflow = await _workflowUtils.ObterWorkflowPorPeriodoAsync(request.IdPeriodo);
            if (workflow == null)
                return new EnvioAvaliacaoResult { Sucesso = false, Status = "NoWorkflow", Mensagem = "Workflow não encontrado" };

            var avaliacao = await ObterOuCriarAvaliacaoAsync(request);
            if (avaliacao == null)
                return new EnvioAvaliacaoResult { Sucesso = false, Status = "Fail", Mensagem = "Falha ao criar avaliação" };

            if (avaliacao.Liberado)
                return new EnvioAvaliacaoResult { Sucesso = false, Status = "NotSend", Mensagem = "Avaliação já enviada" };

            var emailEnviado = await EnviarEmailAvaliacaoAsync(avaliacao, request.IdPrazo);
            if (!emailEnviado)
                return new EnvioAvaliacaoResult { Sucesso = false, Status = "SendNotEmail", Mensagem = "Erro no envio do e-mail" };

            avaliacao.Liberado = true;
            avaliacao.DataLiberacao = DateTime.Now;
            await _context.SaveChangesAsync();

            return new EnvioAvaliacaoResult { Sucesso = true, Status = "OK", Mensagem = "Avaliação enviada com sucesso" };
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return new EnvioAvaliacaoResult { Sucesso = false, Status = "Fail", Mensagem = ex.Message };
        }
    }

    public async Task<(int enviadas, int falhas, int naoEnviadas, string detalhes)> EnviarTodasAvaliacoesAsync(List<string> listaIds, int idDisparo)
    {
        int totalEnviadas = 0, totalFalhas = 0, totalNaoEnviadas = 0;
        var detalhes = "";

        foreach (var item in listaIds)
        {
            var partes = item.Split(';');
            if (partes.Length < 6) continue;

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

            var resultado = await EnviarAvaliacaoAsync(request);

            switch (resultado.Status)
            {
                case "OK":
                    totalEnviadas++;
                    break;
                case "Fail":
                    totalFalhas++;
                    detalhes += $"<br/>Falha: Projeto {request.IdProjeto} / Associado {request.IdAssociado}";
                    break;
                case "NotSend":
                case "SendNotEmail":
                    totalNaoEnviadas++;
                    detalhes += $"<br/>Não enviada: Projeto {request.IdProjeto} / Associado {request.IdAssociado}";
                    break;
            }
        }

        return (totalEnviadas, totalFalhas, totalNaoEnviadas, detalhes);
    }

    public async Task<List<PendenciaModel>> ListarPendenciasAsync()
    {
        try
        {
            var avaliacoes = await _context.Set<Avaliacao>()
                .Include(a => a.Associado)
                .Include(a => a.Projeto)
                .Include(a => a.PeriodoAvaliacao)
                .Include(a => a.Prazo)
                .Where(a => a.PosicaoAtualFluxoAvaliacao != _workflowUtils.ObterEtapaFinalizada())
                .ToListAsync();

            var pendencias = new List<PendenciaModel>();

            foreach (var avaliacao in avaliacoes)
            {
                var pendencia = await CriarPendenciaModelAsync(avaliacao);
                if (pendencia != null)
                    pendencias.Add(pendencia);
            }

            return pendencias;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<(int sucesso, int falhas)> RedispararPendenciasAsync()
    {
        var pendencias = await ListarPendenciasAsync();
        int sucesso = 0, falhas = 0;

        foreach (var pendencia in pendencias)
        {
            try
            {
                var avaliacao = await _context.Set<Avaliacao>()
                    .Include(a => a.Associado)
                    .Include(a => a.Projeto)
                    .Include(a => a.Prazo)
                    .FirstOrDefaultAsync(a => a.IdProjeto == pendencia.IdProjeto && 
                                            a.IdAssociado == pendencia.IdAvaliado && 
                                            a.IdPeriodo == pendencia.IdPeriodo);

                if (avaliacao != null)
                {
                    var emailEnviado = await EnviarEmailPendenciaAsync(avaliacao, pendencia);
                    if (emailEnviado) sucesso++; else falhas++;
                }
            }
            catch
            {
                falhas++;
            }
        }

        return (sucesso, falhas);
    }

    public async Task<List<PeriodoAvaliacao>> ObterPeriodosAsync()
    {
        return await _context.Set<PeriodoAvaliacao>()
            .Where(p => p.ATV)
            .OrderByDescending(p => p.IdPeriodo)
            .ToListAsync();
    }

    public async Task<List<Projeto>> ObterProjetosAsync()
    {
        return await _context.Set<Projeto>()
            .Where(p => p.ATV)
            .OrderBy(p => p.Projeto)
            .ToListAsync();
    }

    public async Task<List<Cliente>> ObterClientesAsync()
    {
        return await _context.Clientes
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<List<ProjetoStatus>> ObterStatusAsync()
    {
        return await _context.Set<ProjetoStatus>()
            .OrderBy(s => s.Status)
            .ToListAsync();
    }

    public async Task<List<Associado>> ObterAssociadosAsync()
    {
        return await _context.Associados
            .Where(a => a.Ativo)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<List<Prazo>> ObterDisparosAsync()
    {
        return await _workflowUtils.ObterTodosPrazosAtivosAsync();
    }

    private async Task<List<ProjetosAssociadosModel>> ObterAssociadosProjetoAsync(int idProjeto, FiltrosEnvioAvaliacao filtros)
    {
        var associados = await _context.Set<ProjetoAssociado>()
            .Include(pa => pa.Associado)
            .ThenInclude(a => a.Cargo)
            .Where(pa => pa.IdProjeto == idProjeto)
            .ToListAsync();

        var resultado = new List<ProjetosAssociadosModel>();

        foreach (var associado in associados)
        {
            var model = new ProjetosAssociadosModel
            {
                Id = associado.Id,
                Associado = associado.Associado,
                DataInicio = associado.DataInicio.ToString("dd/MM/yyyy"),
                DataTermino = associado.DataFim?.ToString("dd/MM/yyyy") ?? "",
                TipoAvaliacao = associado.TipoAvaliacao ?? "desempenho",
                Escopo = associado.Escopo ?? "projeto",
                IdEmail = $"{idProjeto};{associado.IdAssociado};{filtros.IdPeriodo};{associado.TipoAvaliacao};{associado.Escopo};{associado.IdGestor}"
            };

            resultado.Add(model);
        }

        return resultado;
    }

    private async Task<Avaliacao?> ObterOuCriarAvaliacaoAsync(EnvioAvaliacaoRequest request)
    {
        var avaliacao = await _context.Set<Avaliacao>()
            .FirstOrDefaultAsync(a => a.IdProjeto == request.IdProjeto &&
                                    a.IdAssociado == request.IdAssociado &&
                                    a.IdPeriodo == request.IdPeriodo &&
                                    a.TipoAvaliacao == request.TipoAvaliacao &&
                                    a.Escopo == request.Escopo &&
                                    a.IdGestor == request.IdGestor);

        if (avaliacao == null)
        {
            avaliacao = new Avaliacao
            {
                IdEmpresa = 1,
                IdProjeto = request.IdProjeto,
                IdAssociado = request.IdAssociado,
                IdPeriodo = request.IdPeriodo,
                Liberado = false,
                IdStatus = 1,
                DHC = DateTime.Now,
                USR = GetCurrentUserId(),
                TipoAvaliacao = request.TipoAvaliacao,
                Escopo = request.Escopo,
                IdGestor = request.IdGestor,
                IdPrazo = request.IdPrazo,
                PosicaoAtualFluxoAvaliacao = request.TipoAvaliacao == "desempenho" ? 
                    _workflowUtils.ObterEtapaNaoIniciada() : 
                    _workflowUtils.ObterEtapaAvaliacaoGestor()
            };

            _context.Set<Avaliacao>().Add(avaliacao);
            await _context.SaveChangesAsync();
        }

        return avaliacao;
    }

    private async Task<bool> EnviarEmailAvaliacaoAsync(Avaliacao avaliacao, int idPrazo)
    {
        try
        {
            var prazo = await _workflowUtils.ObterPrazoAsync(idPrazo);
            if (prazo == null) return false;

            var associado = await _context.Associados
                .Include(a => a.Cargo)
                .Include(a => a.Mentor)
                .FirstOrDefaultAsync(a => a.Id == avaliacao.IdAssociado);

            var projeto = await _context.Set<Projeto>()
                .Include(p => p.AssociadoGestor)
                .FirstOrDefaultAsync(p => p.IdProjeto == avaliacao.IdProjeto);

            if (associado == null || projeto == null) return false;

            var config = await ObterConfigEmailAsync();
            var destinatario = new Destinatario
            {
                Nome = associado.Nome,
                Email = associado.Email
            };

            var etapa = "AUTO AVALIAÇÃO";
            if (avaliacao.TipoAvaliacao == "lideranca")
            {
                destinatario.Nome = projeto.AssociadoGestor?.Nome ?? "";
                destinatario.Email = projeto.AssociadoGestor?.Email ?? "";
                etapa = "AVALIAÇÃO DE LIDERANÇA";
            }

            var dataFinal = DateTime.Now.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");
            var template = await ObterTemplateEmailAsync();
            var corpo = _emailUtils.ConfigurarCorpoEmail(template, projeto, associado, dataFinal, etapa, destinatario.Nome, associado);

            var mensagem = new Mensagem
            {
                Titulo = $"[RH Peers] - Processo de Avaliação - Etapa: Nova Avaliação - Período: {avaliacao.PeriodoAvaliacao?.Periodo}",
                Corpo = corpo
            };

            return await _emailUtils.EnviarEmailAsync(config, destinatario, mensagem);
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> EnviarEmailPendenciaAsync(Avaliacao avaliacao, PendenciaModel pendencia)
    {
        try
        {
            var config = await ObterConfigEmailAsync();
            var destinatario = new Destinatario
            {
                Nome = pendencia.Respondente,
                Email = pendencia.RespondenteEmail
            };

            var template = await ObterTemplateEmailAsync();
            var corpo = _emailUtils.ConfigurarCorpoEmail(template, avaliacao.Projeto!, avaliacao.Associado!, pendencia.DataLimite, pendencia.Pendencia, destinatario.Nome, avaliacao.Associado!, "Você tem uma avaliação pendente!", true);

            var mensagem = new Mensagem
            {
                Titulo = $"[RH Peers] - Processo de Avaliação - Etapa Pendente - Período: {avaliacao.PeriodoAvaliacao?.Periodo}",
                Corpo = corpo
            };

            return await _emailUtils.EnviarEmailAsync(config, destinatario, mensagem);
        }
        catch
        {
            return false;
        }
    }

    private async Task<PendenciaModel?> CriarPendenciaModelAsync(Avaliacao avaliacao)
    {
        if (avaliacao.Associado == null || avaliacao.Projeto == null || avaliacao.Prazo == null)
            return null;

        var pendencia = new PendenciaModel
        {
            Nome = avaliacao.Associado.Nome,
            Projeto = avaliacao.Projeto.Projeto,
            Periodo = avaliacao.PeriodoAvaliacao?.Periodo ?? "",
            IdProjeto = avaliacao.IdProjeto,
            IdAvaliado = avaliacao.IdAssociado,
            IdPeriodo = avaliacao.IdPeriodo,
            TipoAvaliacao = avaliacao.TipoAvaliacao,
            Escopo = avaliacao.Escopo,
            IdGestor = avaliacao.IdGestor
        };

        var prazo = avaliacao.Prazo;
        var dataLiberacao = avaliacao.DataLiberacao ?? avaliacao.DHC;

        switch (avaliacao.PosicaoAtualFluxoAvaliacao)
        {
            case "NI":
            case "AA":
                pendencia.Pendencia = "Auto Avaliação";
                pendencia.Respondente = avaliacao.Associado.Nome;
                pendencia.RespondenteEmail = avaliacao.Associado.Email;
                var dataFinalAA = dataLiberacao.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");
                pendencia.DataLimite = DateTime.Today >= Convert.ToDateTime(dataFinalAA, CultureInfo.GetCultureInfo("pt-BR")) ?
                    DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") : dataFinalAA;
                break;

            case "AC":
                pendencia.Pendencia = "Avaliação as Cegas";
                var avaliador = await _context.Associados.FindAsync(avaliacao.IdAssociado);
                pendencia.Respondente = avaliador?.Nome ?? "";
                pendencia.RespondenteEmail = avaliador?.Email ?? "";
                var dataFinalAC = dataLiberacao.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");
                pendencia.DataLimite = DateTime.Today >= Convert.ToDateTime(dataFinalAC, CultureInfo.GetCultureInfo("pt-BR")) ?
                    DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") : dataFinalAC;
                break;

            case "AG":
                pendencia.Pendencia = avaliacao.TipoAvaliacao == "desempenho" ? "Avaliação do Gestor" : "Avaliação de Liderança";
                var gestor = await _context.Associados.FindAsync(avaliacao.IdGestor);
                pendencia.Respondente = gestor?.Nome ?? "";
                pendencia.RespondenteEmail = gestor?.Email ?? "";
                var dataFinalAG = dataLiberacao.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");
                pendencia.DataLimite = DateTime.Today >= Convert.ToDateTime(dataFinalAG, CultureInfo.GetCultureInfo("pt-BR")) ?
                    DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") : dataFinalAG;
                break;

            default:
                return null;
        }

        return pendencia;
    }

    private async Task<ConfigEmail> ObterConfigEmailAsync()
    {
        var parametros = await _context.Set<EmailParametro>()
            .FirstOrDefaultAsync(p => p.IdEmpresa == 1);

        return new ConfigEmail
        {
            From = parametros?.RemetenteEmail ?? "",
            SmtpServer = parametros?.SMTPServer ?? "",
            Porta = parametros?.Porta ?? 587,
            Senha = parametros?.Password ?? "",
            UsarSSL = parametros?.UsarSSL ?? true,
            Remetente = parametros?.RemetenteNome ?? ""
        };
    }

    private async Task<string> ObterTemplateEmailAsync()
    {
        var parametros = await _context.Set<EmailParametro>()
            .FirstOrDefaultAsync(p => p.IdEmpresa == 1);

        return parametros?.ModeloEmailInicio ?? "";
    }

    private int GetCurrentUserId()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        return session?.GetInt32("UserId") ?? 1;
    }
}

public class ProjetoAssociado
{
    public int Id { get; set; }
    public int IdProjeto { get; set; }
    public int IdAssociado { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? TipoAvaliacao { get; set; }
    public string? Escopo { get; set; }
    public int IdGestor { get; set; }
    public Associado? Associado { get; set; }
}

public class EmailParametro
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public string RemetenteEmail { get; set; } = string.Empty;
    public string SMTPServer { get; set; } = string.Empty;
    public int Porta { get; set; }
    public string Password { get; set; } = string.Empty;
    public bool UsarSSL { get; set; }
    public string RemetenteNome { get; set; } = string.Empty;
    public string ModeloEmailInicio { get; set; } = string.Empty;
}