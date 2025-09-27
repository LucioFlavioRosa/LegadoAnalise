using System.Threading.Tasks;
using System.Collections.Generic;
using Peers.Moderno.Models;

namespace Services.Performance
{
    public interface IPerformanceMentorService
    {
        Task<MentorPerformanceContext?> GetMentorPerformanceAsync(int idProjeto, int idAssociado, int idPeriodo);
        Task<List<PerformanceModel>> GetPerformanceListAsync(int idAssociado, int idProjeto, int idPeriodo);
        Task<PerformanceNotas?> GetNotasAsync(int idAssociado, int idProjeto, int idPerformance, int idPeriodo);
    }

    public class MentorPerformanceContext
    {
        public Projeto Projeto { get; set; } = new Projeto();
        public PERIODOSAVALIACOES Periodo { get; set; } = new PERIODOSAVALIACOES();
        public Associado Associado { get; set; } = new Associado();
        public Associado Gestor { get; set; } = new Associado();
        public string ClienteNome { get; set; } = string.Empty;
        public string TempoPeers { get; set; } = string.Empty;
        public string TempoCargo { get; set; } = string.Empty;
        public int IdCargoNaAvaliacao { get; set; }
        public int IdNivelNaAvaliacao { get; set; }
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

    public class PerformanceNotas
    {
        public string NotaAvaliado { get; set; } = string.Empty;
        public string ObservacaoAvaliado { get; set; } = string.Empty;
        public string NotaCegas { get; set; } = string.Empty;
        public string ObservacaoCegas { get; set; } = string.Empty;
        public string NotaGestor { get; set; } = string.Empty;
        public string ObservacaoGestor { get; set; } = string.Empty;
        public string NotaFeedback { get; set; } = string.Empty;
        public string ObservacaoFeedback { get; set; } = string.Empty;
    }
}
