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
        var tempoPeers = ""; // Implementar lógica de cálculo
        var tempoCargo = ""; // Implementar lógica de cálculo
        var tempoRestante = await CalcularTempoRestanteAsync(idAvaliacao);
        var performances = await CarregarListaPerformancesAsync(idAssociado, idProjeto, idPeriodo);
        return new AvaliacaoGestorPerformanceDto
        {
            Projeto = new ProjetoDto { IdProjeto = projeto?.Id ?? 0, Nome = projeto?.Nome ?? string.Empty },
            Associado = new AssociadoDto { IdAssociado = associado?.Id ?? 0, Nome = associado?.Nome ?? string.Empty, Cargo = associado?.Cargo?.Nome ?? string.Empty },
            Periodo = new PeriodoDto { IdPeriodo = periodo?.IdPeriodo ?? 0, Periodo = periodo?.Descricao ?? string.Empty },
            Gestor = new AssociadoDto { IdAssociado = gestor?.Id ?? 0, Nome = gestor?.Nome ?? string.Empty, Cargo = gestor?.Cargo?.Nome ?? string.Empty },
            Cliente = cliente,
            TempoPeers = tempoPeers,
            TempoCargo = tempoCargo,
            TempoRestante = tempoRestante,
            Performances = performances
        };
    }

    public async Task<List<PerformanceModel>> CarregarListaPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        // Carrega avaliações existentes
        var avaliacoes = await _db.AvaliacoesPerformance
            .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
            .ToListAsync();
        var performances = await _db.Performances
            .Where(p => p.IdCargo == _db.Associados.Where(a => a.Id == idAssociado).Select(a => a.IdCargo).FirstOrDefault())
            .ToListAsync();
        var lista = new List<PerformanceModel>();
        var abrangenciasContadas = new HashSet<string>();
        foreach (var perf in performances)
        {
            var avaliacao = avaliacoes.FirstOrDefault(a => a.IdPerformance == perf.IdPerformance);
            var model = new PerformanceModel
            {
                IdPerformance = perf.IdPerformance,
                Descricao = perf.Descricao,
                Abaixo = perf.PerformanceAbaixo,
                Esperado = perf.PerformanceEsperado,
                Acima = perf.PerformanceAcima,
                Abrangencia = perf.Abrangencia,
                SeparadorAbrangencia = abrangenciasContadas.Add(perf.Abrangencia) ? string.Empty : "hidden",
                Input = perf.InputAvaliacaoGestor ? string.Empty : "hidden",
                DisclaimerInput = perf.InputAvaliacaoGestor ? string.Empty : "Esta nota não requer preenchimento do gestor",
                NotaAvaliado = avaliacao?.IdNotaNivel1AutoAvaliacao?.ToString() ?? string.Empty,
                NotaCegas = avaliacao?.IdNotaNivel1AvaliacaoCegas?.ToString() ?? string.Empty,
                NotaGestor = avaliacao?.IdNotaNivel1AvaliacaoGestor?.ToString() ?? string.Empty,
                ObservacaoAvaliado = avaliacao?.ComentariosAutoAvaliacao ?? string.Empty,
                ObservacaoCegas = avaliacao?.ComentariosAvaliacaoCegas ?? string.Empty,
                ObservacaoGestor = avaliacao?.ComentariosAvaliacaoGestor ?? string.Empty,
                PodeEditar = avaliacao != null && avaliacao.PosicaoAtualFluxoAvaliacao == 3, // 3 = etapaAvaliacaoGestor
                InputAvaliacaoGestor = perf.InputAvaliacaoGestor,
                NotaPadraoAvaliacaoGestor = perf.NotaPadraoAvaliacaoGestor
            };
            lista.Add(model);
        }
        return lista;
    }

    public async Task<bool> SalvarAvaliacoesAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao, List<PerformanceGestorInputDto> avaliacoes, bool finalizarAvaliacao)
    {
        var avaliacoesDb = await _db.AvaliacoesPerformance
            .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
            .ToListAsync();
        foreach (var input in avaliacoes)
        {
            var avaliacao = avaliacoesDb.FirstOrDefault(a => a.IdPerformance == input.IdPerformance);
            if (avaliacao == null)
                continue;
            if (avaliacao.PosicaoAtualFluxoAvaliacao != 3) // 3 = etapaAvaliacaoGestor
                continue;
            avaliacao.IdNotaNivel1AvaliacaoGestor = input.IdNotaNivel1AvaliacaoGestor;
            avaliacao.ComentariosAvaliacaoGestor = input.ComentariosAvaliacaoGestor?.Trim() ?? string.Empty;
            avaliacao.DHCAvaliacaoGestor = DateTime.Now;
            avaliacao.USRAvaliacaoGestor = 0; // Preencher com usuário logado
            avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;
            avaliacao.IdNotaNivel1Feedback = input.IdNotaNivel1AvaliacaoGestor;
            if (avaliacao.DataHoraInicioAvaliacaoGestor == null || avaliacao.DataHoraInicioAvaliacaoGestor == DateTime.MinValue)
                avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;
            if (finalizarAvaliacao)
                avaliacao.DataHoraFimAvaliacaoGestor = DateTime.Now;
            // Atualiza para feedback
            if (finalizarAvaliacao)
            {
                avaliacao.PosicaoAtualFluxoAvaliacao = 4; // 4 = etapaFeedback
                avaliacao.IdNotaNivel1Feedback = input.IdNotaNivel1AvaliacaoGestor;
                avaliacao.ComentariosFeedback = input.ComentariosAvaliacaoGestor?.Trim() ?? string.Empty;
                avaliacao.DHCFeedback = DateTime.Now;
                avaliacao.USRFeedback = 0; // Preencher com usuário logado
                avaliacao.DataHoraInicioFeedback = DateTime.Now;
            }
        }
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidarPreenchimentoAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
    {
        // Exemplo: validar se todas as performances possuem nota preenchida
        var avaliacoes = await _db.AvaliacoesPerformance
            .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
            .ToListAsync();
        return avaliacoes.All(a => a.IdNotaNivel1AvaliacaoGestor > 0);
    }

    public async Task<bool> PodeEditarAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
    {
        var avaliacoes = await _db.AvaliacoesPerformance
            .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
            .ToListAsync();
        return avaliacoes.Any(a => a.PosicaoAtualFluxoAvaliacao == 3); // 3 = etapaAvaliacaoGestor
    }

    private async Task<string> CalcularTempoRestanteAsync(int idAvaliacao)
    {
        var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
        if (avaliacaoEmail == null || !avaliacaoEmail.DataLiberacao.HasValue)
            return string.Empty;
        var prazo = await _db.Prazos.FirstOrDefaultAsync(p => p.IdPrazo == avaliacaoEmail.IdPrazo);
        if (prazo == null)
            return string.Empty;
        var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor);
        return DateTime.Today >= dataFinal ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") : dataFinal.ToString("dd/MM/yyyy");
    }
}
