using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("RATING")]
    public partial class Rating
    {
        [Key]
        public int IdRating { get; set; }
        public int IdComplexidade { get; set; }
        [Required]
        [StringLength(100)]
        public string Performance { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal FaixaInicial { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal FaixaFinal { get; set; }
        [Column("ATV")]
        public int Atv { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime Dhc { get; set; }

        [ForeignKey(nameof(IdComplexidade))]
        [InverseProperty(nameof(Projetoscomplexidade.Ratings))]
        public virtual Projetoscomplexidade IdComplexidadeNavigation { get; set; }
    }
}
