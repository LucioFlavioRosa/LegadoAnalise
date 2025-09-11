using System;
using System.Collections.Generic;
using System.Linq;
using Business.DataAccess;
using System.Linq.Expressions;

namespace Business.Services
{
    public class ConsideracoesMentorService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConsideracoesMentorService));
        public CONSIDERACOESMENTOR ObterConsideracoesMentorPorId(int idConsideracoesMentor)
        {
            DataModel context = new DataModel();
            return context.CONSIDERACOESMENTOR.FirstOrDefault(cm => cm.idConsideracoesMentor == idConsideracoesMentor);
        }
        public CONSIDERACOESMENTOR ObterConsideracoesMentorPorAtributos(int idMentor, int idAssociado, int idPeriodo, string TipoAvaliacao, string Escopo)
        {
            DataModel context = new DataModel();
            return context.CONSIDERACOESMENTOR.FirstOrDefault(cm => cm.idMentor == idMentor
                                                                && cm.idAssociado == idAssociado
                                                                && cm.idPeriodo == idPeriodo
                                                                && cm.TipoAvaliacao == TipoAvaliacao
                                                                && cm.Escopo == Escopo);
        }

        public CONSIDERACOESMENTOR ObterConsideracoesMentorUltimaDoAvaliado(int idAssociado, int idPeriodo, string TipoAvaliacao, string Escopo)
        {
            CONSIDERACOESMENTOR returnConsideracoesMentor = new CONSIDERACOESMENTOR();
            DataModel context = new DataModel();

            idPeriodo -= 1;
            while (idPeriodo > 0)
            {
                returnConsideracoesMentor =  context.CONSIDERACOESMENTOR.FirstOrDefault(cm => cm.idAssociado == idAssociado
                                                                    && cm.idPeriodo == idPeriodo
                                                                    && cm.TipoAvaliacao == TipoAvaliacao
                                                                    && cm.Escopo == Escopo);
                idPeriodo -= 1;
                if (returnConsideracoesMentor != null){ idPeriodo = -1; }
            }

            return returnConsideracoesMentor;
        }

        public List<CONSIDERACOESMENTOR> ObterConsideracoesMentorListaExport(int idAssociado, int idPeriodo)
        {
            DataModel context = new DataModel();
            Expression<Func<CONSIDERACOESMENTOR, bool>> predicate = cm => cm.idConsideracoesMentor != 0;

            if (idAssociado != 0)
            {
                predicate = predicate.And(cm => cm.idAssociado == idAssociado);
            }
            if (idPeriodo != 0)
            {
                predicate = predicate.And(cm => cm.idPeriodo == idPeriodo);
            }

            return context.CONSIDERACOESMENTOR.Where(predicate).ToList();
        }

        public bool AdicionarConsideracoesMentor(CONSIDERACOESMENTOR consideracoesMentor)
        {
            try
            {
                DataModel context = new DataModel();
                context.CONSIDERACOESMENTOR.Add(consideracoesMentor);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }
            return true;
        }

        public string AtualizarConsideracoesMentor(CONSIDERACOESMENTOR oldConsideracoesMentor, CONSIDERACOESMENTOR newConsideracoesMentor)
        {
            try
            {
                using (DataModel context = new DataModel())
                {

                    var updConsideracoesMentor = context.CONSIDERACOESMENTOR.FirstOrDefault(cm => cm.idConsideracoesMentor == oldConsideracoesMentor.idConsideracoesMentor);
                    updConsideracoesMentor.ElegivelPromocao = newConsideracoesMentor.ElegivelPromocao;
                    updConsideracoesMentor.InputPromocao = newConsideracoesMentor.InputPromocao;
                    updConsideracoesMentor.TrajetoriaAssociado = newConsideracoesMentor.TrajetoriaAssociado;
                    updConsideracoesMentor.PontosFortes = newConsideracoesMentor.PontosFortes;
                    updConsideracoesMentor.PontosFracos = newConsideracoesMentor.PontosFracos;
                    updConsideracoesMentor.LiberadoRH = newConsideracoesMentor.LiberadoRH;
                    updConsideracoesMentor.AcaoComite = newConsideracoesMentor.AcaoComite;
                    updConsideracoesMentor.PontosFortesRH = newConsideracoesMentor.PontosFortesRH;
                    updConsideracoesMentor.PontosFracosRH = newConsideracoesMentor.PontosFracosRH;
                    updConsideracoesMentor.SalarioAtual = newConsideracoesMentor.SalarioAtual;
                    updConsideracoesMentor.SalarioNovo = newConsideracoesMentor.SalarioNovo;
                    updConsideracoesMentor.RegimeContratacaoAtual = newConsideracoesMentor.RegimeContratacaoAtual;
                    updConsideracoesMentor.RegimeContratacaoNovo = newConsideracoesMentor.RegimeContratacaoNovo;
                    updConsideracoesMentor.MentoriaRealizada = newConsideracoesMentor.MentoriaRealizada;
                    updConsideracoesMentor.DataMentoriaRealizada = newConsideracoesMentor.DataMentoriaRealizada;
                    updConsideracoesMentor.DataLiberadoRH = newConsideracoesMentor.DataLiberadoRH;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return ex.InnerException.InnerException.Message;
            }
            return "okay";
        }

    }
}
