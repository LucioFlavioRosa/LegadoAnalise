using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PROJETOSTIPOS")]
    public partial class Projetostipo
    {
        public Projetostipo()
        {
            Projetos = new HashSet<Projeto>();
        }

        [Key]
        public int IdTipo { get; set; }
        [Required]
        [StringLength(50)]
        public string ProjetoTipo { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [InverseProperty(nameof(Projeto.IdTipoNavigation))]
        public virtual ICollection<Projeto> Projetos { get; set; }
    }
}
