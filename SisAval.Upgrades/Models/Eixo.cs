using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("EIXOS")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Eixo1), Name = "UKEIXO", IsUnique = true)]
    public partial class Eixo
    {
        public Eixo()
        {
            Competencia = new HashSet<Competencia>();
            Evolucaocompetencia = new HashSet<Evolucaocompetencia>();
            PremissasRadars = new HashSet<PremissasRadar>();
        }

        [Key]
        public int IdEixo { get; set; }
        [Required]
        [Column("Eixo")]
        [StringLength(120)]
        public string Eixo1 { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.Eixos))]
        public virtual Associado UsrNavigation { get; set; }
        [InverseProperty("IdEixoNavigation")]
        public virtual ICollection<Competencia> Competencia { get; set; }
        [InverseProperty("IdEixoNavigation")]
        public virtual ICollection<Evolucaocompetencia> Evolucaocompetencia { get; set; }
        [InverseProperty(nameof(PremissasRadar.IdEixoNavigation))]
        public virtual ICollection<PremissasRadar> PremissasRadars { get; set; }
    }
}
