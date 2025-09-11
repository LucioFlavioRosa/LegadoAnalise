using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("COMPLEXIDADE")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Complexidade1), Name = "UQ_Complexidade", IsUnique = true)]
    public partial class Complexidade
    {
        [Key]
        public int IdComplexidade { get; set; }
        [Required]
        [Column("Complexidade")]
        [StringLength(120)]
        public string Complexidade1 { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Ponderacao { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal FaixaPonderacaoInicial { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal FaixaPonderacaoFinal { get; set; }
    }
}
