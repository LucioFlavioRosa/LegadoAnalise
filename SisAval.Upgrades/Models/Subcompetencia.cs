using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("SUBCOMPETENCIAS")]
    [Microsoft.EntityFrameworkCore.Index(nameof(SubCompetencia1), Name = "UKSUBCOMPETENCIA", IsUnique = true)]
    public partial class Subcompetencia
    {
        public Subcompetencia()
        {
            Competencia = new HashSet<Competencia>();
        }

        [Key]
        public int IdSubCompetencia { get; set; }
        [Required]
        [Column("SubCompetencia")]
        [StringLength(120)]
        public string SubCompetencia1 { get; set; }
        [Column("USR")]
        public int? Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.Subcompetencia))]
        public virtual Associado UsrNavigation { get; set; }
        [InverseProperty("IdSubCompetenciaNavigation")]
        public virtual ICollection<Competencia> Competencia { get; set; }
    }
}
