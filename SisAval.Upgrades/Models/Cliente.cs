using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("CLIENTES")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Cliente1), Name = "UKCLIENTE", IsUnique = true)]
    public partial class Cliente
    {
        public Cliente()
        {
            Projetos = new HashSet<Projeto>();
        }

        [Key]
        public int IdCliente { get; set; }
        public int IdEmpresa { get; set; }
        public int IdAssociacoResponsavel { get; set; }
        public int IdAssociadoGestor { get; set; }
        [Required]
        [Column("Cliente")]
        [StringLength(120)]
        public string Cliente1 { get; set; }
        [Required]
        [StringLength(120)]
        public string Email { get; set; }
        [Required]
        [StringLength(120)]
        public string Telefones { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
        [StringLength(120)]
        public string GestorCliente { get; set; }
        [Column("CODIGO")]
        [StringLength(120)]
        public string Codigo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Clientes))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.Clientes))]
        public virtual Associado UsrNavigation { get; set; }
        [InverseProperty(nameof(Projeto.IdClienteNavigation))]
        public virtual ICollection<Projeto> Projetos { get; set; }
    }
}
