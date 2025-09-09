using Business.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class CargosNiveisService
    {

        public int nivelJunior = 1;
        public int nivelPleno = 2;
        public int nivelSenior = 3;

        public List<CARGOSNIVEIS> ObterListaNiveis()
        {
            DataModel context = new DataModel();

            List<CARGOSNIVEIS> cargosniveislist = new List<CARGOSNIVEIS>();
            cargosniveislist = context.CARGOSNIVEIS.ToList();
            return cargosniveislist;
        }


        public List<CARGOSNIVEIS> ObterListaNiveis(bool ativos)
        {
            DataModel context = new DataModel();

            List<CARGOSNIVEIS> cargosniveislist = new List<CARGOSNIVEIS>();
            cargosniveislist = context.CARGOSNIVEIS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Nivel).ToList();
            return cargosniveislist;
        }


    }
}
