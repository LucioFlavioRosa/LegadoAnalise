namespace Models
{
    public class CARGOS
    {
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int? idProximoCargo { get; set; }
        public int TempoMinimoPromocao { get; set; }
        public string Funcao { get; set; }
        public string Autonomia { get; set; }
        public string EscopoDeAtuacao { get; set; }
        public string NivelInterlocucao { get; set; }
        public int ATV { get; set; }
        public DateTime DHC { get; set; }
    }
}