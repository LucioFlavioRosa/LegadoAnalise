using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Models;

namespace Peers.Moderno.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ASSOCIADOS> ASSOCIADOS { get; set; }
        public DbSet<CARGOS> CARGOS { get; set; }
        public DbSet<PERFIS> PERFIS { get; set; }
        public DbSet<VERTICAL> VERTICAL { get; set; }
        public DbSet<EMPRESAS> EMPRESAS { get; set; }
        public DbSet<FOTOSASSOCIADOS> FOTOSASSOCIADOS { get; set; }
        public DbSet<PROMOCOES> PROMOCOES { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.CARGOS)
                .WithMany(c => c.ASSOCIADOS)
                .HasForeignKey(a => a.IdCargo);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.PERFIS)
                .WithMany(p => p.ASSOCIADOS)
                .HasForeignKey(a => a.IdPerfil);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.EMPRESAS)
                .WithMany(e => e.ASSOCIADOS)
                .HasForeignKey(a => a.IdEmpresa);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.VERTICAL)
                .WithMany(v => v.ASSOCIADOS)
                .HasForeignKey(a => a.IdVertical);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.ASSOCIADOS2)
                .WithMany()
                .HasForeignKey(a => a.IdAssociadoMentor)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FOTOSASSOCIADOS>()
                .HasOne(f => f.ASSOCIADOS)
                .WithMany()
                .HasForeignKey(f => f.IdAssociado);

            modelBuilder.Entity<PROMOCOES>()
                .HasOne(p => p.ASSOCIADOS)
                .WithMany()
                .HasForeignKey(p => p.idAssociado);

            modelBuilder.Entity<PROMOCOES>()
                .HasOne(p => p.CARGOS)
                .WithMany()
                .HasForeignKey(p => p.idCargoAnterior)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PROMOCOES>()
                .HasOne(p => p.CARGOS1)
                .WithMany()
                .HasForeignKey(p => p.idCargoNovo)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}