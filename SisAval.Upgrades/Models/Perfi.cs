using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PERFIS")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Perfil), Name = "UKPERFIL", IsUnique = true)]
    public partial class Perfi
    {
        public Perfi()
        {
            Associados = new HashSet<Associado>();
        }

        [Key]
        public int IdPerfil { get; set; }
        [Required]
        [StringLength(120)]
        public string Perfil { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [InverseProperty(nameof(Associado.IdPerfilNavigation))]
        public virtual ICollection<Associado> Associados { get; set; }
    }
}
