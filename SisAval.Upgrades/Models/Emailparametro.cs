using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("EMAILPARAMETROS")]
    public partial class Emailparametro
    {
        [Key]
        public int IdParametro { get; set; }
        public int? IdEmpresa { get; set; }
        [StringLength(50)]
        public string RemetenteEmail { get; set; }
        [Column("SMTPServer")]
        [StringLength(50)]
        public string Smtpserver { get; set; }
        public int? Porta { get; set; }
        [StringLength(50)]
        public string Dominio { get; set; }
        [StringLength(50)]
        public string Password { get; set; }
        [Column("UsarSSL")]
        public bool? UsarSsl { get; set; }
        [StringLength(50)]
        public string RemetenteNome { get; set; }
        [StringLength(255)]
        public string ModeloEmailInicio { get; set; }
        [StringLength(255)]
        public string ModeloEmailDiasSemAlteracao { get; set; }
        [StringLength(255)]
        public string ModeloEmailDiasParaTermino { get; set; }
        [StringLength(150)]
        public string ModeloEmailEvolucao { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Emailparametros))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
    }
}
