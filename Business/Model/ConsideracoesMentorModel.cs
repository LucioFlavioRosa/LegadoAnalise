using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ConsideracoesMentorModel
    {
        public int idConsideracoesMentor { get; set; }
		public int idMentor { get; set; }
		public string Mentor { get; set; }
		public int idAssociado { get; set; }
		public string Associado { get; set; }
		public int idPeriodo { get; set; }
		public string Periodo { get; set; }
		public string TipoAvaliacao { get; set; }
		public string Escopo { get; set; }
		public string ElegivelPromocao { get; set; }
		public string InputPromocao { get; set; }
		public string TrajetoriaAssociado { get; set; }
		public string PontosFortes { get; set; }
		public string PontosFracos { get; set; }
		public string MentorPodeVer { get; set; }
		public string AcaoComite { get; set; }
		public string PontosFortesRH { get; set; }
		public string PontosFracosRH { get; set; }
		public string SalarioAtual { get; set; }
		public string SalarioNovo { get; set; }
		public string RegimeContratacaoAtual { get; set; }
		public string RegimeContratacaoNovo { get; set; }
		public string MentoriaRealizada { get; set; }
	}
}
