using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace SisAval.Upgrades.Models
{
    [Table("EMPRESAS")]
    public partial class Empresa
    {
        public Empresa()
        {
            Associados = new HashSet<Associado>();
            Avaliacaos = new HashSet<Avaliacao>();
            Avaliacoescompetencia = new HashSet<Avaliacoescompetencia>();
            Avaliacoesperformances = new HashSet<Avaliacoesperformance>();
            Clientes = new HashSet<Cliente>();
            Competencia = new HashSet<Competencia>();
            Emailparametros = new HashSet<Emailparametro>();
            Performances = new HashSet<Performance>();
            Periodosavaliacos = new HashSet<Periodosavaliaco>();
            Projetos = new HashSet<Projeto>();
            Workflows = new HashSet<Workflow>();
        }

        [Key]
        public int IdEmpresa { get; set; }
        [Required]
        [Column("Empresa")]
        [StringLength(200)]
        public string Empresa1 { get; set; }
        [Column("DHC", TypeName = "datetime")]
        public DateTime? Dhc { get; set; }
        [Column("ATV")]
        public int? Atv { get; set; }

        [InverseProperty(nameof(Associado.IdEmpresaNavigation))]
        public virtual ICollection<Associado> Associados { get; set; }
        [InverseProperty(nameof(Avaliacao.IdEmpresaNavigation))]
        public virtual ICollection<Avaliacao> Avaliacaos { get; set; }
        [InverseProperty("IdEmpresaNavigation")]
        public virtual ICollection<Avaliacoescompetencia> Avaliacoescompetencia { get; set; }
        [InverseProperty(nameof(Avaliacoesperformance.IdEmpresaNavigation))]
        public virtual ICollection<Avaliacoesperformance> Avaliacoesperformances { get; set; }
        [InverseProperty(nameof(Cliente.IdEmpresaNavigation))]
        public virtual ICollection<Cliente> Clientes { get; set; }
        [InverseProperty("IdEmpresaNavigation")]
        public virtual ICollection<Competencia> Competencia { get; set; }
        [InverseProperty(nameof(Emailparametro.IdEmpresaNavigation))]
        public virtual ICollection<Emailparametro> Emailparametros { get; set; }
        [InverseProperty(nameof(Performance.IdEmpresaNavigation))]
        public virtual ICollection<Performance> Performances { get; set; }
        [InverseProperty(nameof(Periodosavaliaco.IdEmpresaNavigation))]
        public virtual ICollection<Periodosavaliaco> Periodosavaliacos { get; set; }
        [InverseProperty(nameof(Projeto.IdEmpresaNavigation))]
        public virtual ICollection<Projeto> Projetos { get; set; }
        [InverseProperty(nameof(Workflow.IdEmpresaNavigation))]
        public virtual ICollection<Workflow> Workflows { get; set; }
    }
}
