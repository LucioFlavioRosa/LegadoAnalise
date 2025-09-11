using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ResultadoProjetosModel
    {
        public int IdProjeto { get; set; }
        public int IdAssociado { get; set; }
        public int IdCargo { get; set; }
        public int IdPeriodo { get; set; }
        public int IdGestor { get; set; }
        public int IdResponsavel { get; set; }
        public int IdAvaliador { get; set; }
        public int IdAvaliacao { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string Periodo { get; set; }
        public string NomeGestor { get; set; }
        public string ProjetoComplexidade { get; set; }
        public Decimal? PesoProjetoComplexidade { get; set; }
        public Decimal? PesoPonderadoProjetoComplexidade { get; set; }
        public string ProjetoNome { get; set; }
        public string ClienteNome { get; set; }
        public DateTime DataInicioAlocado { get; set; }
        public DateTime? DataFimAlocado { get; set; }
        public List<ResultadoCompetenciaModel> ListCompetenciasNivel1 { get; set; }
        public List<ResultadoCompetenciaModel> ListCompetenciasNivel2 { get; set; }
        public List<ResultadoCompetenciaModel> ListSomaCompetenciasN1N2 { get; set; }        
        public Decimal? NotaCompetencialNivel1 { get; set; }
        public Decimal? NotaCompetencialNivel2 { get; set; }
        public Decimal? SomaNotaCompetencialN1N2 { get; set; }        
        public List<ResultadoPerfomanceModel> ListPerfomance { get; set; }        
        public Decimal? SomaPerfomance { get; set; }
        public string RatingPerfomance { get; set; }
        public Decimal? SomaNotaCompetenciaRadar { get; set; }    
        public int DiasProjeto { get; set; }
        public Decimal? CompetenciaCargo { get; set; }
        public Decimal? CompetenciaProximoCargo { get; set; }
        public int IdComplexidade { get; set; }       
    }

    public class ResultadoProjetosModel_Export
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
        public Decimal? PesoProjetoComplexidade { get; set; }
        public Decimal? PesoPonderadoProjetoComplexidade { get; set; }
        public DateTime DataInicioAlocado { get; set; }
        public DateTime? DataFimAlocado { get; set; }
        public Decimal? NotaCompetencialNivel1 { get; set; }
        public Decimal? NotaCompetencialNivel2 { get; set; }
        public Decimal? SomaNotaCompetencialN1N2 { get; set; }
        public Decimal? SomaPerfomance { get; set; }
        public string RatingPerfomance { get; set; }
        public Decimal? SomaNotaCompetenciaRadar { get; set; }
        public int DiasProjeto { get; set; }
        public Decimal? CompetenciaCargo { get; set; }
        public Decimal? CompetenciaProximoCargo { get; set; }
    }


    // RESULTADOS LIDERANÇA
    public class ResultadoLiderancaModel
    {
        public int IdProjeto { get; set; }
        public int IdAssociado { get; set; }
        public int IdCargo { get; set; }
        public int IdPeriodo { get; set; }
        public int IdGestor { get; set; }
        public int IdAvaliacao { get; set; }
        public String Periodo { get; set; }
        public string NomeGestor { get; set; }
        public string ProjetoNome { get; set; }
        public DateTime DataInicioAlocado { get; set; }
        public DateTime? DataFimAlocado { get; set; }
    }
    public class RLM_Resultados
    {
        public List<RLM_Eixo> eixos { get; set; }
        public List<RLM_Liderado> liderados { get; set; }
    }
    public class RLM_Eixo
    {
        public int idEixo { get; set; }
        public string eixo { get; set; }
        public int notas_2 { get; set; }
        public int notas_3 { get; set; }
        public int notas_4 { get; set; }
        public float resultado { get; set; }
        public string resultado_percent { get; set; }
        public List<RLM_Detalhes> detalhes { get; set; }
        public List<RLM_Competencia> competencias { get; set; }
        public List<RLM_SubCompetencias> subcompetencias { get; set; }
        public List<RLM_Liderado> liderados { get; set; }
    }
    public class RLM_Competencia
    {
        public int idCompetencia { get; set; }
        public string competencia { get; set; }
        public string eixo { get; set; }
        public List<RLM_Detalhes> respostas { get; set; }
    }
    public class RLM_Liderado
    {
        public int idLiderado { get; set; }
        public string liderado { get; set; }
        public int numProjetos { get; set; }
        public int numProjetosTabela { get; set; }
        public List<RLM_Projeto> projetos {get; set;}
    }
    public class RLM_Projeto
    {
        public int idProjeto { get; set; }
        public string projeto { get; set; }
        public List<RLM_Detalhes> respostas { get; set; }
    }
    public class RLM_Detalhes
    {
        public int idResposta { get; set; }
        public string eixo { get; set; }
        public int idSubCompetencia { get; set; }
        public string subCompetencia { get; set; }
        public string competencia { get; set; }
        public int idNota { get; set; }
        public string nota { get; set; }
        public string comentario { get; set; }
        public int idLiderado { get; set; }
        public string liderado { get; set; }
        public int idProjeto { get; set; }
        public string projeto { get; set; }
    }
    public class RLM_SubCompetencias
    {
        public int idSubCompetencia { get; set; }
        public string subCompetencia { get; set; }
        public int notas_2 { get; set; }
        public int notas_3 { get; set; }
        public int notas_4 { get; set; }
        public float notas_2_percent { get; set; }
        public float notas_3_percent { get; set; }
        public float notas_4_percent { get; set; }
    }

    // PILLS RADARES RESULTADO
    public class ResultadoPillsRadarModel
    {
        public string Periodo { get; set; }
        public int idPeriodo { get; set; }
        public string id { get; set; }
        public string idElement { get; set; }
        public string arialabelled { get; set; }
        public string JSONRadar { get; set; }
    }
}
