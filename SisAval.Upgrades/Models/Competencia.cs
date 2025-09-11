using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("COMPETENCIAS")]
    public partial class Competencia
    {
        public Competencia()
        {
            Avaliacoescompetencia = new HashSet<Avaliacoescompetencia>();
        }

        [Key]
        public int IdCompetencia { get; set; }
        public int IdEmpresa { get; set; }
        public int IdCargo { get; set; }
        public int IdNivel { get; set; }
        public int IdEixo { get; set; }
        public int IdSubCompetencia { get; set; }
        public int IdDimensao { get; set; }
        [Required]
        [Column("CompetenciaJRDetalhe")]
        [StringLength(8000)]
        public string CompetenciaJrdetalhe { get; set; }
        [Required]
        [Column("CompetenciaJR")]
        [StringLength(8000)]
        public string CompetenciaJr { get; set; }
        [Required]
        [Column("CompetenciaPLDetalhe")]
        [StringLength(8000)]
        public string CompetenciaPldetalhe { get; set; }
        [Required]
        [Column("CompetenciaPL")]
        [StringLength(8000)]
        public string CompetenciaPl { get; set; }
        [Required]
        [Column("CompetenciaSRDetalhe")]
        [StringLength(8000)]
        public string CompetenciaSrdetalhe { get; set; }
        [Required]
        [Column("CompetenciaSR")]
        [StringLength(8000)]
        public string CompetenciaSr { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(IdCargo))]
        [InverseProperty(nameof(Cargo.Competencia))]
        public virtual Cargo IdCargoNavigation { get; set; }
        [ForeignKey(nameof(IdDimensao))]
        [InverseProperty(nameof(Dimenso.Competencia))]
        public virtual Dimenso IdDimensaoNavigation { get; set; }
        [ForeignKey(nameof(IdEixo))]
        [InverseProperty(nameof(Eixo.Competencia))]
        public virtual Eixo IdEixoNavigation { get; set; }
        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Competencia))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(IdNivel))]
        [InverseProperty(nameof(Cargosnivei.Competencia))]
        public virtual Cargosnivei IdNivelNavigation { get; set; }
        [ForeignKey(nameof(IdSubCompetencia))]
        [InverseProperty(nameof(Subcompetencia.Competencia))]
        public virtual Subcompetencia IdSubCompetenciaNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.Competencia))]
        public virtual Associado UsrNavigation { get; set; }
        [InverseProperty("IdCompetenciaNavigation")]
        public virtual ICollection<Avaliacoescompetencia> Avaliacoescompetencia { get; set; }
    }
}
