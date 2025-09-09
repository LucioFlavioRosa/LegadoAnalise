using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("AVALIACAO")]
    public partial class Avaliacao
    {
        [Key]
        [Column("idAvaliacao")]
        public int IdAvaliacao { get; set; }
        [Column("idEmpresa")]
        public int IdEmpresa { get; set; }
        [Column("idProjeto")]
        public int IdProjeto { get; set; }
        [Column("idAssociado")]
        public int IdAssociado { get; set; }
        [Column("idPeriodo")]
        public int IdPeriodo { get; set; }
        [Column("idStatus")]
        public int IdStatus { get; set; }
        public bool Liberado { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataLiberacao { get; set; }
        [Required]
        [StringLength(50)]
        public string PosicaoAtualFluxoAvaliacao { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime Dhc { get; set; }

        [ForeignKey(nameof(IdAssociado))]
        [InverseProperty(nameof(Associado.AvaliacaoIdAssociadoNavigations))]
        public virtual Associado IdAssociadoNavigation { get; set; }
        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Avaliacaos))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(IdPeriodo))]
        [InverseProperty(nameof(Periodosavaliaco.Avaliacaos))]
        public virtual Periodosavaliaco IdPeriodoNavigation { get; set; }
        [ForeignKey(nameof(IdProjeto))]
        [InverseProperty(nameof(Projeto.Avaliacaos))]
        public virtual Projeto IdProjetoNavigation { get; set; }
        [ForeignKey(nameof(IdStatus))]
        [InverseProperty(nameof(Avaliacoesstatus.Avaliacaos))]
        public virtual Avaliacoesstatus IdStatusNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.AvaliacaoUsrNavigations))]
        public virtual Associado UsrNavigation { get; set; }
    }
}
