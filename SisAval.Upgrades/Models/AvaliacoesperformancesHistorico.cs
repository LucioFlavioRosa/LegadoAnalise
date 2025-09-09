using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("AVALIACOESPERFORMANCES_HISTORICO")]
    public partial class AvaliacoesperformancesHistorico
    {
        [Key]
        public int IdAvaliacaoPerformanceHistorico { get; set; }
        public int? IdAvaliacaoPerformance { get; set; }
        public int IdEmpresa { get; set; }
        public int IdAssociado { get; set; }
        [Required]
        [StringLength(200)]
        public string Associado { get; set; }
        public int IdCargo { get; set; }
        [Required]
        [StringLength(200)]
        public string Cargo { get; set; }
        public int IdNivel { get; set; }
        [Required]
        [StringLength(200)]
        public string Nivel { get; set; }
        public int IdProjeto { get; set; }
        [Required]
        [StringLength(200)]
        public string Projeto { get; set; }
        public int IdPeriodo { get; set; }
        [Required]
        [StringLength(200)]
        public string Periodo { get; set; }
        public int IdPerformance { get; set; }
        [Required]
        [StringLength(200)]
        public string Performance { get; set; }
        public int IdAvaliacaoStatus { get; set; }
        [Required]
        [StringLength(200)]
        public string AvaliacaoStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataHoraInicio { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraTermino { get; set; }
        [Required]
        [StringLength(50)]
        public string PosicaoAtualFluxoAvaliacao { get; set; }
        public int IdNotaNivel1AutoAvaliacao { get; set; }
        [Required]
        [StringLength(200)]
        public string NotaNivel1AutoAvaliacao { get; set; }
        public int IdNotaNivel2AutoAvaliacao { get; set; }
        [Required]
        [StringLength(200)]
        public string NotaNivel2AutoAvaliacao { get; set; }
        [Required]
        [StringLength(2000)]
        public string ComentariosAutoAvaliacao { get; set; }
        [Column("DHCAutoAvaliacao", TypeName = "datetime")]
        public DateTime DhcautoAvaliacao { get; set; }
        [Column("USRAutoAvaliacao")]
        public int UsrautoAvaliacao { get; set; }
        public int? IdNotaNivel1AvaliacaoCegas { get; set; }
        [StringLength(200)]
        public string NotaNivel1AvaliacaoCegas { get; set; }
        public int? IdNotaNivel2AvaliacaoCegas { get; set; }
        [StringLength(200)]
        public string NotaNivel2AvaliacaoCegas { get; set; }
        [StringLength(2000)]
        public string ComentariosAvaliacaoCegas { get; set; }
        [Column("DHCAvaliacaoCegas", TypeName = "datetime")]
        public DateTime? DhcavaliacaoCegas { get; set; }
        [Column("USRAvaliacaoCegas")]
        public int? UsravaliacaoCegas { get; set; }
        public int? IdNotaNivel1AvaliacaoGestor { get; set; }
        [StringLength(200)]
        public string Notanivel1AvaliacaoGestor { get; set; }
        public int? IdNotaNivel2AvaliacaoGestor { get; set; }
        [StringLength(200)]
        public string NotaNivel2AvaliacaoGestor { get; set; }
        [StringLength(2000)]
        public string ComentariosAvaliacaoGestor { get; set; }
        [Column("DHCAvaliacaoGestor", TypeName = "datetime")]
        public DateTime? DhcavaliacaoGestor { get; set; }
        [Column("USRAvaliacaoGestor")]
        public int? UsravaliacaoGestor { get; set; }
        public int? IdNotaNivel1Feedback { get; set; }
        [StringLength(200)]
        public string NotaNivel1Feedback { get; set; }
        public int? IdNotaNivel2Feedback { get; set; }
        [StringLength(200)]
        public string NotaNivel2Feedback { get; set; }
        [StringLength(2000)]
        public string ComentariosFeedback { get; set; }
        [Column("DHCFeedback", TypeName = "datetime")]
        public DateTime? Dhcfeedback { get; set; }
        [Column("USRFeedback")]
        public int? Usrfeedback { get; set; }
        public int? IdNotaNivel1AvaliacaoMentor { get; set; }
        [StringLength(200)]
        public string NotaNivel1AvaliacaoMentor { get; set; }
        public int? IdNotaNivel2AvaliacaoMentor { get; set; }
        [StringLength(200)]
        public string NotaNivel2AvaliacaoMentor { get; set; }
        [StringLength(2000)]
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
        [Column(TypeName = "datetime")]
        public DateTime? DataHoraInicioAutoAvaliacao { get; set; }

        [ForeignKey(nameof(IdAvaliacaoPerformance))]
        [InverseProperty(nameof(Avaliacoesperformance.AvaliacoesperformancesHistoricos))]
        public virtual Avaliacoesperformance IdAvaliacaoPerformanceNavigation { get; set; }
    }
}
