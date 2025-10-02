using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models
{
    public class PROMOCOES
    {
        [Key]
        public int idPromocao { get; set; }
        public int idAssociado { get; set; }
        public int idCargoAnterior { get; set; }
        public int idCargoNovo { get; set; }
        public DateTime? DataPromocao { get; set; }
        public string Comentarios { get; set; } = string.Empty;
        public bool ATV { get; set; }
        public DateTime DHC { get; set; }

        [ForeignKey("idAssociado")]
        public ASSOCIADOS? ASSOCIADOS { get; set; }
        [ForeignKey("idCargoAnterior")]
        public CARGOS? CARGOS { get; set; }
        [ForeignKey("idCargoNovo")]
        public CARGOS? CARGOS1 { get; set; }
    }
}