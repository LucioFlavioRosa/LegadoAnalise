using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Performance;
using Services.Common;

namespace Services.Performance
{
    public class PerformanceMentorService : IPerformanceMentorService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAccordionHelper _accordionHelper;

        public PerformanceMentorService(ApplicationDbContext db, IAccordionHelper accordionHelper)
        {
            _db = db;
            _accordionHelper = accordionHelper;
        }

        public async Task<MentorPerformanceContext?> GetMentorPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo)
        {
            var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
            var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
            var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
            var gestor = projeto != null ? await _db.Associados.FirstOrDefaultAsync(a => a.Id == projeto.IdAssociadoGestor) : null;
            var clienteNome = projeto?.Cliente?.Nome ?? string.Empty;

            var tempoPeers = string.Empty;
            var tempoCargo = string.Empty;
            if (associado != null)
            {
                tempoPeers = Util.TempoAssociado(associado.Id, ReturnTempo.texto_TempoDePeers).ToString();
                tempoCargo = Util.TempoAssociado(associado.Id, ReturnTempo.texto_TempoDeCargo).ToString();
            }

            var avaliacaoPerformance = await _db.AvaliacoesPerformance
                .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
                .OrderByDescending(a => a.IdPerformance)
                .FirstOrDefaultAsync();

            var idCargoNaAvaliacao = avaliacaoPerformance?.IdCargo ?? associado?.IdCargo ?? 0;
            var idNivelNaAvaliacao = avaliacaoPerformance?.IdNivel ?? associado?.IdNivel ?? 0;

            return new MentorPerformanceContext
            {
                Projeto = projeto ?? new Projeto(),
                Periodo = periodo ?? new PERIODOSAVALIACOES(),
                Associado = associado ?? new Associado(),
                Gestor = gestor ?? new Associado(),
                ClienteNome = clienteNome,
                TempoPeers = tempoPeers,
                TempoCargo = tempoCargo,
                IdCargoNaAvaliacao = idCargoNaAvaliacao,
                IdNivelNaAvaliacao = idNivelNaAvaliacao
            };
        }

        public async Task<List<PerformanceModel>> GetPerformanceListAsync(int idAssociado, int idProjeto, int idPeriodo)
        {
            var context = await GetMentorPerformanceAsync(idProjeto, idAssociado, idPeriodo);
            if (context == null)
                return new List<PerformanceModel>();

            var listaPerformances = await _db.AvaliacoesPerformance
                .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
                .Select(a => a.IdPerformance)
                .ToListAsync();

            var performances = await _db.Performances
                .Where(p => p.Cargo.IdCargo == context.IdCargoNaAvaliacao && p.CargoNivel.IdNivel == context.IdNivelNaAvaliacao && listaPerformances.Contains(p.IdPerformance))
                .ToListAsync();

            var abrangenciasContadas = new HashSet<string>();
            var listaPerformancesModel = new List<PerformanceModel>();

            foreach (var item in performances.OrderBy(p => p.Abrangencia))
            {
                var headerSeparador = false;
                if (!abrangenciasContadas.Contains(item.Abrangencia ?? ""))
                {
                    abrangenciasContadas.Add(item.Abrangencia ?? "");
                    headerSeparador = true;
                }

                listaPerformancesModel.Add(new PerformanceModel
                {
                    IdPerformance = item.IdPerformance,
                    Descricao = item.Performance ?? string.Empty,
                    Abaixo = item.PerformanceAbaixo ?? string.Empty,
                    Esperado = item.PerformanceEsperado ?? string.Empty,
                    Acima = item.PerformanceAcima ?? string.Empty,
                    Abrangencia = (item.Abrangencia ?? string.Empty).ToUpper(),
                    SeparadorAbrangencia = headerSeparador ? string.Empty : "hidden"
                });
            }

            return listaPerformancesModel;
        }

        public async Task<PerformanceNotas?> GetNotasAsync(int idAssociado, int idProjeto, int idPerformance, int idPeriodo)
        {
            var avaliacao = await _db.AvaliacoesPerformance
                .FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPerformance == idPerformance && a.IdPeriodo == idPeriodo);

            if (avaliacao == null)
                return null;

            var notasService = new NotasAvaliacaoService(_db);

            var notaAvaliado = notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1AutoAvaliacao);
            var notaCegas = notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1AvaliacaoCegas);
            var notaGestor = notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1AvaliacaoGestor);
            var notaFeedback = notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1Feedback);

            return new PerformanceNotas
            {
                NotaAvaliado = notaAvaliado?.CodigoNota ?? string.Empty,
                ObservacaoAvaliado = avaliacao.ComentariosAutoAvaliacao ?? string.Empty,
                NotaCegas = notaCegas?.CodigoNota ?? string.Empty,
                ObservacaoCegas = avaliacao.ComentariosAvaliacaoCegas ?? string.Empty,
                NotaGestor = notaGestor?.CodigoNota ?? string.Empty,
                ObservacaoGestor = avaliacao.ComentariosAvaliacaoGestor ?? string.Empty,
                NotaFeedback = notaFeedback?.CodigoNota ?? string.Empty,
                ObservacaoFeedback = avaliacao.ComentariosFeedback ?? string.Empty
            };
        }
    }

    public static class Util
    {
        public static object TempoAssociado(int idAssociado, string tipo)
        {
            // Implementação dummy para manter compatibilidade; deve ser substituída por lógica real
            return "";
        }
    }

    public static class ReturnTempo
    {
        public const string texto_TempoDePeers = "TempoDePeers";
        public const string texto_TempoDeCargo = "TempoDeCargo";
    }

    public class NotasAvaliacaoService
    {
        private readonly ApplicationDbContext _db;
        public NotasAvaliacaoService(ApplicationDbContext db)
        {
            _db = db;
        }
        public NotaPerformance? ObterNotaPerformance(int? idNota)
        {
            if (!idNota.HasValue)
                return null;
            return _db.AvaliacoesCompetenciasNotas.FirstOrDefault(n => n.IdNota == idNota.Value);
        }
    }

    public class NotaPerformance
    {
        public int IdNota { get; set; }
        public string CodigoNota { get; set; } = string.Empty;
    }
}
