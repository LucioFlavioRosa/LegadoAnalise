using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class FrenteInternaModel
    {
		public int idFrenteInterna { get; set; }
        public string FrenteInterna { get; set; }
        public int TotalLideres { get; set; }
        public string Lideres { get; set; }
        public int ATV { get; set; }
	}

    public class FrentePill
    {
        public string Periodo { get; set; }
        public int idPeriodo { get; set; }
        public string active { get; set; }
        public string id { get; set; }
        public string arialabelled { get; set; }
        public List<AlocacaoInternaPill> alocacoes { get; set; }
    }
    public class AlocacaoInternaPill
    {
        public string Alocacao { get; set; }
        public int idAlocacao { get; set; }
        public List<AvaliacaoAlocacaoPill> Avaliados { get; set; }
    }
    public class AvaliacaoAlocacaoPill
    {
        public int idAvaliacao { get; set; }
        public string Avaliado { get; set; }
        public int idAvaliado { get; set; }
        public string FotoNome { get; set; }
        public int idNota { get; set; }
        public string Nota { get; set; }
        public string Comentarios { get; set; }
        public bool Enabled { get; set; }
        public string ValidadoMD { get; set; }
        public List<NOTASALOCACOESINTERNAS> Notas { get; set; }
    }
    public class NotaPill
    {
        public int idAvaliacao { get; set; }
        public List<NOTASALOCACOESINTERNAS> Notas { get; set; }
    }
    public class AlocacaoExport
    {
        public int idAvaliacaoAlocacao { get; set; }
        public int idAlocacaoInterna { get; set; }
        public string AlocacaoInterna { get; set; }
        public int idPeriodo { get; set; }
        public string Periodo { get; set; }
        public int idLiderAlocacao { get; set; }
        public string LiderAlocacao { get; set; }
        public int idAvaliado { get; set; }
        public string Avaliado { get; set; }
        public int idNota { get; set; }
        public string Nota { get; set; }
        public string Comentarios { get; set; }
        public string DHCNota { get; set; }
        public string ValidadoMD { get; set; }
        public string DHCValidadoMD { get; set; }
    }
}
