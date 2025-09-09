using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ResultadoSomaProjetosModel
    {
        public List<ResultadoCompetenciaModel> ListProjetosSomaCompetenciasN1N2 { get; set; }        
        public List<ResultadoPerfomanceModel> ListProjetosSomaPerfomance { get; set; }
        public Decimal? SomaProjetosPerfomance { get; set; }
        public Decimal? SomaProjetosNotaCompetenciaRadar { get; set; }
        public Decimal? SomaProjetosNotaCompetencialN1N2 { get; set; }
        public string ComplexidadeComite { get; set; }
        public Decimal? CompetenciaCargo { get; set; }
        public Decimal? CompetenciaProximoCargo { get; set; }
        public string RatingPerfomance { get; set; }
    }
    public class ResultadoSomaProjetosModel_Export
    {
        public string Avaliado { get; set; }
        public string Cargo { get; set; }
        public string Periodo { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string ComplexidadeComite { get; set; }
        public Decimal? SomaProjetosPerfomance { get; set; }
        public string RatingPerfomance { get; set; }
        public Decimal? SomaProjetosNotaCompetenciaRadar { get; set; }
        public Decimal? SomaProjetosNotaCompetencialN1N2 { get; set; }
        public Decimal? CompetenciaCargo { get; set; }
        public Decimal? CompetenciaProximoCargo { get; set; }
    }
}
