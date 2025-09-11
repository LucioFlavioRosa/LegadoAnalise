using Business.DataAccess;
using Business.Model;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class DashboardService
    {
        const string etapaNaoIniciada = "AVM";
        const string etapaAutoAvaliacao = "AAV";
        const string etapaAvaliacaoAsCegas = "ACE";
        const string etapaAvaliacaoGestor = "AGE";
        const string etapaFeedback = "FED";
        const string etapaAvaliacaoMentor = "AME";
        const string etapaAvaliacaoFinalizada = "AFI";

        DataModel _context;

        public DashboardService()
        {
            _context = new DataModel();
        }

        public List<DashboardModel> AvaliadosPorProjeto(int idperiodo)
        {

            return (from a in _context.AVALIACAO
                    join p in _context.PROJETOS
                    on a.idProjeto equals p.IdProjeto
                    where a.idPeriodo == idperiodo
                    group new { a.idProjeto, p.Projeto } by new { a.idProjeto, p.Projeto } into grp
                    select new DashboardModel()
                    {
                        Status = grp.Key.Projeto,
                        QtdStatus = grp.Count()
                    }).ToList();

        }

        public DashboardModel AvaliadosPorPeriodo()
        {
            var periodos = _context.PERIODOSAVALIACOES;
            var status = _context.AVALIACAO.GroupBy(x => x.idPeriodo);
            DashboardModel model = new DashboardModel();

            if (status != null)
            {
                model.ListAndamento = new List<DashboardModel>();

                foreach (var item in status)
                {
                    model.ListAndamento.Add(new DashboardModel
                    {
                        Status = periodos.FirstOrDefault(x => x.IdPeriodo == item.Key).Codigo,
                        QtdStatus = item.Count()
                    });
                }
            }

            return model;
        }

        public DashboardModel AndamentoAvaliacao(int idperiodo)
        {
            var status = _context.AVALIACAO.Where(x => x.idPeriodo == idperiodo).GroupBy(x => x.PosicaoAtualFluxoAvaliacao);
            DashboardModel model = new DashboardModel();

            if (status != null)
            {
                model.ListAndamento = new List<DashboardModel>();

                foreach (var item in status)
                {
                    switch (item.Key)
                    {
                        case etapaNaoIniciada:
                            model.ListAndamento.Add(new DashboardModel { Status = "NÃO INICIADO", QtdStatus = item.Count() });
                            break;
                        case etapaAutoAvaliacao:
                            model.ListAndamento.Add(new DashboardModel { Status = "INICIADO", QtdStatus = item.Count() });
                            break;
                        case etapaAvaliacaoAsCegas:
                            model.ListAndamento.Add(new DashboardModel { Status = "AS CEGAS", QtdStatus = item.Count() });
                            break;
                        case etapaAvaliacaoGestor:
                            model.ListAndamento.Add(new DashboardModel { Status = "GESTOR", QtdStatus = item.Count() });
                            break;
                        case etapaFeedback:
                            model.ListAndamento.Add(new DashboardModel { Status = "FEEDBACK", QtdStatus = item.Count() });
                            break;
                        case etapaAvaliacaoMentor:
                            model.ListAndamento.Add(new DashboardModel { Status = "MENTOR", QtdStatus = item.Count() });
                            break;
                        case etapaAvaliacaoFinalizada:
                            model.ListAndamento.Add(new DashboardModel { Status = "FINALIZADAS", QtdStatus = item.Count() });
                            break;
                    }
                }
            }

            return model;
        }

        public DashboardModel Numeros(int idperiodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            DashboardModel model = new DashboardModel();

            var associados = _context.ASSOCIADOS.Where(x => x.ATV == 1);
            model.QtdAssociados = associados.Count();
            model.QtdMentores = associados.GroupBy(x => x.IdAssociadoMentor).Count();
            var avaliacoes = _context.AVALIACAO.Where(x => x.idPeriodo == idperiodo);
            model.QtdAvaliacoes = avaliacoes.Count();

            var periodoresult = _context.PERIODOSAVALIACOES.FirstOrDefault(x => x.IdPeriodo == idperiodo);

            var projetosassociados = _context.PROJETOSASSOCIADOS.Where(x => x.ATV == 1 &&
            x.DataInicio >= periodoresult.DataInicio.Value && x.DataInicio <= periodoresult.DataFim.Value
            && x.DataFim.Value >= periodoresult.DataInicio && x.DataFim.Value <= periodoresult.DataFim.Value);

            model.QtdGestores = projetosassociados.GroupBy(x => x.IdGestor).Count();
            model.QtdAvaliadores = projetosassociados.GroupBy(x => x.IdAvaliador).Count();

            return model;
        }



        public ResultadoProjetosModel RadarConsolidadoCompetencia(int idperiodo)
        {

            var listcompetencias = (from e in _context.EVOLUCAOASSOCIADO
                                    join c in _context.EVOLUCAOCOMPETENCIAS
                                    on e.IdEvolucaoAssociado equals c.IdEvolucaoAssociado
                                    where e.IdPeriodo == idperiodo
                                    select new
                                    {
                                        c.IdEixo,
                                        NotaCompetencia = c.Nota
                                    }).ToList();

            ResultadoProjetosModel projeto = new ResultadoProjetosModel
            {
                ListSomaCompetenciasN1N2 = new List<ResultadoCompetenciaModel>()
            };

            if (listcompetencias != null)
            {
                IEnumerable<EIXOS> listEixos = _context.EIXOS.Where(x => x.ATV.HasValue && x.ATV.Value == 1);

                if (listEixos != null)
                {
                    foreach (var item in listEixos)
                    {
                        var nota = listcompetencias.Where(x => x.IdEixo == item.IdEixo).Average(x => x.NotaCompetencia);
                        projeto.ListSomaCompetenciasN1N2.Add(new ResultadoCompetenciaModel
                        {
                            PercentualSomaNotaFinalN1N2 = nota.HasValue ? Math.Round(nota.Value * 100) : 0,
                            Eixo = item.Eixo
                        });

                    }
                }
            }

            return projeto;

        }

        public List<EvolucaoPerformanceModel> ChartConsolidadoPerformance(int idperiodo)
        {

            return  (from e in _context.EVOLUCAOASSOCIADO
                      join c in _context.EVOLUCAOPERFORMANCE
                      on e.IdEvolucaoAssociado equals c.IdEvolucaoAssociado
                      join p in _context.PERFORMANCES
                      on c.IdPerformance equals p.IdPerformance
                      where e.IdPeriodo == idperiodo
                      group new { c.Nota, p.Performance } by p.Performance into grp
                      select new EvolucaoPerformanceModel
                      {
                          Performance = grp.Key,
                          Nota = grp.Average(a => a.Nota)
                      }).ToList();

        }

    }
}
