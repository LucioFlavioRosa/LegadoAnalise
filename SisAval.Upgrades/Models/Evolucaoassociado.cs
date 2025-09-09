using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("EVOLUCAOASSOCIADO")]
    public partial class Evolucaoassociado
    {
        public Evolucaoassociado()
        {
            Evolucaocompetencia = new HashSet<Evolucaocompetencia>();
            Evolucaoperformances = new HashSet<Evolucaoperformance>();
        }

        [Key]
        public int IdEvolucaoAssociado { get; set; }
        public int IdAssociado { get; set; }
        [Required]
        [StringLength(100)]
        public string Cargo { get; set; }
        public int IdPeriodo { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? NotaCompetencia { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? NotaPerfomance { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime Dhc { get; set; }

        [ForeignKey(nameof(IdAssociado))]
        [InverseProperty(nameof(Associado.Evolucaoassociados))]
        public virtual Associado IdAssociadoNavigation { get; set; }
        [ForeignKey(nameof(IdPeriodo))]
        [InverseProperty(nameof(Periodosavaliaco.Evolucaoassociados))]
        public virtual Periodosavaliaco IdPeriodoNavigation { get; set; }
        [InverseProperty("IdEvolucaoAssociadoNavigation")]
        public virtual ICollection<Evolucaocompetencia> Evolucaocompetencia { get; set; }
        [InverseProperty(nameof(Evolucaoperformance.IdEvolucaoAssociadoNavigation))]
        public virtual ICollection<Evolucaoperformance> Evolucaoperformances { get; set; }
    }
}
