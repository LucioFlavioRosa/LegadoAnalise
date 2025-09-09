using System;

namespace Business.Model
{
    public class AvaliacaoCompetenciaModel
    {
        public string Período { get; set; }
        public string Cargo { get; set; }

        public string Nome_Avaliado { get; set; }
        public string Projeto { get; set; }
        public string Nome_Gestor { get; set; }
        public string Competencia { get; set; }
        public string SubCompetencia { get; set; }
        public string DescricaoCompetencia { get; set; }
        public string RespondenteAutoAvaliacao { get; set; }
        public string NotaCompetenciaAvaliado { get; set; }
        public string ComentariosAutoAvaliacao { get; set; }
        public DateTime? DHCAutoAv { get; set; }
        public string RespondenteAvAsCegas { get; set; }
        public string NotaCompetenciaAvCegas { get; set; }
        public string ComentariosAvCegas { get; set; }
        public DateTime? DHCAvCegas { get; set; }
        public string RespondenteAvGestor { get; set; }
        public string NotaCompetenciaGestor { get; set; }
        public string ComentariosAvGestor { get; set; }
        public DateTime? DHCAvGestor { get; set; }
        public string RespondenteFeedback { get; set; }
        public string NotaCompetenciaFeedback { get; set; }
        public string ComentariosFeedback { get; set; }
        public DateTime? DHCFeedback { get; set; }
        public decimal NotaCompetenciaAvaliadoPercentual { get; set; }
        public decimal NotaCompetenciaGestorPercentual { get; set; }
        public decimal NotaCompetenciaAvaliadoPercentualFinal { get; set; }
        public decimal NotaCompetenciaGestorPercentualFinal { get; set; }
        public int Nivel { get; set; }
        public decimal PercentualCompetenciaAssociado { get; set; }
        public decimal PercentualCompetenciaGestor { get; set; }

        public string NotaAutoAvaliacaoNivel1 { get; set; }
        public string NotaAutoAvaliacaoNivel2 { get; set; }
        public string NotaAvaliacaoCegasNivel1 { get; set; }
        public string NotaAvaliacaoCegasNivel2 { get; set; }
        public string NotaAvaliacaoGestorNivel1 { get; set; }
        public string NotaAvaliacaoGestorNivel2 { get; set; }
        public string NotaAvaliacaoFeedbackNivel1 { get; set; }
        public string NotaAvaliacaoFeedbackNivel2 { get; set; }


        public int IdAvaliacaoCompetencia { get; set; }
        public int IdRegistro { get; set; }
        public int IdPeríodo { get; set; }
        public int IdProjeto { get; set; }
        public int IdAvaliado { get; set; }
        public int IdCargo { get; set; }
        public int IdGestor { get; set; }
    
        public string Tipo { get; set; }
        
        public int IdCompetencia { get; set; }
        
        public int IdSubCompetencia { get; set; }
       
        public string NotaAvaliacao { get; set; }
        public string NotaGestor { get; set; }
        public int IdNotaCompetenciaAvaliado { get; set; }
        
        public int IdNotaCompetenciaGestor { get; set; }
        
        public int Numerador { get; set; }

        public string TipoAvaliacao { get; set; }

        public string Escopo { get; set; }
        // VALIDAR NOTAS DO MESMO PILAR (SUBCOMPETENCIA)
        public int notaValidaNivel1 { get; set; }
        public int notaValidaNivel2 { get; set; }
    }

    public class AvaliacaoLiderancaModel
    {
        public string Periodo { get; set; }
        public string Projeto { get; set; }
        public string Lider { get; set; }
        public string Pilar { get; set; }
        public string SubCompetencia { get; set; }
        public string Descricao { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string Liderado { get; set; }
        public string Nota { get; set; }
        public string Comentarios { get; set; }
        public DateTime? DHCLiderado { get; set; }
    }
}
