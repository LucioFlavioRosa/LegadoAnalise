using System;

namespace Business.Model
{
    // Classe Utilizada na Exportação para Excel.
    // O excel utiliza o mesmo nome das propriedades para o nome das colunas, por isso possui acentuação nos nomes
    // Undescore são automaticamente trocados por espaço no excel.
    // Não alterar essa classe, a menos que seja uma alteração que deva ser refletida no arquivo Exportado do Excel 
    public class AvaliacaoModel : ModelBase
    {
        public int IdPeríodo { get; set; }
        public string Período { get; set; }
        public int IdProjeto { get; set; }
        public string Projeto { get; set; }
        public int IdAvaliado { get; set; }
        public string Nome_Avaliado { get; set; }
        public int IdCargo{ get; set; }
        public string Cargo { get; set; }
        public string Nivel { get; set; }
        public int IdGestor { get; set; }
        public string Nome_Gestor { get; set; }
        public string Tipo { get; set; }
        public string Nível { get; set; }
        public int IdCompetencia { get; set; }
        public string Competencia { get; set; }
        public int IdSubCompetencia { get; set; }
        public string SubCompetencia { get; set; }
        public string NotaAvaliacao { get; set; }
        public string NotaGestor { get; set; }
        public int IdNotaCompetenciaAvaliado { get; set; }
        public string NotaCompetenciaAvaliado { get; set; }
        public int IdNotaCompetenciaGestor { get; set; }
        public string NotaCompetenciaGestor { get; set; }
        public decimal NotaCompetenciaAvaliadoPercentual { get; set; }
        public decimal NotaCompetenciaGestorPercentual { get; set; }
        public decimal NotaCompetenciaAvaliadoPercentualFinal { get; set; }
        public decimal NotaCompetenciaGestorPercentualFinal { get; set; }
        public decimal PercentualCompetencia { get; set; }
        public int Numerador{ get; set; }
        public string NotaAutoAvaliacaoNivel1 { get; set; }
        public string NotaAutoAvaliacaoNivel2 { get; set; }
        public string NotaAvaliacaoCegasNivel1 { get; set; }
        public string NotaAvaliacaoCegasNivel2 { get; set; }
        public string NotaAvaliacaoGestorNivel1 { get; set; }
        public string NotaAvaliacaoGestorNivel2 { get; set; }
        public string NotaAvaliacaoFeedbackNivel1 { get; set; }
        public string NotaAvaliacaoFeedbackNivel2 { get; set; }


    }

    public class AssociacoesModelExport
    {
        public int IdAssociacao { get; set; }
        public int IdProjeto { get; set; }
        public string Projeto { get; set; }
        public int IdAssociado { get; set; }
        public string Associado { get; set; }
        public int? IdAvaliador { get; set; }
        public string Avaliador { get; set; }
        public int? IdGestor { get; set; }
        public string Gestor { get; set; }
        public DateTime InicioAlocacao { get; set; }
        public DateTime? TerminoAlocacao { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string Comentario { get; set; }
        public int? ATV { get; set; }
    }
}