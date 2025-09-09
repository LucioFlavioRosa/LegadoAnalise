using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{

    public class MentoradoRespostaPill
    {
        public string Periodo { get; set; }
        public int idPeriodo { get; set; }
        public string active { get; set; }
        public string id { get; set; }
        public string arialabelled { get; set; }
        public string MentorNome { get; set; }
        public string MentorFoto { get; set; }
        public int CountMentorados { get; set; }
        public int CountMentores { get; set; }
        public List<MentoradoRespostasModel> respostas { get; set; }
        public List<MentoradoRespostasModel> resultados { get; set; }
        public double? mediaMentor { get; set; }
        public double? mediaPeers { get; set; }
        public double? notaMaxima { get; set; }
    }
    public class MentoradoRespostasModel
    {
        

        public int idResposta { get; set; }
        public string Pergunta { get; set; }
        public int idPergunta { get; set; }
        public int idModo { get; set; }
        public int idPeriodo { get; set; }
        public string Comentarios { get; set; }
        public string ComentarioColor { get; set; }
        public bool Enabled { get; set; }
        public List<MentoradoRespostasNotasModel> notas { get; set; }
        public double? ResultadoMentor { get; set; }
        public double? ResultadoPeers { get; set; }
        public double? NotaMaxima { get; set; }
        public string HiddenVelocimetro { get; set; }
        public double VelocValorMentor { get; set; }
        public double VelocValorPeers { get; set; }

        public bool showNotaMentor { get; set; }
    }
    public class MentoradoRespostasNotasModel
    {
        public int idResposta { get; set; }
        public int idNota { get; set; }
        public int Escala { get; set; }
        public string NotaTexto { get; set; }
        public string BackgroundColor { get; set; }
        public string Icone { get; set; }
        public string HideIcone { get; set; }
        public bool Enabled { get; set; }
    }
}
