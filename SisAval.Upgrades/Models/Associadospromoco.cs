using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("ASSOCIADOSPROMOCOES")]
    public partial class Associadospromoco
    {
        [Key]
        public int IdAssociadoPromocao { get; set; }
        public int IdEmpresa { get; set; }
        public int IdAssociado { get; set; }
        public int IdPeriodo { get; set; }
        public int IdCargoAnterior { get; set; }
        public int IdCargoAtual { get; set; }
        [StringLength(200)]
        public string Comentarios { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(IdAssociado))]
        [InverseProperty(nameof(Associado.Associadospromocos))]
        public virtual Associado IdAssociadoNavigation { get; set; }
        [ForeignKey(nameof(IdCargoAnterior))]
        [InverseProperty(nameof(Cargo.AssociadospromocoIdCargoAnteriorNavigations))]
        public virtual Cargo IdCargoAnteriorNavigation { get; set; }
        [ForeignKey(nameof(IdCargoAtual))]
        [InverseProperty(nameof(Cargo.AssociadospromocoIdCargoAtualNavigations))]
        public virtual Cargo IdCargoAtualNavigation { get; set; }
        [ForeignKey(nameof(IdPeriodo))]
        [InverseProperty(nameof(Periodosavaliaco.Associadospromocos))]
        public virtual Periodosavaliaco IdPeriodoNavigation { get; set; }
    }
}
