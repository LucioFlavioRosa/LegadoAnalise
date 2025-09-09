using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class EquipeModel
    {
		public int idEquipe { get; set; }
		public int idProjeto { get; set; }
		public int idAssociado { get; set; }
		public string nome { get; set; }
		public int idAssociadoHierarquia { get; set; }
		public bool RespondeAvaliacaoDesempenho { get; set; }
		public bool RespondeAvaliacaoLideranca { get; set; }
		public int i { get; set; }
		public string FotoNome { get; set; }
	}
}
