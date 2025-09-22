using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Associado> Associados { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Perfil> Perfis { get; set; }
    public DbSet<Promocao> Promocoes { get; set; }
    public DbSet<FotoAssociado> FotosAssociados { get; set; }
    public DbSet<Vertical> Verticais { get; set; }
    public DbSet<Empresa> Empresas { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Associado>(entity =>
        {
            entity.ToTable("ASSOCIADOS");
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasOne(d => d.Cargo)
                .WithMany(p => p.Associados)
                .HasForeignKey(d => d.IdCargo)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Perfil)
                .WithMany(p => p.Associados)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Mentor)
                .WithMany(p => p.Mentorados)
                .HasForeignKey(d => d.IdAssociadoMentor)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.VerticalNavigation)
                .WithMany(p => p.Associados)
                .HasForeignKey(d => d.IdVertical)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.ToTable("CARGOS");
            entity.HasIndex(e => e.Codigo).IsUnique();
        });
        
        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.ToTable("PERFIS");
            entity.HasIndex(e => e.Codigo).IsUnique();
        });
        
        modelBuilder.Entity<Promocao>(entity =>
        {
            entity.ToTable("PROMOCOES");
            
            entity.HasOne(d => d.Associado)
                .WithMany(p => p.PromocoesComoAssociado)
                .HasForeignKey(d => d.IdAssociado)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(d => d.CargoAnterior)
                .WithMany(p => p.PromocoesCargoAnterior)
                .HasForeignKey(d => d.IdCargoAnterior)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.CargoNovo)
                .WithMany(p => p.PromocoesCargoNovo)
                .HasForeignKey(d => d.IdCargoNovo)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<FotoAssociado>(entity =>
        {
            entity.ToTable("FOTOSASSOCIADOS");
            
            entity.HasOne(d => d.Associado)
                .WithMany(p => p.Fotos)
                .HasForeignKey(d => d.IdAssociado)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Vertical>(entity =>
        {
            entity.ToTable("VERTICAL");
        });
        
        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("EMPRESAS");
        });
    }
}