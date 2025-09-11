using Business.DataAccess;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Business.Services
{
    public class ComentariosService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ComentariosService));
        public List<COMENTARIOS> ObterComentarios(int idComentario = -1, int idAssociado = -1, int idPeriodo = -1)
        {
            DataModel context = new DataModel();
            var returnItems = new List<COMENTARIOS>();

            returnItems = context.COMENTARIOS.ToList();
            if (idComentario != -1 ) { returnItems = returnItems.Where(x => x.idComentario == idComentario).ToList();}
            if (idAssociado != -1 ) { returnItems = returnItems.Where(x => x.idAssociado == idAssociado).ToList();}
            if (idPeriodo != -1 ) { returnItems = returnItems.Where(x => x.idPeriodo == idPeriodo).ToList();}

            return returnItems;
        }

        public bool GerirComentario(COMENTARIOS item)
        {
            try
            {
                DataModel context = new DataModel();
                if (item.idComentario <= 0)
                    context.COMENTARIOS.Add(item);
                else
                    context.Entry(context.COMENTARIOS.FirstOrDefault(x => x.idComentario == item.idComentario)).CurrentValues.SetValues(item);

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
