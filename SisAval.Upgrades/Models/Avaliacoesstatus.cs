using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("AVALIACOESSTATUS")]
    public partial class Avaliacoesstatus
    {
        public Avaliacoesstatus()
        {
            Avaliacaos = new HashSet<Avaliacao>();
            Avaliacoescompetencia = new HashSet<Avaliacoescompetencia>();
            Avaliacoesperformances = new HashSet<Avaliacoesperformance>();
        }

        [Key]
        public int IdStatus { get; set; }
        [Required]
        [StringLength(200)]
        public string Status { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [InverseProperty(nameof(Avaliacao.IdStatusNavigation))]
        public virtual ICollection<Avaliacao> Avaliacaos { get; set; }
        [InverseProperty("IdAvaliacaoStatusNavigation")]
        public virtual ICollection<Avaliacoescompetencia> Avaliacoescompetencia { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdAvaliacaoStatusNavigation))]
        public virtual ICollection<Avaliacoesperformance> Avaliacoesperformances { get; set; }
    }
}
