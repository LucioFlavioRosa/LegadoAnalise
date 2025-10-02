using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models
{
    public class ASSOCIADOS
    {
        [Key]
        public int IdAssociado { get; set; }
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Senha { get; set; } = string.Empty;
        public int IdCargo { get; set; }
        public int IdPerfil { get; set; }
        public int IdNivel { get; set; }
        public int IdStatus { get; set; }
        public int IdAssociadoMentor { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public int IdEmpresa { get; set; }
        public string Vertical { get; set; } = string.Empty;
        public string? FotoNome { get; set; }
        public int ATV { get; set; }
        public int USR { get; set; }
        public DateTime DHC { get; set; }
        public int IdVertical { get; set; }

        [ForeignKey("IdCargo")]
        public CARGOS? CARGOS { get; set; }
        [ForeignKey("IdPerfil")]
        public PERFIS? PERFIS { get; set; }
        [ForeignKey("IdEmpresa")]
        public EMPRESAS? EMPRESAS { get; set; }
        [ForeignKey("IdAssociadoMentor")]
        public ASSOCIADOS? ASSOCIADOS2 { get; set; }
    }
}