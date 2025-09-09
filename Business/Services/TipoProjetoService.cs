using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class TipoProjetoService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TipoProjetoService));
        public bool InserirTipoProjeto(PROJETOSTIPOS tipoProjeto)
        {
            try
            {
                DataModel context = new DataModel();
                context.PROJETOSTIPOS.Add(tipoProjeto);
                context.SaveChanges();
                
            }
            catch (Exception ex) 
            { 
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message); 
                return false; 
            }

            return true;
        }

        public List<PROJETOSTIPOS> ObterListaTipos()
        {
            DataModel context = new DataModel();

            List<PROJETOSTIPOS> listatipos = new List<PROJETOSTIPOS>();
            listatipos = context.PROJETOSTIPOS.ToList();
            return listatipos;
        }

        public List<PROJETOSTIPOS> ObterListaTipos(bool ativos)
        {
            DataModel context = new DataModel();

            List<PROJETOSTIPOS> listatipos = new List<PROJETOSTIPOS>();
            listatipos = context.PROJETOSTIPOS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.ProjetoTipo).ToList();
            return listatipos;
        }



        public bool AlterarTipoProjeto(PROJETOSTIPOS projeto)
        {
            try
            {
                DataModel context = new DataModel();
                PROJETOSTIPOS projetoAtual = new PROJETOSTIPOS();
                projetoAtual = context.PROJETOSTIPOS.First(a => a.IdTipo == projeto.IdTipo);

                if (projetoAtual != null)
                {
                    projetoAtual.ProjetoTipo = projeto.ProjetoTipo;
                    projetoAtual.ATV = projeto.ATV;
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


        public PROJETOSTIPOS ObterTipoProjeto(int idTipoProjeto)
        {
            DataModel context = new DataModel();
            return context.PROJETOSTIPOS.FirstOrDefault(c => c.IdTipo == idTipoProjeto);
        }

        public bool ExcluirTipoProjeto(int idTipoProjeto)
        {
            try
            {
                DataModel context = new DataModel();
                PROJETOSTIPOS projeto = new PROJETOSTIPOS();
                projeto = context.PROJETOSTIPOS.First(a => a.IdTipo == idTipoProjeto);

                if (projeto != null)
                {
                    projeto.ATV = 0;
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