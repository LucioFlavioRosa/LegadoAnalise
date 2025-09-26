using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.FeedbackPerformance.Common;

namespace Services.FeedbackPerformance
{
    public interface IFeedbackPerformanceService
    {
        Task<FeedbackPerformanceResult> GetFeedbackPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
        Task<bool> SaveFeedbackPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao, List<FeedbackPerformanceInput> performances, bool finalizarAvaliacao);
        Task<bool> FinalizeFeedbackAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao);
    }

    public class FeedbackPerformanceService : IFeedbackPerformanceService
    {
        private readonly ApplicationDbContext _db;
        private readonly FeedbackPerformanceValidator _validator;
        private readonly FeedbackPerformanceHelper _helper;

        public FeedbackPerformanceService(ApplicationDbContext db, FeedbackPerformanceValidator validator, FeedbackPerformanceHelper helper)
        {
            _db = db;
            _validator = validator;
            _helper = helper;
        }

        public async Task<FeedbackPerformanceResult> GetFeedbackPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
        {
            var validation = _validator.ValidateIds(idProjeto, idAssociado, idPeriodo);
            if (!validation.IsValid)
                return FeedbackPerformanceResult.Failure(validation.Message);

            var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
            var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
            var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
            var gestor = projeto != null ? await _db.Associados.FirstOrDefaultAsync(a => a.Id == projeto.IdAssociadoGestor) : null;

            if (projeto == null || periodo == null || associado == null || gestor == null)
                return FeedbackPerformanceResult.Failure("Dados de projeto, período, associado ou gestor não encontrados.");

            var avaliacaoEmail = await _db.Set<AvaliacaoEmail>().FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
            DateTime? dataLiberacao = avaliacaoEmail?.DataLiberacao;
            int duracaoFeedback = avaliacaoEmail?.PRAZOS?.DuracaoFeedback ?? 0;
            int compensadorFeedback = avaliacaoEmail?.PRAZOS?.CompensadorFeedback ?? 0;
            string dataFinal = dataLiberacao.HasValue ? dataLiberacao.Value.AddDays(duracaoFeedback).ToString("dd/MM/yyyy") : "";
            string tempoRestante = _helper.CalculaTempoRestante(dataFinal, compensadorFeedback);

            var avaliacaoPerformance = await _db.Set<AvaliacaoPerformance>().FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo && a.Ativo);
            List<int> listaIdPerformances = new List<int>();
            if (avaliacaoPerformance != null)
            {
                listaIdPerformances = await _db.Set<AvaliacaoPerformance>()
                    .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo && a.Ativo)
                    .Select(a => a.IdPerformance)
                    .ToListAsync();
            }
            var performances = await _helper.ObterListaPerformancesAsync(_db, associado.IdEmpresa, avaliacaoPerformance?.IdCargo ?? associado.IdCargo, associado.IdNivel, listaIdPerformances.Count > 0 ? listaIdPerformances : null);
            var listaPerformancesModel = _helper.OrganizarPerformances(performances);

            return FeedbackPerformanceResult.Success(new FeedbackPerformanceViewModel
            {
                Projeto = projeto,
                Periodo = periodo,
                Associado = associado,
                Gestor = gestor,
                Cliente = projeto.Cliente,
                TempoRestante = tempoRestante,
                Performances = listaPerformancesModel
            });
        }

        public async Task<bool> SaveFeedbackPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao, List<FeedbackPerformanceInput> performances, bool finalizarAvaliacao)
        {
            foreach (var perf in performances)
            {
                var avaliacao = await _db.Set<AvaliacaoPerformance>().FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPerformance == perf.IdPerformance && a.IdPeriodo == idPeriodo);
                if (avaliacao == null || avaliacao.PosicaoAtualFluxoAvaliacao != AvaliacaoPerformance.EtapaFeedback)
                    return false;
                avaliacao.IdNotaNivel1Feedback = perf.IdNotaNivel1Feedback;
                avaliacao.ComentariosFeedback = perf.ComentariosFeedback?.Trim();
                if (avaliacao.DataHoraInicioFeedback == null || avaliacao.DataHoraInicioFeedback == DateTime.MinValue)
                    avaliacao.DataHoraInicioFeedback = DateTime.Now;
                if (finalizarAvaliacao)
                    avaliacao.DataHoraFimFeedback = DateTime.Now;
                _db.Update(avaliacao);
            }
            await _db.SaveChangesAsync();
            // Atualiza status da avaliação email e avança etapa se necessário
            if (finalizarAvaliacao)
                await FinalizeFeedbackAsync(idProjeto, idAssociado, idPeriodo, idAvaliacao);
            return true;
        }

        public async Task<bool> FinalizeFeedbackAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
        {
            var avaliacaoEmail = await _db.Set<AvaliacaoEmail>().FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
            if (avaliacaoEmail == null)
                return false;
            // Avança etapa e salva
            avaliacaoEmail.PosicaoAtualFluxoAvaliacao = AvaliacaoPerformance.EtapaFeedback;
            _db.Update(avaliacaoEmail);
            await _db.SaveChangesAsync();
            // Chamar lógica de avanço de etapa se existir
            return true;
        }
    }

    public class FeedbackPerformanceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public FeedbackPerformanceViewModel? Data { get; set; }

        public static FeedbackPerformanceResult Failure(string message) => new FeedbackPerformanceResult { Success = false, Message = message };
        public static FeedbackPerformanceResult Success(FeedbackPerformanceViewModel data) => new FeedbackPerformanceResult { Success = true, Data = data };
    }

    public class FeedbackPerformanceInput
    {
        public int IdPerformance { get; set; }
        public int IdNotaNivel1Feedback { get; set; }
        public string? ComentariosFeedback { get; set; }
    }

    public class FeedbackPerformanceViewModel
    {
        public Projeto Projeto { get; set; }
        public PERIODOSAVALIACOES Periodo { get; set; }
        public Associado Associado { get; set; }
        public Associado Gestor { get; set; }
        public Cliente Cliente { get; set; }
        public string TempoRestante { get; set; }
        public List<PerformanceModel> Performances { get; set; } = new();
    }

    public class PerformanceModel
    {
        public int IdPerformance { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? Abaixo { get; set; }
        public string? Esperado { get; set; }
        public string? Acima { get; set; }
        public string Abrangencia { get; set; } = string.Empty;
        public string SeparadorAbrangencia { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        public string DisclaimerInput { get; set; } = string.Empty;
    }

    // Mock de entidades de domínio para garantir compilação (substitua pelos reais do projeto)
    public class AvaliacaoPerformance
    {
        public int IdAssociado { get; set; }
        public int IdProjeto { get; set; }
        public int IdPerformance { get; set; }
        public int IdPeriodo { get; set; }
        public int? IdNotaNivel1Feedback { get; set; }
        public string? ComentariosFeedback { get; set; }
        public DateTime? DataHoraInicioFeedback { get; set; }
        public DateTime? DataHoraFimFeedback { get; set; }
        public int PosicaoAtualFluxoAvaliacao { get; set; }
        public bool Ativo { get; set; }
        public static int EtapaFeedback => 4;
    }
    public class AvaliacaoEmail
    {
        public int idAvaliacao { get; set; }
        public DateTime? DataLiberacao { get; set; }
        public Prazo? PRAZOS { get; set; }
        public int PosicaoAtualFluxoAvaliacao { get; set; }
        public string? TipoAvaliacao { get; set; }
    }
    public class Prazo
    {
        public int DuracaoFeedback { get; set; }
        public int CompensadorFeedback { get; set; }
    }
}
