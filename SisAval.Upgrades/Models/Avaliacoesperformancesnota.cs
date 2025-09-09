using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("AVALIACOESPERFORMANCESNOTAS")]
    public partial class Avaliacoesperformancesnota
    {
        public Avaliacoesperformancesnota()
        {
            AvaliacoesperformanceIdNotaComiteNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceIdNotaNivel1AutoAvaliacaoNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceIdNotaNivel1AvaliacaoCegasNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceIdNotaNivel1AvaliacaoGestorNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceIdNotaNivel1AvaliacaoMentorNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceIdNotaNivel1FeedbackNavigations = new HashSet<Avaliacoesperformance>();
        }

        [Key]
        public int IdNota { get; set; }
        [Required]
        [StringLength(50)]
        public string CodigoNota { get; set; }
        [Required]
        [StringLength(120)]
        public string DescricaoNota { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
        public bool? IndFeedback { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? Peso { get; set; }

        [InverseProperty(nameof(Avaliacoesperformance.IdNotaComiteNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceIdNotaComiteNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdNotaNivel1AutoAvaliacaoNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceIdNotaNivel1AutoAvaliacaoNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdNotaNivel1AvaliacaoCegasNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceIdNotaNivel1AvaliacaoCegasNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdNotaNivel1AvaliacaoGestorNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceIdNotaNivel1AvaliacaoGestorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdNotaNivel1AvaliacaoMentorNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceIdNotaNivel1AvaliacaoMentorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdNotaNivel1FeedbackNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceIdNotaNivel1FeedbackNavigations { get; set; }
    }
}
