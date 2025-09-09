using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class PerformancesService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PerformancesService));
        public bool InserirPerformance(PERFORMANCES performance)
        {
            try
            {
                DataModel context = new DataModel();
                context.PERFORMANCES.Add(performance);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public PERFORMANCES ObterPerformance(int idPerformance)
        {
            DataModel context = new DataModel();
            return context.PERFORMANCES.FirstOrDefault(p => p.IdPerformance == idPerformance);
        }

        public List<PERFORMANCES> ObterListaPerformances()
        {
            DataModel context = new DataModel();
            return context.PERFORMANCES.ToList();
        }

        public List<PERFORMANCES> ObterListaPerformances(int idEmpresa, int idCargo, int idNivel, List<int> listaIdPerformances)
        {
            DataModel context = new DataModel();
            var returnLista = context.PERFORMANCES.Where(c => c.IdEmpresa == idEmpresa && c.IdCargo == idCargo && c.IdNivel == idNivel).OrderBy(o => o.IdPerformance).ToList();

            // AVALIAÇÃO JÁ INICIADA
            if (listaIdPerformances != null)
            {
                returnLista = returnLista.Where(c => listaIdPerformances.Contains(c.IdPerformance)).ToList();
            }
            // NOVA
            else
            {
                returnLista = returnLista.Where(c => c.ATV == 1).ToList();
            }

            return returnLista;
        }


        public bool AlterarPerformance(PERFORMANCES performance)
        {
            try
            {
                DataModel context = new DataModel();
                PERFORMANCES performanceAtual = new PERFORMANCES();
                performanceAtual = context.PERFORMANCES.First(a => a.IdPerformance == performance.IdPerformance);

                if (performanceAtual != null)
                {
                    performanceAtual.IdCargo = performance.IdCargo;
                    performanceAtual.IdNivel = performance.IdNivel;
                    performanceAtual.ATV = performance.ATV;
                    performanceAtual.Performance = performance.Performance;
                    performanceAtual.PerformanceAbaixo = performance.PerformanceAbaixo;
                    performanceAtual.PerformanceEsperado = performance.PerformanceEsperado;
                    performanceAtual.PerformanceAcima = performance.PerformanceAcima;
                    performanceAtual.Abrangencia = performance.Abrangencia;
                    // INPUTS
                    performanceAtual.InputAutoavaliacao = performance.InputAutoavaliacao;
                    performanceAtual.NotaPadraoAutoAvaliacao = performance.NotaPadraoAutoAvaliacao;
                    performanceAtual.InputAvaliacaoAsCegas= performance.InputAvaliacaoAsCegas;
                    performanceAtual.NotaPadraoAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas;
                    performanceAtual.InputAvaliacaoGestor = performance.InputAvaliacaoGestor;
                    performanceAtual.NotaPadraoAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor;
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }


        public bool ExcuirPerformance(int idPerformance)
        {
            try
            {
                DataModel context = new DataModel();
                PERFORMANCES performanceAtual = new PERFORMANCES();
                performanceAtual = context.PERFORMANCES.First(a => a.IdPerformance == idPerformance);

                if (performanceAtual != null)
                {
                    performanceAtual.ATV = 0;
                }
                context.SaveChanges();
            }
            catch (Exception ex) 
            { 
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message); 
                return false; 
            }

            return true;
        }


        


    }
}