using Business.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class StatusService
    {
        public List<PROJETOSSTATUS> ListaStatusProjetos()
        {
            DataModel context = new DataModel();
            return context.PROJETOSSTATUS.Where(s => s.ATV == 1).OrderBy(s => s.IdStatus).ToList();
        }

        public List<AVALIACOESSTATUS> ListaStatusAvaliacoes()
        {
            DataModel context = new DataModel();
            return context.AVALIACOESSTATUS.Where(s => s.ATV == 1).OrderBy(s => s.IdStatus).ToList();
        }

        public AVALIACOESSTATUS ObterStatusAvaliacao(int idStatus)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESSTATUS.FirstOrDefault(s => s.IdStatus == idStatus);
        }

        public AVALIACOESSTATUS ObterStatusAvaliacao(string descricao)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESSTATUS.FirstOrDefault(s => s.Status == descricao);
        }

        public PROJETOSSTATUS ObterStatusProjeto(int idStatus)
        {
            DataModel context = new DataModel();
            return context.PROJETOSSTATUS.FirstOrDefault(s => s.IdStatus == idStatus);
        }
    }
}
