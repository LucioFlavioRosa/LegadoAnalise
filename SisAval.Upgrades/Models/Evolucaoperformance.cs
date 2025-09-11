using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("EVOLUCAOPERFORMANCE")]
    public partial class Evolucaoperformance
    {
        [Key]
        public int IdEvoPerformance { get; set; }
        public int IdEvolucaoAssociado { get; set; }
        public int IdPerformance { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Nota { get; set; }

        [ForeignKey(nameof(IdEvolucaoAssociado))]
        [InverseProperty(nameof(Evolucaoassociado.Evolucaoperformances))]
        public virtual Evolucaoassociado IdEvolucaoAssociadoNavigation { get; set; }
        [ForeignKey(nameof(IdPerformance))]
        [InverseProperty(nameof(Performance.Evolucaoperformances))]
        public virtual Performance IdPerformanceNavigation { get; set; }
    }
}
