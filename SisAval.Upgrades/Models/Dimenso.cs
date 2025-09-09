using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("DIMENSOES")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Dimensao), Name = "UKDIMENSAO", IsUnique = true)]
    public partial class Dimenso
    {
        public Dimenso()
        {
            Competencia = new HashSet<Competencia>();
        }

        [Key]
        public int IdDimensao { get; set; }
        [Required]
        [StringLength(120)]
        public string Dimensao { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.Dimensos))]
        public virtual Associado UsrNavigation { get; set; }
        [InverseProperty("IdDimensaoNavigation")]
        public virtual ICollection<Competencia> Competencia { get; set; }
    }
}
