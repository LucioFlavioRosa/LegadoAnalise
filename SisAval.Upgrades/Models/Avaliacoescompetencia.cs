using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("AVALIACOESCOMPETENCIAS")]
    public partial class Avaliacoescompetencia
    {
        public Avaliacoescompetencia()
        {
            AvaliacoescompetenciasHistoricos = new HashSet<AvaliacoescompetenciasHistorico>();
        }

        [Key]
        public int IdAvaliacaoCompetencia { get; set; }
        public int IdEmpresa { get; set; }
        public int IdAssociado { get; set; }
        public int IdCargo { get; set; }
        public int IdNivel { get; set; }
        public int IdProjeto { get; set; }
        public int IdPeriodo { get; set; }
        public int IdCompetencia { get; set; }
        public int IdAvaliacaoStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataHoraInicio { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraTermino { get; set; }
        [Required]
        [StringLength(50)]
        public string PosicaoAtualFluxoAvaliacao { get; set; }
        public int IdNotaNivel1AutoAvaliacao { get; set; }
        public int IdNotaNivel2AutoAvaliacao { get; set; }
        [Required]
        [StringLength(5000)]
        public string ComentariosAutoAvaliacao { get; set; }
        [Column("DHCAutoAvaliacao", TypeName = "datetime")]
        public DateTime DhcautoAvaliacao { get; set; }
        [Column("USRAutoAvaliacao")]
        public int UsrautoAvaliacao { get; set; }
        public int? IdNotaNivel1AvaliacaoCegas { get; set; }
        public int? IdNotaNivel2AvaliacaoCegas { get; set; }
        [StringLength(5000)]
        public string ComentariosAvaliacaoCegas { get; set; }
        [Column("DHCAvaliacaoCegas", TypeName = "datetime")]
        public DateTime? DhcavaliacaoCegas { get; set; }
        [Column("USRAvaliacaoCegas")]
        public int? UsravaliacaoCegas { get; set; }
        public int? IdNotaNivel1AvaliacaoGestor { get; set; }
        public int? IdNotaNivel2AvaliacaoGestor { get; set; }
        [StringLength(5000)]
        public string ComentariosAvaliacaoGestor { get; set; }
        [Column("DHCAvaliacaoGestor", TypeName = "datetime")]
        public DateTime? DhcavaliacaoGestor { get; set; }
        [Column("USRAvaliacaoGestor")]
        public int? UsravaliacaoGestor { get; set; }
        public int? IdNotaNivel1Feedback { get; set; }
        public int? IdNotaNivel2Feedback { get; set; }
        [StringLength(5000)]
        public string ComentariosFeedback { get; set; }
        [Column("DHCFeedback", TypeName = "datetime")]
        public DateTime? Dhcfeedback { get; set; }
        [Column("USRFeedback")]
        public int? Usrfeedback { get; set; }
        public int? IdNotaNivel1AvaliacaoMentor { get; set; }
        public int? IdNotaNivel2AvaliacaoMentor { get; set; }
        [StringLength(5000)]
        public string ComentariosAvaliacaoMentor { get; set; }
        [Column("DHCAvaliacaoMentor", TypeName = "datetime")]
        public DateTime? DhcavaliacaoMentor { get; set; }
        [Column("USRAvaliacaoMentor")]
        public int? UsravaliacaoMentor { get; set; }
        public int? Notificado { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraNotificacao { get; set; }
        [Column("USR")]
        public int Usr { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraInicioAutoAvaliacao { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraFimAutoAvaliacao { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraInicioAvaliacaoCegas { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraFimAvaliacaoCegas { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraInicioAvaliacaoGestor { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraFimAvaliacaoGestor { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraInicioFeedback { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraFimFeedback { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraInicioAvaliacaoMentor { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraFimAvaliacaoMentor { get; set; }
        public int? IdAvaliacaoPerformance { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaSubCompetenciaAvaliado { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaSubCompetenciaGestor { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaCompetenciaAvaliado { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaCompetenciaGestor { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaFinalNivel1 { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaFinalNivel2 { get; set; }
        public int? IdNotaNivel1Comite { get; set; }
        public int? IdNotaNivel2Comite { get; set; }

        [ForeignKey(nameof(IdAssociado))]
        [InverseProperty(nameof(Associado.AvaliacoescompetenciaIdAssociadoNavigations))]
        public virtual Associado IdAssociadoNavigation { get; set; }
        [ForeignKey(nameof(IdAvaliacaoPerformance))]
        [InverseProperty(nameof(Avaliacoesperformance.Avaliacoescompetencia))]
        public virtual Avaliacoesperformance IdAvaliacaoPerformanceNavigation { get; set; }
        [ForeignKey(nameof(IdAvaliacaoStatus))]
        [InverseProperty(nameof(Avaliacoesstatus.Avaliacoescompetencia))]
        public virtual Avaliacoesstatus IdAvaliacaoStatusNavigation { get; set; }
        [ForeignKey(nameof(IdCargo))]
        [InverseProperty(nameof(Cargo.Avaliacoescompetencia))]
        public virtual Cargo IdCargoNavigation { get; set; }
        [ForeignKey(nameof(IdCompetencia))]
        [InverseProperty(nameof(Competencia.Avaliacoescompetencia))]
        public virtual Competencia IdCompetenciaNavigation { get; set; }
        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Avaliacoescompetencia))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(IdNivel))]
        [InverseProperty(nameof(Cargosnivei.Avaliacoescompetencia))]
        public virtual Cargosnivei IdNivelNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1AutoAvaliacao))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel1AutoAvaliacaoNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel1AutoAvaliacaoNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1AvaliacaoCegas))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel1AvaliacaoCegasNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel1AvaliacaoCegasNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1AvaliacaoGestor))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel1AvaliacaoGestorNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel1AvaliacaoGestorNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1AvaliacaoMentor))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel1AvaliacaoMentorNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel1AvaliacaoMentorNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1Comite))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel1ComiteNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel1ComiteNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1Feedback))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel1FeedbackNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel1FeedbackNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel2AutoAvaliacao))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel2AutoAvaliacaoNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel2AutoAvaliacaoNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel2AvaliacaoCegas))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel2AvaliacaoCegasNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel2AvaliacaoCegasNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel2AvaliacaoGestor))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel2AvaliacaoGestorNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel2AvaliacaoGestorNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel2AvaliacaoMentor))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel2AvaliacaoMentorNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel2AvaliacaoMentorNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel2Comite))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel2ComiteNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel2ComiteNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel2Feedback))]
        [InverseProperty(nameof(Avaliacoescompetenciasnota.AvaliacoescompetenciaIdNotaNivel2FeedbackNavigations))]
        public virtual Avaliacoescompetenciasnota IdNotaNivel2FeedbackNavigation { get; set; }
        [ForeignKey(nameof(IdPeriodo))]
        [InverseProperty(nameof(Periodosavaliaco.Avaliacoescompetencia))]
        public virtual Periodosavaliaco IdPeriodoNavigation { get; set; }
        [ForeignKey(nameof(IdProjeto))]
        [InverseProperty(nameof(Projeto.Avaliacoescompetencia))]
        public virtual Projeto IdProjetoNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.AvaliacoescompetenciaUsrNavigations))]
        public virtual Associado UsrNavigation { get; set; }
        [ForeignKey(nameof(UsrautoAvaliacao))]
        [InverseProperty(nameof(Associado.AvaliacoescompetenciaUsrautoAvaliacaoNavigations))]
        public virtual Associado UsrautoAvaliacaoNavigation { get; set; }
        [ForeignKey(nameof(UsravaliacaoCegas))]
        [InverseProperty(nameof(Associado.AvaliacoescompetenciaUsravaliacaoCegasNavigations))]
        public virtual Associado UsravaliacaoCegasNavigation { get; set; }
        [ForeignKey(nameof(UsravaliacaoGestor))]
        [InverseProperty(nameof(Associado.AvaliacoescompetenciaUsravaliacaoGestorNavigations))]
        public virtual Associado UsravaliacaoGestorNavigation { get; set; }
        [ForeignKey(nameof(Usrfeedback))]
        [InverseProperty(nameof(Associado.AvaliacoescompetenciaUsrfeedbackNavigations))]
        public virtual Associado UsrfeedbackNavigation { get; set; }
        [InverseProperty(nameof(AvaliacoescompetenciasHistorico.IdAvaliacaoCompetenciaNavigation))]
        public virtual ICollection<AvaliacoescompetenciasHistorico> AvaliacoescompetenciasHistoricos { get; set; }
    }
}
