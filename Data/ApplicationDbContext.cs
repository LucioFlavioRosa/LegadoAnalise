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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações das entidades
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
    }
}