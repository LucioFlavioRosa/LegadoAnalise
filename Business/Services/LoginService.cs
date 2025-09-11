using Business.DataAccess;
using System.Linq;

namespace Business.Services
{
    public class LoginService
    {
        public ASSOCIADOS BuscarAssociado(ASSOCIADOS associado)
        {
            DataModel context = new DataModel();
            ASSOCIADOS associadolista = new ASSOCIADOS();
            associadolista = context.ASSOCIADOS.First(a => a.IdAssociado == associado.IdAssociado);

            string login = associado.Email;
            string senha = associado.Senha;

            return associadolista;
        }

        public ASSOCIADOS EfetuarLogin(string email, string senha)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.FirstOrDefault(a => a.Email.ToLower() == email.ToLower() && a.Senha == senha && a.ATV == 1);
        }
    }
}