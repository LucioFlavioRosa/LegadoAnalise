using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class AvaliacaoMentoradoModel
    {
        public string NomeMentorado { get; set; }
        public string StatusAvaliacao { get; set; }
        public string NomeMentor { get; set; }
        public string DescricaoQuestao { get; set; }
        public string DescricaoResposta { get; set; }
        public int? ResultadoNota { get; set; }
        public string Comentarios { get; set; }
        public double? ResultadoGeral { get; set; }
    }
}
