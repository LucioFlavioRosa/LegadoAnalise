using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class EixoService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EixoService));
        public bool InserirEixo(EIXOS eixo)
        {
            try
            {
                DataModel context = new DataModel();
                context.EIXOS.Add(eixo);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public List<EIXOS> ListaEixos()
        {
            DataModel context = new DataModel();
            return context.EIXOS.ToList();
        }

        public List<EIXOS> ListaEixos(bool ativos, string tipoaval = "")
        {
            DataModel context = new DataModel();
            if (tipoaval == ""){return context.EIXOS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Eixo).ToList();}
            else { return context.EIXOS.Where(a => a.ATV == (ativos ? 1 : 0) && a.TipoAvaliacao == tipoaval).OrderBy(a => a.Eixo).ToList(); }

        }
        

        public EIXOS ObterEixo(int idEixo)
        {
            DataModel context = new DataModel();
            return context.EIXOS.FirstOrDefault(e => e.IdEixo == idEixo);
        }



        public bool AlterarEixo(EIXOS eixo)
        {
            try
            {
                DataModel context = new DataModel();
                EIXOS eixoAtual = new EIXOS();
                eixoAtual = context.EIXOS.First(a => a.IdEixo == eixo.IdEixo);

                if (eixoAtual != null)
                {
                    eixoAtual.Eixo = eixo.Eixo;
                    eixoAtual.ATV = eixo.ATV;
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


        public bool ExcluirEixo(int idEixo)
        {
            try
            {
                DataModel context = new DataModel();
                EIXOS eixoAtual = new EIXOS();
                eixoAtual = context.EIXOS.First(a => a.IdEixo == idEixo);

                if (eixoAtual != null)
                {
                    eixoAtual.ATV = 0;
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
