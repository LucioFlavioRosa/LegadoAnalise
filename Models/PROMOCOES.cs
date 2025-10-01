using System;

namespace Peers.Moderno.Models
{
    public class PROMOCOES
    {
        public int idPromocao { get; set; }
        public int idAssociado { get; set; }
        public int idCargoAnterior { get; set; }
        public int idCargoNovo { get; set; }
        public DateTime DataPromocao { get; set; }
        public string Comentarios { get; set; }
        public bool ATV { get; set; }
        public DateTime DHC { get; set; }

        public ASSOCIADOS ASSOCIADOS { get; set; }
        public CARGOS CARGOS { get; set; }
        public CARGOS CARGOS1 { get; set; }
    }
}