using Peers.Moderno.Services.Common;
using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Avaliacoes;

public interface IAvaliacaoService
{
    Task<List<ComboItem>> CarregaProjetosAsync(int usuarioId);
    Task<List<ComboItem>> CarregaClientesAsync();
    Task<List<ComboItem>> CarregaPeriodosAsync(int idEmpresa);
    Task<List<ComboItem>> CarregaStatusAsync();
    Task<List<ProjetoModel>> BuscarAvaliacoesAsync(
        int usuarioId,
        int? projetoId,
        int? clienteId,
        int? periodoId,
        int? statusId
    );
    Task<FinalizacaoAvaliacaoResult> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId);
}

public class AvaliacaoService : IAvaliacaoService
{
    private readonly ApplicationDbContext _db;
    private readonly IUserContextService _userContextService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly ITelemetryService _telemetryService;

    public AvaliacaoService(
        ApplicationDbContext db,
        IUserContextService userContextService,
        IMessageBoxService messageBoxService,
        ITelemetryService telemetryService)
    {
        _db = db;
        _userContextService = userContextService;
        _messageBoxService = messageBoxService;
        _telemetryService = telemetryService;
    }

    public async Task<List<ComboItem>> CarregaProjetosAsync(int usuarioId)
    {
        var projetos = await _db.Projetos
            .Where(p => p.AssociadosProjeto.Any(ap => ap.IdAssociado == usuarioId) && p.Status == 1)
            .OrderBy(p => p.Nome)
            .ToListAsync();
        return ComboHelper.CreateProjetosCombo(projetos, p => p.Id.ToString(), p => p.Nome);
    }

    public async Task<List<ComboItem>> CarregaClientesAsync()
    {
        var clientes = await _db.Clientes.Where(c => c.ATV).OrderBy(c => c.Nome).ToListAsync();
        return ComboHelper.CreateClientesCombo(clientes, c => c.IdCliente.ToString(), c => c.Nome);
    }

    public async Task<List<ComboItem>> CarregaPeriodosAsync(int idEmpresa)
    {
        var periodos = await _db.Dimensoes.Where(p => p.IdEmpresa == idEmpresa).OrderByDescending(p => p.IdDimensao).ToListAsync();
        return ComboHelper.CreatePeriodosCombo(periodos, p => p.IdDimensao.ToString(), p => p.Nome);
    }

    public Task<List<ComboItem>> CarregaStatusAsync()
    {
        return Task.FromResult(ComboHelper.GetStatusProjetoItems());
    }

    public async Task<List<ProjetoModel>> BuscarAvaliacoesAsync(
        int usuarioId,
        int? projetoId,
        int? clienteId,
        int? periodoId,
        int? statusId)
    {
        var projetosQuery = _db.Projetos
            .Include(p => p.Cliente)
            .Include(p => p.AssociadoResponsavel)
            .Include(p => p.AssociadoGestor)
            .Include(p => p.TipoProjeto)
            .Include(p => p.Complexidade)
            .Where(p => p.AssociadosProjeto.Any(ap => ap.IdAssociado == usuarioId));

        if (projetoId.HasValue && projetoId.Value > 0)
            projetosQuery = projetosQuery.Where(p => p.Id == projetoId.Value);
        if (clienteId.HasValue && clienteId.Value > 0)
            projetosQuery = projetosQuery.Where(p => p.IdCliente == clienteId.Value);
        if (statusId.HasValue && statusId.Value > 0)
            projetosQuery = projetosQuery.Where(p => p.Status == statusId.Value);

        var projetos = await projetosQuery.ToListAsync();
        var result = new List<ProjetoModel>();

        foreach (var projeto in projetos)
        {
            var linhaProjeto = new ProjetoModel
            {
                Id = projeto.Id,
                DataInicio = projeto.DataInicio.ToString("dd/MM/yyyy"),
                DataTermino = projeto.DataFim?.ToString("dd/MM/yyyy"),
                Gestor = projeto.AssociadoGestor,
                Responsavel = projeto.AssociadoResponsavel,
                Status = new { Status = ComboHelper.GetStatusText(projeto.Status) },
                Cliente = projeto.Cliente,
                Nome = projeto.Nome,
                Associados = new List<ProjetosAssociadosModel>(),
                Lideres = new List<ProjetosAssociadosModel>()
            };
            // Lógica de preenchimento de Associados e Lideres será detalhada nos próximos passos
            result.Add(linhaProjeto);
        }
        _telemetryService.TrackEvent("BuscarAvaliacoesAsync", new Dictionary<string, string> {
            { "UserId", usuarioId.ToString() },
            { "ProjetosCount", result.Count.ToString() }
        });
        return result;
    }

    public async Task<FinalizacaoAvaliacaoResult> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId)
    {
        // Implementação detalhada será feita em etapas futuras
        return new FinalizacaoAvaliacaoResult { Sucesso = false, Mensagem = "Funcionalidade em desenvolvimento." };
    }
}

public class ProjetoModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? DataInicio { get; set; }
    public string? DataTermino { get; set; }
    public object? Gestor { get; set; }
    public object? Responsavel { get; set; }
    public object? Status { get; set; }
    public object? Cliente { get; set; }
    public List<ProjetosAssociadosModel> Associados { get; set; } = new();
    public List<ProjetosAssociadosModel> Lideres { get; set; } = new();
}

public class ProjetosAssociadosModel
{
    // Propriedades a serem detalhadas conforme evolução da migração
}

public class FinalizacaoAvaliacaoResult
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}