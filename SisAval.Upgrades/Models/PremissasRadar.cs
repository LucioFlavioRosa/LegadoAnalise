using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PREMISSAS_RADAR")]
    public partial class PremissasRadar
    {
        [Key]
        public int IdPremissa { get; set; }
        public int IdEmpresa { get; set; }
        public int IdEixo { get; set; }
        public int IdCargo { get; set; }
        public int IdNivel { get; set; }
        public int ValorRadarPeers { get; set; }
        public int ValorBaseAutoAvaliacao { get; set; }
        public int ValorBaseAvaliacaoGestor { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(IdCargo))]
        [InverseProperty(nameof(Cargo.PremissasRadars))]
        public virtual Cargo IdCargoNavigation { get; set; }
        [ForeignKey(nameof(IdEixo))]
        [InverseProperty(nameof(Eixo.PremissasRadars))]
        public virtual Eixo IdEixoNavigation { get; set; }
        [ForeignKey(nameof(IdNivel))]
        [InverseProperty(nameof(Cargosnivei.PremissasRadars))]
        public virtual Cargosnivei IdNivelNavigation { get; set; }
    }
}
