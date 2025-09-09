using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class PDIPillsModel
    {
        public string id { get;set; }
        public string href { get; set; }
        public string active { get; set; }
        public string ariacontrols { get; set; }
        public string ariaselected { get; set; }
        public string classe { get; set; }
        public string Periodo { get; set; }
    }
    public class PDIPeriodosModel
    {
        public string Periodo { get; set; }
        public int idPeriodo { get; set; }
        public string active { get; set; }
        public string id { get; set; }
        public string arialabelled { get; set; }
        public List<PDIColunasModel> PDIColunas { get; set; }
    }
    public class PDIColunasModel
    {
        public List<PDIRespostasModel> PDIRespostas { get; set; }
    }
    public class PDIRespostasModel
    {
		public int idPDIQuestoes { get; set; }
		public int idPDIResposta { get; set; }
        public string Titulo { get; set; }
        public string TituloStyle { get; set; }
        public string Subtitulo { get; set; }
        public string SubtituloStyle { get; set; }
        public string Icone { get; set; }
        public string FlexGrow { get; set; }
        public string BorderStyle { get; set; }
        public string Resposta { get; set; }
        public bool RespostaEnabled { get; set; }
        public string OnInput { get; set; }
	}
}
