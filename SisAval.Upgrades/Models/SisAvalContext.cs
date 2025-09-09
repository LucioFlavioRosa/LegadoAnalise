using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SisAval.Upgrades.Models
{
    public partial class SisAvalContext : DbContext
    {
        public SisAvalContext()
        {
        }

        public SisAvalContext(DbContextOptions<SisAvalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Associado> Associados { get; set; }
        public virtual DbSet<Associadospromoco> Associadospromocoes { get; set; }
        public virtual DbSet<Associadostemp> Associadostemps { get; set; }
        public virtual DbSet<Avaliacao> Avaliacaos { get; set; }
        public virtual DbSet<Avaliacoescompetencia> Avaliacoescompetencias { get; set; }
        public virtual DbSet<AvaliacoescompetenciasHistorico> AvaliacoescompetenciasHistoricos { get; set; }
        public virtual DbSet<Avaliacoescompetenciasnota> Avaliacoescompetenciasnotas { get; set; }
        public virtual DbSet<Avaliacoesperformance> Avaliacoesperformances { get; set; }
        public virtual DbSet<AvaliacoesperformancesHistorico> AvaliacoesperformancesHistoricos { get; set; }
        public virtual DbSet<Avaliacoesperformancesnota> Avaliacoesperformancesnotas { get; set; }
        public virtual DbSet<Avaliacoesstatus> Avaliacoesstatuses { get; set; }
        public virtual DbSet<Cargo> Cargos { get; set; }
        public virtual DbSet<Cargosnivei> Cargosniveis { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Competencia> Competencias { get; set; }
        public virtual DbSet<Complexidade> Complexidades { get; set; }
        public virtual DbSet<Dimenso> Dimensoes { get; set; }
        public virtual DbSet<Eixo> Eixos { get; set; }
        public virtual DbSet<Emailparametro> Emailparametros { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Evolucaoassociado> Evolucaoassociados { get; set; }
        public virtual DbSet<Evolucaocompetencia> Evolucaocompetencias { get; set; }
        public virtual DbSet<Evolucaoperformance> Evolucaoperformances { get; set; }
        public virtual DbSet<Perfi> Perfis { get; set; }
        public virtual DbSet<Performance> Performances { get; set; }
        public virtual DbSet<Periodosavaliaco> Periodosavaliacoes { get; set; }
        public virtual DbSet<PremissasRadar> PremissasRadars { get; set; }
        public virtual DbSet<Projeto> Projetos { get; set; }
        public virtual DbSet<Projetosassociado> Projetosassociados { get; set; }
        public virtual DbSet<Projetoscomplexidade> Projetoscomplexidades { get; set; }
        public virtual DbSet<Projetosstatus> Projetosstatuses { get; set; }
        public virtual DbSet<Projetostipo> Projetostipos { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Subcompetencia> Subcompetencias { get; set; }
        public virtual DbSet<Workflow> Workflows { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=SistemaAvaliacao_PROD;User Id=sa;Password=Consul@2021;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Associado>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FotoNome).IsUnicode(false);

                entity.Property(e => e.Nome).IsUnicode(false);

                entity.Property(e => e.Senha).IsUnicode(false);

                entity.HasOne(d => d.IdAssociadoMentorNavigation)
                    .WithMany(p => p.InverseIdAssociadoMentorNavigation)
                    .HasForeignKey(d => d.IdAssociadoMentor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOS_ASSOCIADOS");

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.Associados)
                    .HasForeignKey(d => d.IdCargo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOS_CARGOS");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Associados)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOS_EMPRESAS");

                entity.HasOne(d => d.IdNivelNavigation)
                    .WithMany(p => p.Associados)
                    .HasForeignKey(d => d.IdNivel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOS_NIVEL");

                entity.HasOne(d => d.IdPerfilNavigation)
                    .WithMany(p => p.Associados)
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOS_PERFIS");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.InverseUsrNavigation)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOS_ASSOCIADOS1");
            });

            modelBuilder.Entity<Associadospromoco>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Comentarios).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdAssociadoNavigation)
                    .WithMany(p => p.Associadospromocos)
                    .HasForeignKey(d => d.IdAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOSPROMOCOES_ASSOCIADOS");

                entity.HasOne(d => d.IdCargoAnteriorNavigation)
                    .WithMany(p => p.AssociadospromocoIdCargoAnteriorNavigations)
                    .HasForeignKey(d => d.IdCargoAnterior)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOSPROMOCOES_CARGOS");

                entity.HasOne(d => d.IdCargoAtualNavigation)
                    .WithMany(p => p.AssociadospromocoIdCargoAtualNavigations)
                    .HasForeignKey(d => d.IdCargoAtual)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOSPROMOCOES_CARGOS1");

                entity.HasOne(d => d.IdPeriodoNavigation)
                    .WithMany(p => p.Associadospromocos)
                    .HasForeignKey(d => d.IdPeriodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ASSOCIADOSPROMOCOES_PERIODOSAVALIACOES");
            });

            modelBuilder.Entity<Associadostemp>(entity =>
            {
                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Nome).IsUnicode(false);

                entity.Property(e => e.Senha).IsUnicode(false);
            });

            modelBuilder.Entity<Avaliacao>(entity =>
            {
                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PosicaoAtualFluxoAvaliacao).IsUnicode(false);

                entity.HasOne(d => d.IdAssociadoNavigation)
                    .WithMany(p => p.AvaliacaoIdAssociadoNavigations)
                    .HasForeignKey(d => d.IdAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACAO_ASSOCIADOS");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Avaliacaos)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACAO_EMPRESAS");

                entity.HasOne(d => d.IdPeriodoNavigation)
                    .WithMany(p => p.Avaliacaos)
                    .HasForeignKey(d => d.IdPeriodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACAO_PERIODOSAVALIACOES");

                entity.HasOne(d => d.IdProjetoNavigation)
                    .WithMany(p => p.Avaliacaos)
                    .HasForeignKey(d => d.IdProjeto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACAO_PROJETOS");

                entity.HasOne(d => d.IdStatusNavigation)
                    .WithMany(p => p.Avaliacaos)
                    .HasForeignKey(d => d.IdStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACAO_AVALIACOESSTATUS");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.AvaliacaoUsrNavigations)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Avaliacoescompetencia>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.ComentariosAutoAvaliacao).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoCegas).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoGestor).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoMentor).IsUnicode(false);

                entity.Property(e => e.ComentariosFeedback).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PosicaoAtualFluxoAvaliacao).IsUnicode(false);

                entity.HasOne(d => d.IdAssociadoNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdAssociadoNavigations)
                    .HasForeignKey(d => d.IdAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_ASSOCIADOS");

                entity.HasOne(d => d.IdAvaliacaoPerformanceNavigation)
                    .WithMany(p => p.Avaliacoescompetencia)
                    .HasForeignKey(d => d.IdAvaliacaoPerformance)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESPERFORMANCE");

                entity.HasOne(d => d.IdAvaliacaoStatusNavigation)
                    .WithMany(p => p.Avaliacoescompetencia)
                    .HasForeignKey(d => d.IdAvaliacaoStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESSTATUS");

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.Avaliacoescompetencia)
                    .HasForeignKey(d => d.IdCargo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_CARGOS");

                entity.HasOne(d => d.IdCompetenciaNavigation)
                    .WithMany(p => p.Avaliacoescompetencia)
                    .HasForeignKey(d => d.IdCompetencia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_COMPETENCIAS");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Avaliacoescompetencia)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_EMPRESAS");

                entity.HasOne(d => d.IdNivelNavigation)
                    .WithMany(p => p.Avaliacoescompetencia)
                    .HasForeignKey(d => d.IdNivel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_CARGOSNIVEIS");

                entity.HasOne(d => d.IdNotaNivel1AutoAvaliacaoNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel1AutoAvaliacaoNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1AutoAvaliacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS");

                entity.HasOne(d => d.IdNotaNivel1AvaliacaoCegasNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel1AvaliacaoCegasNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1AvaliacaoCegas)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS2");

                entity.HasOne(d => d.IdNotaNivel1AvaliacaoGestorNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel1AvaliacaoGestorNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1AvaliacaoGestor)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS4");

                entity.HasOne(d => d.IdNotaNivel1AvaliacaoMentorNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel1AvaliacaoMentorNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1AvaliacaoMentor)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS8");

                entity.HasOne(d => d.IdNotaNivel1ComiteNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel1ComiteNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1Comite)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_IdNotaNivel1Comite");

                entity.HasOne(d => d.IdNotaNivel1FeedbackNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel1FeedbackNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1Feedback)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS6");

                entity.HasOne(d => d.IdNotaNivel2AutoAvaliacaoNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel2AutoAvaliacaoNavigations)
                    .HasForeignKey(d => d.IdNotaNivel2AutoAvaliacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS1");

                entity.HasOne(d => d.IdNotaNivel2AvaliacaoCegasNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel2AvaliacaoCegasNavigations)
                    .HasForeignKey(d => d.IdNotaNivel2AvaliacaoCegas)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS3");

                entity.HasOne(d => d.IdNotaNivel2AvaliacaoGestorNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel2AvaliacaoGestorNavigations)
                    .HasForeignKey(d => d.IdNotaNivel2AvaliacaoGestor)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS5");

                entity.HasOne(d => d.IdNotaNivel2AvaliacaoMentorNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel2AvaliacaoMentorNavigations)
                    .HasForeignKey(d => d.IdNotaNivel2AvaliacaoMentor)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS9");

                entity.HasOne(d => d.IdNotaNivel2ComiteNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel2ComiteNavigations)
                    .HasForeignKey(d => d.IdNotaNivel2Comite)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_IdNotaNivel2Comite");

                entity.HasOne(d => d.IdNotaNivel2FeedbackNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaIdNotaNivel2FeedbackNavigations)
                    .HasForeignKey(d => d.IdNotaNivel2Feedback)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_AVALIACOESCOMPETENCIASNOTAS7");

                entity.HasOne(d => d.IdPeriodoNavigation)
                    .WithMany(p => p.Avaliacoescompetencia)
                    .HasForeignKey(d => d.IdPeriodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_PERIODOSAVALIACOES");

                entity.HasOne(d => d.IdProjetoNavigation)
                    .WithMany(p => p.Avaliacoescompetencia)
                    .HasForeignKey(d => d.IdProjeto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_PROJETOS");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaUsrNavigations)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_ASSOCIADOS6");

                entity.HasOne(d => d.UsrautoAvaliacaoNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaUsrautoAvaliacaoNavigations)
                    .HasForeignKey(d => d.UsrautoAvaliacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_ASSOCIADOS2");

                entity.HasOne(d => d.UsravaliacaoCegasNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaUsravaliacaoCegasNavigations)
                    .HasForeignKey(d => d.UsravaliacaoCegas)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_ASSOCIADOS3");

                entity.HasOne(d => d.UsravaliacaoGestorNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaUsravaliacaoGestorNavigations)
                    .HasForeignKey(d => d.UsravaliacaoGestor)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_ASSOCIADOS4");

                entity.HasOne(d => d.UsrfeedbackNavigation)
                    .WithMany(p => p.AvaliacoescompetenciaUsrfeedbackNavigations)
                    .HasForeignKey(d => d.Usrfeedback)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_ASSOCIADOS5");
            });

            modelBuilder.Entity<AvaliacoescompetenciasHistorico>(entity =>
            {
                entity.Property(e => e.Associado).IsUnicode(false);

                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.AvaliacaoStatus).IsUnicode(false);

                entity.Property(e => e.Cargo).IsUnicode(false);

                entity.Property(e => e.ComentariosAutoAvaliacao).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoCegas).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoGestor).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoMentor).IsUnicode(false);

                entity.Property(e => e.ComentariosFeedback).IsUnicode(false);

                entity.Property(e => e.Competencia).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dimensao).IsUnicode(false);

                entity.Property(e => e.Eixo).IsUnicode(false);

                entity.Property(e => e.Nivel).IsUnicode(false);

                entity.Property(e => e.NotaNivel1AutoAvaliacao).IsUnicode(false);

                entity.Property(e => e.NotaNivel1AvaliacaoCegas).IsUnicode(false);

                entity.Property(e => e.NotaNivel1AvaliacaoMentor).IsUnicode(false);

                entity.Property(e => e.NotaNivel1Feedback).IsUnicode(false);

                entity.Property(e => e.NotaNivel2AutoAvaliacao).IsUnicode(false);

                entity.Property(e => e.NotaNivel2AvaliacaoCegas).IsUnicode(false);

                entity.Property(e => e.NotaNivel2AvaliacaoGestor).IsUnicode(false);

                entity.Property(e => e.NotaNivel2AvaliacaoMentor).IsUnicode(false);

                entity.Property(e => e.NotaNivel2Feedback).IsUnicode(false);

                entity.Property(e => e.Notanivel1AvaliacaoGestor).IsUnicode(false);

                entity.Property(e => e.Periodo).IsUnicode(false);

                entity.Property(e => e.PosicaoAtualFluxoAvaliacao).IsUnicode(false);

                entity.Property(e => e.Projeto).IsUnicode(false);

                entity.Property(e => e.SubCompetencia).IsUnicode(false);

                entity.HasOne(d => d.IdAvaliacaoCompetenciaNavigation)
                    .WithMany(p => p.AvaliacoescompetenciasHistoricos)
                    .HasForeignKey(d => d.IdAvaliacaoCompetencia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESCOMPETENCIAS_HISTORICO_AVALIACOESCOMPETENCIAS");
            });

            modelBuilder.Entity<Avaliacoescompetenciasnota>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.CodigoNota).IsUnicode(false);

                entity.Property(e => e.CodigoNotaAvaliador).IsUnicode(false);

                entity.Property(e => e.DescricaoNota).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Avaliacoesperformance>(entity =>
            {
                entity.HasKey(e => e.IdAvaliacaoPerformance)
                    .HasName("PK_AVALIACOESPERFORMANCE");

                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.ComentariosAutoAvaliacao).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoCegas).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoGestor).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoMentor).IsUnicode(false);

                entity.Property(e => e.ComentariosFeedback).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PosicaoAtualFluxoAvaliacao).IsUnicode(false);

                entity.HasOne(d => d.IdAssociadoNavigation)
                    .WithMany(p => p.AvaliacoesperformanceIdAssociadoNavigations)
                    .HasForeignKey(d => d.IdAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_ASSOCIADOS");

                entity.HasOne(d => d.IdAvaliacaoStatusNavigation)
                    .WithMany(p => p.Avaliacoesperformances)
                    .HasForeignKey(d => d.IdAvaliacaoStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_AVALIACOESSTATUS");

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.Avaliacoesperformances)
                    .HasForeignKey(d => d.IdCargo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_CARGOS");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Avaliacoesperformances)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_EMPRESAS");

                entity.HasOne(d => d.IdNivelNavigation)
                    .WithMany(p => p.Avaliacoesperformances)
                    .HasForeignKey(d => d.IdNivel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_CARGOSNIVEIS");

                entity.HasOne(d => d.IdNotaComiteNavigation)
                    .WithMany(p => p.AvaliacoesperformanceIdNotaComiteNavigations)
                    .HasForeignKey(d => d.IdNotaComite)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_IdNotaComite");

                entity.HasOne(d => d.IdNotaNivel1AutoAvaliacaoNavigation)
                    .WithMany(p => p.AvaliacoesperformanceIdNotaNivel1AutoAvaliacaoNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1AutoAvaliacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_AVALIACAOPERFORMANCESNOTAS");

                entity.HasOne(d => d.IdNotaNivel1AvaliacaoCegasNavigation)
                    .WithMany(p => p.AvaliacoesperformanceIdNotaNivel1AvaliacaoCegasNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1AvaliacaoCegas)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_AVALIACAOPERFORMANCESNOTAS2");

                entity.HasOne(d => d.IdNotaNivel1AvaliacaoGestorNavigation)
                    .WithMany(p => p.AvaliacoesperformanceIdNotaNivel1AvaliacaoGestorNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1AvaliacaoGestor)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_AVALIACAOPERFORMANCESNOTAS4");

                entity.HasOne(d => d.IdNotaNivel1AvaliacaoMentorNavigation)
                    .WithMany(p => p.AvaliacoesperformanceIdNotaNivel1AvaliacaoMentorNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1AvaliacaoMentor)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_AVALIACAOPERFORMANCESNOTAS8");

                entity.HasOne(d => d.IdNotaNivel1FeedbackNavigation)
                    .WithMany(p => p.AvaliacoesperformanceIdNotaNivel1FeedbackNavigations)
                    .HasForeignKey(d => d.IdNotaNivel1Feedback)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_AVALIACAOPERFORMANCESNOTAS6");

                entity.HasOne(d => d.IdPeriodoNavigation)
                    .WithMany(p => p.Avaliacoesperformances)
                    .HasForeignKey(d => d.IdPeriodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_PERIODOSAVALIACOES");

                entity.HasOne(d => d.IdProjetoNavigation)
                    .WithMany(p => p.Avaliacoesperformances)
                    .HasForeignKey(d => d.IdProjeto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_PROJETOS");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.AvaliacoesperformanceUsrNavigations)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_ASSOCIADOS1");

                entity.HasOne(d => d.UsrautoAvaliacaoNavigation)
                    .WithMany(p => p.AvaliacoesperformanceUsrautoAvaliacaoNavigations)
                    .HasForeignKey(d => d.UsrautoAvaliacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_ASSOCIADOS2");

                entity.HasOne(d => d.UsravaliacaoCegasNavigation)
                    .WithMany(p => p.AvaliacoesperformanceUsravaliacaoCegasNavigations)
                    .HasForeignKey(d => d.UsravaliacaoCegas)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_ASSOCIADOS3");

                entity.HasOne(d => d.UsravaliacaoGestorNavigation)
                    .WithMany(p => p.AvaliacoesperformanceUsravaliacaoGestorNavigations)
                    .HasForeignKey(d => d.UsravaliacaoGestor)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_ASSOCIADOS4");

                entity.HasOne(d => d.UsravaliacaoMentorNavigation)
                    .WithMany(p => p.AvaliacoesperformanceUsravaliacaoMentorNavigations)
                    .HasForeignKey(d => d.UsravaliacaoMentor)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_ASSOCIADOS6");

                entity.HasOne(d => d.UsrfeedbackNavigation)
                    .WithMany(p => p.AvaliacoesperformanceUsrfeedbackNavigations)
                    .HasForeignKey(d => d.Usrfeedback)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_ASSOCIADOS5");
            });

            modelBuilder.Entity<AvaliacoesperformancesHistorico>(entity =>
            {
                entity.HasKey(e => e.IdAvaliacaoPerformanceHistorico)
                    .HasName("PK_AVALIACOESPERFORMANCE_HISTORICO");

                entity.Property(e => e.Associado).IsUnicode(false);

                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.AvaliacaoStatus).IsUnicode(false);

                entity.Property(e => e.Cargo).IsUnicode(false);

                entity.Property(e => e.ComentariosAutoAvaliacao).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoCegas).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoGestor).IsUnicode(false);

                entity.Property(e => e.ComentariosAvaliacaoMentor).IsUnicode(false);

                entity.Property(e => e.ComentariosFeedback).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Nivel).IsUnicode(false);

                entity.Property(e => e.NotaNivel1AutoAvaliacao).IsUnicode(false);

                entity.Property(e => e.NotaNivel1AvaliacaoCegas).IsUnicode(false);

                entity.Property(e => e.NotaNivel1AvaliacaoMentor).IsUnicode(false);

                entity.Property(e => e.NotaNivel1Feedback).IsUnicode(false);

                entity.Property(e => e.NotaNivel2AutoAvaliacao).IsUnicode(false);

                entity.Property(e => e.NotaNivel2AvaliacaoCegas).IsUnicode(false);

                entity.Property(e => e.NotaNivel2AvaliacaoGestor).IsUnicode(false);

                entity.Property(e => e.NotaNivel2AvaliacaoMentor).IsUnicode(false);

                entity.Property(e => e.NotaNivel2Feedback).IsUnicode(false);

                entity.Property(e => e.Notanivel1AvaliacaoGestor).IsUnicode(false);

                entity.Property(e => e.Performance).IsUnicode(false);

                entity.Property(e => e.Periodo).IsUnicode(false);

                entity.Property(e => e.PosicaoAtualFluxoAvaliacao).IsUnicode(false);

                entity.Property(e => e.Projeto).IsUnicode(false);

                entity.HasOne(d => d.IdAvaliacaoPerformanceNavigation)
                    .WithMany(p => p.AvaliacoesperformancesHistoricos)
                    .HasForeignKey(d => d.IdAvaliacaoPerformance)
                    .HasConstraintName("FK_AVALIACOESPERFORMANCES_HISTORICO_AVALIACOESPERFORMANCES");
            });

            modelBuilder.Entity<Avaliacoesperformancesnota>(entity =>
            {
                entity.HasKey(e => e.IdNota)
                    .HasName("PK_AVALIACAOPERFORMANCESNOTAS");

                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.CodigoNota).IsUnicode(false);

                entity.Property(e => e.DescricaoNota).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Avaliacoesstatus>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).IsUnicode(false);
            });

            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Cargo1).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdProximoCargoNavigation)
                    .WithMany(p => p.InverseIdProximoCargoNavigation)
                    .HasForeignKey(d => d.IdProximoCargo)
                    .HasConstraintName("FK_CARGOS_PROXIMOCARGOS");
            });

            modelBuilder.Entity<Cargosnivei>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Nivel).IsUnicode(false);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Cliente1).IsUnicode(false);

                entity.Property(e => e.Codigo).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.GestorCliente).IsUnicode(false);

                entity.Property(e => e.Telefones).IsUnicode(false);

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CLIENTES_EMPRESAS");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CLIENTES_ASSOCIADOS");
            });

            modelBuilder.Entity<Competencia>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.CompetenciaJr).IsUnicode(false);

                entity.Property(e => e.CompetenciaJrdetalhe).IsUnicode(false);

                entity.Property(e => e.CompetenciaPl).IsUnicode(false);

                entity.Property(e => e.CompetenciaPldetalhe).IsUnicode(false);

                entity.Property(e => e.CompetenciaSr).IsUnicode(false);

                entity.Property(e => e.CompetenciaSrdetalhe).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.Competencia)
                    .HasForeignKey(d => d.IdCargo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPETENCIAS_CARGOS");

                entity.HasOne(d => d.IdDimensaoNavigation)
                    .WithMany(p => p.Competencia)
                    .HasForeignKey(d => d.IdDimensao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPETENCIAS_DIMENSOES");

                entity.HasOne(d => d.IdEixoNavigation)
                    .WithMany(p => p.Competencia)
                    .HasForeignKey(d => d.IdEixo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPETENCIAS_EIXOS");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Competencia)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPETENCIAS_EMPRESAS");

                entity.HasOne(d => d.IdNivelNavigation)
                    .WithMany(p => p.Competencia)
                    .HasForeignKey(d => d.IdNivel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPETENCIAS_CARGOSNIVEIS");

                entity.HasOne(d => d.IdSubCompetenciaNavigation)
                    .WithMany(p => p.Competencia)
                    .HasForeignKey(d => d.IdSubCompetencia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPETENCIAS_SUBCOMPETENCIAS");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.Competencia)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPETENCIAS_ASSOCIADOS");
            });

            modelBuilder.Entity<Complexidade>(entity =>
            {
                entity.HasKey(e => e.IdComplexidade)
                    .HasName("PK_Complexidade");

                entity.Property(e => e.Complexidade1).IsUnicode(false);
            });

            modelBuilder.Entity<Dimenso>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dimensao).IsUnicode(false);

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.Dimensos)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DIMENSOES_ASSOCIADOS");
            });

            modelBuilder.Entity<Eixo>(entity =>
            {
                entity.HasKey(e => e.IdEixo)
                    .HasName("PK_EIXOSPERFORMANCE");

                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Eixo1).IsUnicode(false);

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.Eixos)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EIXOS_ASSOCIADOS");
            });

            modelBuilder.Entity<Emailparametro>(entity =>
            {
                entity.Property(e => e.Dominio).IsUnicode(false);

                entity.Property(e => e.ModeloEmailDiasParaTermino).IsUnicode(false);

                entity.Property(e => e.ModeloEmailDiasSemAlteracao).IsUnicode(false);

                entity.Property(e => e.ModeloEmailEvolucao).IsUnicode(false);

                entity.Property(e => e.ModeloEmailInicio).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.RemetenteEmail).IsUnicode(false);

                entity.Property(e => e.RemetenteNome).IsUnicode(false);

                entity.Property(e => e.Smtpserver).IsUnicode(false);

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Emailparametros)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("FK_EMAILPARAMETROS_EMPRESAS");
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Empresa1).IsUnicode(false);
            });

            modelBuilder.Entity<Evolucaoassociado>(entity =>
            {
                entity.HasKey(e => e.IdEvolucaoAssociado)
                    .HasName("PK__EVOLUCAO__1F062A41DF5E5112");

                entity.Property(e => e.Cargo).IsUnicode(false);

                entity.HasOne(d => d.IdAssociadoNavigation)
                    .WithMany(p => p.Evolucaoassociados)
                    .HasForeignKey(d => d.IdAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_evolucaoassociado_idassociado");

                entity.HasOne(d => d.IdPeriodoNavigation)
                    .WithMany(p => p.Evolucaoassociados)
                    .HasForeignKey(d => d.IdPeriodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_evolucaoassociado_idperiodo");
            });

            modelBuilder.Entity<Evolucaocompetencia>(entity =>
            {
                entity.HasKey(e => e.IdEvoCompetencia)
                    .HasName("PK__EVOLUCAO__A88D164A6148709D");

                entity.HasOne(d => d.IdEixoNavigation)
                    .WithMany(p => p.Evolucaocompetencia)
                    .HasForeignKey(d => d.IdEixo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_evolucaocompetencias_ideixo");

                entity.HasOne(d => d.IdEvolucaoAssociadoNavigation)
                    .WithMany(p => p.Evolucaocompetencia)
                    .HasForeignKey(d => d.IdEvolucaoAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_evolucaocompetencias_idevolucaoassociado");
            });

            modelBuilder.Entity<Evolucaoperformance>(entity =>
            {
                entity.HasKey(e => e.IdEvoPerformance)
                    .HasName("PK__EVOLUCAO__9AAD220446F9E3F7");

                entity.HasOne(d => d.IdEvolucaoAssociadoNavigation)
                    .WithMany(p => p.Evolucaoperformances)
                    .HasForeignKey(d => d.IdEvolucaoAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_evolucaoperformance_idevolucaoassociado");

                entity.HasOne(d => d.IdPerformanceNavigation)
                    .WithMany(p => p.Evolucaoperformances)
                    .HasForeignKey(d => d.IdPerformance)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_evolucaoperformance_idperformance");
            });

            modelBuilder.Entity<Perfi>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Perfil).IsUnicode(false);
            });

            modelBuilder.Entity<Performance>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Performance1).IsUnicode(false);

                entity.Property(e => e.PerformanceAbaixo).IsUnicode(false);

                entity.Property(e => e.PerformanceAcima).IsUnicode(false);

                entity.Property(e => e.PerformanceEsperado).IsUnicode(false);

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.Performances)
                    .HasForeignKey(d => d.IdCargo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PERFORMANCES_CARGOS");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Performances)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PERFORMANCES_EMPRESAS");

                entity.HasOne(d => d.IdNivelNavigation)
                    .WithMany(p => p.Performances)
                    .HasForeignKey(d => d.IdNivel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PERFORMANCES_CARGOSNIVEIS");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.Performances)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PERFORMANCES_ASSOCIADOS");
            });

            modelBuilder.Entity<Periodosavaliaco>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Codigo).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Periodo).IsUnicode(false);

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Periodosavaliacos)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("FK_PERIODOSAVALIACOES_EMPRESAS");
            });

            modelBuilder.Entity<PremissasRadar>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("((1))");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.PremissasRadars)
                    .HasForeignKey(d => d.IdCargo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PREMISSAS_RADAR_CARGOS");

                entity.HasOne(d => d.IdEixoNavigation)
                    .WithMany(p => p.PremissasRadars)
                    .HasForeignKey(d => d.IdEixo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PREMISSAS_RADAR_EIXOS");

                entity.HasOne(d => d.IdNivelNavigation)
                    .WithMany(p => p.PremissasRadars)
                    .HasForeignKey(d => d.IdNivel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PREMISSAS_RADAR_CARGOSNIVEIS");
            });

            modelBuilder.Entity<Projeto>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Codigo).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Projeto1).IsUnicode(false);

                entity.HasOne(d => d.IdAssociadoGestorNavigation)
                    .WithMany(p => p.ProjetoIdAssociadoGestorNavigations)
                    .HasForeignKey(d => d.IdAssociadoGestor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOS_ASSOCIADOS1");

                entity.HasOne(d => d.IdAssociadoResponsavelNavigation)
                    .WithMany(p => p.ProjetoIdAssociadoResponsavelNavigations)
                    .HasForeignKey(d => d.IdAssociadoResponsavel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOS_ASSOCIADOS");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Projetos)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOS_CLIENTES");

                entity.HasOne(d => d.IdComplexidadeNavigation)
                    .WithMany(p => p.Projetos)
                    .HasForeignKey(d => d.IdComplexidade)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOS_PROJETOSCOMPLEXIDADES");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Projetos)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOS_EMPRESAS");

                entity.HasOne(d => d.IdStatusNavigation)
                    .WithMany(p => p.Projetos)
                    .HasForeignKey(d => d.IdStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOS_PROJETOSSTATUS");

                entity.HasOne(d => d.IdTipoNavigation)
                    .WithMany(p => p.Projetos)
                    .HasForeignKey(d => d.IdTipo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOS_PROJETOSTIPOS");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.ProjetoUsrNavigations)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOS_ASSOCIADOS2");
            });

            modelBuilder.Entity<Projetosassociado>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Comentario).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdAssociadoNavigation)
                    .WithMany(p => p.ProjetosassociadoIdAssociadoNavigations)
                    .HasForeignKey(d => d.IdAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOSASSOCIADOS_ASSOCIADOS");

                entity.HasOne(d => d.IdAvaliadorNavigation)
                    .WithMany(p => p.ProjetosassociadoIdAvaliadorNavigations)
                    .HasForeignKey(d => d.IdAvaliador)
                    .HasConstraintName("FK_PROJETOSASSOCIADOS_AVALIADOR");

                entity.HasOne(d => d.IdGestorNavigation)
                    .WithMany(p => p.ProjetosassociadoIdGestorNavigations)
                    .HasForeignKey(d => d.IdGestor)
                    .HasConstraintName("FK_PROJETOSASSOCIADOS_GESTOR");

                entity.HasOne(d => d.IdProjetoNavigation)
                    .WithMany(p => p.Projetosassociados)
                    .HasForeignKey(d => d.IdProjeto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOSASSOCIADOS_PROJETOS2");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.ProjetosassociadoUsrNavigations)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PROJETOSASSOCIADOS_ASSOCIADOS1");
            });

            modelBuilder.Entity<Projetoscomplexidade>(entity =>
            {
                entity.HasKey(e => e.IdComplexidade)
                    .HasName("PK_COMPLEXIDADES");

                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Codigo).IsUnicode(false);

                entity.Property(e => e.Complexidade).IsUnicode(false);

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Usr).IsFixedLength(true);
            });

            modelBuilder.Entity<Projetosstatus>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).IsUnicode(false);
            });

            modelBuilder.Entity<Projetostipo>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("('1')");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ProjetoTipo).IsUnicode(false);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.IdRating)
                    .HasName("PK__RATING__27C557CF09D0FB54");

                entity.Property(e => e.Performance).IsUnicode(false);

                entity.HasOne(d => d.IdComplexidadeNavigation)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.IdComplexidade)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rating_idcomplexidade");
            });

            modelBuilder.Entity<Subcompetencia>(entity =>
            {
                entity.Property(e => e.SubCompetencia1).IsUnicode(false);

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.Subcompetencia)
                    .HasForeignKey(d => d.Usr)
                    .HasConstraintName("FK_SUBCOMPETENCIAS_ASSOCIADOS");
            });

            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.Property(e => e.Atv).HasDefaultValueSql("((1))");

                entity.Property(e => e.Dhc).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Workflows)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WORKFLOW_EMPRESAS");

                entity.HasOne(d => d.IdPeriodoNavigation)
                    .WithMany(p => p.Workflows)
                    .HasForeignKey(d => d.IdPeriodo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WORKFLOW_PERIODOSAVALIACOES");

                entity.HasOne(d => d.UsrNavigation)
                    .WithMany(p => p.Workflows)
                    .HasForeignKey(d => d.Usr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WORKFLOW_ASSOCIADOS");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
