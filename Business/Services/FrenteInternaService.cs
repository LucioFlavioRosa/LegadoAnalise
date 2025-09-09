using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class FrenteInternaService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FrenteInternaService));
        public List<FRENTEINTERNA> ObterFrentesInternas()
        {
            DataModel context = new DataModel();

            List<FRENTEINTERNA> returnFrentesInternas = new List<FRENTEINTERNA>();
            returnFrentesInternas = context.FRENTEINTERNA.ToList();
            return returnFrentesInternas;
        }
        public List<FRENTEINTERNA> ObterFrenteInterna(int idFrenteInterna = -1)
        {
            DataModel context = new DataModel();

            List<FRENTEINTERNA> returnFrentesInternas = new List<FRENTEINTERNA>();
            returnFrentesInternas = context.FRENTEINTERNA.ToList();

            if (idFrenteInterna > -1) { returnFrentesInternas = returnFrentesInternas.Where(x => x.idFrenteInterna == idFrenteInterna).ToList(); }

            return returnFrentesInternas;
        }
        public List<LIDERESFRENTEINTERNA> ObterLideresFrentesInternas()
        {
            DataModel context = new DataModel();

            var returnLideresFrentesInternas = new List<LIDERESFRENTEINTERNA>();
            returnLideresFrentesInternas = context.LIDERESFRENTEINTERNA.ToList();
            return returnLideresFrentesInternas;
        }
        public List<LIDERESFRENTEINTERNA> ObterLiderFrenteInterna(int idLiderFrenteInterna = -1, int idAssociado = -1, int idFrenteInterna = -1, bool? ATV = true)
        {
            DataModel context = new DataModel();

            var returnLiderFrentesInternas = new List<LIDERESFRENTEINTERNA>();
            returnLiderFrentesInternas = context.LIDERESFRENTEINTERNA.ToList();

            if (idLiderFrenteInterna > -1) { returnLiderFrentesInternas = returnLiderFrentesInternas.Where(x => x.idLiderFrenteInterna == idLiderFrenteInterna).ToList(); }
            if (idAssociado > -1) { returnLiderFrentesInternas = returnLiderFrentesInternas.Where(x => x.idAssociado == idAssociado).ToList(); }
            if (idFrenteInterna > -1) { returnLiderFrentesInternas = returnLiderFrentesInternas.Where(x => x.idFrenteInterna == idFrenteInterna).ToList(); }
            if (ATV != null) { returnLiderFrentesInternas = returnLiderFrentesInternas.Where(x => x.ATV == ATV).ToList(); }

            return returnLiderFrentesInternas;
        }
        public List<PARTICIPANTESFRENTEINTERNA> ObterParticipantesFrentesInternas(int idParticipanteFrenteInterna = -1, int idFrenteInterna = -1)
        {
            DataModel context = new DataModel();

            var returnItems = new List<PARTICIPANTESFRENTEINTERNA>();
            returnItems = context.PARTICIPANTESFRENTEINTERNA.ToList();

            if (idParticipanteFrenteInterna > -1) { returnItems = returnItems.Where(x => x.idParticipanteFrenteInterna == idParticipanteFrenteInterna).ToList(); }
            if (idFrenteInterna > -1) { returnItems = returnItems.Where(x => x.idFrenteInterna == idFrenteInterna).ToList(); }

            return returnItems;
        }
        public List<AVALIACOESALOCACOESINTERNAS> ObterAvaliacoesAlocacoesInternas(int idAvaliacaoAlocacaoInterna = -1, int idAlocacaoInterna = -1, int idAvaliador = -1, int idAssociado = -1, int idPeriodo = -1)
        {
            DataModel context = new DataModel();

            var returnItems = new List<AVALIACOESALOCACOESINTERNAS>();
            returnItems = context.AVALIACOESALOCACOESINTERNAS.ToList();

            if (idAvaliacaoAlocacaoInterna > -1) { returnItems = returnItems.Where(x => x.idAvaliacaoAlocacaoInterna == idAvaliacaoAlocacaoInterna).ToList(); }
            if (idAvaliador > -1) { returnItems = returnItems.Where(x => x.idAvaliador == idAvaliador).ToList(); }
            if (idAlocacaoInterna > -1) { returnItems = returnItems.Where(x => x.idAlocacaoInterna == idAlocacaoInterna).ToList(); }
            if (idAssociado > -1) { returnItems = returnItems.Where(x => x.idAssociado == idAssociado).ToList(); }
            if (idPeriodo > -1) { returnItems = returnItems.Where(x => x.idPeriodo == idPeriodo).ToList(); }

            return returnItems;
        }
        public List<NOTASALOCACOESINTERNAS> ObterNotasAlocacoesInternas(int idNotaAlocacaoInterna = -1, bool? ATV = true)
        {
            DataModel context = new DataModel();

            var returnItems = new List<NOTASALOCACOESINTERNAS>();
            returnItems = context.NOTASALOCACOESINTERNAS.ToList();

            if (idNotaAlocacaoInterna > -1) { returnItems = returnItems.Where(x => x.idNotaAlocacaoInterna == idNotaAlocacaoInterna).ToList(); }
            if (ATV != null) { returnItems = returnItems.Where(x => x.ATV == ATV).ToList(); }
            return returnItems;
        }
        public bool GerirFrenteInterna(FRENTEINTERNA item)
        {
            try
            {
                DataModel context = new DataModel();
                if (item.idFrenteInterna <= 0)
                    context.FRENTEINTERNA.Add(item);
                else
                    context.Entry(context.FRENTEINTERNA.FirstOrDefault(x => x.idFrenteInterna == item.idFrenteInterna)).CurrentValues.SetValues(item);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }
            return true;
        }
        public bool GerirLiderFrenteInterna(LIDERESFRENTEINTERNA item)
        {
            try
            {
                DataModel context = new DataModel();
                if (item.idLiderFrenteInterna <= 0)
                    context.LIDERESFRENTEINTERNA.Add(item);
                else
                    context.Entry(context.LIDERESFRENTEINTERNA.FirstOrDefault(x => x.idLiderFrenteInterna == item.idLiderFrenteInterna)).CurrentValues.SetValues(item);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }
            return true;
        }
        public bool GerirParticipanteFrenteInterna(PARTICIPANTESFRENTEINTERNA item)
        {
            try
            {
                DataModel context = new DataModel();
                if (item.idParticipanteFrenteInterna <= 0)
                    context.PARTICIPANTESFRENTEINTERNA.Add(item);
                else
                    context.Entry(context.PARTICIPANTESFRENTEINTERNA.FirstOrDefault(x => x.idParticipanteFrenteInterna == item.idParticipanteFrenteInterna)).CurrentValues.SetValues(item);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }
            return true;
        }
        public bool GerirAvaliacaoAlocacaoInterna(AVALIACOESALOCACOESINTERNAS item)
        {
            try
            {
                DataModel context = new DataModel();
                if (item.idAvaliacaoAlocacaoInterna <= 0)
                    context.AVALIACOESALOCACOESINTERNAS.Add(item);
                else
                    context.Entry(context.AVALIACOESALOCACOESINTERNAS.FirstOrDefault(x => x.idAvaliacaoAlocacaoInterna == item.idAvaliacaoAlocacaoInterna)).CurrentValues.SetValues(item);

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
