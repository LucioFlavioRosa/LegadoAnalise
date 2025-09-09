using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class DimensoesService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DimensoesService));
        public bool InserirDimensao(DIMENSOES dimensao)
        {
            try
            {
                DataModel context = new DataModel();
                context.DIMENSOES.Add(dimensao);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public DIMENSOES ObterDimensao(int idDimensao)
        {
            DataModel context = new DataModel();
            return context.DIMENSOES.FirstOrDefault(a => a.IdDimensao == idDimensao);
        }

        public List<DIMENSOES> ListaDimensoes()
        {
            DataModel context = new DataModel();
            return context.DIMENSOES.OrderBy(d => d.IdDimensao).ToList();
        }


        public List<DIMENSOES> ListaDimensoes(bool ativos, string tipoaval = "")
        {
            DataModel context = new DataModel();
            if (tipoaval == ""){
                return context.DIMENSOES.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Dimensao).ToList();}
            else{
                return context.DIMENSOES.Where(a => a.ATV == (ativos ? 1 : 0) && a.TipoAvaliacao == tipoaval).OrderBy(a => a.Dimensao).ToList();}
        }


        public bool AlterarDimensao(DIMENSOES dimensao)
        {
            try
            {
                DataModel context = new DataModel();
                DIMENSOES dimensaoAtual = new DIMENSOES();
                dimensaoAtual = context.DIMENSOES.First(a => a.IdDimensao == dimensao.IdDimensao);

                if (dimensaoAtual != null)
                {
                    dimensaoAtual.Dimensao = dimensao.Dimensao;
                    dimensaoAtual.ATV = dimensao.ATV;
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




        public bool ExcluirDimensao(int idDimensao)
        {
            try
            {
                DataModel context = new DataModel();
                DIMENSOES dimensaoAtual = new DIMENSOES();
                dimensaoAtual = context.DIMENSOES.First(a => a.IdDimensao == idDimensao);

                if (dimensaoAtual != null)
                {
                    dimensaoAtual.ATV = 0;
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
