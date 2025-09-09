using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class PerformanceModel
    {
        public int IdPerformance { get; set; }
        public string Descricao { get; set; }
        public string Abaixo { get; set; }
        public string Esperado { get; set; }
        public string Acima { get; set; }

        // ABRANGENCIA
        public string SeparadorAbrangencia { get; set; }
        public string Abrangencia { get; set; }

        // INPUT
        public string Input { get; set; }
        public string DisclaimerInput { get; set; }
    }
    public class PerformanceNotasModelExport
    {
        public int IdNotaPerformance { get; set; }
        public int IdAssociado { get; set; }
        public string Associado { get; set; }
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int IdProjeto { get; set; }
        public string Projeto { get; set; }
        public int IdPeriodo { get; set; }
        public string Periodo { get; set; }
        public int IdPerformance { get; set; }
        public string Performance { get; set; }
        public int IdNotaAutoAvaliacao { get; set; }
        public string NotaAutoAvaliacao { get; set; }
        public string ComentariosAutoAvaliacao { get; set; }
        public int? IdNotaAvaliacaoAsCegas { get; set; }
        public string NotaAvaliacaoAsCegas { get; set; }
        public string ComentariosAvaliacaoAsCegas { get; set; }
        public int? IdNotaAvaliacaoGestor { get; set; }
        public string NotaAvaliacaoGestor { get; set; }
        public string ComentariosAvaliacaoGestor { get; set; }
        public int? IdNotaFeedback { get; set; }
        public string NotaFeedback { get; set; }
        public string ComentariosFeedback { get; set; }
        public int? IdNotaComite { get; set; }
        public string NotaComite { get; set; }
        public decimal? NotaFinal { get; set; }
    }
    // EXPORT E IMPORT
    public class PerformanceModelExport
    {
        public int IdPerformance { get; set; }
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public string Performance { get; set; }
        public string DescricaoAbaixo { get; set; }
        public string DescricaoEsperado { get; set; }
        public string DescricaoAcima { get; set; }
        public int ATV { get; set; }
        public string Abrangencia { get; set; }
        public int InputAutoAvaliacao { get; set; }
        public int? IdNotaPadraoAutoAvaliacao { get; set; }
        public string NotaPadraoAutoAvaliacao { get; set; }
        public int InputAvaliacaoAsCegas { get; set; }
        public int? IdNotaPadraoAvaliacaoAsCegas { get; set; }
        public string NotaPadraoAvaliacaoAsCegas { get; set; }
        public int InputAvaliacaoGestor { get; set; }
        public int? IdNotaPadraoAvaliacaoGestor { get; set; }
        public string NotaPadraoAvaliacaoGestor { get; set; }
    }
}
