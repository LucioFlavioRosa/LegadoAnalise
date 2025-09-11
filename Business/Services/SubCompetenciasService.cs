using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Services
{
    public class SubCompetenciasService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SubCompetenciasService));
        public bool InserirSubCompetencia(SUBCOMPETENCIAS subCompetencia)
        {
            try
            {
                DataModel context = new DataModel();
                context.SUBCOMPETENCIAS.Add(subCompetencia);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public SUBCOMPETENCIAS ObterCompetencia(int idSubCompetencia)
        {
            DataModel context = new DataModel();
            return context.SUBCOMPETENCIAS.FirstOrDefault(a => a.IdSubCompetencia == idSubCompetencia);
        }

        public List<SUBCOMPETENCIAS> ListaSubCompetencias()
        {
            DataModel context = new DataModel();
            return context.SUBCOMPETENCIAS.ToList();
        }

        public List<SUBCOMPETENCIAS> ListaSubCompetencias(bool ativos, string tipoaval = "")
        {
            DataModel context = new DataModel();
            return context.SUBCOMPETENCIAS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.SubCompetencia).ToList();
            
        }
        




        public bool AlterarSubCompetencia(SUBCOMPETENCIAS subCompetencia)
        {
            try
            {
                DataModel context = new DataModel();
                SUBCOMPETENCIAS subCompetenciaAtual = new SUBCOMPETENCIAS();
                subCompetenciaAtual = context.SUBCOMPETENCIAS.First(a => a.IdSubCompetencia == subCompetencia.IdSubCompetencia);

                if (subCompetenciaAtual != null)
                {
                    subCompetenciaAtual.SubCompetencia = subCompetencia.SubCompetencia;
                    subCompetenciaAtual.ATV = subCompetencia.ATV;
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


        public bool ExcluirSubCompetencia(int idSubCompetencia)
        {
            try
            {
                DataModel context = new DataModel();
                SUBCOMPETENCIAS subCompetenciaAtual = new SUBCOMPETENCIAS();
                subCompetenciaAtual = context.SUBCOMPETENCIAS.First(a => a.IdSubCompetencia == idSubCompetencia);

                if (subCompetenciaAtual != null)
                {
                    subCompetenciaAtual.ATV = 0;
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