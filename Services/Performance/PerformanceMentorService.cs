using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Business.Services;
using Business.Util;
using Microsoft.EntityFrameworkCore;
using Services.Performance;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Services.Performance
{
    public class PerformanceMentorService : IPerformanceMentorService
    {
        private readonly ApplicationDbContext _db;
        private readonly ProjetosService _projetosService;
        private readonly PeriodoService _periodoService;
        private readonly AssociadosService _associadosService;
        private readonly ClientesService _clientesService;
        private readonly AvaliacoesService _avaliacoesService;
        private readonly PerformancesService _performancesService;
        private readonly NotasAvaliacaoService _notasService;
        private readonly Util _util;

        public PerformanceMentorService(
            ApplicationDbContext db,
            ProjetosService projetosService,
            PeriodoService periodoService,
            AssociadosService associadosService,
            ClientesService clientesService,
            AvaliacoesService avaliacoesService,
            PerformancesService performancesService,
            NotasAvaliacaoService notasService,
            Util util)
        {
            _db = db;
            _projetosService = projetosService;
            _periodoService = periodoService;
            _associadosService = associadosService;
            _clientesService = clientesService;
            _avaliacoesService = avaliacoesService;
            _performancesService = performancesService;
            _notasService = notasService;
            _util = util;
        }

        public async Task<MentorPerformanceResult> GetMentorPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo)
        {
            var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
            var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
            var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
            var gestor = projeto != null ? await _db.Associados.FirstOrDefaultAsync(a => a.Id == projeto.IdAssociadoGestor) : null;
            var cliente = projeto != null ? await _db.Clientes.FirstOrDefaultAsync(c => c.IdCliente == projeto.IdCliente) : null;

            var tempoPeers = associado != null ? _util.TempoAssociado(associado.Id, ReturnTempo.texto_TempoDePeers).ToString() : string.Empty;
            var tempoCargo = associado != null ? _util.TempoAssociado(associado.Id, ReturnTempo.texto_TempoDeCargo).ToString() : string.Empty;

            return new MentorPerformanceResult
            {
                Associado = associado ?? new Associado(),
                Projeto = projeto ?? new Projeto(),
                Periodo = periodo ?? new PERIODOSAVALIACOES(),
                Gestor = gestor ?? new Associado(),
                Cliente = cliente ?? new Cliente(),
                TempoPeers = tempoPeers,
                TempoCargo = tempoCargo
            };
        }

        public async Task<List<PerformanceModel>> GetPerformanceListAsync(int idAssociado, int idProjeto, int idPeriodo)
        {
            var avaliacaoPerformance = await _avaliacoesService.ObterAvaliacaoPerformance(idAssociado, idProjeto, idPeriodo, true);
            int idCargoNaAvaliacao = avaliacaoPerformance?.IdCargo ?? 0;
            int idNivelNaAvaliacao = avaliacaoPerformance?.IdNivel ?? 0;

            var listaPerformances = await _avaliacoesService.ObterAvaliacoesPerformances(idAssociado, idProjeto, idPeriodo, true);
            var listaIdPerformances = listaPerformances.Select(perf => perf.IdPerformance).ToList();
            var performances = await _performancesService.ObterListaPerformancesAsync(
                idAssociado > 0 ? (await _db.Associados.FirstOrDefaultAsync(a => a.Id == idAssociado)).IdEmpresa : 0,
                idCargoNaAvaliacao,
                idNivelNaAvaliacao,
                listaIdPerformances
            );

            var abrangenciasContadas = new HashSet<string>();
            var listaPerformancesModel = new List<PerformanceModel>();

            foreach (var item in performances)
            {
                var headerSeparador = !abrangenciasContadas.Contains(item.Abrangencia);
                if (headerSeparador)
                {
                    abrangenciasContadas.Add(item.Abrangencia);
                }
                listaPerformancesModel.Add(new PerformanceModel
                {
                    IdPerformance = item.IdPerformance,
                    Descricao = item.Performance,
                    Abaixo = item.PerformanceAbaixo,
                    Esperado = item.PerformanceEsperado,
                    Acima = item.PerformanceAcima,
                    Abrangencia = item.Abrangencia != null ? item.Abrangencia.ToUpper() : string.Empty,
                    SeparadorAbrangencia = headerSeparador ? string.Empty : "hidden"
                });
            }

            return listaPerformancesModel;
        }

        public async Task<PerformanceNotasResult> GetNotasAsync(int idAssociado, int idProjeto, int idPerformance, int idPeriodo)
        {
            var avaliacao = await _avaliacoesService.ObterAvaliacaoPerformance(idAssociado, idProjeto, idPerformance, idPeriodo);
            var result = new PerformanceNotasResult();

            if (avaliacao != null)
            {
                var notaAvaliado = _notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1AutoAvaliacao);
                result.NotaAvaliado = notaAvaliado?.CodigoNota ?? string.Empty;
                result.ComentarioAvaliado = avaliacao.ComentariosAutoAvaliacao ?? string.Empty;

                var notaCegas = _notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1AvaliacaoCegas);
                result.NotaCegas = notaCegas?.CodigoNota ?? string.Empty;
                result.ComentarioCegas = avaliacao.ComentariosAvaliacaoCegas ?? string.Empty;

                var notaGestor = _notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1AvaliacaoGestor);
                result.NotaGestor = notaGestor?.CodigoNota ?? string.Empty;
                result.ComentarioGestor = avaliacao.ComentariosAvaliacaoGestor ?? string.Empty;

                var notaFeedback = _notasService.ObterNotaPerformance(avaliacao.IdNotaNivel1Feedback);
                result.NotaFeedback = notaFeedback?.CodigoNota ?? string.Empty;
                result.ComentarioFeedback = avaliacao.ComentariosFeedback ?? string.Empty;
            }

            return result;
        }
    }
}
