using Peers.Moderno.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Services.Performance
{
    public interface IPerformanceMentorService
    {
        Task<MentorPerformanceResult> GetMentorPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo);
        Task<List<PerformanceModel>> GetPerformanceListAsync(int idAssociado, int idProjeto, int idPeriodo);
        Task<PerformanceNotasResult> GetNotasAsync(int idAssociado, int idProjeto, int idPerformance, int idPeriodo);
    }

    public class MentorPerformanceResult
    {
        public Associado Associado { get; set; } = new();
        public Projeto Projeto { get; set; } = new();
        public PERIODOSAVALIACOES Periodo { get; set; } = new();
        public Associado Gestor { get; set; } = new();
        public Cliente Cliente { get; set; } = new();
        public string TempoPeers { get; set; } = string.Empty;
        public string TempoCargo { get; set; } = string.Empty;
    }

    public class PerformanceNotasResult
    {
        public string NotaAvaliado { get; set; } = string.Empty;
        public string ComentarioAvaliado { get; set; } = string.Empty;
        public string NotaCegas { get; set; } = string.Empty;
        public string ComentarioCegas { get; set; } = string.Empty;
        public string NotaGestor { get; set; } = string.Empty;
        public string ComentarioGestor { get; set; } = string.Empty;
        public string NotaFeedback { get; set; } = string.Empty;
        public string ComentarioFeedback { get; set; } = string.Empty;
    }

    public class PerformanceModel
    {
        public int IdPerformance { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Abaixo { get; set; } = string.Empty;
        public string Esperado { get; set; } = string.Empty;
        public string Acima { get; set; } = string.Empty;
        public string Abrangencia { get; set; } = string.Empty;
        public string SeparadorAbrangencia { get; set; } = string.Empty;
    }
}
