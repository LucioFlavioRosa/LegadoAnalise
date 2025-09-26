using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.AvaliacoesGestor.Common;
using Services.Common;

namespace Services.AvaliacoesGestor.Common;

public class AvaliacoesGestorService : IAvaliacoesGestorService
{
    private readonly ApplicationDbContext _db;
    private readonly ComboHelper _comboHelper;

    public AvaliacoesGestorService(ApplicationDbContext db, ComboHelper comboHelper)
    {
        _db = db;
        _comboHelper = comboHelper;
    }

    public async Task<AvaliacaoGestorPerformanceDto> CarregarDadosAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
    {
        var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
        var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
        var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
        var gestor = projeto != null ? await _db.Associados.FirstOrDefaultAsync(a => a.Id == projeto.IdAssociadoGestor) : null;
        var cliente = projeto?.Cliente?.Nome ?? string.Empty;
        var performances = await ObterListaPerformancesAsync(idAssociado, idProjeto, idPeriodo);

        // Simulação de cálculo de tempo (ajustar para lógica real se necessário)
        string tempoPeers = "";
        string tempoCargo = "";
        string tempoRestante = "";
        // TODO: Integrar com serviço de cálculo de tempo se necessário

        return new AvaliacaoGestorPerformanceDto
        {
            Projeto = new ProjetoDto { Id = projeto?.Id ?? 0, Nome = projeto?.Nome ?? string.Empty, Cliente = cliente },
            Associado = new AssociadoDto { Id = associado?.Id ?? 0, Nome = associado?.Nome ?? string.Empty, Cargo = associado?.Cargo?.Nome ?? string.Empty },
            Periodo = new PeriodoDto { Id = periodo?.IdPeriodo ?? 0, Nome = periodo?.Nome ?? string.Empty },
            Gestor = new AssociadoDto { Id = gestor?.Id ?? 0, Nome = gestor?.Nome ?? string.Empty, Cargo = gestor?.Cargo?.Nome ?? string.Empty },
            TempoPeers = tempoPeers,
            TempoCargo = tempoCargo,
            TempoRestante = tempoRestante,
            Performances = performances
        };
    }

    public async Task<List<PerformanceModel>> ObterListaPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        // Busca todas as performances relacionadas ao associado, projeto e periodo
        var avaliacoes = await _db.AvaliacoesPerformance
            .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
            .ToListAsync();

        var performances = await _db.Performances
            .Where(p => avaliacoes.Select(a => a.IdPerformance).Contains(p.IdPerformance))
            .ToListAsync();

        var lista = new List<PerformanceModel>();
        foreach (var perf in performances)
        {
            var avaliacao = avaliacoes.FirstOrDefault(a => a.IdPerformance == perf.IdPerformance);
            lista.Add(new PerformanceModel
            {
                IdPerformance = perf.IdPerformance,
                Descricao = perf.Descricao,
                Abaixo = perf.PerformanceAbaixo,
                Esperado = perf.PerformanceEsperado,
                Acima = perf.PerformanceAcima,
                Abrangencia = perf.Abrangencia,
                SeparadorAbrangencia = "", // Lógica de separador pode ser ajustada via helper
                Input = perf.InputAvaliacaoGestor ? string.Empty : "hidden",
                DisclaimerInput = perf.InputAvaliacaoGestor ? string.Empty : "Esta nota não requer preenchimento do gestor"
            });
        }
        return lista;
    }

    public async Task<bool> SalvarAvaliacoesAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao, List<AvaliacaoGestorPerformanceInput> avaliacoes, bool finalizarAvaliacao = false)
    {
        var avaliacoesDb = await _db.AvaliacoesPerformance
            .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
            .ToListAsync();

        foreach (var input in avaliacoes)
        {
            var avaliacao = avaliacoesDb.FirstOrDefault(a => a.IdPerformance == input.IdPerformance);
            if (avaliacao == null)
                continue;
            avaliacao.IdNotaNivel1AvaliacaoGestor = input.IdNotaNivel1AvaliacaoGestor;
            avaliacao.ComentariosAvaliacaoGestor = input.ComentariosAvaliacaoGestor?.Trim() ?? string.Empty;
            avaliacao.DHCAvaliacaoGestor = DateTime.Now;
            // Simulação de usuário logado - ajustar para pegar do contexto
            avaliacao.USRAvaliacaoGestor = 0;
            avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;
            avaliacao.IdNotaNivel1Feedback = input.IdNotaNivel1AvaliacaoGestor;
            if (avaliacao.DataHoraInicioAvaliacaoGestor == null || avaliacao.DataHoraInicioAvaliacaoGestor == DateTime.MinValue)
                avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;
            if (finalizarAvaliacao)
                avaliacao.DataHoraFimAvaliacaoGestor = DateTime.Now;
            // Fluxo de feedback
            if (finalizarAvaliacao)
            {
                avaliacao.PosicaoAtualFluxoAvaliacao = 4; // Exemplo: etapaFeedback
                avaliacao.DataHoraFimAvaliacaoGestor = DateTime.Now;
            }
        }
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidarPreenchimentoAsync(int idProjeto, int idAssociado, int idPeriodo)
    {
        var avaliacoes = await _db.AvaliacoesPerformance
            .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
            .ToListAsync();
        return avaliacoes.All(a => a.IdNotaNivel1AvaliacaoGestor > 0);
    }

    public async Task<bool> PodeEditarAsync(int idAssociado, int idProjeto, int idPeriodo, int idPerformance)
    {
        var avaliacao = await _db.AvaliacoesPerformance
            .FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo && a.IdPerformance == idPerformance);
        return avaliacao != null && avaliacao.PosicaoAtualFluxoAvaliacao == 3; // Exemplo: etapaAvaliacaoGestor
    }

    public async Task<PerformanceNotasDto> ObterNotasAsync(int idAssociado, int idProjeto, int idPeriodo, int idPerformance)
    {
        var avaliacao = await _db.AvaliacoesPerformance
            .FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo && a.IdPerformance == idPerformance);
        if (avaliacao == null)
            return new PerformanceNotasDto();
        // Simulação de busca das notas (ajustar para lógica real)
        return new PerformanceNotasDto
        {
            NotaAvaliado = "",
            ObservacaoAvaliado = avaliacao.ComentariosAutoAvaliacao ?? string.Empty,
            NotaCegas = "",
            ObservacaoCegas = avaliacao.ComentariosAvaliacaoCegas ?? string.Empty,
            NotaGestor = avaliacao.IdNotaNivel1AvaliacaoGestor.ToString(),
            ObservacaoGestor = avaliacao.ComentariosAvaliacaoGestor ?? string.Empty
        };
    }

    public async Task<PerformanceComboDto> ObterCombosAsync()
    {
        return new PerformanceComboDto
        {
            Notas = ComboHelper.GetNotasPerformanceItems()
        };
    }
}
