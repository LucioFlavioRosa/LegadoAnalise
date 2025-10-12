using System.Collections.Generic;

namespace SistemaAvaliacao.Models
{
    public class CargoViewModel
    {
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int? ProximoCargoId { get; set; }
        public string ProximoCargoNome { get; set; }
        public int TempoMinimoMeses { get; set; }
        public string Funcao { get; set; }
        public string Autonomia { get; set; }
        public string EscopoAtuacao { get; set; }
        public string NivelInterlocucao { get; set; }
        public bool Ativo { get; set; }
        public IEnumerable<CargoViewModel> ListaCargos { get; set; }
        public IEnumerable<StatusCargoItem> StatusOptions { get; set; }
    }

    public class StatusCargoItem
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
