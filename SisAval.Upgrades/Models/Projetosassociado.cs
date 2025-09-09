using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PROJETOSASSOCIADOS")]
    public partial class Projetosassociado
    {
        [Key]
        public int IdProjetoAssociado { get; set; }
        public int IdProjeto { get; set; }
        public int IdAssociado { get; set; }
        public int? IdGestor { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataInicio { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataFim { get; set; }
        [StringLength(500)]
        public string Comentario { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
        public int? IdAvaliador { get; set; }

        [ForeignKey(nameof(IdAssociado))]
        [InverseProperty(nameof(Associado.ProjetosassociadoIdAssociadoNavigations))]
        public virtual Associado IdAssociadoNavigation { get; set; }
        [ForeignKey(nameof(IdAvaliador))]
        [InverseProperty(nameof(Associado.ProjetosassociadoIdAvaliadorNavigations))]
        public virtual Associado IdAvaliadorNavigation { get; set; }
        [ForeignKey(nameof(IdGestor))]
        [InverseProperty(nameof(Associado.ProjetosassociadoIdGestorNavigations))]
        public virtual Associado IdGestorNavigation { get; set; }
        [ForeignKey(nameof(IdProjeto))]
        [InverseProperty(nameof(Projeto.Projetosassociados))]
        public virtual Projeto IdProjetoNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.ProjetosassociadoUsrNavigations))]
        public virtual Associado UsrNavigation { get; set; }
    }
}
