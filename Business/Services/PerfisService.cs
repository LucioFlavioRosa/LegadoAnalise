using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Services
{
    public class PerfisService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PerfisService));
        public bool InserirPerfil(PERFIS perfil)
        {
            try
            {
                DataModel context = new DataModel();
                context.PERFIS.Add(perfil);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }



        public PERFIS ObterPerfil(PERFIS perfil)
        {
            DataModel context = new DataModel();
            return context.PERFIS.FirstOrDefault(a => a.IdPerfil == perfil.IdPerfil);
        }

        public List<PERFIS> ObterListaPerfis(bool ativos)
        {
            DataModel context = new DataModel();            
            return context.PERFIS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Perfil).ToList();
        }


        public List<PERFIS> ObterListaPerfis()
        {
            DataModel context = new DataModel();
            return context.PERFIS.OrderBy(a => a.Perfil).ToList();
        }


        public bool AlterarPerfil(PERFIS perfil)
        {
            try
            {
                DataModel context = new DataModel();
                PERFIS perfilAtual = new PERFIS();
                perfilAtual  = context.PERFIS.First(a => a.IdPerfil == perfil.IdPerfil);

                if (perfilAtual != null)
                {
                    perfilAtual.Perfil = perfil.Perfil;
                    perfilAtual.ATV = perfil.ATV;
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



        public bool ExcluirPerfil(int idPerfil)
        {
            try
            {
                DataModel context = new DataModel();
                PERFIS perfilAtual = new PERFIS();
                perfilAtual = context.PERFIS.First(a => a.IdPerfil == idPerfil);

                if (perfilAtual != null)
                {
                    perfilAtual.ATV = 0;
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