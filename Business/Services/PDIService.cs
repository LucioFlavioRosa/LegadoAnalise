using Business.DataAccess;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Business.Services
{
    public class PDIService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PDIService));
        public List<PDI_QUESTOES> ObterPDIQuestoes(int idPDIQuestoes = -1, int ATV = 1)
        {
            DataModel context = new DataModel();
            var returnItems = new List<PDI_QUESTOES>();

            returnItems = context.PDI_QUESTOES.ToList();
            if (ATV != -1 ) { returnItems = returnItems.Where(x => x.ATV == ATV).ToList();}
            if (idPDIQuestoes != -1 ) { returnItems = returnItems.Where(x => x.idPDIQuestoes == idPDIQuestoes).ToList();}

            return returnItems;
        }
        public List<PDI_RESPOSTAS> ObterPDIRespostas(int idPDIRespostas = -1, int idAssociado = -1, int idPeriodo = -1, int idPDIQuestao = -1)
        {
            DataModel context = new DataModel();
            var returnItems = new List<PDI_RESPOSTAS>();

            returnItems = context.PDI_RESPOSTAS.ToList();
            if (idPDIRespostas != -1) { returnItems = returnItems.Where(x => x.idPDIRespostas == idPDIRespostas).ToList(); }
            if (idAssociado != -1) { returnItems = returnItems.Where(x => x.idAssociado == idAssociado).ToList(); }
            if (idPeriodo != -1) { returnItems = returnItems.Where(x => x.idPeriodo == idPeriodo).ToList(); }
            if (idPDIQuestao != -1) { returnItems = returnItems.Where(x => x.idPDIQuestao == idPDIQuestao).ToList(); }

            return returnItems;
        }

        public bool GerirPDIResposta(PDI_RESPOSTAS item)
        {
            try
            {
                DataModel context = new DataModel();
                if (item.idPDIRespostas <= 0)
                    context.PDI_RESPOSTAS.Add(item);
                else
                    context.Entry(context.PDI_RESPOSTAS.FirstOrDefault(x => x.idPDIRespostas == item.idPDIRespostas)).CurrentValues.SetValues(item);

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
