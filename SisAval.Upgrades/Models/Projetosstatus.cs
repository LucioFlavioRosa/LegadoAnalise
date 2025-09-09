using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PROJETOSSTATUS")]
    public partial class Projetosstatus
    {
        public Projetosstatus()
        {
            Projetos = new HashSet<Projeto>();
        }

        [Key]
        public int IdStatus { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [InverseProperty(nameof(Projeto.IdStatusNavigation))]
        public virtual ICollection<Projeto> Projetos { get; set; }
    }
}
