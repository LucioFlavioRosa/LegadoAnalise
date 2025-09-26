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
    private readonly IComboHelper _comboHelper;
    private readonly IFormatHelper _formatHelper;

    public AvaliacoesGestorService(ApplicationDbContext db, IComboHelper comboHelper, IFormatHelper formatHelper)
    {
        _db = db;
        _comboHelper = comboHelper;
        _formatHelper = formatHelper;
    }

    public async Task<AvaliacaoGestorPerformanceDto> ObterDadosAvaliacaoGestorPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
    {
        var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
        var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
        var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
        var gestor = projeto != null ? await _db.Associados.FirstOrDefaultAsync(a => a.Id == projeto.IdAssociadoGestor) : null;
        var cliente = projeto?.Cliente;

        var tempoPeers = "";
        var tempoCargo = "";
        if (associado != null)
        {
            tempoPeers = ""; // Implementar lógica de cálculo se necessário
            tempoCargo = ""; // Implementar lógica de cálculo se necessário
        }

        var tempoRestante = "";
        var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
        if (avaliacaoEmail != null && avaliacaoEmail.DataLiberacao.HasValue)
        {
            var prazo = await _db.Prazos.FirstOrDefaultAsync(p => p.IdPrazo == avaliacaoEmail.PRAZOS);
            if (prazo != null)
            {
                var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");
                tempoRestante = DateTime.Today >= DateTime.ParseExact(dataFinal, "dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"))
                    ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy")
                    : dataFinal;
            }
        }

        var performances = await ObterListaPerformancesAsync(idAssociado, idProjeto, idPeriodo);

        return new AvaliacaoGestorPerformanceDto
        {
            Projeto = projeto!,
            Associado = associado!,
            Periodo = periodo!,
            Gestor = gestor!,
            Cliente = cliente!,
            TempoPeers = tempoPeers,
            TempoCargo = tempoCargo,
            TempoRestante = tempoRestante,
            Performances = performances
        };
    }

    public async Task<List<PerformanceModel>> ObterListaPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        // Lógica baseada no code-behind legado
        var performances = new List<PerformanceModel>();
        var avaliacaoPerformance = await _db.AvaliacoesPerformance.FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo);
        List<Performance> listaPerformances;
        if (avaliacaoPerformance != null)
        {
            var listaIdPerformances = await _db.AvaliacoesPerformance.Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo).Select(a => a.IdPerformance).ToListAsync();
            listaPerformances = await _db.Performances.Where(p => listaIdPerformances.Contains(p.IdPerformance)).ToListAsync();
        }
        else
        {
            var associado = await _db.Associados.FirstOrDefaultAsync(a => a.Id == idAssociado);
            if (associado == null) return performances;
            listaPerformances = await _db.Performances.Where(p => p.IdCargo == associado.IdCargo).ToListAsync();
        }
        // Organiza por abrangência
        var abrangenciasContadas = new HashSet<string>();
        foreach (var perf in listaPerformances)
        {
            var model = new PerformanceModel
            {
                IdPerformance = perf.IdPerformance,
                Descricao = perf.Performance,
                Abaixo = perf.PerformanceAbaixo,
                Esperado = perf.PerformanceEsperado,
                Acima = perf.PerformanceAcima,
                Abrangencia = perf.Abrangencia,
                SeparadorAbrangencia = abrangenciasContadas.Contains(perf.Abrangencia) ? "hidden" : "",
                Input = perf.InputAvaliacaoGestor ? "" : "",
                DisclaimerInput = perf.InputAvaliacaoGestor ? "" : "Esta nota não requer preenchimento do gestor"
            };
            abrangenciasContadas.Add(perf.Abrangencia);
            performances.Add(model);
        }
        return performances;
    }

    public async Task<bool> SalvarAvaliacoesGestorPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao, List<AvaliacaoGestorPerformanceInput> avaliacoes, bool finalizarAvaliacao = false)
    {
        try
        {
            foreach (var input in avaliacoes)
            {
                var avaliacao = await _db.AvaliacoesPerformance.FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPerformance == input.IdPerformance && a.IdPeriodo == idPeriodo);
                if (avaliacao == null) continue;
                if (avaliacao.PosicaoAtualFluxoAvaliacao != "Em Av. Gestor")
                {
                    return false;
                }
                avaliacao.IdNotaNivel1AvaliacaoGestor = input.IdNotaNivel1AvaliacaoGestor;
                avaliacao.ComentariosAvaliacaoGestor = input.ComentariosAvaliacaoGestor?.Trim() ?? "";
                avaliacao.DHCAvaliacaoGestor = DateTime.Now;
                avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;
                avaliacao.IdNotaNivel1Feedback = input.IdNotaNivel1AvaliacaoGestor;
                if (avaliacao.DataHoraInicioAvaliacaoGestor == null || avaliacao.DataHoraInicioAvaliacaoGestor == DateTime.MinValue)
                    avaliacao.DataHoraInicioAvaliacaoGestor = DateTime.Now;
                if (finalizarAvaliacao)
                    avaliacao.DataHoraFimAvaliacaoGestor = DateTime.Now;
                // Atualização para próxima etapa
                if (finalizarAvaliacao)
                {
                    avaliacao.PosicaoAtualFluxoAvaliacao = "Em Feedback";
                    avaliacao.DataHoraInicioFeedback = DateTime.Now;
                }
                _db.AvaliacoesPerformance.Update(avaliacao);
            }
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> PodeEditarAvaliacaoGestorAsync(int idAssociado, int idProjeto, int idPeriodo, int idPerformance)
    {
        var avaliacao = await _db.AvaliacoesPerformance.FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPerformance == idPerformance && a.IdPeriodo == idPeriodo);
        return avaliacao != null && avaliacao.PosicaoAtualFluxoAvaliacao == "Em Av. Gestor";
    }

    public async Task<NotasGestorPerformanceDto> ObterNotasGestorPerformanceAsync(int idAssociado, int idProjeto, int idPeriodo, int idPerformance)
    {
        var avaliacao = await _db.AvaliacoesPerformance.FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPerformance == idPerformance && a.IdPeriodo == idPeriodo);
        if (avaliacao == null)
        {
            return new NotasGestorPerformanceDto { PodeEditar = false };
        }
        var notaAvaliado = _comboHelper.GetSelectedText(_comboHelper.GetNotasPerformanceItems(), avaliacao.IdNotaNivel1AutoAvaliacao?.ToString());
        var notaCegas = _comboHelper.GetSelectedText(_comboHelper.GetNotasPerformanceItems(), avaliacao.IdNotaNivel1AvaliacaoCegas?.ToString());
        var notaGestor = _comboHelper.GetSelectedText(_comboHelper.GetNotasPerformanceItems(), avaliacao.IdNotaNivel1AvaliacaoGestor?.ToString());
        return new NotasGestorPerformanceDto
        {
            NotaAvaliado = notaAvaliado,
            ObservacaoAvaliado = avaliacao.ComentariosAutoAvaliacao,
            NotaCegas = notaCegas,
            ObservacaoCegas = avaliacao.ComentariosAvaliacaoCegas,
            NotaGestor = notaGestor,
            ObservacaoGestor = avaliacao.ComentariosAvaliacaoGestor,
            PodeEditar = avaliacao.PosicaoAtualFluxoAvaliacao == "Em Av. Gestor"
        };
    }
}
