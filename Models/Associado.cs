using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models
{
    public class Associado
    {
        [Key]
        public int IdAssociado { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Senha { get; set; }
        public int IdCargo { get; set; }
        public int IdAssociadoMentor { get; set; }
        public int IdPerfil { get; set; }
        public int IdVertical { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public int ATV { get; set; }
        public int IdEmpresa { get; set; }
        public int IdNivel { get; set; }
        public int IdStatus { get; set; }
        public string Vertical { get; set; }
        public string FotoNome { get; set; }
        public int USR { get; set; }
        public DateTime DHC { get; set; }

        [ForeignKey("IdCargo")]
        public virtual Cargo CARGOS { get; set; }
        [ForeignKey("IdPerfil")]
        public virtual Perfil PERFIS { get; set; }
        [ForeignKey("IdVertical")]
        public virtual Vertical VERTICAL { get; set; }
        [ForeignKey("IdAssociadoMentor")]
        public virtual Associado ASSOCIADOS2 { get; set; }
        [ForeignKey("IdEmpresa")]
        public virtual Empresa EMPRESAS { get; set; }
        public virtual ICollection<Promocao> Promocoes { get; set; }
        public virtual FotoAssociado FotoAssociado { get; set; }
    }
}