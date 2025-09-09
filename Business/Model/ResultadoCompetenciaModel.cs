using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ResultadoCompetenciaModel
    {
        public int Nivel { get; set; }
        public int IdEixo { get; set; }
        public int IdCargo { get; set; }
        public string Eixo { get; set; }
        public Decimal? NotaFinalNivel1 { get; set; }
        public Decimal? NotaFinalNivel2 { get; set; }
        public Decimal? PercentualNotaFinalNivel1 { get; set; }
        public Decimal? PercentualNotaFinalNivel1Avaliado { get; set; }
        public Decimal? PercentualNotaFinalNivel2 { get; set; }
        public Decimal? PercentualNotaFinalNivel2Avaliado { get; set; }
        public Decimal? PercentualSomaNotaFinalN1N2 { get; set; }
        public Decimal? PercentualSomaNotaFinalN1N2Avaliado { get; set; }
        public Decimal? NotaCompetenciaRadar { get; set; }
        public Decimal? NotaCompetenciaRadarAvaliado { get; set; }
        public Decimal? NotaProjetosCompetencia { get; set; }
        public Decimal? NotaProjetoCompetenciaRadar { get; set; }

        public string ComentarioFeedback { get; set; }
        public string ComentarioAvaliado { get; set; }
        public int IdNotaNivel1AutoAvaliacao { get; set; }
        public string NotaNivel1AutoAvaliacao { get; set; }
        public int IdNotaNivel2AutoAvaliacao { get; set; }
        public string NotaNivel2AutoAvaliacao { get; set; }
        public int? IdNotaNivel1Feedback { get; set; }
        public string NotaNivel1Feedback { get; set; }
        public int? IdNotaNivel2Feedback { get; set; }
        public string NotaNivel2Feedback { get; set; }
        public int IdAvaliacaoCompetencia { get; set; }
        public Decimal? NotaCompetenciaAvaliado { get; set; }
        public Decimal? NotaCompetenciaGestor { get; set; }
        public Decimal? NotaSubcompetenciaAvaliadoN1 { get; set; }
        public Decimal? NotaSubcompetenciaGestorN1 { get; set; }
        public Decimal? NotaSubcompetenciaAvaliadoN2 { get; set; }
        public Decimal? NotaSubcompetenciaGestorN2 { get; set; }
        public string DetalheNivelAtual { get; set; }
        public string CompetenciaAtual { get; set; }
        public string CompetenciaProximo { get; set; }
        public string DetalheProximoNivel { get; set; }
        public int? IdNotaNivel1Comite { get; set; }
        public int? IdNotaNivel2Comite { get; set; }
        public Decimal? NotaFinalNivel1Avaliado { get; set; }
        public Decimal? NotaFinalNivel2Avaliado { get; set; }
        // NOVAS REGRAS DE AUTOPREENCHIMENTO
        public COMPETENCIAS Competencia { get;set;}
        public int idAvaliado { get; set; }
        public PERIODOSAVALIACOES Periodo { get; set; }
        public bool enableNivel1 { get; set; }
        public bool enableNivel2 { get; set; }

    }

    public class ResultadoCompetenciaModel_Export
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
        public string Pilar { get; set; }
        public Decimal? NotaFinalNivel1 { get; set; }
        public Decimal? NotaFinalNivel2 { get; set; }
        public Decimal? PercentualNotaFinalNivel1 { get; set; }
        public Decimal? PercentualNotaFinalNivel1Avaliado { get; set; }
        public Decimal? PercentualNotaFinalNivel2 { get; set; }
        public Decimal? PercentualNotaFinalNivel2Avaliado { get; set; }
        public Decimal? PercentualSomaNotaFinalN1N2 { get; set; }
        public Decimal? PercentualSomaNotaFinalN1N2Avaliado { get; set; }
        public Decimal? NotaCompetenciaRadar { get; set; }
        public Decimal? NotaCompetenciaRadarAvaliado { get; set; }
        public Decimal? NotaProjetosCompetencia { get; set; }
        public Decimal? NotaProjetoCompetenciaRadar { get; set; }
        public string ComentarioFeedback { get; set; }
        public string ComentarioAvaliado { get; set; }
        public int IdNotaNivel1AutoAvaliacao { get; set; }
        public string NotaNivel1AutoAvaliacao { get; set; }
        public int IdNotaNivel2AutoAvaliacao { get; set; }
        public string NotaNivel2AutoAvaliacao { get; set; }
        public int? IdNotaNivel1Feedback { get; set; }
        public string NotaNivel1Feedback { get; set; }
        public int? IdNotaNivel2Feedback { get; set; }
        public string NotaNivel2Feedback { get; set; }
        public int IdAvaliacaoCompetencia { get; set; }
        public Decimal? NotaCompetenciaAvaliado { get; set; }
        public Decimal? NotaCompetenciaGestor { get; set; }
        public Decimal? NotaSubcompetenciaAvaliadoN1 { get; set; }
        public Decimal? NotaSubcompetenciaGestorN1 { get; set; }
        public Decimal? NotaSubcompetenciaAvaliadoN2 { get; set; }
        public Decimal? NotaSubcompetenciaGestorN2 { get; set; }
        public string DetalheNivelAtual { get; set; }
        public string CompetenciaAtual { get; set; }
        public string CompetenciaProximo { get; set; }
        public string DetalheProximoNivel { get; set; }
        public int? IdNotaNivel1Comite { get; set; }
        public int? IdNotaNivel2Comite { get; set; }
        public Decimal? NotaFinalNivel1Avaliado { get; set; }
        public Decimal? NotaFinalNivel2Avaliado { get; set; }

    }

    public class ResultadoTotalCompetenciaModel_Export
    {
        public string Avaliado { get; set; }
        public string Cargo { get; set; }
        public string Periodo { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string Pilar { get; set; }
        public Decimal? NotaFinalNivel1 { get; set; }
        public Decimal? NotaFinalNivel2 { get; set; }
        public Decimal? PercentualNotaFinalNivel1 { get; set; }
        public Decimal? PercentualNotaFinalNivel1Avaliado { get; set; }
        public Decimal? PercentualNotaFinalNivel2 { get; set; }
        public Decimal? PercentualNotaFinalNivel2Avaliado { get; set; }
        public Decimal? PercentualSomaNotaFinalN1N2 { get; set; }
        public Decimal? PercentualSomaNotaFinalN1N2Avaliado { get; set; }
        public Decimal? NotaCompetenciaRadar { get; set; }
        public Decimal? NotaCompetenciaRadarAvaliado { get; set; }
        public Decimal? NotaProjetosCompetencia { get; set; }
        public Decimal? NotaProjetoCompetenciaRadar { get; set; }
        public Decimal? NotaCompetenciaAvaliado { get; set; }
        public Decimal? NotaCompetenciaGestor { get; set; }
        public Decimal? NotaSubcompetenciaAvaliadoN1 { get; set; }
        public Decimal? NotaSubcompetenciaGestorN1 { get; set; }
        public Decimal? NotaSubcompetenciaAvaliadoN2 { get; set; }
        public Decimal? NotaSubcompetenciaGestorN2 { get; set; }
        public string DetalheNivelAtual { get; set; }
        public string CompetenciaAtual { get; set; }
        public string CompetenciaProximo { get; set; }
        public string DetalheProximoNivel { get; set; }
        public int? IdNotaNivel1Comite { get; set; }
        public int? IdNotaNivel2Comite { get; set; }
        public Decimal? NotaFinalNivel1Avaliado { get; set; }
        public Decimal? NotaFinalNivel2Avaliado { get; set; }

    }

}
