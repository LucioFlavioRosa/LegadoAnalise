using Business.DataAccess;

namespace Business.Model
{
    public class AvaliacaoEmailModel : ModelBase
    {
        public PROJETOS Projeto { get; set; }
        public CLIENTES Cliente { get; set; }
        public ASSOCIADOS Associado { get; set; }
        public ASSOCIADOS Gestor { get; set; }
        public PERIODOSAVALIACOES Periodo { get; set; }
        public AVALIACOESSTATUS Status { get; set; }
        public string DataInicio { get; set; }
        public string DataTermino { get; set; }
        public string Fase { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
    }
}
