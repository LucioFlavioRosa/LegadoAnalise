using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("EVOLUCAOCOMPETENCIAS")]
    public partial class Evolucaocompetencia
    {
        [Key]
        public int IdEvoCompetencia { get; set; }
        public int IdEvolucaoAssociado { get; set; }
        public int IdEixo { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? Nota { get; set; }

        [ForeignKey(nameof(IdEixo))]
        [InverseProperty(nameof(Eixo.Evolucaocompetencia))]
        public virtual Eixo IdEixoNavigation { get; set; }
        [ForeignKey(nameof(IdEvolucaoAssociado))]
        [InverseProperty(nameof(Evolucaoassociado.Evolucaocompetencia))]
        public virtual Evolucaoassociado IdEvolucaoAssociadoNavigation { get; set; }
    }
}
