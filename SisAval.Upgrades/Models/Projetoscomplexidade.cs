using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PROJETOSCOMPLEXIDADES")]
    public partial class Projetoscomplexidade
    {
        public Projetoscomplexidade()
        {
            Projetos = new HashSet<Projeto>();
            Ratings = new HashSet<Rating>();
        }

        [Key]
        public int IdComplexidade { get; set; }
        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }
        [Required]
        [StringLength(120)]
        public string Complexidade { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Ponderacao { get; set; }
        [Column("USR")]
        [StringLength(10)]
        public string Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? FaixaInicial { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? FaixaFinal { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Peso { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? PesoPonderado { get; set; }

        [InverseProperty(nameof(Projeto.IdComplexidadeNavigation))]
        public virtual ICollection<Projeto> Projetos { get; set; }
        [InverseProperty(nameof(Rating.IdComplexidadeNavigation))]
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
