using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("AVALIACOESCOMPETENCIASNOTAS")]
    public partial class Avaliacoescompetenciasnota
    {
        public Avaliacoescompetenciasnota()
        {
            AvaliacoescompetenciaIdNotaNivel1AutoAvaliacaoNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel1AvaliacaoCegasNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel1AvaliacaoGestorNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel1AvaliacaoMentorNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel1ComiteNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel1FeedbackNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel2AutoAvaliacaoNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel2AvaliacaoCegasNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel2AvaliacaoGestorNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel2AvaliacaoMentorNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel2ComiteNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaIdNotaNivel2FeedbackNavigations = new HashSet<Avaliacoescompetencia>();
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
        [StringLength(50)]
        public string CodigoNotaAvaliador { get; set; }
        public bool? IndFeedback { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? Peso { get; set; }

        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel1AutoAvaliacaoNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel1AutoAvaliacaoNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel1AvaliacaoCegasNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel1AvaliacaoCegasNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel1AvaliacaoGestorNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel1AvaliacaoGestorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel1AvaliacaoMentorNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel1AvaliacaoMentorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel1ComiteNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel1ComiteNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel1FeedbackNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel1FeedbackNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel2AutoAvaliacaoNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel2AutoAvaliacaoNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel2AvaliacaoCegasNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel2AvaliacaoCegasNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel2AvaliacaoGestorNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel2AvaliacaoGestorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel2AvaliacaoMentorNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel2AvaliacaoMentorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel2ComiteNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel2ComiteNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdNotaNivel2FeedbackNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdNotaNivel2FeedbackNavigations { get; set; }
    }
}
