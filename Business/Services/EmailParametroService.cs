using Business.DataAccess;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class EmailParametroService
    {
        public EMAILPARAMETROS ObterParametro(int idEmpresa)
        {
            DataModel context = new DataModel();
            return context.EMAILPARAMETROS.FirstOrDefault(a => a.IdEmpresa == idEmpresa);
        }
    }
}
