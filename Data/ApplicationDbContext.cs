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

        public DbSet<ASSOCIADOS> Associados { get; set; }
        public DbSet<CARGOS> Cargos { get; set; }
        public DbSet<PERFIS> Perfis { get; set; }
        public DbSet<VERTICAL> Verticais { get; set; }
        public DbSet<EMPRESAS> Empresas { get; set; }
        public DbSet<PROMOCOES> Promocoes { get; set; }
        public DbSet<FOTOSASSOCIADOS> FotosAssociados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.CARGOS)
                .WithMany(c => c.Associados)
                .HasForeignKey(a => a.IdCargo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.PERFIS)
                .WithMany(p => p.Associados)
                .HasForeignKey(a => a.IdPerfil)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.EMPRESAS)
                .WithMany(e => e.Associados)
                .HasForeignKey(a => a.IdEmpresa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.ASSOCIADOS2)
                .WithMany()
                .HasForeignKey(a => a.IdAssociadoMentor)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne<VERTICAL>()
                .WithMany(v => v.Associados)
                .HasForeignKey(a => a.IdVertical)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PROMOCOES>()
                .HasOne(p => p.ASSOCIADOS)
                .WithMany()
                .HasForeignKey(p => p.idAssociado)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<FOTOSASSOCIADOS>()
                .HasOne(f => f.Associado)
                .WithMany()
                .HasForeignKey(f => f.IdAssociado)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}