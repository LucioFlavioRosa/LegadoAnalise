using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Models;

namespace Peers.Moderno.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Competencia> Competencias { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Eixo> Eixos { get; set; }
    public DbSet<SubCompetencia> SubCompetencias { get; set; }
    public DbSet<Dimensao> Dimensoes { get; set; }
    public DbSet<RelacaoCargoSubcompetencia> RelacoesCargosSubcompetencias { get; set; }
    public DbSet<AvaliacaoCompetenciaNota> AvaliacoesCompetenciasNotas { get; set; }
    public DbSet<ModoCalculoCompetencia> ModosCalculosCompetencias { get; set; }
    public DbSet<PremissasRadar> PremissasRadar { get; set; }
    public DbSet<CargoNivel> CargosNiveis { get; set; }
    // Novos DbSets para Associados
    public DbSet<Associado> Associados { get; set; }
    public DbSet<Promocao> Promocoes { get; set; }
    public DbSet<Perfil> Perfis { get; set; }
    public DbSet<Vertical> Verticais { get; set; }
    // Novo DbSet para Clientes
    public DbSet<Cliente> Clientes { get; set; }
    // Novo DbSet para Complexidades
    public DbSet<ProjetoComplexidade> ProjetosComplexidades { get; set; }
    // Novo DbSet para Performance
    public DbSet<Performance> Performances { get; set; }
    // Novo DbSet para Perguntas de Encerramento
    public DbSet<PerguntaEncerramento> PerguntasEncerramento { get; set; }
    // Novo DbSet para Prazos
    public DbSet<Prazo> Prazos { get; set; }
    // Novos DbSets para Projetos
    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<AssociadoProjeto> AssociadosProjetos { get; set; }
    public DbSet<TipoProjeto> TiposProjetos { get; set; }

    // DbSets para PDI
    public DbSet<PDI_QUESTOES> PDIQuestoes { get; set; }
    public DbSet<PDI_RESPOSTAS> PDIRespostas { get; set; }
    public DbSet<PERIODOSAVALIACOES> PeriodosAvaliacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações das entidades existentes
        modelBuilder.Entity<Competencia>(entity =>
        {
            entity.HasKey(e => e.IdCompetencia);
            entity.ToTable("COMPETENCIAS");
            
            entity.HasOne(d => d.Cargo)
                .WithMany(p => p.Competencias)
                .HasForeignKey(d => d.IdCargo);
                
            entity.HasOne(d => d.Eixo)
                .WithMany(p => p.Competencias)
                .HasForeignKey(d => d.IdEixo);
                
            entity.HasOne(d => d.SubCompetencia)
                .WithMany(p => p.Competencias)
                .HasForeignKey(d => d.IdSubCompetencia);
                
            entity.HasOne(d => d.Dimensao)
                .WithMany(p => p.Competencias)
                .HasForeignKey(d => d.IdDimensao);
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.IdCargo);
            entity.ToTable("CARGOS");
            entity.Property(e => e.Nome).HasColumnName("Cargo");
            
            // Self-referencing relationship for ProximoCargo
            entity.HasOne(e => e.ProximoCargo)
                .WithMany()
                .HasForeignKey(e => e.IdProximoCargo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Eixo>(entity =>
        {
            entity.HasKey(e => e.IdEixo);
            entity.ToTable("EIXOS");
            entity.Property(e => e.Nome).HasColumnName("Eixo");
        });

        modelBuilder.Entity<SubCompetencia>(entity =>
        {
            entity.HasKey(e => e.IdSubCompetencia);
            entity.ToTable("SUBCOMPETENCIAS");
            entity.Property(e => e.Nome).HasColumnName("SubCompetencia").HasMaxLength(500).IsRequired();
            entity.Property(e => e.ATV).HasColumnName("ATV").HasDefaultValue(true);
            entity.Property(e => e.TipoAvaliacao).HasColumnName("TipoAvaliacao").HasMaxLength(50);
            entity.Property(e => e.DHC).HasColumnName("DHC").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.USR).HasColumnName("USR").IsRequired();
            entity.Property(e => e.DataAtualizacao).HasColumnName("DataAtualizacao");
            
            entity.HasIndex(e => e.Nome).HasDatabaseName("IX_SUBCOMPETENCIAS_Nome");
            entity.HasIndex(e => e.TipoAvaliacao).HasDatabaseName("IX_SUBCOMPETENCIAS_TipoAvaliacao");
        });

        modelBuilder.Entity<Dimensao>(entity =>
        {
            entity.HasKey(e => e.IdDimensao);
            entity.ToTable("DIMENSOES");
            entity.Property(e => e.Nome).HasColumnName("Dimensao");
        });

        modelBuilder.Entity<RelacaoCargoSubcompetencia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("RELACAO_CARGO_SUBCOMPETENCIA");
            entity.Property(e => e.IdCargo).HasColumnName("idCargo");
            entity.Property(e => e.IdSubcompetencia).HasColumnName("idSubcompetencia");
        });

        modelBuilder.Entity<AvaliacaoCompetenciaNota>(entity =>
        {
            entity.HasKey(e => e.IdNota);
            entity.ToTable("AVALIACOESCOMPETENCIASNOTAS");
        });

        modelBuilder.Entity<ModoCalculoCompetencia>(entity =>
        {
            entity.HasKey(e => e.IdModo);
            entity.ToTable("MODOSCALCULOSCOMPETENCIAS");
            entity.Property(e => e.IdModo).HasColumnName("idModo");
        });

        modelBuilder.Entity<PremissasRadar>(entity =>
        {
            entity.HasKey(e => e.IdPremissa);
            entity.ToTable("PREMISSAS_RADAR");
            
            entity.HasOne(d => d.Eixo)
                .WithMany()
                .HasForeignKey(d => d.IdEixo);
                
            entity.HasOne(d => d.Cargo)
                .WithMany()
                .HasForeignKey(d => d.IdCargo);
                
            entity.HasOne(d => d.CargoNivel)
                .WithMany(p => p.PremissasRadar)
                .HasForeignKey(d => d.IdNivel);
        });

        modelBuilder.Entity<CargoNivel>(entity =>
        {
            entity.HasKey(e => e.IdNivel);
            entity.ToTable("CARGOSNIVEIS");
        });

        // Configurações das novas entidades de Associados
        modelBuilder.Entity<Associado>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("ASSOCIADOS");
            entity.Property(e => e.Id).HasColumnName("IdAssociado");
            
            entity.HasOne(d => d.Cargo)
                .WithMany()
                .HasForeignKey(d => d.IdCargo)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Mentor)
                .WithMany(p => p.Mentorados)
                .HasForeignKey(d => d.IdMentor)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Perfil)
                .WithMany(p => p.Associados)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Vertical)
                .WithMany(p => p.Associados)
                .HasForeignKey(d => d.IdVertical)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Promocao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("PROMOCOES");
            entity.Property(e => e.Id).HasColumnName("IdPromocao");
            
            entity.HasOne(d => d.Associado)
                .WithMany(p => p.Promocoes)
                .HasForeignKey(d => d.IdAssociado)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.CargoAnterior)
                .WithMany()
                .HasForeignKey(d => d.IdCargoAnterior)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.CargoNovo)
                .WithMany()
                .HasForeignKey(d => d.IdCargoNovo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("PERFIS");
            entity.Property(e => e.Id).HasColumnName("IdPerfil");
            entity.Property(e => e.Nome).HasColumnName("Perfil");
        });

        modelBuilder.Entity<Vertical>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("VERTICAIS");
            entity.Property(e => e.Id).HasColumnName("IdVertical");
            entity.Property(e => e.Nome).HasColumnName("Descricao");
        });

        // Configuração da entidade Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente);
            entity.ToTable("CLIENTES");
            
            entity.HasOne(d => d.AssociadoResponsavel)
                .WithMany()
                .HasForeignKey(d => d.IdAssociacoResponsavel)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade ProjetoComplexidade
        modelBuilder.Entity<ProjetoComplexidade>(entity =>
        {
            entity.HasKey(e => e.IdComplexidade);
            entity.ToTable("PROJETOSCOMPLEXIDADES");
        });

        // Configuração da entidade Performance
        modelBuilder.Entity<Performance>(entity =>
        {
            entity.HasKey(e => e.IdPerformance);
            entity.ToTable("PERFORMANCES");
            
            entity.HasOne(d => d.Cargo)
                .WithMany()
                .HasForeignKey(d => d.IdCargo)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.CargoNivel)
                .WithMany()
                .HasForeignKey(d => d.IdNivel)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.NotaAutoAvaliacao)
                .WithMany()
                .HasForeignKey(d => d.NotaPadraoAutoAvaliacao)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.NotaAvaliacaoAsCegas)
                .WithMany()
                .HasForeignKey(d => d.NotaPadraoAvaliacaoAsCegas)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.NotaAvaliacaoGestor)
                .WithMany()
                .HasForeignKey(d => d.NotaPadraoAvaliacaoGestor)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade PerguntaEncerramento
        modelBuilder.Entity<PerguntaEncerramento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("PERGUNTAS_ENCERRAMENTO");
            
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsRequired();
                
            entity.Property(e => e.Descricao)
                .HasMaxLength(1000)
                .IsRequired();
                
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // Configuração da entidade Prazo
        modelBuilder.Entity<Prazo>(entity =>
        {
            entity.HasKey(e => e.IdPrazo);
            entity.ToTable("PRAZOS");
            
            entity.Property(e => e.NomeDisparo)
                .HasMaxLength(500)
                .IsRequired();
                
            entity.Property(e => e.DHC)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // Configurações das entidades de Projetos
        modelBuilder.Entity<Projeto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("PROJETOS");
            entity.Property(e => e.Id).HasColumnName("IdProjeto");
            
            entity.HasOne(d => d.Cliente)
                .WithMany()
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.AssociadoResponsavel)
                .WithMany()
                .HasForeignKey(d => d.IdAssociadoResponsavel)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.AssociadoGestor)
                .WithMany()
                .HasForeignKey(d => d.IdAssociadoGestor)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.TipoProjeto)
                .WithMany()
                .HasForeignKey(d => d.IdTipoProjeto)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Complexidade)
                .WithMany()
                .HasForeignKey(d => d.IdComplexidade)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AssociadoProjeto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("ASSOCIADOS_PROJETOS");
            
            entity.HasOne(d => d.Projeto)
                .WithMany(p => p.AssociadosProjeto)
                .HasForeignKey(d => d.IdProjeto)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Associado)
                .WithMany()
                .HasForeignKey(d => d.IdAssociado)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.CargoProjeto)
                .WithMany()
                .HasForeignKey(d => d.IdCargoProjeto)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Avaliador)
                .WithMany()
                .HasForeignKey(d => d.IdAvaliador)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TipoProjeto>(entity =>
        {
            entity.HasKey(e => e.IdTipo);
            entity.ToTable("TIPOS_PROJETOS");
            entity.Property(e => e.Nome).HasColumnName("ProjetoTipo");
        });

        // Configuração das entidades PDI
        modelBuilder.Entity<PDI_QUESTOES>(entity =>
        {
            entity.HasKey(e => e.idPDIQuestoes);
            entity.ToTable("PDI_QUESTOES");
        });
        modelBuilder.Entity<PDI_RESPOSTAS>(entity =>
        {
            entity.HasKey(e => e.idPDIRespostas);
            entity.ToTable("PDI_RESPOSTAS");
            entity.HasOne(e => e.PERIODOSAVALIACOES)
                .WithMany()
                .HasForeignKey(e => e.idPeriodo);
            entity.HasOne(e => e.PDI_QUESTOES)
                .WithMany()
                .HasForeignKey(e => e.idPDIQuestao);
        });
        modelBuilder.Entity<PERIODOSAVALIACOES>(entity =>
        {
            entity.HasKey(e => e.IdPeriodo);
            entity.ToTable("PERIODOSAVALIACOES");
        });
    }
}