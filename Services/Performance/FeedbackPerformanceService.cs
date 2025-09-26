using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Peers.Moderno.Services.Performance.Common;
using Peers.Moderno.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Performance;

public class FeedbackPerformanceService : IFeedbackPerformanceService
{
    private readonly ApplicationDbContext _db;
    private readonly ITelemetryService _telemetryService;

    public FeedbackPerformanceService(ApplicationDbContext db, ITelemetryService telemetryService)
    {
        _db = db;
        _telemetryService = telemetryService;
    }

    public async Task<FeedbackPerformanceViewModel> GetFeedbackAsync(int projetoId, int associadoId, int periodoId)
    {
        var projeto = await _db.Projetos.Include(p => p.Cliente).Include(p => p.AssociadoGestor).FirstOrDefaultAsync(p => p.Id == projetoId);
        var associado = await _db.Associados.FirstOrDefaultAsync(a => a.Id == associadoId);
        var periodo = await _db.PERIODOSAVALIACOES.FirstOrDefaultAsync(p => p.IdPeriodo == periodoId);
        var gestor = projeto?.AssociadoGestor;
        var cliente = projeto?.Cliente;
        var tempoRestante = ""; // Implementar cálculo se necessário

        var items = await GetPerformanceItemsAsync(projetoId, associadoId, periodoId);

        return new FeedbackPerformanceViewModel
        {
            ProjetoId = projetoId,
            ProjetoNome = projeto?.Nome ?? string.Empty,
            AssociadoId = associadoId,
            AssociadoNome = associado?.Nome ?? string.Empty,
            PeriodoId = periodoId,
            PeriodoDescricao = periodo?.Descricao ?? string.Empty,
            ClienteNome = cliente?.Nome ?? string.Empty,
            GestorNome = gestor?.Nome ?? string.Empty,
            TempoRestante = tempoRestante,
            PerformanceItems = items
        };
    }

    public async Task<List<PerformanceItemViewModel>> GetPerformanceItemsAsync(int projetoId, int associadoId, int periodoId)
    {
        var query = from perf in _db.Performances
                    join cargo in _db.Cargos on perf.IdCargo equals cargo.IdCargo
                    where perf.Ativo == true
                    select new PerformanceItemViewModel
                    {
                        IdPerformance = perf.IdPerformance,
                        Descricao = perf.PerformanceDescricao,
                        Abaixo = perf.DescricaoAbaixo,
                        Esperado = perf.DescricaoEsperado,
                        Acima = perf.DescricaoAcima,
                        NotaAvaliado = null, // Buscar nota do avaliado se existir
                        NotaCegas = null,    // Buscar nota às cegas se existir
                        NotaGestor = null,   // Buscar nota do gestor se existir
                        NotaSelecionada = null, // Buscar nota selecionada se existir
                        ObservacaoAvaliado = "", // Buscar observação do avaliado
                        ObservacaoGestor = "",   // Buscar observação do gestor
                        ConsideracaoFeedback = "", // Buscar considerações feedback
                        Abrangencia = perf.Abrangencia,
                        DisclaimerInput = FeedbackPerformanceHelper.GetDisclaimerInputText(perf.InputAutoavaliacao),
                        SeparadorAbrangencia = FeedbackPerformanceHelper.GetSeparadorAbrangencia(perf.Abrangencia)
                    };
        var items = await query.ToListAsync();
        // Aqui pode-se buscar e preencher as notas/observações reais do banco, se necessário
        return items;
    }

    public async Task<bool> SaveFeedbackAsync(FeedbackPerformanceSaveModel saveModel)
    {
        try
        {
            foreach (var notaObs in saveModel.NotasObservacoes)
            {
                // Exemplo: salvar/atualizar nota e observação para cada performance
                // Implementação real depende do modelo de dados
                // ...
            }
            await _db.SaveChangesAsync();
            _telemetryService.TrackEvent("FeedbackPerformanceSaved", new Dictionary<string, string> {
                { "ProjetoId", saveModel.ProjetoId.ToString() },
                { "AssociadoId", saveModel.AssociadoId.ToString() },
                { "PeriodoId", saveModel.PeriodoId.ToString() }
            });
            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Method", "SaveFeedbackAsync" },
                { "ProjetoId", saveModel.ProjetoId.ToString() },
                { "AssociadoId", saveModel.AssociadoId.ToString() },
                { "PeriodoId", saveModel.PeriodoId.ToString() }
            });
            return false;
        }
    }

    public async Task<bool> FinalizarFeedbackAsync(int projetoId, int associadoId, int periodoId)
    {
        try
        {
            // Implementar lógica de finalização
            await _db.SaveChangesAsync();
            _telemetryService.TrackEvent("FeedbackPerformanceFinalizado", new Dictionary<string, string> {
                { "ProjetoId", projetoId.ToString() },
                { "AssociadoId", associadoId.ToString() },
                { "PeriodoId", periodoId.ToString() }
            });
            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string> {
                { "Method", "FinalizarFeedbackAsync" },
                { "ProjetoId", projetoId.ToString() },
                { "AssociadoId", associadoId.ToString() },
                { "PeriodoId", periodoId.ToString() }
            });
            return false;
        }
    }

    public async Task<bool> ValidarNotasPreenchidasAsync(int projetoId, int associadoId, int periodoId)
    {
        // Implementar lógica de validação de notas preenchidas
        return true;
    }
}
