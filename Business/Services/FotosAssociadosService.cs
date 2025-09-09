using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class FotosAssociadosService
    {
        public List<FOTOSASSOCIADOS> ListarFotos()
        {
            DataModel context = new DataModel();
            return context.FOTOSASSOCIADOS.ToList();
        }

        public void AdicionarFoto(FOTOSASSOCIADOS foto)
        {
            using (var context = new DataModel())
            {
                context.FOTOSASSOCIADOS.Add(foto);
                context.SaveChanges();
            }
        }

        public FOTOSASSOCIADOS ObterFotoPorId(int id)
        {
            using (var context = new DataModel())
            {
                return context.FOTOSASSOCIADOS.FirstOrDefault(f => f.IdFoto == id);
            }
        }

        public FOTOSASSOCIADOS ObterFotoPorAssociado(int idAssociado)
        {
            using (var context = new DataModel())
            {
                return context.FOTOSASSOCIADOS.FirstOrDefault(f => f.IdAssociado == idAssociado);
            }
        }

        public void AtualizarFoto(FOTOSASSOCIADOS foto)
        {
            using (var context = new DataModel())
            {
                var existente = context.FOTOSASSOCIADOS.FirstOrDefault(f => f.IdFoto == foto.IdFoto);
                if (existente != null)
                {
                    existente.IdAssociado = foto.IdAssociado;
                    existente.NomeFoto = foto.NomeFoto;
                    existente.AssociadoFoto = foto.AssociadoFoto;
                    existente.Imagem = foto.Imagem;
                    context.SaveChanges();
                }
            }
        }

        public void RemoverFoto(int id)
        {
            using (var context = new DataModel())
            {
                var foto = context.FOTOSASSOCIADOS.FirstOrDefault(f => f.IdFoto == id);
                if (foto != null)
                {
                    context.FOTOSASSOCIADOS.Remove(foto);
                    context.SaveChanges();
                }
            }
        }
    }
}
