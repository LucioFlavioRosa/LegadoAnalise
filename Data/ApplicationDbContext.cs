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
            entity.Property(e => e.Nome).HasColumnName("SubCompetencia");
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
    }
}