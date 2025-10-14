namespace SistemaAvaliacao.Domain.Models
{
    public class Cargo
    {
        public int IdCargo { get; set; }
        public string NomeCargo { get; set; }
        public int? ProximoCargoId { get; set; }
        public int TempoMinimoPromocao { get; set; }
        public string Funcao { get; set; }
        public string Autonomia { get; set; }
        public string EscopoAtuacao { get; set; }
        public string NivelInterlocucao { get; set; }
        public bool Status { get; set; }
    }
}
