using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("AVALIACOESPERFORMANCES")]
    public partial class Avaliacoesperformance
    {
        public Avaliacoesperformance()
        {
            Avaliacoescompetencia = new HashSet<Avaliacoescompetencia>();
            AvaliacoesperformancesHistoricos = new HashSet<AvaliacoesperformancesHistorico>();
        }

        [Key]
        public int IdAvaliacaoPerformance { get; set; }
        public int IdEmpresa { get; set; }
        public int IdAssociado { get; set; }
        public int IdCargo { get; set; }
        public int IdNivel { get; set; }
        public int IdProjeto { get; set; }
        public int IdPeriodo { get; set; }
        public int IdPerformance { get; set; }
        public int IdAvaliacaoStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataHoraInicio { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraTermino { get; set; }
        [Required]
        [StringLength(50)]
        public string PosicaoAtualFluxoAvaliacao { get; set; }
        public int IdNotaNivel1AutoAvaliacao { get; set; }
        [Required]
        [StringLength(5000)]
        public string ComentariosAutoAvaliacao { get; set; }
        [Column("DHCAutoAvaliacao", TypeName = "datetime")]
        public DateTime DhcautoAvaliacao { get; set; }
        [Column("USRAutoAvaliacao")]
        public int UsrautoAvaliacao { get; set; }
        public int? IdNotaNivel1AvaliacaoCegas { get; set; }
        [StringLength(5000)]
        public string ComentariosAvaliacaoCegas { get; set; }
        [Column("DHCAvaliacaoCegas", TypeName = "datetime")]
        public DateTime? DhcavaliacaoCegas { get; set; }
        [Column("USRAvaliacaoCegas")]
        public int? UsravaliacaoCegas { get; set; }
        public int? IdNotaNivel1AvaliacaoGestor { get; set; }
        [StringLength(5000)]
        public string ComentariosAvaliacaoGestor { get; set; }
        [Column("DHCAvaliacaoGestor", TypeName = "datetime")]
        public DateTime? DhcavaliacaoGestor { get; set; }
        [Column("USRAvaliacaoGestor")]
        public int? UsravaliacaoGestor { get; set; }
        public int? IdNotaNivel1Feedback { get; set; }
        [StringLength(5000)]
        public string ComentariosFeedback { get; set; }
        [Column("DHCFeedback", TypeName = "datetime")]
        public DateTime? Dhcfeedback { get; set; }
        [Column("USRFeedback")]
        public int? Usrfeedback { get; set; }
        public int? IdNotaNivel1AvaliacaoMentor { get; set; }
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
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaSubCompetenciaAvaliado { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaSubCompetenciaGestor { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaCompetenciaAvaliado { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaCompetenciaGestor { get; set; }
        public int? IdNotaComite { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? NotaFinal { get; set; }

        [ForeignKey(nameof(IdAssociado))]
        [InverseProperty(nameof(Associado.AvaliacoesperformanceIdAssociadoNavigations))]
        public virtual Associado IdAssociadoNavigation { get; set; }
        [ForeignKey(nameof(IdAvaliacaoStatus))]
        [InverseProperty(nameof(Avaliacoesstatus.Avaliacoesperformances))]
        public virtual Avaliacoesstatus IdAvaliacaoStatusNavigation { get; set; }
        [ForeignKey(nameof(IdCargo))]
        [InverseProperty(nameof(Cargo.Avaliacoesperformances))]
        public virtual Cargo IdCargoNavigation { get; set; }
        [ForeignKey(nameof(IdEmpresa))]
        [InverseProperty(nameof(Empresa.Avaliacoesperformances))]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey(nameof(IdNivel))]
        [InverseProperty(nameof(Cargosnivei.Avaliacoesperformances))]
        public virtual Cargosnivei IdNivelNavigation { get; set; }
        [ForeignKey(nameof(IdNotaComite))]
        [InverseProperty(nameof(Avaliacoesperformancesnota.AvaliacoesperformanceIdNotaComiteNavigations))]
        public virtual Avaliacoesperformancesnota IdNotaComiteNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1AutoAvaliacao))]
        [InverseProperty(nameof(Avaliacoesperformancesnota.AvaliacoesperformanceIdNotaNivel1AutoAvaliacaoNavigations))]
        public virtual Avaliacoesperformancesnota IdNotaNivel1AutoAvaliacaoNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1AvaliacaoCegas))]
        [InverseProperty(nameof(Avaliacoesperformancesnota.AvaliacoesperformanceIdNotaNivel1AvaliacaoCegasNavigations))]
        public virtual Avaliacoesperformancesnota IdNotaNivel1AvaliacaoCegasNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1AvaliacaoGestor))]
        [InverseProperty(nameof(Avaliacoesperformancesnota.AvaliacoesperformanceIdNotaNivel1AvaliacaoGestorNavigations))]
        public virtual Avaliacoesperformancesnota IdNotaNivel1AvaliacaoGestorNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1AvaliacaoMentor))]
        [InverseProperty(nameof(Avaliacoesperformancesnota.AvaliacoesperformanceIdNotaNivel1AvaliacaoMentorNavigations))]
        public virtual Avaliacoesperformancesnota IdNotaNivel1AvaliacaoMentorNavigation { get; set; }
        [ForeignKey(nameof(IdNotaNivel1Feedback))]
        [InverseProperty(nameof(Avaliacoesperformancesnota.AvaliacoesperformanceIdNotaNivel1FeedbackNavigations))]
        public virtual Avaliacoesperformancesnota IdNotaNivel1FeedbackNavigation { get; set; }
        [ForeignKey(nameof(IdPeriodo))]
        [InverseProperty(nameof(Periodosavaliaco.Avaliacoesperformances))]
        public virtual Periodosavaliaco IdPeriodoNavigation { get; set; }
        [ForeignKey(nameof(IdProjeto))]
        [InverseProperty(nameof(Projeto.Avaliacoesperformances))]
        public virtual Projeto IdProjetoNavigation { get; set; }
        [ForeignKey(nameof(Usr))]
        [InverseProperty(nameof(Associado.AvaliacoesperformanceUsrNavigations))]
        public virtual Associado UsrNavigation { get; set; }
        [ForeignKey(nameof(UsrautoAvaliacao))]
        [InverseProperty(nameof(Associado.AvaliacoesperformanceUsrautoAvaliacaoNavigations))]
        public virtual Associado UsrautoAvaliacaoNavigation { get; set; }
        [ForeignKey(nameof(UsravaliacaoCegas))]
        [InverseProperty(nameof(Associado.AvaliacoesperformanceUsravaliacaoCegasNavigations))]
        public virtual Associado UsravaliacaoCegasNavigation { get; set; }
        [ForeignKey(nameof(UsravaliacaoGestor))]
        [InverseProperty(nameof(Associado.AvaliacoesperformanceUsravaliacaoGestorNavigations))]
        public virtual Associado UsravaliacaoGestorNavigation { get; set; }
        [ForeignKey(nameof(UsravaliacaoMentor))]
        [InverseProperty(nameof(Associado.AvaliacoesperformanceUsravaliacaoMentorNavigations))]
        public virtual Associado UsravaliacaoMentorNavigation { get; set; }
        [ForeignKey(nameof(Usrfeedback))]
        [InverseProperty(nameof(Associado.AvaliacoesperformanceUsrfeedbackNavigations))]
        public virtual Associado UsrfeedbackNavigation { get; set; }
        [InverseProperty("IdAvaliacaoPerformanceNavigation")]
        public virtual ICollection<Avaliacoescompetencia> Avaliacoescompetencia { get; set; }
        [InverseProperty(nameof(AvaliacoesperformancesHistorico.IdAvaliacaoPerformanceNavigation))]
        public virtual ICollection<AvaliacoesperformancesHistorico> AvaliacoesperformancesHistoricos { get; set; }
    }
}
