using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("PROJETOS")]
    public partial class Projeto
    {
        public Projeto()
        {
            Avaliacaos = new HashSet<Avaliacao>();
            Avaliacoescompetencia = new HashSet<Avaliacoescompetencia>();
            Avaliacoesperformances = new HashSet<Avaliacoesperformance>();
            Projetosassociados = new HashSet<Projetosassociado>();
        }

        [Key]
        public int IdProjeto { get; set; }
        public int IdEmpresa { get; set; }
        public int IdCliente { get; set; }
        public int IdAssociadoResponsavel { get; set; }
        public int IdAssociadoGestor { get; set; }
        public int IdStatus { get; set; }
        public int IdTipo { get; set; }
        public int IdComplexidade { get; set; }
        [Required]
        [Column("Projeto")]
        [StringLength(500)]
        public string Projeto1 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataInicio { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataFim { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
        [StringLength(120)]
        public string Codigo { get; set; }

        [ForeignKey(nameof(IdAssociadoGestor))]
        [InverseProperty(nameof(Associado.ProjetoIdAssociadoGestorNavigations))]
        public virtual Associado IdAssociadoGestorNavigation { get; set; }
        [ForeignKey(nameof(IdAssociadoResponsavel))]
        [InverseProperty(nameof(Associado.ProjetoIdAssociadoResponsavelNavigations))]
        public virtual Associado IdAssociadoResponsavelNavigation { get; set; }
        [ForeignKey(nameof(IdCliente))]
        [InverseProperty(nameof(Cliente.Projetos))]
        public virtual Cliente IdClienteNavigation { get; set; }
        [ForeignKey(nameof(IdComplexidade))]
        [InverseProperty(nameof(Projetoscomplexidade.Projetos))]
        public virtual Projetoscomplexidade IdComplexidadeNavigation { get; set; }
        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Projetos))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(IdStatus))]
        [InverseProperty(nameof(Projetosstatus.Projetos))]
        public virtual Projetosstatus IdStatusNavigation { get; set; }
        [ForeignKey(nameof(IdTipo))]
        [InverseProperty(nameof(Projetostipo.Projetos))]
        public virtual Projetostipo IdTipoNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.ProjetoUsrNavigations))]
        public virtual Associado UsrNavigation { get; set; }
        [InverseProperty(nameof(Avaliacao.IdProjetoNavigation))]
        public virtual ICollection<Avaliacao> Avaliacaos { get; set; }
        [InverseProperty("IdProjetoNavigation")]
        public virtual ICollection<Avaliacoescompetencia> Avaliacoescompetencia { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdProjetoNavigation))]
        public virtual ICollection<Avaliacoesperformance> Avaliacoesperformances { get; set; }
        [InverseProperty(nameof(Projetosassociado.IdProjetoNavigation))]
        public virtual ICollection<Projetosassociado> Projetosassociados { get; set; }
    }
}
