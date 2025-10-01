using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models
{
    public class Promocao
    {
        [Key]
        public int IdPromocao { get; set; }
        public int idAssociado { get; set; }
        public int idCargoAnterior { get; set; }
        public int idCargoNovo { get; set; }
        public DateTime? DataPromocao { get; set; }
        public string Comentarios { get; set; }
        public bool ATV { get; set; }
        public DateTime DHC { get; set; }

        [ForeignKey("idAssociado")]
        public virtual Associado ASSOCIADOS { get; set; }
        [ForeignKey("idCargoAnterior")]
        public virtual Cargo CARGOS { get; set; }
        [ForeignKey("idCargoNovo")]
        public virtual Cargo CARGOS1 { get; set; }
    }
}