using Services.FrentesInternas.Common;
using Services.FrentesInternas.Common.Models;
using Services.Common;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;

namespace Services.FrentesInternas;

public class FrentesInternasService : IFrentesInternasService
{
    private readonly ApplicationDbContext _db;
    private readonly IUserContextService _userContextService;
    private readonly IComboHelper _comboHelper;
    private readonly ITelemetryService _telemetryService;

    public FrentesInternasService(
        ApplicationDbContext db,
        IUserContextService userContextService,
        IComboHelper comboHelper,
        ITelemetryService telemetryService)
    {
        _db = db;
        _userContextService = userContextService;
        _comboHelper = comboHelper;
        _telemetryService = telemetryService;
    }

    public async Task<List<PDIPillsModel>> CarregarPeriodosAsync(int usuarioId)
    {
        // Exemplo: buscar períodos e montar pills
        // Substitua por lógica real conforme entidades
        var periodos = await _db.Prazos.OrderByDescending(p => p.IdPrazo).ToListAsync();
        var ultimoPeriodo = periodos.FirstOrDefault();
        var pills = new List<PDIPillsModel>();
        foreach (var periodo in periodos)
        {
            pills.Add(new PDIPillsModel
            {
                id = $"tab-{periodo.IdPrazo}-tab",
                href = $"#tab-{periodo.IdPrazo}",
                ariacontrols = $"tab-{periodo.IdPrazo}",
                ariaselected = periodo.IdPrazo == ultimoPeriodo?.IdPrazo ? "true" : "false",
                active = periodo.IdPrazo == ultimoPeriodo?.IdPrazo ? "active" : string.Empty,
                classe = periodo.IdPrazo == ultimoPeriodo?.IdPrazo ? "btn btn-danger" : "btn btn-facebook",
                Periodo = periodo.NomeDisparo
            });
        }
        return pills;
    }

    public async Task<List<FrentePill>> CarregarAvaliacoesAsync(int usuarioId)
    {
        // Exemplo: buscar avaliações do usuário logado
        // Substitua por lógica real conforme entidades
        var periodos = await _db.Prazos.OrderByDescending(p => p.IdPrazo).ToListAsync();
        var ultimoPeriodo = periodos.FirstOrDefault();
        var pills = new List<FrentePill>();
        foreach (var periodo in periodos)
        {
            var pill = new FrentePill
            {
                idPeriodo = periodo.IdPrazo,
                Periodo = periodo.NomeDisparo,
                id = $"tab-{periodo.IdPrazo}",
                active = periodo.IdPrazo == ultimoPeriodo?.IdPrazo ? "active in show" : string.Empty,
                arialabelled = $"tab-{periodo.IdPrazo}-tab",
                alocacoes = new List<AlocacaoInternaPill>()
            };
            // Exemplo: buscar alocações e avaliações (substitua por lógica real)
            // pill.alocacoes = ...
            pills.Add(pill);
        }
        return pills;
    }

    public async Task ValidarAlocacoesInternasAsync(int idPeriodo, int usuarioId)
    {
        // Lógica de validação de alocações internas
        // Exemplo: implementar conforme regra de negócio
        _telemetryService.TrackEvent("ValidarAlocacoesInternas", new Dictionary<string, string> { { "Periodo", idPeriodo.ToString() }, { "UsuarioId", usuarioId.ToString() } });
        await Task.CompletedTask;
    }

    public async Task AtualizarNotaAsync(int idAvaliacao, int idNota)
    {
        // Exemplo: atualizar nota na base
        // Substitua por lógica real
        _telemetryService.TrackEvent("AtualizarNota", new Dictionary<string, string> { { "AvaliacaoId", idAvaliacao.ToString() }, { "NotaId", idNota.ToString() } });
        await Task.CompletedTask;
    }

    public async Task AtualizarComentarioAsync(int idAvaliacao, string comentario)
    {
        // Exemplo: atualizar comentário na base
        _telemetryService.TrackEvent("AtualizarComentario", new Dictionary<string, string> { { "AvaliacaoId", idAvaliacao.ToString() } });
        await Task.CompletedTask;
    }

    public async Task AtualizarValidadoAsync(int idAvaliacao, bool validado)
    {
        // Exemplo: atualizar flag de validado na base
        _telemetryService.TrackEvent("AtualizarValidado", new Dictionary<string, string> { { "AvaliacaoId", idAvaliacao.ToString() }, { "Validado", validado.ToString() } });
        await Task.CompletedTask;
    }
}