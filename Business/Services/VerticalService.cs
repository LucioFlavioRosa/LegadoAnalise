using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class VerticalService
    {
        public List<VERTICAL> ListarVerticais()
        {
            DataModel context = new DataModel();
            return context.VERTICAL.ToList();
        }
    }
}
