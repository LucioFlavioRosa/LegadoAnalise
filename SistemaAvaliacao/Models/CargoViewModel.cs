namespace SistemaAvaliacao.Models
{
    public class CargoViewModel
    {
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int? ProximoCargoId { get; set; }
        public string ProximoCargo { get; set; }
        public int TempoMinimoPromocaoMeses { get; set; }
        public string Funcao { get; set; }
        public string Autonomia { get; set; }
        public string EscopoAtuacao { get; set; }
        public string NivelInterlocucao { get; set; }
        public bool Ativo { get; set; }
    }
}