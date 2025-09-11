using System;

namespace Business.Model
{
    public class CargoModel : ModelBase
    {
        public string Descricao { get; set; }
    }

    public class DescricoesCargoModel : ModelBase
    {
        public string Titulo { get; set; }
        public string TituloExibicao { get; set; }
        public string DescricaoCompetencia_Atual { get; set; }
        public string DescricaoCompetencia_Proximo { get; set; }
    }

    public class CargosModelExport
    {
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int? IdProximoCargo { get; set; }
        public string ProximoCargo { get; set; }
        public int TempoMinimoPromocao { get; set; }
        public string Funcao { get; set; }
        public string Autonomia { get; set; }
        public string EscopoDeAtuacao { get; set; }
        public string NivelInterlocucao { get; set; }
        public int ATV { get; set; }
    }

    public class PromocoesModelExport
    {
        public int IdPromocao { get; set; }
        public int? IdAssociado { get; set; }
        public string Associado { get; set; }
        public int? IdCargoAnterior { get; set; }
        public string CargoAnterior { get; set; }
        public int? IdCargoNovo { get; set; }
        public string CargoNovo { get; set; }
        public DateTime? DataPromocao { get; set; }
        public string Comentarios { get; set; }
        public bool ATV { get; set; }
    }
}
