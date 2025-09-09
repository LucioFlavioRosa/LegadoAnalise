using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class MentoriaService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MentoriaService));
        DataModel context = new DataModel();
        public List<MENTORPERGUNTASMODO> ObterMentorPerguntasModo(int idModo = -1)
        {
            var returnList = new List<MENTORPERGUNTASMODO>();
            returnList = context.MENTORPERGUNTASMODO.ToList();

            if (idModo > -1) { returnList = returnList.Where(x => x.idMentorPerguntaModo == idModo).ToList(); }

            return returnList;
        }
        public List<MENTORPERGUNTAS> ObterMentorPerguntas(int idPergunta = -1)
        {
            var returnList = new List<MENTORPERGUNTAS>();
            returnList = context.MENTORPERGUNTAS.ToList();

            if (idPergunta > -1) { returnList = returnList.Where(x => x.idMentorPergunta == idPergunta).ToList(); }

            return returnList;
        }
        public List<MENTORPERGUNTASNOTAS> ObterMentorNotas(int idNota = -1)
        {
            var returnList = new List<MENTORPERGUNTASNOTAS>();
            returnList = context.MENTORPERGUNTASNOTAS.ToList();

            if (idNota > -1) { returnList = returnList.Where(x => x.idNota == idNota).ToList(); }

            return returnList;
        }
        public List<MENTORADORESPOSTAS> ObterMentoradoRespostas(int idResposta = -1, int idPergunta = -1, int idMentorado = -1, int idMentor = -1, int idPeriodo = -1)
        {
            var returnList = new List<MENTORADORESPOSTAS>();
            returnList = context.MENTORADORESPOSTAS.ToList();

            if (idResposta > -1) { returnList = returnList.Where(x => x.idResposta == idResposta).ToList(); }
            if (idPergunta > -1) { returnList = returnList.Where(x => x.idPergunta == idPergunta).ToList(); }
            if (idMentorado > -1) { returnList = returnList.Where(x => x.idMentorado == idMentorado).ToList(); }
            if (idMentor > -1) { returnList = returnList.Where(x => x.idMentor == idMentor).ToList(); }
            if (idPeriodo > -1) { returnList = returnList.Where(x => x.idPeriodo == idPeriodo).ToList(); }

            return returnList;
        }
        public bool GerirMentoradoRespostas(MENTORADORESPOSTAS item)
        {
            try
            {
                DataModel context = new DataModel();
                if (item.idResposta <= 0)
                    context.MENTORADORESPOSTAS.Add(item);
                else
                    context.Entry(context.MENTORADORESPOSTAS.FirstOrDefault(x => x.idResposta == item.idResposta)).CurrentValues.SetValues(item);

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
