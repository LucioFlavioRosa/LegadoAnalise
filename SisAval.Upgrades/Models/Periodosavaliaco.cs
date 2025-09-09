using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PERIODOSAVALIACOES")]
    public partial class Periodosavaliaco
    {
        public Periodosavaliaco()
        {
            Associadospromocos = new HashSet<Associadospromoco>();
            Avaliacaos = new HashSet<Avaliacao>();
            Avaliacoescompetencia = new HashSet<Avaliacoescompetencia>();
            Avaliacoesperformances = new HashSet<Avaliacoesperformance>();
            Evolucaoassociados = new HashSet<Evolucaoassociado>();
            Workflows = new HashSet<Workflow>();
        }

        [Key]
        public int IdPeriodo { get; set; }
        public int? IdEmpresa { get; set; }
        [Required]
        [StringLength(10)]
        public string Codigo { get; set; }
        [Required]
        [StringLength(120)]
        public string Periodo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataInicio { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataFim { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Periodosavaliacos))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [InverseProperty(nameof(Associadospromoco.IdPeriodoNavigation))]
        public virtual ICollection<Associadospromoco> Associadospromocos { get; set; }
        [InverseProperty(nameof(Avaliacao.IdPeriodoNavigation))]
        public virtual ICollection<Avaliacao> Avaliacaos { get; set; }
        [InverseProperty("IdPeriodoNavigation")]
        public virtual ICollection<Avaliacoescompetencia> Avaliacoescompetencia { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdPeriodoNavigation))]
        public virtual ICollection<Avaliacoesperformance> Avaliacoesperformances { get; set; }
        [InverseProperty(nameof(Evolucaoassociado.IdPeriodoNavigation))]
        public virtual ICollection<Evolucaoassociado> Evolucaoassociados { get; set; }
        [InverseProperty(nameof(Workflow.IdPeriodoNavigation))]
        public virtual ICollection<Workflow> Workflows { get; set; }
    }
}
