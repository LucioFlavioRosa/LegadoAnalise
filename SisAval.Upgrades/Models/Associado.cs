using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("ASSOCIADOS")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Email), Name = "UKEMAIL", IsUnique = true)]
    public partial class Associado
    {
        public Associado()
        {
            Associadospromocos = new HashSet<Associadospromoco>();
            AvaliacaoIdAssociadoNavigations = new HashSet<Avaliacao>();
            AvaliacaoUsrNavigations = new HashSet<Avaliacao>();
            AvaliacoescompetenciaIdAssociadoNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaUsrNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaUsrautoAvaliacaoNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaUsravaliacaoCegasNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaUsravaliacaoGestorNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoescompetenciaUsrfeedbackNavigations = new HashSet<Avaliacoescompetencia>();
            AvaliacoesperformanceIdAssociadoNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceUsrNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceUsrautoAvaliacaoNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceUsravaliacaoCegasNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceUsravaliacaoGestorNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceUsravaliacaoMentorNavigations = new HashSet<Avaliacoesperformance>();
            AvaliacoesperformanceUsrfeedbackNavigations = new HashSet<Avaliacoesperformance>();
            Clientes = new HashSet<Cliente>();
            Competencia = new HashSet<Competencia>();
            Dimensos = new HashSet<Dimenso>();
            Eixos = new HashSet<Eixo>();
            Evolucaoassociados = new HashSet<Evolucaoassociado>();
            InverseIdAssociadoMentorNavigation = new HashSet<Associado>();
            InverseUsrNavigation = new HashSet<Associado>();
            Performances = new HashSet<Performance>();
            ProjetoIdAssociadoGestorNavigations = new HashSet<Projeto>();
            ProjetoIdAssociadoResponsavelNavigations = new HashSet<Projeto>();
            ProjetoUsrNavigations = new HashSet<Projeto>();
            ProjetosassociadoIdAssociadoNavigations = new HashSet<Projetosassociado>();
            ProjetosassociadoIdAvaliadorNavigations = new HashSet<Projetosassociado>();
            ProjetosassociadoIdGestorNavigations = new HashSet<Projetosassociado>();
            ProjetosassociadoUsrNavigations = new HashSet<Projetosassociado>();
            Subcompetencia = new HashSet<Subcompetencia>();
            Workflows = new HashSet<Workflow>();
        }

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
        [StringLength(50)]
        public string FotoNome { get; set; }

        [ForeignKey(nameof(IdAssociadoMentor))]
        [InverseProperty(nameof(Associado.InverseIdAssociadoMentorNavigation))]
        public virtual Associado IdAssociadoMentorNavigation { get; set; }
        [ForeignKey(nameof(IdCargo))]
        [InverseProperty(nameof(Cargo.Associados))]
        public virtual Cargo IdCargoNavigation { get; set; }
        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Associados))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(IdNivel))]
        [InverseProperty(nameof(Cargosnivei.Associados))]
        public virtual Cargosnivei IdNivelNavigation { get; set; }
        [ForeignKey(nameof(IdPerfil))]
        [InverseProperty(nameof(Perfi.Associados))]
        public virtual Perfi IdPerfilNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.InverseUsrNavigation))]
        public virtual Associado UsrNavigation { get; set; }
        [InverseProperty(nameof(Associadospromoco.IdAssociadoNavigation))]
        public virtual ICollection<Associadospromoco> Associadospromocos { get; set; }
        [InverseProperty(nameof(Avaliacao.IdAssociadoNavigation))]
        public virtual ICollection<Avaliacao> AvaliacaoIdAssociadoNavigations { get; set; }
        [InverseProperty(nameof(Avaliacao.UsrNavigation))]
        public virtual ICollection<Avaliacao> AvaliacaoUsrNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.IdAssociadoNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaIdAssociadoNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.UsrNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaUsrNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.UsrautoAvaliacaoNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaUsrautoAvaliacaoNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.UsravaliacaoCegasNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaUsravaliacaoCegasNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.UsravaliacaoGestorNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaUsravaliacaoGestorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoescompetencia.UsrfeedbackNavigation))]
        public virtual ICollection<Avaliacoescompetencia> AvaliacoescompetenciaUsrfeedbackNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdAssociadoNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceIdAssociadoNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.UsrNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceUsrNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.UsrautoAvaliacaoNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceUsrautoAvaliacaoNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.UsravaliacaoCegasNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceUsravaliacaoCegasNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.UsravaliacaoGestorNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceUsravaliacaoGestorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.UsravaliacaoMentorNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceUsravaliacaoMentorNavigations { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.UsrfeedbackNavigation))]
        public virtual ICollection<Avaliacoesperformance> AvaliacoesperformanceUsrfeedbackNavigations { get; set; }
        [InverseProperty(nameof(Cliente.UsrNavigation))]
        public virtual ICollection<Cliente> Clientes { get; set; }
        [InverseProperty("UsrNavigation")]
        public virtual ICollection<Competencia> Competencia { get; set; }
        [InverseProperty(nameof(Dimenso.UsrNavigation))]
        public virtual ICollection<Dimenso> Dimensos { get; set; }
        [InverseProperty(nameof(Eixo.UsrNavigation))]
        public virtual ICollection<Eixo> Eixos { get; set; }
        [InverseProperty(nameof(Evolucaoassociado.IdAssociadoNavigation))]
        public virtual ICollection<Evolucaoassociado> Evolucaoassociados { get; set; }
        [InverseProperty(nameof(Associado.IdAssociadoMentorNavigation))]
        public virtual ICollection<Associado> InverseIdAssociadoMentorNavigation { get; set; }
        [InverseProperty(nameof(Associado.UsrNavigation))]
        public virtual ICollection<Associado> InverseUsrNavigation { get; set; }
        [InverseProperty(nameof(Performance.UsrNavigation))]
        public virtual ICollection<Performance> Performances { get; set; }
        [InverseProperty(nameof(Projeto.IdAssociadoGestorNavigation))]
        public virtual ICollection<Projeto> ProjetoIdAssociadoGestorNavigations { get; set; }
        [InverseProperty(nameof(Projeto.IdAssociadoResponsavelNavigation))]
        public virtual ICollection<Projeto> ProjetoIdAssociadoResponsavelNavigations { get; set; }
        [InverseProperty(nameof(Projeto.UsrNavigation))]
        public virtual ICollection<Projeto> ProjetoUsrNavigations { get; set; }
        [InverseProperty(nameof(Projetosassociado.IdAssociadoNavigation))]
        public virtual ICollection<Projetosassociado> ProjetosassociadoIdAssociadoNavigations { get; set; }
        [InverseProperty(nameof(Projetosassociado.IdAvaliadorNavigation))]
        public virtual ICollection<Projetosassociado> ProjetosassociadoIdAvaliadorNavigations { get; set; }
        [InverseProperty(nameof(Projetosassociado.IdGestorNavigation))]
        public virtual ICollection<Projetosassociado> ProjetosassociadoIdGestorNavigations { get; set; }
        [InverseProperty(nameof(Projetosassociado.UsrNavigation))]
        public virtual ICollection<Projetosassociado> ProjetosassociadoUsrNavigations { get; set; }
        [InverseProperty("UsrNavigation")]
        public virtual ICollection<Subcompetencia> Subcompetencia { get; set; }
        [InverseProperty(nameof(Workflow.UsrNavigation))]
        public virtual ICollection<Workflow> Workflows { get; set; }
    }
}
