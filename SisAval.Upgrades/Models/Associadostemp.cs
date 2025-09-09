using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("ASSOCIADOSTEMP")]
    public partial class Associadostemp
    {
        [Key]
        public int IdAssociado { get; set; }
        public int IdEmpresa { get; set; }
        public int IdAssociadoMentor { get; set; }
        public int IdCargo { get; set; }
        public int IdPerfil { get; set; }
        public int IdNivel { get; set; }
        public int IdStatus { get; set; }
        [Required]
        [StringLength(120)]
        public string Nome { get; set; }
        [Required]
        [StringLength(120)]
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        public string Senha { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
    }
}
