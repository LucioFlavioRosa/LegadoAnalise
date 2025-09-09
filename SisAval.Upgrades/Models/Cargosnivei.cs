using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("CARGOSNIVEIS")]
    public partial class Cargosnivei
    {
        public Cargosnivei()
        {
            Associados = new HashSet<Associado>();
            Avaliacoescompetencia = new HashSet<Avaliacoescompetencia>();
            Avaliacoesperformances = new HashSet<Avaliacoesperformance>();
            Competencia = new HashSet<Competencia>();
            Performances = new HashSet<Performance>();
            PremissasRadars = new HashSet<PremissasRadar>();
        }

        [Key]
        public int IdNivel { get; set; }
        [Required]
        [StringLength(120)]
        public string Nivel { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [InverseProperty(nameof(Associado.IdNivelNavigation))]
        public virtual ICollection<Associado> Associados { get; set; }
        [InverseProperty("IdNivelNavigation")]
        public virtual ICollection<Avaliacoescompetencia> Avaliacoescompetencia { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdNivelNavigation))]
        public virtual ICollection<Avaliacoesperformance> Avaliacoesperformances { get; set; }
        [InverseProperty("IdNivelNavigation")]
        public virtual ICollection<Competencia> Competencia { get; set; }
        [InverseProperty(nameof(Performance.IdNivelNavigation))]
        public virtual ICollection<Performance> Performances { get; set; }
        [InverseProperty(nameof(PremissasRadar.IdNivelNavigation))]
        public virtual ICollection<PremissasRadar> PremissasRadars { get; set; }
    }
}
