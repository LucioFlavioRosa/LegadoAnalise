using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ResultadoAssociadoModel
    {
        public ASSOCIADOS Associado { get; set; }
        public List<ResultadoProjetosModel> ListProjetos { get; set; }
    }

    public class ResultadoAssociadoModel_Export
    {
        public List<ResultadoProjetosModel_Export> ListProjetos { get; set; }
    }
}
