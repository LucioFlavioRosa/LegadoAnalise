using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ResultadoPerfomanceModel
    {
        public int IdPerformance { get; set; }
        public string Perfomance { get; set; }
        public Decimal? NotaPerfomance { get; set; }      
        public Decimal? NotaPerfomancePonderada { get; set; }

        public int IdAvaliacaoPerformance { get; set; }
        public int IdNotaNivel1AutoAvaliacao { get; set; }
        public string NotaNivel1AutoAvaliacao { get; set; }
        public int? IdNotaNivel1Feedback { get; set; }
        public string NotaNivel1Feedback { get; set; }
        public int? IdNotaComite { get; set; }
        public string ComentariosAutoAvaliacao { get; set; }
        public string ComentarioFeedback { get; set; }

    }

    public class ResultadoPerfomanceModel_Export
    {
        public string Projeto { get; set; }
        public string Avaliado { get; set; }
        public string Cargo { get; set; }
        public string Periodo { get; set; }
        public string Avaliador { get; set; }
        public string GestorProjeto { get; set; }
        public string Responsavel { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string ProjetoComplexidade { get; set; }
        public string Perfomance { get; set; }
        public Decimal? NotaPerfomance { get; set; }
        public Decimal? NotaPerfomancePonderada { get; set; }
        public string NotaNivel1AutoAvaliacao { get; set; }
        public string NotaNivel1Feedback { get; set; }
        public string ComentariosAutoAvaliacao { get; set; }
        public string ComentarioFeedback { get; set; }

    }

    public class ResultadoTotalPerfomanceModel_Export
    {
        public string Avaliado { get; set; }
        public string Cargo { get; set; }
        public string Periodo { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string Perfomance { get; set; }
        public Decimal? NotaPerfomance { get; set; }
        public Decimal? NotaPerfomancePonderada { get; set; }

    }
}
