using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("WORKFLOW")]
    public partial class Workflow
    {
        [Key]
        public int IdWorkflow { get; set; }
        public int IdEmpresa { get; set; }
        public int IdPeriodo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataInicio { get; set; }
        public int DiasAutoAvaliacao { get; set; }
        public int DiasAvaliacaoCegas { get; set; }
        public int DiasAvaliacaoGestor { get; set; }
        public int DiasFeedback { get; set; }
        public int DiasAlertaSemAlteracao { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Workflows))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(IdPeriodo))]
        [InverseProperty(nameof(Periodosavaliaco.Workflows))]
        public virtual Periodosavaliaco IdPeriodoNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.Workflows))]
        public virtual Associado UsrNavigation { get; set; }
    }
}
