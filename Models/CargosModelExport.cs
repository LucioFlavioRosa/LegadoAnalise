namespace Models
{
    public class CargosModelExport
    {
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int IdProximoCargo { get; set; }
        public string ProximoCargo { get; set; }
        public int TempoMinimoPromocao { get; set; }
        public string Funcao { get; set; }
        public string Autonomia { get; set; }
        public string EscopoDeAtuacao { get; set; }
        public string NivelInterlocucao { get; set; }
        public int ATV { get; set; }
    }
}