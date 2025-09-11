using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PERFORMANCES")]
    public partial class Performance
    {
        public Performance()
        {
            Evolucaoperformances = new HashSet<Evolucaoperformance>();
        }

        [Key]
        public int IdPerformance { get; set; }
        public int IdEmpresa { get; set; }
        public int IdCargo { get; set; }
        public int IdNivel { get; set; }
        [Required]
        [Column("Performance")]
        [StringLength(8000)]
        public string Performance1 { get; set; }
        [Required]
        [StringLength(8000)]
        public string PerformanceAbaixo { get; set; }
        [Required]
        [StringLength(8000)]
        public string PerformanceEsperado { get; set; }
        [Required]
        [StringLength(8000)]
        public string PerformanceAcima { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(IdCargo))]
        [InverseProperty(nameof(Cargo.Performances))]
        public virtual Cargo IdCargoNavigation { get; set; }
        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Performances))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(IdNivel))]
        [InverseProperty(nameof(Cargosnivei.Performances))]
        public virtual Cargosnivei IdNivelNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.Performances))]
        public virtual Associado UsrNavigation { get; set; }
        [InverseProperty(nameof(Evolucaoperformance.IdPerformanceNavigation))]
        public virtual ICollection<Evolucaoperformance> Evolucaoperformances { get; set; }
    }
}
