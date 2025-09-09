using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Business.Services
{
    public class PeriodoService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PeriodoService));
        public void AlterarPeriodo(PERIODOSAVALIACOES periodo)
        {
            try
            {
                DataModel context = new DataModel();
                var obj = context.PERIODOSAVALIACOES.SingleOrDefault(x => x.IdPeriodo == periodo.IdPeriodo);
                context.Entry(obj).CurrentValues.SetValues(periodo);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw ex;
            }

        }

        public void InserirPeriodo(PERIODOSAVALIACOES periodo)
        {
            try
            {
                DataModel context = new DataModel();
                context.PERIODOSAVALIACOES.Add(periodo);
                context.SaveChanges();

            }
            catch(Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw ex;
            }
                        
        }

        public List<PERIODOSAVALIACOES> ListaPeriodos(int idEmpresa)
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.Where(s => s.IdEmpresa == idEmpresa && s.ATV == 1).ToList();
        }

        public List<PERIODOSAVALIACOES> ListaPeriodos(int idEmpresa, DateTime dataInicio, DateTime dataFim)
        {
            DataModel context = new DataModel();
            Expression<Func<PERIODOSAVALIACOES, bool>> predicate = null;

            // Overlap Date
            // bool overlap = a.start < b.end && b.start < a.end;
            predicate = p => p.IdEmpresa == idEmpresa && p.ATV == 1 &&
                (dataInicio <= p.DataFim && p.DataInicio <= dataFim);

            var returnPeriodos = context.PERIODOSAVALIACOES.Where(predicate).ToList();

            if (returnPeriodos == null || returnPeriodos.Count == 0)
            {
                returnPeriodos = new List<PERIODOSAVALIACOES>() { context.PERIODOSAVALIACOES.OrderByDescending(p => p.IdPeriodo).FirstOrDefault(p => p.ATV == 1 && p.DataInicio >= dataInicio) };
            }

            if (returnPeriodos == null || returnPeriodos.Count == 0 || returnPeriodos[0] == null)
            {
                returnPeriodos = new List<PERIODOSAVALIACOES>() { context.PERIODOSAVALIACOES.OrderByDescending(p => p.IdPeriodo).FirstOrDefault(p => p.ATV == 1) };
            }

            return returnPeriodos;
        }

        public PERIODOSAVALIACOES ObterPeriodo(int idPeriodo)
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.FirstOrDefault(p => p.IdPeriodo == idPeriodo);
        }
        public PERIODOSAVALIACOES ObterPeriodoCodigo(string codigo)
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.FirstOrDefault(p => p.Codigo == codigo);
        }

        public PERIODOSAVALIACOES ObterPeriodoAtual()
        {
            DataModel context = new DataModel();
            var dataAtual = DateTime.Now;
            return context.PERIODOSAVALIACOES.FirstOrDefault(p => dataAtual >= p.DataInicio && dataAtual <= p.DataFim);
        }
        public PERIODOSAVALIACOES ObterPeriodoUltimo()
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.OrderByDescending(p => p.IdPeriodo).FirstOrDefault(p => p.ATV == 1);
        }

        public PERIODOSAVALIACOES ObterPeriodoUltimoLiberadoLideranca()
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.OrderByDescending(p => p.IdPeriodo).FirstOrDefault(p => p.ATV == 1 && p.fl_lib_res_lideranca);
        }

        public PERIODOSAVALIACOES ObterPeriodoUltimoLiberadoMentoria()
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.OrderByDescending(p => p.IdPeriodo).FirstOrDefault(p => p.ATV == 1 && p.fl_lib_res_mentoria);
        }

        public PERIODOSAVALIACOES VerificaExistenciaPeriodo(DateTime dataInicio, DateTime dataFim, int idEmpresa)
        {
            DataModel context = new DataModel();

            PERIODOSAVALIACOES periodo = null;

            periodo = context.PERIODOSAVALIACOES.FirstOrDefault(p => p.IdEmpresa == idEmpresa && dataInicio >= p.DataInicio && dataInicio <= p.DataFim && p.ATV == 1);

            if (periodo == null)
            {
                periodo = context.PERIODOSAVALIACOES.OrderByDescending(p => p.IdPeriodo).FirstOrDefault(p => p.IdEmpresa == idEmpresa && dataInicio >= p.DataInicio);
            }

            return periodo;
                       
        }

        public List<PERIODOSAVALIACOES> ListaTodosPeriodos(int idEmpresa)
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.Where(s => s.IdEmpresa == idEmpresa).ToList();
        }


        public List<PERIODOSAVALIACOES> ListaTodosPeriodosLiberadosLideranca(int idEmpresa)
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.Where(s => s.IdEmpresa == idEmpresa && s.ATV == 1 && s.fl_lib_res_lideranca ).ToList();
        }

        public List<PERIODOSAVALIACOES> ListaTodosPeriodosLiberadosMentoria(int idEmpresa)
        {
            DataModel context = new DataModel();
            return context.PERIODOSAVALIACOES.Where(s => s.IdEmpresa == idEmpresa && s.ATV == 1 && s.fl_lib_res_mentoria).ToList();
        }
    }
}
