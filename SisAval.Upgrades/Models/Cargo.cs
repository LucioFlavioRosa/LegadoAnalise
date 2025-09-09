using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("CARGOS")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Cargo1), Name = "UQ_Cargo", IsUnique = true)]
    public partial class Cargo
    {
        public Cargo()
        {
            Associados = new HashSet<Associado>();
            AssociadospromocoIdCargoAnteriorNavigations = new HashSet<Associadospromoco>();
            AssociadospromocoIdCargoAtualNavigations = new HashSet<Associadospromoco>();
            Avaliacoescompetencia = new HashSet<Avaliacoescompetencia>();
            Avaliacoesperformances = new HashSet<Avaliacoesperformance>();
            Competencia = new HashSet<Competencia>();
            InverseIdProximoCargoNavigation = new HashSet<Cargo>();
            Performances = new HashSet<Performance>();
            PremissasRadars = new HashSet<PremissasRadar>();
        }

        [Key]
        public int IdCargo { get; set; }
        [Required]
        [Column("Cargo")]
        [StringLength(120)]
        public string Cargo1 { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
        [Column("idProximoCargo")]
        public int? IdProximoCargo { get; set; }

        [ForeignKey(nameof(IdProximoCargo))]
        [InverseProperty(nameof(Cargo.InverseIdProximoCargoNavigation))]
        public virtual Cargo IdProximoCargoNavigation { get; set; }
        [InverseProperty(nameof(Associado.IdCargoNavigation))]
        public virtual ICollection<Associado> Associados { get; set; }
        [InverseProperty(nameof(Associadospromoco.IdCargoAnteriorNavigation))]
        public virtual ICollection<Associadospromoco> AssociadospromocoIdCargoAnteriorNavigations { get; set; }
        [InverseProperty(nameof(Associadospromoco.IdCargoAtualNavigation))]
        public virtual ICollection<Associadospromoco> AssociadospromocoIdCargoAtualNavigations { get; set; }
        [InverseProperty("IdCargoNavigation")]
        public virtual ICollection<Avaliacoescompetencia> Avaliacoescompetencia { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdCargoNavigation))]
        public virtual ICollection<Avaliacoesperformance> Avaliacoesperformances { get; set; }
        [InverseProperty("IdCargoNavigation")]
        public virtual ICollection<Competencia> Competencia { get; set; }
        [InverseProperty(nameof(Cargo.IdProximoCargoNavigation))]
        public virtual ICollection<Cargo> InverseIdProximoCargoNavigation { get; set; }
        [InverseProperty(nameof(Performance.IdCargoNavigation))]
        public virtual ICollection<Performance> Performances { get; set; }
        [InverseProperty(nameof(PremissasRadar.IdCargoNavigation))]
        public virtual ICollection<PremissasRadar> PremissasRadars { get; set; }
    }
}
