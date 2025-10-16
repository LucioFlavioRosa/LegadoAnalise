using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cargo> Cargos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.ProximoCargo)
                .WithMany()
                .HasForeignKey(c => c.ProximoCargoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
