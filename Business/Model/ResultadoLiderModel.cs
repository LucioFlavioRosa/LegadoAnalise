using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ResultadoLiderModel
    {
        public string Lider { get; set; }
        public string Ciclo { get; set; }
        public List<ResultadoLiderModel_Ciclos> Ciclos { get; set; }
        public List<ResultadoLiderModel_Projetos> Projetos { get; set; }
        public List<Dictionary<object, object>> resultadoTotal { get; set; }
        public ResultadoLiderModel_Pilares resultadoPilares { get; set; }
        public ResultadoLiderModel_Subcompetencias resultadoSubcompetencias { get; set; }
        public List<ResultadoLiderModel_Subcompetencias_Delta> deltaMaior { get; set; }
        public List<ResultadoLiderModel_Subcompetencias_Delta> deltaMenor { get; set; }
        public List<ResultadoLiderModel_Palavras> Palavras { get; set; }
    }
    public class ResultadoLiderModel_Ciclos
    {
        public int idPeriodo { get; set; }
        public string Ciclo { get; set; }
    }
    public class ResultadoLiderModel_Projetos
    {
        public int idProjeto { get; set; }
        public string Projeto { get; set; }
    }
    public class ResultadoLiderModel_Pilares
    {
        public List<string> Pilares { get; set; }
        public List<decimal?> resultadoPilaresTodos { get; set; }
        public List<decimal?> resultadoPilaresLider { get; set; }
    }
    public class ResultadoLiderModel_Subcompetencias
    {
        public List<string> Subcompetencias { get; set; }
        public List<decimal?> resultadoSubcompetenciasTodos { get; set; }
        public List<decimal?> resultadoSubcompetenciasLider { get; set; }
    }
    public class ResultadoLiderModel_Palavras
    {
        public string Palavra { get; set; }
        public string DataWeight { get; set; }
    }
    public class ResultadoLiderModel_Subcompetencias_Delta
    {
        public string Subcompetencia { get; set; }
        public decimal? Resultado { get; set; }
    }

    // EXPORT
    public class ResultadoLiderModel_Export_Periodo
    {
        public string Lider { get; set; }
        public string Periodo { get; set; }
        public decimal? MediaLiderPeriodo { get; set; }
        public decimal? MediaPeersPeriodo { get; set; }

    }
    public class ResultadoLiderModel_Export_Projeto
    {
        public string Lider { get; set; }
        public string Periodo { get; set; }
        public int idProjeto { get; set; }
        public string Projeto { get; set; }
        public decimal? MediaLiderProjeto { get; set; }
        public decimal? MediaPeersProjeto { get; set; }
        
    }
    public class ResultadoLiderModel_Export_Pilar
    {
        public string Lider { get; set; }
        public string Periodo { get; set; }
        public string Pilar { get; set; }
        public decimal? MediaLiderPilar { get; set; }
        public decimal? MediaPeersPilar { get; set; }
    }
    public class ResultadoLiderModel_Export_Subcompetencia
    {
        public string Lider { get; set; }
        public string Periodo { get; set; }
        public string Subcompetencia { get; set; }
        public decimal? MediaLiderSubcompetencia { get; set; }
        public decimal? MediaPeersSubcompetencia { get; set; }
    }

    // REF. AV DE LIDERANÇA COMO COMPETÊNCIA EM AV. DE DESEMPENHO
    public class ResultadoLider_AvDesempenho
    {
        public int countAvaliacoesTotal { get; set; }
        public int countAvaliacoesRespondidas { get; set; }
        public int idPeriodo { get; set; }
        public decimal? mediaTotal { get; set; }
        public string textoMediaTotal { get; set; }
    }
}
